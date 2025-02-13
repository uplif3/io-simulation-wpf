using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace io_simulation_wpf.Controls
{
    public class SeesawGraphControl : Control
    {
        static SeesawGraphControl()
        {
        }

        #region DependencyProperties

        public static readonly DependencyProperty ReferenceProperty =
            DependencyProperty.Register(nameof(Reference), typeof(double), typeof(SeesawGraphControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnGraphDataChanged));

        public static readonly DependencyProperty BallProperty =
            DependencyProperty.Register(nameof(Ball), typeof(double), typeof(SeesawGraphControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnGraphDataChanged));

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(nameof(Angle), typeof(double), typeof(SeesawGraphControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, OnGraphDataChanged));

        private static void OnGraphDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SeesawGraphControl)d).UpdateGraphData();
        }

        public double Reference
        {
            get => (double)GetValue(ReferenceProperty);
            set => SetValue(ReferenceProperty, value);
        }


        public double Ball
        {
            get => (double)GetValue(BallProperty);
            set => SetValue(BallProperty, value);
        }

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        #endregion

        private const int GraphWidth = 600;
        private const int GraphHeight = 150;
        private const int GraphMaxPoints = 100;

        private readonly Queue<Point> _referenceGraph = new();
        private readonly Queue<Point> _ballGraph = new();
        private readonly Queue<Point> _angleGraph = new();
        private int _graphIndex = 0;

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var graphBase = new Point(10, 10);
            DrawGraph(dc, graphBase);
        }

        private void DrawGraph(DrawingContext dc, Point graphBase)
        {
            var gridColor = new SolidColorBrush(Color.FromRgb(0x40, 0x40, 0x40));
            var refColor = new SolidColorBrush(Color.FromRgb(0xFC, 0xF8, 0x00));
            var ballColor = new SolidColorBrush(Color.FromRgb(0xF0, 0xF0, 0xF0));
            var angleColor = new SolidColorBrush(Color.FromRgb(0x00, 0x4D, 0xE6));

            // Hintergrund zeichnen
            dc.DrawRectangle(new SolidColorBrush(Color.FromRgb(0, 0, 0)), null,
                new Rect(graphBase.X, graphBase.Y, GraphWidth, GraphHeight));

            // Gitter zeichnen
            for (int i = 0; i < GraphWidth; i += 50)
            {
                var p1 = new Point(graphBase.X + i, graphBase.Y);
                var p2 = new Point(graphBase.X + i, graphBase.Y + GraphHeight);
                dc.DrawLine(new Pen(gridColor, 1), p1, p2);
            }

            for (int i = 0; i < GraphHeight; i += 30)
            {
                var p1 = new Point(graphBase.X, graphBase.Y + i);
                var p2 = new Point(graphBase.X + GraphWidth, graphBase.Y + i);
                dc.DrawLine(new Pen(gridColor, 1), p1, p2);
            }

            // Zeichne Kurven
            DrawGraphCurve(dc, _referenceGraph, refColor, graphBase);
            DrawGraphCurve(dc, _ballGraph, ballColor, graphBase);
            DrawGraphCurve(dc, _angleGraph, angleColor, graphBase);
        }

        private void DrawGraphCurve(DrawingContext dc, Queue<Point> data, Brush color, Point graphBase)
        {
            if (data.Count < 2) return;

            var pen = new Pen(color, 2);
            var prev = data.Peek();

            foreach (var point in data)
            {
                // Die Punkte werden relativ zur Graph-Höhe verschoben
                var p1 = new Point(graphBase.X + prev.X * (GraphWidth / GraphMaxPoints), graphBase.Y + (GraphHeight / 2) - prev.Y);
                var p2 = new Point(graphBase.X + point.X * (GraphWidth / GraphMaxPoints), graphBase.Y + (GraphHeight / 2) - point.Y);

                dc.DrawLine(pen, p1, p2);
                prev = point;
            }
        }


        public void UpdateGraphData()
        {
            if (_graphIndex >= GraphMaxPoints)
            {
                _graphIndex = 0; // 🔄 Zurücksetzen, wenn der Graph voll ist
            }

            // Skalierung der Werte für das Zeichnen
            double referenceScaled = (Reference / 0.6) * (GraphHeight / 2);
            double ballScaled = (Ball / 0.6) * (GraphHeight / 2);
            double angleScaled = (Angle / 15.0) * (GraphHeight / 2);

            // Neuen Punkt hinzufügen
            _referenceGraph.Enqueue(new Point(_graphIndex, referenceScaled));
            _ballGraph.Enqueue(new Point(_graphIndex, ballScaled));
            _angleGraph.Enqueue(new Point(_graphIndex, angleScaled));

            // Alte Werte entfernen, damit die Queue nicht zu lang wird
            if (_referenceGraph.Count > GraphMaxPoints) _referenceGraph.Dequeue();
            if (_ballGraph.Count > GraphMaxPoints) _ballGraph.Dequeue();
            if (_angleGraph.Count > GraphMaxPoints) _angleGraph.Dequeue();

            _graphIndex++; // Weiterzählen

            // 🔄 Erzwinge Neuzeichnen
            InvalidateVisual();
        }

    }
}
