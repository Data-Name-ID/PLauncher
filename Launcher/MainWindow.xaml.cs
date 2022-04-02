using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using static Launcher.Scripts.Main;
using Launcher.Scripts;
using System;

namespace Launcher {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            moving_grid.MouseLeftButtonDown += (s, e) => { DragMove(); };
            close_btn.MouseLeftButtonUp += (s, e) => { Application.Current.Shutdown(); };

            add_btn.Click += (s, e) => { add_program(); update_list(); };
            delete_btn.Click += (s, e) => { delete_program(); };

            update_list();
        }

        private List<string> get_list() {
            var programs = new List<string>();

            Directory.CreateDirectory(inis_path);
            foreach (var file in Directory.GetFiles(inis_path, "*.ini")) {
                var ini = new IniFile(file);

                var program = Path.GetFileNameWithoutExtension(file);

                if (program != "") {
                    program += $" - [{ini.Read("Path", "Main")}]";
                    if (program.Length > 74)
                        program = program.Substring(0, 72) + "..]";

                    programs.Add(program);
                }
            }

            return programs;
        }

        private void add_program() {
            new Program("Добавление программы").ShowDialog();
        }

        private void change_program() {
            new Program("Изменение программы").Show();
        }

        private void delete_program() {
            var ini = inis_path + ((string)prog_list.SelectedItem).Split('-')[0].Trim() + ".ini";
            if (File.Exists(ini)) {
                File.Delete(ini);
                update_list();
            }
        }

        private void update_list() {
            prog_list.ItemsSource = get_list();
        }

        private void create_slink(object sender, RoutedEventArgs e) {

        }

        private void create_dlink(object sender, RoutedEventArgs e) {
            string name = ((MenuItem)sender).Header.ToString();
            // <путь_к_файлу>, <путь_к_ярлыку>, <аргументы_командной_строки>, <описание>
            WLink.Create(Assembly.GetExecutingAssembly().Location, Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $@"\{name}.lnk", "", "Описание");
        }

        private void start_program() {

        }
    }
}
