using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Accord.Math;
using Accord.Math.Transforms;
using Data;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Point = Data.Point;

namespace DFT
{
    public static class Ex
    {
        public static IEnumerable<DrawingData> DFT(this double[] source)
        {
            var N = source.Count();
            for (var k = 0; k < N; k++)
            {
                var re = 0.0;
                var im = 0.0;
                for (var n = 0; n < N; n++)
                {
                    var phi  = (2*Math.PI * k * n) / N;
                    re += source[n] * Math.Cos(phi);
                    im -= source[n] * Math.Sin(phi);
                }

                re = re / N;
                im = im / N;

                var freq = k;
                var amp = Math.Sqrt(re * re + im * im);
                var phase = Math.Atan2(im, re);
                yield return new DrawingData() {Amplitude = amp, Frequency = freq, Phase = phase};
            }
        }


        public static IEnumerable<T> Reduce<T>(this T[] source,int n)
        {
            for (int i = 0; i < source.Length; i+=n)
            {
                yield return source[i];
            }
        }

    
        public static SKPoint ToPoint(this IEnumerable<DrawingData> drawingData,
            double time, double x, double y,double rotation)
        {
            foreach (var item in drawingData)
            {
                x += item.Amplitude * Math.Cos(item.Frequency * time +rotation+ item.Phase);
                y += item.Amplitude * Math.Sin(item.Frequency * time + rotation + item.Phase);
            }
            return new SKPoint((float)x, (float)y);
        }
    }

    public class DrawingData
    {
        public double Amplitude { get; set; }
        public double Frequency { get; set; }
        public double Phase { get; set; }

    }
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            Setup();
        }
        double time = 0;
        private List<SKPoint> _path=new List<SKPoint>();

        public void Setup()
        {
            var points = Drawing.TwitterImagePoints;
            Count = points.Length;
            _complexesX = points.Select(item => item.x).ToArray().DFT().ToList();
            _complexesY = points.Select(item => item.y).ToArray().DFT().ToList();


          
        }

        public int Count { get; set; }
        private IEnumerable<DrawingData> _complexesY;
        private IEnumerable<DrawingData> _complexesX;

        private  async void SKCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            var combinedX= _complexesX.ToPoint(time,500,500,0);
            var combinedY = _complexesY.ToPoint(time, 500, 500,Math.PI/2);

            _path.Insert(0, new SKPoint(combinedX.X,combinedY.Y));

            canvas.DrawPoints(SKPointMode.Polygon, _path.ToArray(), new SKPaint()
            {
                Color = SKColors.Black,
                StrokeWidth = 5,
                StrokeJoin = SKStrokeJoin.Round,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true,
                IsStroke = true
            });
            



            time += (2f * Math.PI) / Count;
            if (time > 2f * Math.PI)
            {
                time = 0;
                _path.Clear();
            } 
            await Task.Delay(1);
            SkCanvasView.InvalidateSurface();
        }          
    }
}
