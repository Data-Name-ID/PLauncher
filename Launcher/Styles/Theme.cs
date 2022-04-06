using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static Launcher.Scripts.Main;

namespace Launcher.Styles {
    internal class Theme {
        public static void change_theme(string name = null) {
            var ini = new IniFile(settings);

            string _name;
            if (name == null) {
                var theme_name = ini.Read("Theme", "Main");
                if (theme_name == "Dark")
                    _name = "Light";
                else
                    _name = "Dark";
            } else {
                _name = name;
            }

            load_theme(_name);

            ini.Write("Theme", _name, "Main");
        }

        public static void load_theme(string name) {
            var theme = Application.LoadComponent(new Uri($@"/./Styles/{name}.xaml", UriKind.Relative)) as ResourceDictionary;
            Application.Current.Resources.MergedDictionaries.Add(theme);
        }
    }
}
