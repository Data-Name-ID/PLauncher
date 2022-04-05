using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using static Launcher.Scripts.Main;
using Launcher.Scripts;
using System;
using System.Threading.Tasks;

namespace Launcher {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            moving_grid.MouseLeftButtonDown += (s, e) => { DragMove(); };
            close_btn.MouseLeftButtonUp += (s, e) => { Application.Current.Shutdown(); };

            add_btn.Click += (s, e) => { add_program(); update_list(); };
            delete_btn.Click += (s, e) => { delete_program(prog_list.SelectedItem); };
            //prog_list.MouseLeftButtonUp += (s, e) => { prog_list.SelectedIndex = -1; };

            update_list();
        }

        private string format_str(string str_1, string str_2) {
            var result = $"{str_1} - [{str_2}]";
            if (result.Length > 74)
                result = result.Substring(0, 72) + "..]";

            return result;
        }

        private string reformat_str(string str) {
            var result = str.Split('-')[0].Trim();

            return result;
        }

        private List<string> get_list() {
            var programs = new List<string>();

            Directory.CreateDirectory(inis_path);
            foreach (var file in Directory.GetFiles(inis_path, "*.ini")) {
                var ini = new IniFile(file);
                var program_name = Path.GetFileNameWithoutExtension(file);

                if (program_name != "")
                    programs.Add(format_str(program_name, ini.Read("Path", "Main")));
            }

            return programs;
        }

        private void add_program() {
            new Program("Добавление программы").ShowDialog();
        }

        private void change_program() {
            new Program("Изменение программы").ShowDialog();
        }

        private void delete_program(object item) {
            if (item != null) {
                var ini = inis_path + reformat_str((string)item) + ".ini";

                if (File.Exists(ini)) {
                    File.Delete(ini);
                    update_list();
                }
            }
        }

        private void update_list() {
            prog_list.ItemsSource = get_list();
        }

        private async void cm_start_program(object sender, RoutedEventArgs e) {
            var name = get_cm_item_name(sender);
            await start_program(name);
        }

        private void cm_add_program(object sender, RoutedEventArgs e) {
            add_program();
            update_list();
        }

        private void cm_refresh_list(object sender, RoutedEventArgs e) {
            update_list();
        }

        public void cm_create_dlink(object sender, RoutedEventArgs e) {
            create_link(sender, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
        }

        private void cm_create_slink(object sender, RoutedEventArgs e) {
            create_link(sender, Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + @"\Programs");
        }

        private void cm_delete_program(object sender, RoutedEventArgs e) {
            delete_program(get_cm_item_name(sender));
        }

        private string get_cm_item_name(object sender) {
            return reformat_str(((ListBoxItem)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget).Content.ToString());
        }

        private void create_link(object sender, string path) {
            string name = get_cm_item_name(sender);
            string icon_path = new IniFile(inis_path + name + ".ini").Read("Path", "Main");

            WLink.Create($@"{path}\{name}.lnk", Assembly.GetExecutingAssembly().Location, $"-{name}", $"Запустить программу {name} с помощью PLauncher", icon_path);
        }        
    }
}
