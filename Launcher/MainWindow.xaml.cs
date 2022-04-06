using Launcher.Scripts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using static Launcher.Scripts.Main;
using static Launcher.Styles.Theme;

namespace Launcher {
    public partial class MainWindow : Window {

        public MainWindow() {

            // Проверка на наличие аргумента
            // Если присутствует - запускаем программу
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2) {
                hide_window();
                start_program(args[1].Trim('-')).Wait();

                Application.Current.Shutdown();
            }

            InitializeComponent();

            moving_grid.MouseLeftButtonDown += (s, e) => { DragMove(); };
            close_btn.MouseLeftButtonUp += (s, e) => { Application.Current.Shutdown(); };

            add_btn.Click += (s, e) => { add_program(); update_list(); };
            delete_btn.Click += (s, e) => {
                if (prog_list.SelectedItem != null)
                    delete_program(reformat_str(prog_list.SelectedItem.ToString()));
            };
            prog_list.MouseDoubleClick += (s, e) => { prog_list.SelectedIndex = -1; };

            moving_grid.MouseRightButtonUp += (s, e) => { change_theme(); };

            update_list();
        }

        private void hide_window() {
            WindowState = WindowState.Minimized;
            Visibility = Visibility.Hidden;
            IsTabStop = false;
            ShowInTaskbar = false;
        }

        private string format_str(string str_1, string str_2) {
            var result = $"{str_1} - [{str_2}]";
            if (result.Length > 71)
                result = result.Substring(0, 69) + "..]";

            return result;
        }

        private string reformat_str(string str) {
            var result = str.Substring(0, str.LastIndexOf('-') - 1).Trim();

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

        private void change_program(string name) {
            new Program("Изменение программы", name).ShowDialog();
        }

        private void delete_program(object item) {
            if (item != null) {
                var ini = inis_path + item + ".ini";

                var d_lnk_name = $@"{desktop_path + item}.lnk";
                var s_lnk_name = $@"{start_path + item}.lnk";

                if (File.Exists(d_lnk_name) && CMessageBox.Show("На рабочем столе есть ярлык на запуск этой программы, вы хотите удалить его?", "Предупреждение", new string[] { "Да", "Нет" }, 1) != "Нет")
                    File.Delete(d_lnk_name);

                if (File.Exists(s_lnk_name) && CMessageBox.Show("В меню пуск есть ярлык на запуск этой программы, вы хотите удалить его?", "Предупреждение", new string[] { "Да", "Нет" }, 1) != "Нет")
                    File.Delete(s_lnk_name);

                if (File.Exists(ini))
                    File.Delete(ini);

                update_list();
            }
        }

        private void update_list() {
            prog_list.ItemsSource = get_list();
        }


        private void cm_add_program(object sender, RoutedEventArgs e) {
            add_program();
            update_list();
        }

        private void cm_refresh_list(object sender, RoutedEventArgs e) {
            update_list();
        }

        private void cm_open_folder(object sender, RoutedEventArgs e) {
            Process.Start(inis_path);
        }


        private async void cm_start_program(object sender, RoutedEventArgs e) {
            var name = get_cm_item_name(sender);
            await start_program(name);
        }

        public void cm_create_dlink(object sender, RoutedEventArgs e) {
            create_link(sender, desktop_path);
        }

        private void cm_create_slink(object sender, RoutedEventArgs e) {
            create_link(sender, start_path);
        }

        private void cm_edit_program(object sender, RoutedEventArgs e) {
            change_program(get_cm_item_name(sender));
            update_list();
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
