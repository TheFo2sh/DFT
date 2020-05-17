using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Point = Data.Point;

namespace DFT
{
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
            var points = Drawing.FactoryImagePoints;
            Count = points.Length;
            _complex = points.DFT().ToArray();


          
        }

        public int Count { get; set; }
        private Complex[] _complex;

        private  async void SKCanvasView_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.Black);
            var point= _complex.ToPoint(time,this.Width/2+100,this.Height/2);

            _path.Insert(0, point);

            canvas.DrawPoints(SKPointMode.Polygon, _path.ToArray(), new SKPaint()
            {
                Color = SKColors.White,
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
