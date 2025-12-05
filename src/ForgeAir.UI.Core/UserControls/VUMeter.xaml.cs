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

        private double CurrentValue = 0;

        public VUMeter()
        {
            InitializeComponent();
            UpdateGradient(0);
            UpdateFillWidth(0);
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
            if (newValue == double.NaN) return;
            var animation = new DoubleAnimation(CurrentValue, newValue, new Duration(TimeSpan.FromMilliseconds(200)))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            animation.CurrentTimeInvalidated += (s, e) =>
            {
                var clock = (AnimationClock)s;
                if (clock.CurrentProgress.HasValue)
                {
                    // Interpolated value
                    double animValue = CurrentValue + (newValue - CurrentValue) * clock.CurrentProgress.Value;

                    UpdateFillWidth(animValue);
                    UpdateGradient(animValue);
                }
            };

            animation.Completed += (s, e) =>
            {
                CurrentValue = newValue;
                UpdateFillWidth(CurrentValue);
                UpdateGradient(CurrentValue);
            };

            this.BeginAnimation(DummyAnimationProperty, animation, HandoffBehavior.SnapshotAndReplace);
        }

        // Dummy DP to host the animation (not used directly)
        public static readonly DependencyProperty DummyAnimationProperty =
            DependencyProperty.Register("DummyAnimation", typeof(double), typeof(VUMeter), new PropertyMetadata(0d));

        private void UpdateFillWidth(double level)
        {
            double maxWidth = this.ActualWidth - 16;
            double fillWidth = (level / MaxLevel) * maxWidth;
            FillBar.Width = fillWidth;
        }


        private void UpdateGradient(double level)
        {
            var brush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };


            // Green zone: 0% - 60%
            brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#32D700"), 0.0));
            brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#32D700"), 0.4));


            // Yellow/Orange zone: 60% - 85%
            brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#E1FF00"), 0.8));


            if (level >= 95)
            {
                // Red zone: 95% - 100%
                brush.GradientStops.Add(new GradientStop(Colors.Red, 0.95));
                brush.GradientStops.Add(new GradientStop(Colors.Red, 1.0));
            }
            else
            {
                // If below 95%, extend OrangeRed to the top so Red is hidden
                brush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#E1FF00"), 1.0));
            }

            FillBar.Fill = brush;
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateFillWidth(CurrentValue);
        }

    }
}
