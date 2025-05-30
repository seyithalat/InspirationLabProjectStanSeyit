using System;
using System.Windows;
using Microsoft.Win32;

namespace InspirationLabProjectStanSeyit
{
    public partial class App : Application
    {
        public App()
        {
            SetBrowserFeatureControl();
            // Database initialization removed; handled elsewhere if necessary
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // Start with the main window instead of login
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
        // Sets the Internet Explorer browser emulation mode for the application.
        // This ensures that web content is rendered using IE11's rendering engine.
        private void SetBrowserFeatureControl()
        {
            // Get the name of the current application executable
            string appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

            // Create or open the registry key for IE browser emulation settings
            
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION"))
            {
                // Set the browser emulation mode to IE11 (11001)
                // 11 = IE11
                // 001 = Standards mode
                key.SetValue(appName, 11001, RegistryValueKind.DWord);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Database cleanup removed; handled elsewhere if needed
            base.OnExit(e);
        }
    }
}