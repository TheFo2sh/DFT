using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Accord.Math;
using Accord.Math.Transforms;
using Data;
using SkiaSharp;

namespace DFT
{
    public static class Ex
    {
       
        public static Complex[] DFT(this IEnumerable<Point> source)
        {
            var complex = source.Select(item => new Complex(item.x, item.y)).ToArray();
            FourierTransform2.DFT(complex, FourierTransform.Direction.Backward);
            return complex;

        }


     
        public static SKPoint ToPoint(this IEnumerable<Complex> drawingData,
            double time, double x, double y)
        {
            for (int i = 0; i < drawingData.Count(); i++)
            {
                var item = drawingData.ElementAt(i);
                x += item.Magnitude * Math.Cos(i * time + item.Phase);
                y += item.Magnitude * Math.Sin(i * time +  item.Phase);
            }

            return new SKPoint((float)x,(float)y);


        }
    }
}