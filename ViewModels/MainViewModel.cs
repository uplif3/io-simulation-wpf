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

            // 2) Model-Instanzen anlegen
            var ioModel = new IOModel();
            //var debugModel = new Models.DebugModel();
            //var logModel = new Models.LogModel();

            // 3) Sub-ViewModels anlegen & referenzieren
            IOVM = new IOViewModel(_serialPortService);
            DebugVM = new DebugViewModel();
            //LogVM = new LogViewModel(logModel);

            // 4) Am DataReceived-Event lauschen
            _serialPortService.LineReceived += (sender, line) =>
            {
                // Wir sind hier vermutlich in einem Hintergrundthread
                // => also in den UI-Thread per Dispatcher
                Application.Current.Dispatcher.Invoke(() => ParseLine(line));
            };
        }

        private void ParseLine(string line)
        {
            // Grobe Beispiel-Logik: 
            // Unterscheide, ob "IO=...", "DEBUG=...", "LOG=..."
            if (line.StartsWith("IO="))
            {
                // => IO-spezifische Daten
                //   Wir rufen die ParseIOData-Methode im IOViewModel auf:
                IOVM.ParseData(line.Substring(3));
            }
            else if (line.StartsWith("DEBUG="))
            {
                // => Debug-Meldung
                var msg = line.Substring(6);
                DebugVM?.AddDebugMessage(msg);
            }
            else if (line.StartsWith("LOG="))
            {
                // => Log-Meldung
                var msg = line.Substring(4);
                LogVM?.AddLogMessage(msg);
            }
            else
            {
                // Unbekannt -> pack's ins Debug oder Log, wie du magst
                DebugVM?.AddDebugMessage("Unknown line: " + line);
            }
        }
    }
}
