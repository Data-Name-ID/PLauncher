using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using static Launcher.Scripts.Main;

namespace Launcher {
    public partial class Program : Window {
        public Program(string title) {
            InitializeComponent();

            moving_grid.MouseLeftButtonDown += (s, e) => { DragMove(); };
            close_btn.MouseLeftButtonUp += (s, e) => { Close(); };

            title_tb.Text = title;
            path_btn.Click += (s, e) => { get_path(); };
            ok_btn.Click += (s, e) => {
                if (validate()) {
                    change_ini();
                    Close();
                }
            };
        }

        private void get_path() {
            var fd = new OpenFileDialog() {
                Filter = "Исполняемый файл|*.exe|Batch File|*.bat|Command Script|*.cmd|Windows Installer Package|*.msi|VBScript File|*.vps"
            };
            if (fd.ShowDialog() == true) {
                path_tb.Text = fd.FileName;

                if(name_tb.Text == "")
                    name_tb.Text = Path.GetFileNameWithoutExtension(fd.FileName);
            }
        }

        private void change_ini() {
            var ini = new IniFile($"{inis_path + name_tb.Text}.ini");
            ini.Write("Path", path_tb.Text, "Main");
            ini.Write("Services", services_tb.Text, "Main");
            ini.Write("Processes", processes_tb.Text, "Main");
        }

        private bool validate() {
            if (name_tb.Text == "") {
                CMessageBox.Show("Пустое имя не допустимо!", "Ошибка", new string[] { "Хорошо" });
                return false;
            }

            if (!new Regex("[^\\/:*?\"<>|]").Match(name_tb.Text).Success) {
                CMessageBox.Show("В имени содержатся недопустимые символы\n\\ / : * ? \" < > |", "Ошибка", new string[] { "Хорошо" });
                return false;
            }

            if (File.Exists($"{inis_path + name_tb.Text}.ini") && CMessageBox.Show("Программа с таким именем уже существует в списке!\nДанные будут перезаписаны, вы хотите продолжить?", "Предупреждение", new string[] { "Да", "Нет" }, 1) == "Нет")
                return false;

            if (!File.Exists(path_tb.Text) && CMessageBox.Show("Программа по указанному пути не существует!\nВы хотите продолжить?", "Предупреждение", new string[] { "Да", "Нет" }, 1) == "Нет")
                return false;

            return true;
        }
    }
}
