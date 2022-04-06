using Launcher.Scripts;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows;
using static Launcher.Scripts.Main;

namespace Launcher {
    public partial class Program : Window {

        private string old_name;
        private string old_path;

        public Program(string title, string name = null) {

            InitializeComponent();

            moving_grid.MouseLeftButtonDown += (s, e) => { DragMove(); };
            close_btn.MouseLeftButtonUp += (s, e) => { Close(); };

            title_tb.Text = title;
            path_btn.Click += (s, e) => { get_path(); };
            ok_btn.Click += (s, e) => { change_ini(); };

            if (name != null) {
                var ini = new IniFile($"{inis_path + name}.ini");

                name_tb.Text = name;
                path_tb.Text = ini.Read("Path", "Main");
                services_tb.Text = ini.Read("Services", "Main");
                processes_tb.Text = ini.Read("Processes", "Main");

                old_name = name;
                old_path = path_tb.Text;
            }
        }

        private void change_ini() {
            if (!validate())
                return;

            if (old_name != null && old_name != name_tb.Text)
                File.Move($"{inis_path + old_name}.ini", $"{inis_path + name_tb.Text}.ini");

            if (old_name != name_tb.Text || old_path != path_tb.Text) {
                update_link(desktop_path, "На рабочем столе");
                update_link(start_path, "В меню пуск");
            }

            var ini = new IniFile($"{inis_path + name_tb.Text}.ini");

            ini.Write("Path", path_tb.Text, "Main");
            ini.Write("Services", services_tb.Text, "Main");
            ini.Write("Processes", processes_tb.Text, "Main");

            Close();
        }

        private bool validate() {
            if (name_tb.Text == "") {
                CMessageBox.Show("Пустое имя не допустимо!", "Ошибка", new string[] { "Хорошо" });
                return false;
            }

            foreach (var s in "\\/:*?\"<>|")
                if (name_tb.Text.Contains(s.ToString())) {
                    CMessageBox.Show("В имени содержатся недопустимые символы\n\\ / : * ? \" < > |", "Ошибка", new string[] { "Хорошо" });
                    return false;
                }

            if (name_tb.Text.Length > 70) {
                CMessageBox.Show("Слишком длинное имя! (> 70 символов)", "Ошибка", new string[] { "Хорошо" });
                return false;
            }

            if (File.Exists($"{inis_path + name_tb.Text}.ini") && old_name == null && CMessageBox.Show("Программа с таким именем уже существует в списке!\nДанные будут перезаписаны, вы хотите продолжить?", "Предупреждение", new string[] { "Да", "Нет" }, 1) == "Нет")
                return false;

            if (!File.Exists(path_tb.Text) && CMessageBox.Show("Программа по указанному пути не существует!\nВы хотите продолжить?", "Предупреждение", new string[] { "Да", "Нет" }, 1) == "Нет")
                return false;

            return true;
        }

        private void get_path() {
            var fd = new OpenFileDialog() {
                Filter = "Исполняемый файл|*.exe|Batch File|*.bat|Command Script|*.cmd|Windows Installer Package|*.msi|VBScript File|*.vps"
            };

            if (fd.ShowDialog() == true) {
                path_tb.Text = fd.FileName;

                if (name_tb.Text == "")
                    name_tb.Text = Path.GetFileNameWithoutExtension(fd.FileName);
            }
        }

        private void update_link(string path, string message) {
            if (File.Exists($@"{path + old_name}.lnk") && CMessageBox.Show($"{message} есть ярлык на запуск этой программы, вы хотите актуализировать его в связи с изменениями?", "Предупреждение", new string[] { "Да", "Нет" }, 1) != "Нет") {
                File.Delete($@"{path + old_name}.lnk");

                string icon_path = new IniFile(inis_path + name_tb.Text + ".ini").Read("Path", "Main");
                WLink.Create($@"{path + name_tb.Text}.lnk", Assembly.GetExecutingAssembly().Location, $"-{name_tb.Text}", $"Запустить программу {name_tb.Text} с помощью PLauncher", icon_path);
            }
        }
    }
}
