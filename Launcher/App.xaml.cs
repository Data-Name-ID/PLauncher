using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Launcher {
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application {
        void On_Startup(object sender, StartupEventArgs e) {
            ResourceDictionary theme = Application.LoadComponent(new Uri($@"/./Styles/Light.xaml", UriKind.Relative)) as ResourceDictionary;
            Current.Resources.MergedDictionaries.Add(theme);
        }
        void On_Exception(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show(e.Exception.ToString());
        }
        void On_Exit(object sender, ExitEventArgs e) {

        }
    }
}
