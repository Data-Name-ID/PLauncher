using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Launcher {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            moving_grid.MouseLeftButtonDown += (s, e) => { DragMove(); };
            close_btn.MouseLeftButtonUp += (s, e) => { Application.Current.Shutdown(); };
        }

        private void get_list() {

        }

        private void update_list() {

        }

        private void create_link() {

        }

        private void start_program() {

        }
    }
}
