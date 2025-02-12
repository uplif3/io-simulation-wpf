using System;
using System.Windows;
using io_simulation_wpf.Models;
using io_simulation_wpf.Services;
using io_simulation_wpf.ViewModels;

namespace io_simulation_wpf.ViewModels
{
    public class MainViewModel
    {
        private readonly SerialPortService _serialPortService;

        // Sub-ViewModels
        public IOViewModel IOVM { get; }
        public DebugViewModel? DebugVM { get; }
        public LogViewModel? LogVM { get; }

        public MainViewModel()
        {
            // 1) Serial-Port-Service anlegen
            _serialPortService = new SerialPortService("COM4", 9600);

            // 3) Sub-ViewModels anlegen & referenzieren
            IOVM = new IOViewModel(_serialPortService);
            DebugVM = new DebugViewModel();
            LogVM = new LogViewModel();

            // 4) Am DataReceived-Event lauschen
            _serialPortService.LineReceived += (sender, line) =>
            {
                // Wir sind hier vermutlich in einem Hintergrundthread
                // => also in den UI-Thread per Dispatcher
                Application.Current.Dispatcher.Invoke(() => ProcessPacket(line));
            };
        }

        private void ProcessPacket(string line)
        {
            if (line.StartsWith("d0"))
            {
                IOVM.ProcessLedPacket(line.Substring(2));
                DebugVM?.AddDebugMessage("Recv: " + line);
            }
            else if (line.StartsWith("dD"))
            {
                DebugVM?.AddDebugMessage(line.Substring(2));
            }
            else if (line.StartsWith("dL"))
            {
                LogVM?.AddLogMessage(line);
            }
            else
            {
                DebugVM?.AddDebugMessage("Unknown line: " + line);
            }
        }
    }
}
