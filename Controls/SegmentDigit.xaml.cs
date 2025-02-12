using System.Windows;
using System.Windows.Controls;

namespace io_simulation_wpf.Controls
{
    public partial class SegmentDigit : UserControl
    {
        public static readonly DependencyProperty DigitProperty =
            DependencyProperty.Register(
                nameof(Digit),
                typeof(string),
                typeof(SegmentDigit),
                new PropertyMetadata("0", OnDigitChanged));

        public string Digit
        {
            get => (string)GetValue(DigitProperty);
            set => SetValue(DigitProperty, value);
        }

        public SegmentDigit()
        {
            InitializeComponent();
            UpdateSegments("0");
        }

        private static void OnDigitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SegmentDigit;
            control?.UpdateSegments(e.NewValue as string);
        }

        /// <summary>
        /// Aktiviert/Deaktiviert die Segmente je nach Ziffer.
        /// </summary>
        private void UpdateSegments(string? newDigit)
        {
            // Falls unbekannte Ziffer, machen wir z.B. alle Segmente dunkel.
            if (string.IsNullOrEmpty(newDigit))
                newDigit = "?";

            bool segA = false, segB = false, segC = false,
                 segD = false, segE = false, segF = false, segG = false;

            switch (newDigit)
            {
                case "0":
                    segA = segB = segC = segD = segE = segF = true;
                    segG = false;
                    break;
                case "1":
                    segB = segC = true;
                    break;
                case "2":
                    segA = segB = segD = segE = segG = true;
                    break;
                case "3":
                    segA = segB = segC = segD = segG = true;
                    break;
                case "4":
                    segB = segC = segF = segG = true;
                    break;
                case "5":
                    segA = segC = segD = segF = segG = true;
                    break;
                case "6":
                    segA = segC = segD = segE = segF = segG = true;
                    break;
                case "7":
                    segA = segB = segC = true;
                    break;
                case "8":
                    segA = segB = segC = segD = segE = segF = segG = true;
                    break;
                case "9":
                    segA = segB = segC = segD = segF = segG = true;
                    break;
                default:
                    // "?" -> Alle aus oder du färbst alle rot als "Fehler"
                    segA = segB = segC = segD = segE = segF = segG = false;
                    break;
            }

            // Segmente an- oder abschalten
            SegA.Opacity = segA ? 1 : 0.1;
            SegB.Opacity = segB ? 1 : 0.1;
            SegC.Opacity = segC ? 1 : 0.1;
            SegD.Opacity = segD ? 1 : 0.1;
            SegE.Opacity = segE ? 1 : 0.1;
            SegF.Opacity = segF ? 1 : 0.1;
            SegG.Opacity = segG ? 1 : 0.1;
        }
    }
}
