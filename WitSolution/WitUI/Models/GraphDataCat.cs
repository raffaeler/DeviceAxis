using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using WitCom;
using WitUI.Configurations;

namespace WitUI.Models
{
    public class GraphDataCat : PlotModel
    {
        private TimeSpan _startTime;
        private int _clipMax;
        public GraphDataCat(TimeSpan startTime, string title,
            ChartConfig chartConfig, CatSelection catSelection)
        {
            _startTime = startTime;
            this.Title = title;
            _clipMax = chartConfig.ClipMax;

            X = new LineSeries()
            {
                Title = "X",
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb(0xff, 0, 0),
            };

            Y = new LineSeries()
            {
                Title = "Y",
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb(0, 0xff, 0),
            };

            Z = new LineSeries()
            {
                Title = "Z",
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb(0, 0, 0xff),
            };


            switch (catSelection)
            {
                case CatSelection.GroupByLinearAcceleration:
                    Axes.Add(new LinearAxis
                    {
                        Position = AxisPosition.Left,
                        Minimum = chartConfig.LinearAccelerationMin,
                        Maximum = chartConfig.LinearAccelerationMax,
                    });
                    break;

                case CatSelection.GroupByAngularVelocity:
                    Axes.Add(new LinearAxis
                    {
                        Position = AxisPosition.Left,
                        Minimum = chartConfig.AngularVelocityMin,
                        Maximum = chartConfig.AngularVelocityMax,
                    });
                    break;

                case CatSelection.GroupByAngle:
                    Axes.Add(new LinearAxis
                    {
                        Position = AxisPosition.Left,
                        Minimum = chartConfig.AngleMin,
                        Maximum = chartConfig.AngleMax,
                    });
                    break;
            }


            this.IsLegendVisible = true;

            this.Series.Add(X);
            this.Series.Add(Y);
            this.Series.Add(Z);

            //this.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
        }

        public LineSeries X { get; }
        public LineSeries Y { get; }
        public LineSeries Z { get; }

        public void Push(WitFrame witFrame, CatSelection catSelection)
        {
            var delta = (witFrame.Clock - _startTime).TotalMilliseconds;
            if (catSelection == CatSelection.GroupByLinearAcceleration)
            {
                X.Points.Add(new DataPoint(delta, witFrame.LinearAcceleration.X));
                Y.Points.Add(new DataPoint(delta, witFrame.LinearAcceleration.Y));
                Z.Points.Add(new DataPoint(delta, witFrame.LinearAcceleration.Z));
            }
            else if (catSelection == CatSelection.GroupByAngularVelocity)
            {
                X.Points.Add(new DataPoint(delta, witFrame.AngularVelocity.X));
                Y.Points.Add(new DataPoint(delta, witFrame.AngularVelocity.Y));
                Z.Points.Add(new DataPoint(delta, witFrame.AngularVelocity.Z));
            }
            else if (catSelection == CatSelection.GroupByAngle)
            {
                X.Points.Add(new DataPoint(delta, witFrame.Angle.X));
                Y.Points.Add(new DataPoint(delta, witFrame.Angle.Y));
                Z.Points.Add(new DataPoint(delta, witFrame.Angle.Z));
            }

            ClipLeft(X.Points, _clipMax);
            ClipLeft(Y.Points, _clipMax);
            ClipLeft(Z.Points, _clipMax);
        }

        public void Update()
        {
            this.InvalidatePlot(true);
        }

        private void ClipLeft(List<DataPoint> points, int max)
        {
            if (points.Count > max) points.RemoveAt(0);
        }

    }
}
