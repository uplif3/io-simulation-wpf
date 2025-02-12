using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using io_simulation_wpf.Models;
using io_simulation_wpf.Services;

namespace io_simulation_wpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SerialPortService _serialPortService;
        private object? _activeSpecialView;

        // Sub-ViewModels
        public IOViewModel IOVM { get; }
        public DebugViewModel? DebugVM { get; }
        public LogViewModel? LogVM { get; }

        /// <summary>
        /// Enthält das aktuell aktive Special View (z. B. ein spezielles ViewModel) oder null, wenn keiner aktiv ist.
        /// </summary>
        public object? ActiveSpecialView
        {
            get => _activeSpecialView;
            set
            {
                if (_activeSpecialView != value)
                {
                    _activeSpecialView = value;
                    OnPropertyChanged();
                    // Da sich dadurch die Position der IOView ändern soll, auch IOViewRow neu melden:
                    OnPropertyChanged(nameof(IOViewRow));
                }
            }
        }

        /// <summary>
        /// Liefert den Grid.Row-Wert für die IOView.
        /// Wird 0 zurückgegeben, wenn kein Special View aktiv ist,
        /// und 1 (oder ein anderer gewünschter Wert), wenn ein Special View angezeigt wird.
        /// </summary>
        public int IOViewRow => ActiveSpecialView == null ? 0 : 1;

        public MainViewModel()
        {
            // 1) Serial-Port-Service anlegen
            _serialPortService = new SerialPortService("COM4", 9600);

            // 2) Sub-ViewModels anlegen & referenzieren
            IOVM = new IOViewModel(_serialPortService);
            DebugVM = new DebugViewModel();
            LogVM = new LogViewModel();

            // 3) Am DataReceived-Event lauschen (hier wird in den UI-Thread gewechselt)
            _serialPortService.LineReceived += (sender, line) =>
            {
                Application.Current.Dispatcher.Invoke(() => ProcessPacket(line));
            };
        }

        /// <summary>
        /// Verarbeitet eingehende serielle Daten.
        /// Hier werden z. B. LED-Daten an die IOViewModel weitergereicht
        /// und andere Nachrichten als Debug-/Log-Meldungen verarbeitet.
        /// </summary>
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
            else if (line.StartsWith("dS"))
            {
                OnSpecialViewRequested(int.Parse(line.Substring(2)));
            }
            else
            {
                DebugVM?.AddDebugMessage("Unknown line: " + line);
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn ein spezieller Screen (Extra View) angefordert wird.
        /// Anhand des screenIdentifier (z. B. "Screen1", "Screen2" etc.) wird der entsprechende Special View erstellt.
        /// </summary>
        public void OnSpecialViewRequested(int screenIdentifier)
        {
            // Je nach Identifier den entsprechenden Special Screen einblenden.
            // Hier als Beispiel mit zwei möglichen Screens – du kannst das nach Bedarf erweitern.
            switch (screenIdentifier)
            {
                case 1:
                    ActiveSpecialView = new AlarmclockViewModel();
                    break;
                case 2:
                    ActiveSpecialView = new SeesawViewModel();
                    break;
                default:
                    // Falls der Identifier unbekannt ist, wird kein Special View gesetzt.
                    ActiveSpecialView = null;
                    break;
            }
        }

        /// <summary>
        /// Löscht bzw. deaktiviert den aktiven Special View.
        /// Dadurch wird ActiveSpecialView auf null gesetzt und IOViewRow (über die Binding-Logik) ändert sich wieder.
        /// </summary>
        public void ClearSpecialView()
        {
            ActiveSpecialView = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
