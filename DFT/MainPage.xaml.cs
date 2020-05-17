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
        public static Complex[] DFT(this double[] source)
        {
            var complexes=source.Select(x => new Complex(x, 0)).ToArray();
            FourierTransform2.DFT(complexes,FourierTransform.Direction.Backward);
            return complexes;
            
        }


        public static SKPoint ToPoint(this Complex item,int freq, double time, double x, double y, double rotation)
        {
            x += item.Magnitude * Math.Cos(freq * time + rotation + item.Phase);
            y += item.Magnitude * Math.Sin(freq * time + rotation + item.Phase);
            return new SKPoint((float)x, (float)y);

        }
        public static SKPoint ToPoint(this IEnumerable<Complex> drawingData,
            double time, float x, float y,double rotation)
        {
           return drawingData.Select((complex, index) => (complex, index)).Aggregate(new SKPoint(x,y),
                (acumulitave, item) => item.complex.ToPoint(item.index, time, acumulitave.X, acumulitave.Y, rotation));
           
        }
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
        private List<Complex> _complexesY;
        private List<Complex> _complexesX;

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
