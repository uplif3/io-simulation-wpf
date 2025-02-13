using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace io_simulation_wpf.ViewModels
{
    public class AlarmclockViewModel : INotifyPropertyChanged
    {
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

        private char _hoursTens = '0';
        public char HoursTens
        {
            get => _hoursTens;
            set { _hoursTens = value; OnPropertyChanged(); }
        }

        private char _hoursOnes = '0';
        public char HoursOnes
        {
            get => _hoursOnes;
            set { _hoursOnes = value; OnPropertyChanged(); }
        }

        private char _minutesTens = '0';
        public char MinutesTens
        {
            get => _minutesTens;
            set { _minutesTens = value; OnPropertyChanged(); }
        }

        private char _minutesOnes = '0';
        public char MinutesOnes
        {
            get => _minutesOnes;
            set { _minutesOnes = value; OnPropertyChanged(); }
        }

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

        private readonly Dictionary<string, char> segMap = new Dictionary<string, char>
        {
            { "3f", '0' },
            { "06", '1' },
            { "5b", '2' },
            { "4f", '3' },
            { "66", '4' },
            { "6d", '5' },
            { "7d", '6' },
            { "07", '7' },
            { "7f", '8' },
            { "6f", '9' }
        };

        public AlarmclockViewModel()
        {

        }

        /// <summary>
        /// Zerlegt den Hex-String (4 Bytes) und aktualisiert die Ziffern sowie LED-Indikatoren.
        /// Byte3: Stunden-Zehner (Bit 7 ignoriert)
        /// Byte2: Stunden-Einer (Bit 7 = ALARM)
        /// Byte1: Minuten-Zehner (Bit 7 = Doppelpunkt; hier nicht weiter verwendet)
        /// Byte0: Minuten-Einer (Bit 7 = BEEP)
        /// </summary>
        private void UpdateProperties()
        {
            if (string.IsNullOrEmpty(Raw) || Raw.Length < 8)
            {
                HoursTens = HoursOnes = MinutesTens = MinutesOnes = ' ';
                AlarmVisibility = BeepVisibility = Visibility.Collapsed;
                return;
            }

            try
            {
                string raw = Raw;
                byte byte3 = Convert.ToByte(raw.Substring(0, 2), 16);
                byte byte2 = Convert.ToByte(raw.Substring(2, 2), 16);
                byte byte1 = Convert.ToByte(raw.Substring(4, 2), 16);
                byte byte0 = Convert.ToByte(raw.Substring(6, 2), 16);

                char DecodeDigit(byte b)
                {
                    byte value = (byte)(b & 0x7F); // Bit 7 entfernen
                    string hexStr = value.ToString("x2");
                    return segMap.TryGetValue(hexStr, out char digit) ? digit : '?';
                }

                HoursTens = DecodeDigit(byte3);
                HoursOnes = DecodeDigit(byte2);
                MinutesTens = DecodeDigit(byte1);
                MinutesOnes = DecodeDigit(byte0);

                // LED-Indikatoren: Bit 7 von Byte2 = ALARM, Bit 7 von Byte0 = BEEP
                bool alarmActive = (byte2 & 0x80) != 0;
                bool beepActive = (byte0 & 0x80) != 0;
                AlarmVisibility = alarmActive ? Visibility.Visible : Visibility.Collapsed;
                BeepVisibility = beepActive ? Visibility.Visible : Visibility.Collapsed;

                bool colonActive = (byte1 & 0x80) != 0;
                IsColonOn = colonActive;
            }
            catch (Exception)
            {
                HoursTens = HoursOnes = MinutesTens = MinutesOnes = ' ';
                AlarmVisibility = BeepVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Öffentliche Methode, um einen Hex-String zu übergeben und die Anzeige zu aktualisieren.
        /// </summary>
        /// <param name="hexData">Der Hex-String (z. B. "3f064f6d")</param>
        public void SetHexString(string hexData)
        {
            Raw = hexData;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isColonOn;
        public bool IsColonOn
        {
            get => _isColonOn;
            set { _isColonOn = value; OnPropertyChanged(); }
        }
    }
}
