using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace io_simulation_wpf.ViewModels
{
    public class AlarmclockViewModel : INotifyPropertyChanged
    {
        // Das empfangene Rohdaten-String (z. B. "3f064f6d")
        private string _raw = "";
        public string Raw
        {
            get => _raw;
            set
            {
                if (_raw != value)
                {
                    _raw = value;
                    OnPropertyChanged();
                    UpdateProperties();
                }
            }
        }

        // Anzeigeeigenschaften für die Ziffern
        private string _hoursTens = "";
        public string HoursTens
        {
            get => _hoursTens;
            set { _hoursTens = value; OnPropertyChanged(); }
        }

        private string _hoursOnes = "";
        public string HoursOnes
        {
            get => _hoursOnes;
            set { _hoursOnes = value; OnPropertyChanged(); }
        }

        private string _minutesTens = "";
        public string MinutesTens
        {
            get => _minutesTens;
            set { _minutesTens = value; OnPropertyChanged(); }
        }

        private string _minutesOnes = "";
        public string MinutesOnes
        {
            get => _minutesOnes;
            set { _minutesOnes = value; OnPropertyChanged(); }
        }

        // Anzeigeeigenschaften für die LED-Indikatoren
        private Visibility _alarmVisibility = Visibility.Collapsed;
        public Visibility AlarmVisibility
        {
            get => _alarmVisibility;
            set { _alarmVisibility = value; OnPropertyChanged(); }
        }

        private Visibility _beepVisibility = Visibility.Collapsed;
        public Visibility BeepVisibility
        {
            get => _beepVisibility;
            set { _beepVisibility = value; OnPropertyChanged(); }
        }

        // Der Doppelpunkt soll blinken – hierfür eine Eigenschaft
        private Visibility _colonVisibility = Visibility.Visible;
        public Visibility ColonVisibility
        {
            get => _colonVisibility;
            set { _colonVisibility = value; OnPropertyChanged(); }
        }

        // Mapping von 2-stelligen Hex-Zeichenfolgen zu Ziffern
        private readonly Dictionary<string, string> segMap = new Dictionary<string, string>
        {
            { "3f", "0" },
            { "06", "1" },
            { "5b", "2" },
            { "4f", "3" },
            { "66", "4" },
            { "6d", "5" },
            { "7d", "6" },
            { "07", "7" },
            { "7f", "8" },
            { "6f", "9" }
        };

        public AlarmclockViewModel()
        {
            // Optional: Simuliere empfangene Daten (zum Testen)
            // Raw = "3f064f6d"; // Beispiel: zeigt 00:00 an, passe entsprechend an!

            // Starte einen Timer, um den Doppelpunkt blinken zu lassen (alle 0,5 Sekunden umschalten)
            DispatcherTimer blinkTimer = new DispatcherTimer();
            blinkTimer.Interval = TimeSpan.FromSeconds(0.5);
            blinkTimer.Tick += (s, e) =>
            {
                // Wenn der Doppelpunkt aktuell sichtbar ist, ausblenden und umgekehrt.
                ColonVisibility = (ColonVisibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            };
            blinkTimer.Start();
        }

        /// <summary>
        /// Dekodiert die Rohdaten und aktualisiert die Anzeigeeigenschaften.
        /// Das Protokoll:
        /// - Der Hex-String besteht aus 8 Zeichen (4 Bytes).
        /// - Byte3: Stunden Zehner (Bit 7 nicht verwendet)
        /// - Byte2: Stunden Einer (Bit 7 enthält LED Alarm)
        /// - Byte1: Minuten Zehner (Bit 7 enthält Doppelpunktinfo)
        /// - Byte0: Minuten Einer (Bit 7 enthält LED Beep)
        /// </summary>
        private void UpdateProperties()
        {
            // Überprüfen, ob der Rohdatenstring mindestens 8 Zeichen hat.
            if (string.IsNullOrEmpty(Raw) || Raw.Length < 8)
            {
                HoursTens = HoursOnes = MinutesTens = MinutesOnes = "";
                AlarmVisibility = BeepVisibility = Visibility.Collapsed;
                return;
            }

            try
            {
                // Zerlege den Hex-String in 4 Bytes
                string raw = Raw;
                byte byte3 = Convert.ToByte(raw.Substring(0, 2), 16);
                byte byte2 = Convert.ToByte(raw.Substring(2, 2), 16);
                byte byte1 = Convert.ToByte(raw.Substring(4, 2), 16);
                byte byte0 = Convert.ToByte(raw.Substring(6, 2), 16);

                // Hilfsmethode: Entferne das MSB (Bit 7) und dekodiere die Ziffer anhand der segMap
                string DecodeDigit(byte b)
                {
                    byte value = (byte)(b & 0x7F); // Bit 7 ausblenden
                    string hexStr = value.ToString("x2"); // 2-stelliger Hex-String in Kleinbuchstaben
                    return segMap.TryGetValue(hexStr, out string digit) ? digit : "?";
                }

                // Ziffern extrahieren
                HoursTens = DecodeDigit(byte3);
                HoursOnes = DecodeDigit(byte2);
                MinutesTens = DecodeDigit(byte1);
                MinutesOnes = DecodeDigit(byte0);

                // LED-Indikatoren aus Bit 7 extrahieren
                bool alarmActive = (byte2 & 0x80) != 0;
                bool beepActive = (byte0 & 0x80) != 0;
                bool colonActive = (byte1 & 0x80) != 0;

                AlarmVisibility = alarmActive ? Visibility.Visible : Visibility.Collapsed;
                BeepVisibility = beepActive ? Visibility.Visible : Visibility.Collapsed;

                // Optional: Falls der Doppelpunkt nicht aktiv sein soll, kannst du ihn ausblenden.
                // (Hier übernimmt der Blinktimer die Steuerung; wenn colonActive false ist, kannst du ihn auch dauerhaft ausblenden.)
                if (!colonActive)
                {
                    ColonVisibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                // Bei Fehlern die Anzeigen leeren
                HoursTens = HoursOnes = MinutesTens = MinutesOnes = "";
                AlarmVisibility = BeepVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Methode, um von außen einen Hex-String zu übergeben.
        /// Das setzt den Rohdaten-String und aktualisiert damit die Anzeige.
        /// </summary>
        /// <param name="hexData">Der Hex-String (z. B. "3f064f6d")</param>
        public void SetHexString(string hexData)
        {
            Raw = hexData;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
