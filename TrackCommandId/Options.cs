using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Shell;

namespace TrackCommandId
{
    public class Options : DialogPage
    {
        // General
        private const string GeneralCategory = "General";

        [Category(GeneralCategory)]
        [DisplayName("Show on shortcut")]
        [Description("Determines if the status bar should show shortcuts for commands that were invoked by a shortcut.")]
        [DefaultValue(false)]
        
        public bool ShowOnShortcut { get; set; }

        // Status Bar
        private const string StatusBarCategory = "Statur Bar";

        private const string Clipboard = "Clipboard";

        [Category(StatusBarCategory)]
        [DisplayName("Log to Status Bar")]
        [Description("Log all keyboard shortcuts for commands captured to the status bar.")]
        [DefaultValue(true)]
        public bool LogToStatusBar { get; set; } = true;


        [Category(Clipboard)]
        [DisplayName("Auto Copy")]
        [Description("captured the command name and copy to clipboard")]
        [DefaultValue(false)]
        public bool IsAutoCopy { get; set; } = false;


        // Output Window
        private const string OutputWindowCategory = "Output Window";

        [Category(OutputWindowCategory)]
        [DisplayName("Log to Output Window")]
        [Description("Log all keyboard shortcuts for commands captured to the Output Window.")]
        [DefaultValue(true)]
        public bool LogToOutputWindow { get; set; } = true;

        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();

            Saved?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fires when the options have been saved.
        /// </summary>
        public event EventHandler Saved;
    }
}
