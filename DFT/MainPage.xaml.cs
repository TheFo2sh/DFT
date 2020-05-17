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
            var complexes = source.Select(x =>  new Complex(x, 0)).ToArray();
            FourierTransform2.DFT(complexes, FourierTransform.Direction.Backward);
            return complexes;

        }
        public static Complex[][] DFT(this double[][] source)
        {
            List<Complex[]> res = new List<Complex[]>();
            var val = source.Select(item => item[0]).ToArray().DFT();
            var val2 = source.Select(item => item[1]).ToArray().DFT();
            for (int i = 0; i < val.Count(); i++)
            {
                res.Add(new Complex[] { val[i], val2[i] });
            }

            return res.ToArray();
            var complexes=source.Select(item =>item.Select(x=> new Complex(x, 0)).ToArray()).ToArray();
            FourierTransform2.DFT2(complexes,FourierTransform.Direction.Backward);
            
            return complexes;
            
        }


     
        public static SKPoint ToPoint(this List<Complex[]> drawingData,
            double time, double x, double y)
        {
            for (int i = 0; i < drawingData.Count(); i++)
            {
                var item = drawingData[i];
                x += item[0].Magnitude * Math.Cos(i * time + 0 + item[0].Phase);
                y += item[1].Magnitude * Math.Sin(i * time + Math.PI/2 + item[1].Phase);
            }

            return new SKPoint((float) x, (float) y);


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
            var items = points.Select(item => new[] {item.x, item.y}).ToArray();
            _complex = items.DFT().ToList();


          
        }

        public int Count { get; set; }
        private List<Complex[]> _complex;

        private  async void SKCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            var point= _complex.ToPoint(time,500,500);

            _path.Insert(0, point);

            canvas.DrawPoints(SKPointMode.Polygon, _path.ToArray(), new SKPaint()
            {
                Color = SKColors.Black,
                StrokeWidth = 5,
                StrokeJoin = SKStrokeJoin.Round,
                FilterQuality = SKFilterQuality.High,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true,
                HintingLevel = SKPaintHinting.NoHinting,
                Style = SKPaintStyle.Fill,
                DeviceKerningEnabled = true
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
