using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace io_simulation_wpf.Controls
{
    public class SeesawControl : Control
    {
        static SeesawControl()
        {
        }

        #region DependencyProperties

        public static readonly DependencyProperty ReferenceProperty =
            DependencyProperty.Register(
                nameof(Reference),
                typeof(double),
                typeof(SeesawControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Reference
        {
            get => (double)GetValue(ReferenceProperty);
            set => SetValue(ReferenceProperty, value);
        }

        public static readonly DependencyProperty BallProperty =
            DependencyProperty.Register(
                nameof(Ball),
                typeof(double),
                typeof(SeesawControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Ball
        {
            get => (double)GetValue(BallProperty);
            set => SetValue(BallProperty, value);
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(
                nameof(Angle),
                typeof(double),
                typeof(SeesawControl),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Angle
        {
            get => (double)GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public static readonly DependencyProperty BoingProperty =
            DependencyProperty.Register(
                nameof(Boing),
                typeof(bool),
                typeof(SeesawControl),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool Boing
        {
            get => (bool)GetValue(BoingProperty);
            set => SetValue(BoingProperty, value);
        }

        #endregion

        public SeesawControl()
        {
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double width = ActualWidth;
            double height = ActualHeight;
            double marginBottom = 10.0; // Abstand zum unteren Rand

            var basePoint = new Point(width / 2, height - marginBottom);
            DrawSeesaw(dc, basePoint, width, height);
        }

        private void DrawSeesaw(DrawingContext dc, Point basePt, double ctrlWidth, double ctrlHeight)
        {
            var colorStand = Color.FromRgb(0x60, 0x60, 0x60);
            var colorRamp = Color.FromRgb(0x00, 0x4D, 0xE6);
            var colorBall = Color.FromRgb(0xF0, 0xF0, 0xF0);
            var colorRef = Color.FromRgb(0xFC, 0xF8, 0x00);
            var colorBoingOn = Color.FromRgb(0xCC, 0x00, 0x00);
            var colorBoingOff = Color.FromRgb(0x60, 0x60, 0x60);

            double seesawWidth = 420.0;
            double standWidth = 36.0;
            double standHeight = 70.0;
            double rBall = 9.0;
            double markerWidth = 6.0;
            double markerHeight = 11.0;

            //STAND
            var p1 = ToCanvas(basePt, -standWidth / 2, 0);
            var p2 = ToCanvas(basePt, 0, standHeight);
            var p3 = ToCanvas(basePt, standWidth / 2, 0);

            var standFigure = new PathFigure { StartPoint = p1 };
            standFigure.Segments.Add(new LineSegment(p2, true));
            standFigure.Segments.Add(new LineSegment(p3, true));
            standFigure.IsClosed = true;

            var standGeometry = new PathGeometry(new[] { standFigure });
            dc.DrawGeometry(new SolidColorBrush(colorStand), null, standGeometry);

            //REFERENCE MARKER
            double markerRawX = Reference / 0.6 * ((seesawWidth / 2) - 1.5 * rBall);
            double markerRawY = 180.0 - 1.0;

            var m1 = ToCanvas(basePt, markerRawX - markerWidth, markerRawY);
            var m2 = ToCanvas(basePt, markerRawX + markerWidth, markerRawY);
            var m3 = ToCanvas(basePt, markerRawX, markerRawY - markerHeight);

            var markerFigure = new PathFigure { StartPoint = m1 };
            markerFigure.Segments.Add(new LineSegment(m2, true));
            markerFigure.Segments.Add(new LineSegment(m3, true));
            markerFigure.IsClosed = true;

            var markerGeometry = new PathGeometry(new[] { markerFigure });
            dc.DrawGeometry(new SolidColorBrush(colorRef), null, markerGeometry);

            //RAMP-LINE
            double angleRad = Angle * Math.PI / 180.0;
            double tanAngle = Math.Tan(angleRad);

            double seesawPivotY = 90.0;
            double seesawHalfWidth = seesawWidth / 2.0;
            double yOffset = tanAngle * seesawHalfWidth;

            double yLeft = seesawPivotY + yOffset;
            double yRight = seesawPivotY - yOffset;

            // Wippen-Endpunkte setzen
            var rampP1 = ToCanvas(basePt, -seesawHalfWidth, yLeft);
            var rampP2 = ToCanvas(basePt, seesawHalfWidth, yRight);

            // Linie zeichnen
            var rampPen = new Pen(new SolidColorBrush(colorRamp), 3.0);
            dc.DrawLine(rampPen, rampP1, rampP2);


            //BALL
            double ballX = Ball / 0.6 * seesawHalfWidth;
            double ballY = seesawPivotY - tanAngle * ballX + rBall;

            var ballCenter = ToCanvas(basePt, ballX, ballY);
            dc.DrawEllipse(new SolidColorBrush(colorBall), null, ballCenter, rBall, rBall);

            //BOING
            var boingColor = Boing ? colorBoingOn : colorBoingOff;
            var text = new FormattedText(
                "Boing!",
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                14.0,
                new SolidColorBrush(boingColor),
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            var boingPos = ToCanvas(basePt, -(seesawWidth / 2) - 40, 10);
            dc.DrawText(text, boingPos);
        }

        private Point ToCanvas(Point basePt, double relX, double relY)
        {
            double x = basePt.X + relX;
            double y = basePt.Y - relY;
            return new Point(x, y);
        }
    }
}
