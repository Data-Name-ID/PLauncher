using System.Windows;
using System.Windows.Controls;

namespace Launcher {
    public partial class CMessageBox : Window {
        private static string result;

        public CMessageBox() {
            InitializeComponent();

            moving_grid.MouseLeftButtonDown += (s, e) => { DragMove(); };
            close_btn.MouseLeftButtonUp += (s, e) => { Close(); };
        }

        public static string Show(string message, string title, string[] buttons, byte default_btn=0) {
            var mb = new CMessageBox();

            mb.title_tb.Text = title;
            mb.message_tb.Text = message;

            foreach (string btn_name in buttons) {
                var btn = new Button() {
                    Content = btn_name,
                    Margin = new Thickness(15, 0, 0, 0)
                };
                btn.Click += (s, e) => { result = ((Button)s).Content.ToString(); mb.Close(); };
                mb.stack_btns.Children.Add(btn);
            }

            result = buttons[default_btn];

            mb.ShowDialog();

            return result;
        }
    }
}
