using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using io_simulation_wpf.Models;

namespace io_simulation_wpf.ViewModels
{
    public class SeesawViewModel : INotifyPropertyChanged
    {
        private double _reference;
        private double _ball;
        private double _angle;
        private bool _boing;

        public DebugModel Model { get; }

        public SeesawViewModel()
        {
            Model = new DebugModel();
        }

        /// <summary>
        /// Wertebereich: -0.5 .. +0.5 (Meter)
        /// </summary>
        public double Reference
        {
            get => _reference;
            set { _reference = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Wertebereich: -0.6 .. +0.6 (Meter)
        /// </summary>
        public double Ball
        {
            get => _ball;
            set { _ball = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Wertebereich: -15 .. +15 (Grad)
        /// </summary>
        public double Angle
        {
            get => _angle;
            set { _angle = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Boing-Zustand: true/false
        /// </summary>
        public bool Boing
        {
            get => _boing;
            set { _boing = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Parst einen empfangenen Packet-String und setzt die entsprechenden Werte.
        /// 
        /// Das Packet hat folgendes Format (Beispiel):
        ///   - Die ersten 2 Zeichen werden übersprungen.
        ///   - Anschließend folgt ein Hex-String, der mindestens 13 Zeichen lang sein muss.
        /// 
        /// Folgende Berechnungen werden durchgeführt:
        ///   - reference = parseSignedHex(hexStr[0..3]) / 50000.0
        ///   - ball      = parseSignedHex(hexStr[4..7]) / 50000.0
        ///   - angle     = parseSignedHex(hexStr[8..11]) / 2000.0
        ///   - boing     = (hexStr[12] == 't')
        /// </summary>
        /// <param name="packet">Der eingehende Packet-String</param>
        public void SetValues(string packet)
        {
            if (string.IsNullOrWhiteSpace(packet) || packet.Length < 13)
                return;

            // Überspringe die ersten 2 Zeichen und trimme den Rest
            //string dataHex = packet.Substring(2).Trim();

            // Parsing der einzelnen Werte
            int rawReference = ParseSignedHex(packet.Substring(0, 4));
            int rawBall = ParseSignedHex(packet.Substring(4, 4));
            int rawAngle = ParseSignedHex(packet.Substring(8, 4));

            // Setze die Properties entsprechend
            Reference = rawReference / 50000.0;
            Ball = rawBall / 50000.0;
            Angle = rawAngle / 2000.0;

            // Das 13. Zeichen (Index 12) gibt den Boing-Zustand an
            Boing = packet.Length > 12 && packet[12] == 't';
        }

        /// <summary>
        /// Parst einen Hex-String in einen vorzeichenbehafteten Integer.
        /// Falls das höchste Bit (0x8000) gesetzt ist, wird der Wert als negativer Wert interpretiert.
        /// </summary>
        /// <param name="hexStr">Der Hex-String (z.B. "FF12")</param>
        /// <returns>Der vorzeichenbehaftete Integer</returns>
        private int ParseSignedHex(string hexStr)
        {
            // Parse den Hex-String; wir nutzen NumberStyles, damit auch führende Nullen kein Problem sind.
            int num = int.Parse(hexStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

            // Wenn das 16. Bit gesetzt ist, interpretiere den Wert als negativ (Zweierkomplement)
            if ((num & 0x8000) != 0)
            {
                num -= 0x10000;
            }
            return num;
        }

        public void AddDebugMessage(string msg)
        {
            Model.Messages.Add(msg);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
