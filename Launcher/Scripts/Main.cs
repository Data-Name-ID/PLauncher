using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Launcher.Scripts {
    public partial class Main {
        public static string inis_path = AppDomain.CurrentDomain.BaseDirectory + @"Data\";

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

                Process.Start(new ProcessStartInfo {
                    FileName = path,
                    WorkingDirectory = Path.GetDirectoryName(path)
                }).WaitForExit();

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
