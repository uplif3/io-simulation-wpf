using System.Windows;
using System.Windows.Controls;

namespace io_simulation_wpf.Controls
{
    public partial class SegmentColon : UserControl
    {
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register(
                nameof(IsOn),
                typeof(bool),
                typeof(SegmentColon),
                new PropertyMetadata(false, OnIsOnChanged));

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set => SetValue(IsOnProperty, value);
        }

        public SegmentColon()
        {
            InitializeComponent();
            UpdateColon(false);
        }

        private static void OnIsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var colon = d as SegmentColon;
            colon?.UpdateColon((bool)e.NewValue);
        }

        private void UpdateColon(bool isOn)
        {
            double opacity = isOn ? 1.0 : 0.1;
            DotTop.Opacity = opacity;
            DotBottom.Opacity = opacity;
        }
    }
}
