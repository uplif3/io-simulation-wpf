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

        private readonly double[] _referenceGraph = new double[GraphMaxPoints];
        private readonly double[] _ballGraph = new double[GraphMaxPoints];
        private readonly double[] _angleGraph = new double[GraphMaxPoints];

        private int _currentIndex = 0;
        private int _count = 0;

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

        public void UpdateGraphData()
        {
            // Skalierung
            double referenceScaled = (Reference / 0.6) * (GraphHeight / 2);
            double ballScaled = (Ball / 0.6) * (GraphHeight / 2);
            double angleScaled = (Angle / 15.0) * (GraphHeight / 2);

            _referenceGraph[_currentIndex] = referenceScaled;
            _ballGraph[_currentIndex] = ballScaled;
            _angleGraph[_currentIndex] = angleScaled;

            _currentIndex++;
            if (_currentIndex >= GraphMaxPoints)
            {
                _currentIndex = 0; 
            }

            if (_count < GraphMaxPoints)
            {
                _count++;
            }

            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var graphBase = new Point(10, 10);

            DrawBackgroundAndGrid(dc, graphBase);

            if (_count == 0) return;

            var refColor = new SolidColorBrush(Color.FromRgb(0xFC, 0xF8, 0x00));
            var ballColor = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
            var angleColor = new SolidColorBrush(Color.FromRgb(0x00, 0x4D, 0xE6));

            int pointsToDraw = Math.Min(_count, GraphMaxPoints);
            double xStep = (GraphMaxPoints > 1)
                ? (double)GraphWidth / (GraphMaxPoints - 1)
                : GraphWidth;

            DrawRingCurve(dc, _referenceGraph, pointsToDraw, xStep, refColor, graphBase);
            DrawRingCurve(dc, _ballGraph, pointsToDraw, xStep, ballColor, graphBase);
            DrawRingCurve(dc, _angleGraph, pointsToDraw, xStep, angleColor, graphBase);

            double cursorX = graphBase.X + _currentIndex * xStep;
            var cursorPen = new Pen(Brushes.Orange, 1);
            dc.DrawLine(cursorPen,
                new Point(cursorX, graphBase.Y),
                new Point(cursorX, graphBase.Y + GraphHeight));
        }

        private void DrawBackgroundAndGrid(DrawingContext dc, Point graphBase)
        {
            dc.DrawRectangle(Brushes.Black, null,
                new Rect(graphBase.X, graphBase.Y, GraphWidth, GraphHeight));

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

        private void DrawRingCurve(
            DrawingContext dc,
            double[] data,    
            int count,         
            double xStep,
            Brush color,
            Point graphBase)
        {
            var pen = new Pen(color, 2);
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

                double y1 = graphBase.Y + (GraphHeight / 2) - data[i];
                double y2 = graphBase.Y + (GraphHeight / 2) - data[i2];

                if (!double.IsNaN(y1) && !double.IsNaN(y2))
                {
                    dc.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
                }
            }
        }
    }
}
