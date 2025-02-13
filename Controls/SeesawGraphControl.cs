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
        private const int GraphMaxPoints = 100;

        // Hier speichern wir die SCALED-Y-Werte, index = x-Position
        private readonly double[] _referenceGraph = new double[GraphMaxPoints];
        private readonly double[] _ballGraph = new double[GraphMaxPoints];
        private readonly double[] _angleGraph = new double[GraphMaxPoints];

        // Index, an den als Nächstes geschrieben wird
        private int _currentIndex = 0;
        private int _count = 0;

        // Ab hier deine DependencyProperties
        // -----------------------------------

        public SeesawGraphControl()
        {
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

        // -----------------------------------

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var graphBase = new Point(10, 10);

            // 1) Hintergrund und Gitter zeichnen
            DrawBackgroundAndGrid(dc, graphBase);

            // 2) Kurven zeichnen
            var refColor = new SolidColorBrush(Color.FromRgb(0xFC, 0xF8, 0x00));
            var ballColor = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
            var angleColor = new SolidColorBrush(Color.FromRgb(0x00, 0x4D, 0xE6));

            DrawGraphCurve(dc, _referenceGraph, refColor, graphBase);
            DrawGraphCurve(dc, _ballGraph, ballColor, graphBase);
            DrawGraphCurve(dc, _angleGraph, angleColor, graphBase);

            // 3) Cursor zeichnen
            DrawCursor(dc, graphBase);
        }

        private void DrawBackgroundAndGrid(DrawingContext dc, Point graphBase)
        {
            // Hintergrund
            dc.DrawRectangle(Brushes.Black, null,
                new Rect(graphBase.X, graphBase.Y, GraphWidth, GraphHeight));

            // Gitter
            var gridPen = new Pen(new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40)), 1);
            for (int i = 0; i < GraphWidth; i += 50)
            {
                var p1 = new Point(graphBase.X + i, graphBase.Y);
                var p2 = new Point(graphBase.X + i, graphBase.Y + GraphHeight);
                dc.DrawLine(gridPen, p1, p2);
            }
            for (int i = 0; i < GraphHeight; i += 30)
            {
                var p1 = new Point(graphBase.X, graphBase.Y + i);
                var p2 = new Point(graphBase.X + GraphWidth, graphBase.Y + i);
                dc.DrawLine(gridPen, p1, p2);
            }
        }

        private void DrawGraphCurve(DrawingContext dc, double[] data, Brush color, Point graphBase)
        {
            var pen = new Pen(color, 2);

            // X-Schritt = gesamte Breite / (Anzahl Punkte - 1)
            double xStep = (GraphMaxPoints > 1)
                ? (double)GraphWidth / (GraphMaxPoints - 1)
                : GraphWidth;

            // Wir zeichnen Linien von i zu i+1
            for (int i = 0; i < GraphMaxPoints - 1; i++)
            {
                int i2 = i + 1;

                // Wenn wir am Wrap-Punkt sind: Keine Linie ziehen
                // z.B. wenn i == _currentIndex und i2 = _currentIndex+1 -> bedeutet "frisch überschrieben"
                if (i == _currentIndex && i2 == (_currentIndex + 1) % GraphMaxPoints)
                {
                    // optional: so kannst du "Linie unterbrechen" beim Neustart
                    continue;
                }

                // X-Koordinaten
                double x1 = graphBase.X + i * xStep;
                double x2 = graphBase.X + i2 * xStep;

                // Y-Koordinaten (Mitte = graphBase.Y + GraphHeight/2)
                double y1 = graphBase.Y + (GraphHeight / 2) - data[i];
                double y2 = graphBase.Y + (GraphHeight / 2) - data[i2];

                dc.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
            }
        }

        private void DrawCursor(DrawingContext dc, Point graphBase)
        {
            // Cursor soll genau an _currentIndex stehen
            double xStep = (GraphMaxPoints > 1)
                ? (double)GraphWidth / (GraphMaxPoints - 1)
                : GraphWidth;

            double cursorX = graphBase.X + _currentIndex * xStep;

            var cursorPen = new Pen(Brushes.Orange, 1);
            dc.DrawLine(cursorPen,
                        new Point(cursorX, graphBase.Y),
                        new Point(cursorX, graphBase.Y + GraphHeight));
        }

        public void UpdateGraphData()
        {
            // Skalieren
            double referenceScaled = (Reference / 0.6) * (GraphHeight / 2);
            double ballScaled = (Ball / 0.6) * (GraphHeight / 2);
            double angleScaled = (Angle / 15.0) * (GraphHeight / 2);

            // Schreiben
            _referenceGraph[_currentIndex] = referenceScaled;
            _ballGraph[_currentIndex] = ballScaled;
            _angleGraph[_currentIndex] = angleScaled;

            // Index weiter
            _currentIndex++;
            if (_currentIndex >= GraphMaxPoints)
            {
                _currentIndex = 0;
            }

            // Hochzählen, wie viele Werte wir schon haben, aber nicht über Max
            _count = Math.Min(_count + 1, GraphMaxPoints);

            InvalidateVisual();
        }

    }

}
