using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using System;
using System.Threading.Tasks;
using task = System.Threading.Tasks.Task;

namespace TrackCommandId
{

    internal sealed class OutputPanel
    {
        private readonly string _name;
        private readonly Guid _guid = new Guid();
        private readonly IVsOutputWindowPane _pane;

        public static OutputPanel Instance
        {
            get;
            private set;
        }


        public OutputPanel(IVsOutputWindow output, string name)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _name = name;
            //
            output.CreatePane(ref _guid, _name, 1, 1);
            output.GetPane(_guid, out _pane);
            // create ok....
        }
        public static async Task<OutputPanel> InitializeAsync(AsyncPackage package, string name)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var output = await package.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Instance = new OutputPanel(output, name);
            return Instance;
        }

        public async task OutAsync(object message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            _ = _pane.OutputString(DateTime.Now.ToShortTimeString() + ": " + message + Environment.NewLine);
        }
    }
}