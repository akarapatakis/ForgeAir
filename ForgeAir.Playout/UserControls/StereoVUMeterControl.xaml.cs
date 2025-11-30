using System.Windows;
using System.Windows.Controls;

namespace ForgeAir.Playout.UserControls
{
    public partial class StereoVUMeterControl : UserControl
    {
        public StereoVUMeterControl()
        {
            InitializeComponent();
        }

        public double LeftVUMeter
        {
            get => (double)GetValue(LeftVUMeterProperty);
            set => SetValue(LeftVUMeterProperty, value);
        }

        public static readonly DependencyProperty LeftVUMeterProperty =
            DependencyProperty.Register(nameof(LeftVUMeter), typeof(double), typeof(StereoVUMeterControl),
                new PropertyMetadata(0.0, OnLeftVUMeterChanged));

        private static void OnLeftVUMeterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StereoVUMeterControl control)
                control.leftVU.Level = (double)e.NewValue;
        }

        public double RightVUMeter
        {
            get => (double)GetValue(RightVUMeterProperty);
            set => SetValue(RightVUMeterProperty, value);
        }

        public static readonly DependencyProperty RightVUMeterProperty =
            DependencyProperty.Register(nameof(RightVUMeter), typeof(double), typeof(StereoVUMeterControl),
                new PropertyMetadata(0.0, OnRightVUMeterChanged));

        private static void OnRightVUMeterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StereoVUMeterControl control)
                control.rightVU.Level = (double)e.NewValue;
        }
    }
}
