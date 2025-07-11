using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ForgeAir.UI.Core.Controls
{
    public partial class VUMeter : UserControl
    {
        private const double MaxLevel = 100.0;

        private double _currentValue = 0;

        public VUMeter()
        {
            InitializeComponent();
            UpdateGradient(0);
            UpdateFillHeight(0);
        }

        public double Level
        {
            get { return (double)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        // DependencyProperty for binding Level
        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(double), typeof(VUMeter),
                new PropertyMetadata(0d, OnLevelChanged));

        private static void OnLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (VUMeter)d;
            double newValue = (double)e.NewValue;

            // Clamp to 0..100
            if (newValue < 0) newValue = 0;
            if (newValue > MaxLevel) newValue = MaxLevel;

            control.AnimateLevelChange(newValue);
        }

        private void AnimateLevelChange(double newValue)
        {
            // Animate the height and update gradient stops based on newValue

            // Animate _currentValue to newValue smoothly over 200 ms
            var animation = new DoubleAnimation(_currentValue, newValue, new Duration(TimeSpan.FromMilliseconds(200)))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            animation.CurrentTimeInvalidated += (s, e) =>
            {
                var clock = (AnimationClock)s;
                if (clock.CurrentProgress.HasValue)
                {
                    // Interpolated value
                    double animValue = _currentValue + (newValue - _currentValue) * clock.CurrentProgress.Value;

                    UpdateFillHeight(animValue);
                    UpdateGradient(animValue);
                }
            };

            animation.Completed += (s, e) =>
            {
                _currentValue = newValue;
                UpdateFillHeight(_currentValue);
                UpdateGradient(_currentValue);
            };

            this.BeginAnimation(DummyAnimationProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        // Dummy DP to host the animation (not used directly)
        public static readonly DependencyProperty DummyAnimationProperty =
            DependencyProperty.Register("DummyAnimation", typeof(double), typeof(VUMeter), new PropertyMetadata(0d));

        private void UpdateFillHeight(double level)
        {
            // Height of FillBar = proportional to level (0..100) of UserControl height minus margin
            double maxHeight = this.ActualHeight - 16; // 8 top + 8 bottom margin approx

            if (maxHeight < 0)
                maxHeight = 0;

            double fillHeight = (level / MaxLevel) * maxHeight;

            FillBar.Height = fillHeight;
        }

        private void UpdateGradient(double level)
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 1),
                EndPoint = new Point(0, 0)
            };

            // Green zone: 0% - 60%
            brush.GradientStops.Add(new GradientStop(Colors.Green, 0.0));
            brush.GradientStops.Add(new GradientStop(Colors.Green, 0.6));

            // Yellow/Orange zone: 60% - 85%
            brush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.6));
            brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFC107"), 0.75)); // Amber-ish
            brush.GradientStops.Add(new GradientStop(Colors.Orange, 0.85));

            // Orange/Red-ish zone: 85% - 95%
            brush.GradientStops.Add(new GradientStop(Colors.OrangeRed, 0.85));
            brush.GradientStops.Add(new GradientStop(Colors.OrangeRed, 0.95));

            if (level >= 95)
            {
                // Red zone: 95% - 100%
                brush.GradientStops.Add(new GradientStop(Colors.Red, 0.95));
                brush.GradientStops.Add(new GradientStop(Colors.Red, 1.0));
            }
            else
            {
                // If below 95%, extend OrangeRed to the top so Red is hidden
                brush.GradientStops.Add(new GradientStop(Colors.OrangeRed, 1.0));
            }

            FillBar.Fill = brush;
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateFillHeight(_currentValue);
        }
    }
}
