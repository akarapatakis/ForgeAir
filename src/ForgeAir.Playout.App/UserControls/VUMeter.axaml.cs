using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace ForgeAir.Playout.App.UserControls;

public partial class VUMeter : UserControl
{
    private const double MaxLevel = 100.0;

    private Border _fillBar;

    public VUMeter()
    {
        _fillBar = this.FindControl<Border>("FillBar");
    }

    // ===== AvaloniaProperty =====
    public double Level
    {
        get => GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    public static readonly StyledProperty<double> LevelProperty =
        AvaloniaProperty.Register<VUMeter, double>(nameof(Level));

    private static void OnLevelChanged(AvaloniaObject sender, bool before)
    {
        if (before) return;

        var control = (VUMeter)sender;
        control.UpdateFill(control.Level);
    }

    private double _currentValue = 0;

    private void UpdateFill(double newValue)
    {
        newValue = Math.Clamp(newValue, 0, MaxLevel);

        double targetWidth = (newValue / MaxLevel) * Math.Max(0, Bounds.Width - 8);

        // Animate width
        var transition = new DoubleTransition
        {
            Property = WidthProperty,
            Duration = TimeSpan.FromMilliseconds(180),
            Easing = new QuadraticEaseOut()
        };

        _fillBar.Transitions = new Transitions { transition };
        _fillBar.Width = targetWidth;

        _currentValue = newValue;
    }

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        UpdateFill(_currentValue); // recalc on resize
    }
}