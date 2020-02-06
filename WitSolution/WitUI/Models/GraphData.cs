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
    public class GraphData : PlotModel
    {
        private TimeSpan _startTime;
        private int _clipMax;
        public GraphData(TimeSpan startTime, string title,
            ChartConfig chartConfig)
        {
            _startTime = startTime;
            this.Title = title;
            _clipMax = chartConfig.ClipMax;

            LinearAcceleration = new LineSeries()
            {
                Title = "LinearAccelereation",
                YAxisKey = "LinearAccelereation",
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb(0xff, 0, 0),
            };

            AngularVelocity = new LineSeries()
            {
                Title = "AngularVelocity",
                YAxisKey = "AngularVelocity",
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb(0, 0xff, 0),
            };

            Angle = new LineSeries()
            {
                Title = "Angle",
                YAxisKey = "Angle",
                LineStyle = LineStyle.Solid,
                Color = OxyColor.FromRgb(0, 0, 0xff),
            };


            Axes.Add(new LinearAxis
            {
                Key= "LinearAccelereation",
                Position = AxisPosition.Left,
                Minimum = chartConfig.LinearAccelerationMin,
                Maximum = chartConfig.LinearAccelerationMax,
            });


            Axes.Add(new LinearAxis
            {
                Key= "AngularVelocity",
                Position = AxisPosition.Left,
                Minimum = chartConfig.AngularVelocityMin,
                Maximum = chartConfig.AngularVelocityMax,
            });

            Axes.Add(new LinearAxis
            {
                Key= "Angle",
                Position = AxisPosition.Left,
                Minimum = chartConfig.AngleMin,
                Maximum = chartConfig.AngleMax,
            });

            //Axes.Add(new LinearAxis
            //{
            //    Position = AxisPosition.Left,
            //    Minimum = Math.Min(Math.Min(chartConfig.LinearAccelerationMin, chartConfig.AngularVelocityMin), chartConfig.AngleMin),
            //    Maximum = Math.Max(Math.Max(chartConfig.LinearAccelerationMax, chartConfig.AngularVelocityMax), chartConfig.AngleMax),
            //});

            this.IsLegendVisible = true;


            this.Series.Add(LinearAcceleration);
            this.Series.Add(AngularVelocity);
            this.Series.Add(Angle);

            //this.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
        }

        public LineSeries LinearAcceleration { get; }
        public LineSeries AngularVelocity { get; }
        public LineSeries Angle { get; }

        public void Push(WitFrame witFrame, DataSelection dataSelection)
        {
            var delta = (witFrame.Clock - _startTime).TotalMilliseconds;
            if (dataSelection == DataSelection.GroupByX)
            {
                LinearAcceleration.Points.Add(new DataPoint(delta, witFrame.LinearAcceleration.X));
                AngularVelocity.Points.Add(new DataPoint(delta, witFrame.AngularVelocity.X));
                Angle.Points.Add(new DataPoint(delta, witFrame.Angle.X));
            }
            else if (dataSelection == DataSelection.GroupByY)
            {
                LinearAcceleration.Points.Add(new DataPoint(delta, witFrame.LinearAcceleration.Y));
                AngularVelocity.Points.Add(new DataPoint(delta, witFrame.AngularVelocity.Y));
                Angle.Points.Add(new DataPoint(delta, witFrame.Angle.Y));
            }
            else if (dataSelection == DataSelection.GroupByZ)
            {
                LinearAcceleration.Points.Add(new DataPoint(delta, witFrame.LinearAcceleration.Z));
                AngularVelocity.Points.Add(new DataPoint(delta, witFrame.AngularVelocity.Z));
                Angle.Points.Add(new DataPoint(delta, witFrame.Angle.Z));
            }

            ClipLeft(LinearAcceleration.Points, _clipMax);
            ClipLeft(AngularVelocity.Points, _clipMax);
            ClipLeft(Angle.Points, _clipMax);
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
