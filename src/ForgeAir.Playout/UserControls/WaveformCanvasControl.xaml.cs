using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForgeAir.Playout.UserControls
{
    /// <summary>
    /// Interaction logic for WaveformCanvasControl.xaml
    /// </summary>
    public partial class WaveformCanvasControl : Canvas
    {
        public float[] Waveform
        {
            get => (float[])GetValue(WaveformProperty);
            set => SetValue(WaveformProperty, value);
        }
        public WaveformCanvasControl()
        {
            SizeChanged += (s, e) => Draw();

        }
        public static readonly DependencyProperty WaveformProperty =
            DependencyProperty.Register(nameof(Waveform), typeof(float[]),
                typeof(WaveformCanvasControl),
                new PropertyMetadata(null, OnWaveformChanged));

        private static void OnWaveformChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvas = (WaveformCanvasControl)d;
            canvas.Draw();
        }

        private void Draw()
        {
            Children.Clear();

            if (Waveform == null || Waveform.Length == 0)
                return;

            double width = ActualWidth;
            double height = ActualHeight;

            if (width == 0 || height == 0) return;

            double xScale = width / Waveform.Length;

            for (int i = 0; i < Waveform.Length; i++)
            {
                double y = Waveform[i] * height / 2;

                Line line = new Line
                {
                    X1 = i * xScale,
                    Y1 = height / 2 - y,
                    X2 = i * xScale,
                    Y2 = height / 2 + y,
                    Stroke = Brushes.Lime,
                    StrokeThickness = 1
                };

                Children.Add(line);
            }
        }
    }
}
