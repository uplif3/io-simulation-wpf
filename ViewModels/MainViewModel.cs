using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
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
        public AlarmclockViewModel? ClockVM { get; }
        public SeesawViewModel? SeesawVM { get; }

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

        public ObservableCollection<string> AvailablePorts { get; set; }
        public ICommand DisconnectCommand { get; }
        public ICommand SelectPortCommand { get; }


        public MainViewModel()
        {
            _serialPortService = new SerialPortService();

            AvailablePorts = new ObservableCollection<string>(_serialPortService.GetAvailablePorts());

            SelectPortCommand = new RelayCommand(param => SelectPort(param));
            DisconnectCommand = new RelayCommand(param => Disconnect());

            IOVM = new IOViewModel(_serialPortService);
            DebugVM = new DebugViewModel();
            LogVM = new LogViewModel();
            ClockVM = new AlarmclockViewModel();
            SeesawVM = new SeesawViewModel();

            _serialPortService.LineReceived += (sender, line) =>
            {
                Application.Current.Dispatcher.Invoke(() => ProcessPacket(line));
            };
        }

        private void SelectPort(object param)
        {
            if (param is string portName)
            {
                try
                {
                    _serialPortService.Close();
                    _serialPortService.PortName = portName;
                    _serialPortService.BaudRate = 9600;
                    _serialPortService.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Fehler beim Verbinden mit {portName}: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Disconnect()
        {
            _serialPortService.Close();
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
            }
            else if (line.StartsWith("d1"))
            {
                ClockVM?.SetHexString(line.Substring(2));
            }
            else if (line.StartsWith("d2")) 
            {
                 SeesawVM?.SetValues(line.Substring(2));
            }
            else if (line.StartsWith("dD"))
            {
                DebugVM?.AddDebugMessage(line.Substring(2));
            }
            else if (line.StartsWith("dL"))
            {
                LogVM?.AddLogMessage(line.Substring(2));
            }
            else if (line.StartsWith("dS"))
            {
                OnSpecialViewRequested(int.Parse(line.Substring(2)));
            }
            else if (line.StartsWith("?01"))
            {
                _serialPortService.WriteLine(IOVM.Model.GetSwitchStateInHex());
            }
            else if (line.StartsWith("?02"))
            {
                _serialPortService.WriteLine(IOVM.Model.GetButtonStateInHex());
            }
            else if (line.StartsWith("?0a"))
            {
                _serialPortService.WriteLine(IOVM.Model.GetScale0InHex());
            }
            else if (line.StartsWith("?0b"))
            {
                _serialPortService.WriteLine(IOVM.Model.GetScale1InHex());
            }
            else if (line.StartsWith("?T"))
            {
                _serialPortService.WriteLine($"dT{DateTime.Now.ToString("yyyyMMddHHmmss")}");
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
                    ActiveSpecialView = ClockVM;
                    break;
                case 2:
                    ActiveSpecialView = SeesawVM;
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
