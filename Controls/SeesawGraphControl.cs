using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace io_simulation_wpf.Controls
{
    public class SeesawGraphControl : Control
    {
        private const int GraphWidth = 600;
        private const int GraphHeight = 150;
        private const int GraphMaxPoints = 200;

        // Speichern die "Y-Werte" bereits skaliert für alle drei Kurven
        private readonly double[] _referenceGraph = new double[GraphMaxPoints];
        private readonly double[] _ballGraph = new double[GraphMaxPoints];
        private readonly double[] _angleGraph = new double[GraphMaxPoints];

        // Ringpuffer: _currentIndex zeigt an, wo als nächstes geschrieben wird
        // _count sagt, wie viele Werte bisher valid sind (max GraphMaxPoints).
        private int _currentIndex = 0;
        private int _count = 0;

        // -------------------------------
        //  Konstruktor und DependencyProps
        // -------------------------------
        public SeesawGraphControl()
        {
            // Optional: Anfangs mit NaN füllen
            for (int i = 0; i < GraphMaxPoints; i++)
            {
                _referenceGraph[i] = double.NaN;
                _ballGraph[i] = double.NaN;
                _angleGraph[i] = double.NaN;
            }
        }

        public double Reference
        {
            get => (double)GetValue(ReferenceProperty);
            set => SetValue(ReferenceProperty, value);
        }
        public static readonly DependencyProperty ReferenceProperty =
            DependencyProperty.Register(nameof(Reference), typeof(double),
                typeof(SeesawGraphControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender,
                    OnGraphDataChanged));

        public double Ball
        {
            get => (double)GetValue(BallProperty);
            set => SetValue(BallProperty, value);
        }
        public static readonly DependencyProperty BallProperty =
            DependencyProperty.Register(nameof(Ball), typeof(double),
                typeof(SeesawGraphControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender,
                    OnGraphDataChanged));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(nameof(Angle), typeof(double),
                typeof(SeesawGraphControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender,
                    OnGraphDataChanged));

        private static void OnGraphDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeesawGraphControl)d).UpdateGraphData();
        }

        // -------------------------------
        //   UpdateGraphData: Ringpuffer befüllen
        // -------------------------------
        public void UpdateGraphData()
        {
            // Beispielhafte Skalierung
            double referenceScaled = (Reference / 0.6) * (GraphHeight / 2);
            double ballScaled = (Ball / 0.6) * (GraphHeight / 2);
            double angleScaled = (Angle / 15.0) * (GraphHeight / 2);

            // In den Puffer schreiben
            _referenceGraph[_currentIndex] = referenceScaled;
            _ballGraph[_currentIndex] = ballScaled;
            _angleGraph[_currentIndex] = angleScaled;

            // Index weiterschieben
            _currentIndex++;
            if (_currentIndex >= GraphMaxPoints)
            {
                _currentIndex = 0;  // Wrap-around
            }

            // Hochzählen bis zum Maximum
            if (_count < GraphMaxPoints)
            {
                _count++;
            }

            // Neu zeichnen
            InvalidateVisual();
        }

        // -------------------------------
        //   Zeichnen
        // -------------------------------
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var graphBase = new Point(10, 10);

            // 1) Hintergrund und Gitter
            DrawBackgroundAndGrid(dc, graphBase);

            // Wenn noch gar keine Daten da sind, nichts malen
            if (_count == 0) return;

            // 2) Kurven zeichnen
            // Wir durchlaufen im Index 0.._count-1 (oder 0..GraphMaxPoints-1,
            // wenn schon voll). Jede Stelle i hat x = graphBase.X + i*xStep.
            // Die Linie wird unterbrochen, sobald wir am "frisch überschriebenen"
            // Index ankommen.
            var refColor = new SolidColorBrush(Color.FromRgb(0xFC, 0xF8, 0x00));
            var ballColor = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
            var angleColor = new SolidColorBrush(Color.FromRgb(0x00, 0x4D, 0xE6));

            // Anzahl tatsächlich zu zeichnender Punkte
            int pointsToDraw = Math.Min(_count, GraphMaxPoints);
            // X-Abstand pro Punkt
            double xStep = (GraphMaxPoints > 1)
                ? (double)GraphWidth / (GraphMaxPoints - 1)
                : GraphWidth;

            // Jede Kurve separat
            DrawRingCurve(dc, _referenceGraph, pointsToDraw, xStep, refColor, graphBase);
            DrawRingCurve(dc, _ballGraph, pointsToDraw, xStep, ballColor, graphBase);
            DrawRingCurve(dc, _angleGraph, pointsToDraw, xStep, angleColor, graphBase);

            // 3) Cursor zeichnen -> an der Stelle _currentIndex
            double cursorX = graphBase.X + _currentIndex * xStep;
            var cursorPen = new Pen(Brushes.Orange, 1);
            dc.DrawLine(cursorPen,
                new Point(cursorX, graphBase.Y),
                new Point(cursorX, graphBase.Y + GraphHeight));
        }

        private void DrawBackgroundAndGrid(DrawingContext dc, Point graphBase)
        {
            // Hintergrund
            dc.DrawRectangle(Brushes.Black, null,
                new Rect(graphBase.X, graphBase.Y, GraphWidth, GraphHeight));

            // Gitterlinien
            var gridPen = new Pen(new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40)), 1);

            // Vertikale Linien alle 50 Pixel
            for (int i = 0; i < GraphWidth; i += 50)
            {
                var p1 = new Point(graphBase.X + i, graphBase.Y);
                var p2 = new Point(graphBase.X + i, graphBase.Y + GraphHeight);
                dc.DrawLine(gridPen, p1, p2);
            }

            // Horizontale Linien alle 30 Pixel
            for (int i = 0; i < GraphHeight; i += 30)
            {
                var p1 = new Point(graphBase.X, graphBase.Y + i);
                var p2 = new Point(graphBase.X + GraphWidth, graphBase.Y + i);
                dc.DrawLine(gridPen, p1, p2);
            }
        }

        /// <summary>
        /// Zeichnet eine Kurve aus dem Ringpuffer in „Index‐Reihenfolge“ 
        /// (0..1..2...), wobei am Übergang (i+1==_currentIndex) unterbrochen wird,
        /// weil dort gerade neu überschrieben wurde.
        /// </summary>
        private void DrawRingCurve(
            DrawingContext dc,
            double[] data,     // der Ringpuffer
            int count,         // so viele Werte sind gültig (bis _count)
            double xStep,
            Brush color,
            Point graphBase)
        {
            var pen = new Pen(color, 2);

            // Bis count - 1, weil wir immer von i zu i+1 zeichnen
            int lastIndex = count - 1;
            for (int i = 0; i < lastIndex; i++)
            {
                int i2 = i + 1;

                // Falls i2 == _currentIndex, bedeutet das: 
                // Position i2 ist die "frisch überschriebenene" Stelle => 
                // Bruch in der Linie, also nicht zeichnen.
                if (i2 == _currentIndex)
                    continue;

                double x1 = graphBase.X + i * xStep;
                double x2 = graphBase.X + i2 * xStep;

                // y = vertikale Mitte - Wert
                double y1 = graphBase.Y + (GraphHeight / 2) - data[i];
                double y2 = graphBase.Y + (GraphHeight / 2) - data[i2];

                // (Nur zeichnen, wenn es keine NaNs sind.)
                if (!double.IsNaN(y1) && !double.IsNaN(y2))
                {
                    dc.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
                }
            }

            // Optional: Wenn der Puffer *voll* ist (_count==GraphMaxPoints),
            // könnte man jetzt noch die Verbindung von "ganz hinten" (Index lastIndex)
            // zu Index 0 zeichnen - aber **nur** dann, wenn 0 != _currentIndex.
            // Oft will man diese Verbindung aber nicht, weil es dort sowieso 
            // einen Sprung gäbe.
        }
    }
}
