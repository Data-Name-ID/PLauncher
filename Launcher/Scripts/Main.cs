using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading.Tasks;

namespace Launcher.Scripts {
    public partial class Main {

        public static string inis_path = AppDomain.CurrentDomain.BaseDirectory + @"Data\";
        public static string settings = AppDomain.CurrentDomain.BaseDirectory + @"Data\Settings";

        public static string desktop_path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\";
        public static string start_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + @"\Programs\";

        public static Task start_program(string name) {
            return Task.Run(() => {
                var ini = new IniFile(inis_path + name + ".ini");

                string path = ini.Read("Path", "Main");
                string services = ini.Read("Services", "Main");
                string processes = ini.Read("Processes", "Main");

                foreach (var service in services.Split(';')) {
                    cmd($"sc config \"{service.Trim()}\" start=Demand");
                    cmd($"sc start \"{service.Trim()}\"");
                }

                var main_process = Process.Start(new ProcessStartInfo {
                    FileName = path,
                    WorkingDirectory = Path.GetDirectoryName(path)
                });

                main_process.WaitForExit();

                var mos = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", main_process.Id));
                foreach (ManagementObject mo in mos.Get())
                    Process.GetProcessById(Convert.ToInt32(mo["ProcessID"])).WaitForExit();

                foreach (var processe in processes.Split(';')) {
                    cmd($"taskkill /f /im \"{processe.Trim()}\"");
                }

                foreach (var service in services.Split(';')) {
                    cmd($"sc stop \"{service.Trim()}\"");
                }
            });
        }

        private static void cmd(string command) {
            Process.Start(new ProcessStartInfo {
                FileName = "cmd",
                Arguments = $"/c {command}",
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();
        }
    }
}
