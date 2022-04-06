using System.IO;
using System.Windows;
using System.Windows.Threading;
using static Launcher.Scripts.Main;
using static Launcher.Styles.Theme;

namespace Launcher {
    public partial class App : Application {

        void On_Startup(object sender, StartupEventArgs e) {
            if (!File.Exists(settings))
                change_theme("Dark");
            else
                load_theme(new IniFile(settings).Read("Theme", "Main"));
        }

        void On_Exception(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show(e.Exception.ToString());
        }

        void On_Exit(object sender, ExitEventArgs e) {

        }
    }
}
