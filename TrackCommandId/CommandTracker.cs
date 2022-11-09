using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using EnvDTE;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Windows = EnvDTE.Windows;

namespace TrackCommandId
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CommandTracker
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private readonly DTE _dte;

        private readonly Key[] _keys = { Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt, Key.LeftShift, Key.RightShift };
        private bool _showShortcut;
        private static readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private static readonly string[] _ignoredCmd =
{
            "Edit.GoToFindCombo",
            "Debug.LocationToolbar.ProcessCombo",
            "Debug.LocationToolbar.ThreadCombo",
            "Debug.LocationToolbar.StackFrameCombo",
            "Build.SolutionPlatforms",
            "Build.SolutionConfigurations"
        };

        private readonly Options _options;

        private readonly IVsStatusbar _statusbar;
        private readonly OutputPanel _outputPanel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandTracker"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="options">Command service to add command to, not null.</param>
        /// <param name="dte"></param>
        /// <param name="statusbar"></param>
        /// <param name="outputPanel"></param>
        private CommandTracker(AsyncPackage package, Options options, DTE dte, IVsStatusbar statusbar,
            OutputPanel outputPanel)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            _dte = dte;
            _statusbar = statusbar;
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _outputPanel = outputPanel;
            _initCmdEvents();

        }

        private void _initCmdEvents()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // get the command event
            var cmdEvents = _dte.Events.CommandEvents;
            cmdEvents.BeforeExecute += BeforeExecute;
            cmdEvents.AfterExecute += AfterExecute;
        }



        private void BeforeExecute(string Guid, int ID, object CustomIn, object CustomOut, ref bool CancelDefault)
        {
            _showShortcut = _options.ShowOnShortcut || !_keys.Any(key => Keyboard.IsKeyDown(key));
        }

        private void AfterExecute(string Guid, int ID, object CustomIn, object CustomOut)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            //
            if (!(CustomIn is null) || !(CustomOut is null))
            {
                return;
            }

            try
            {
                Command cmd = null;
                try
                {
                    cmd = _dte.Commands.Item(Guid, ID);
                }
                catch (ArgumentException)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(cmd?.Name) || ShouldCommandBeIgnored(cmd))
                {
                    return;
                }
                var shortcut = GetShortcut(cmd);

                var shortDescBuilder = new StringBuilder()
                    .Append(cmd.Name);
                // 判断是否有快捷键
                if (!string.IsNullOrWhiteSpace(shortcut))
                {
                    shortDescBuilder.Append(":");
                    shortDescBuilder.Append(shortcut);
                    return;
                }

                if (_options.LogToStatusBar)
                {
                    // 设置状态栏
                    _statusbar.SetText(shortDescBuilder.ToString());
                }

                // 判断是否自动放到剪切板里
                if (_options.IsAutoCopy)
                {
                    Clipboard.SetText(cmd.Name);
                }

                if (_options.LogToOutputWindow)
                {
                    _outputPanel.OutAsync($"{cmd.Name} ({shortcut})").ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _outputPanel.OutAsync(ex).ConfigureAwait(false);
            }
        }

        private static string GetShortcut(Command cmd)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (cmd == null || string.IsNullOrEmpty(cmd.Name))
            {
                return null;
            }

            string key = cmd.Guid + cmd.ID;

            if (_cache.ContainsKey(key))
            {
                return _cache[key];
            }

            string bindings = ((object[])cmd.Bindings).FirstOrDefault() as string;

            if (!string.IsNullOrEmpty(bindings))
            {
                int index = bindings.IndexOf(':') + 2;
                string shortcut = bindings.Substring(index);

                if (!IsShortcutInteresting(shortcut))
                {
                    shortcut = null;
                }

                if (!_cache.ContainsKey(key))
                {
                    _cache.Add(key, shortcut);
                }

                return shortcut;
            }

            return null;
        }
        private static bool IsShortcutInteresting(string shortcut)
        {
            if (string.IsNullOrWhiteSpace(shortcut))
            {
                return false;
            }

            if (!shortcut.Contains("Ctrl") && !shortcut.Contains("Alt") && !shortcut.Contains("Shift"))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CommandTracker Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="options"></param>
        /// <param name="outputPanel"></param>
        public static async Task InitializeAsync(AsyncPackage package, Options options, OutputPanel outputPanel)
        {
            // Switch to the main thread - the call to AddCommand in CommandTracker's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
            var statusbar = await package.GetServiceAsync(typeof(SVsStatusbar)) as IVsStatusbar;
            Instance = new CommandTracker(package, options, dte, statusbar, outputPanel);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "CommandTracker";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private static bool ShouldCommandBeIgnored(Command cmd)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (_ignoredCmd.Contains(cmd.Name, StringComparer.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

    }
}
