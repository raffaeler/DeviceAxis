using System;
using System.Collections.Generic;
using System.Text;
using OxyPlot;
using OxyPlot.Series;
using WitCom;

namespace WitUI.Models
{
    public class GraphData : PlotModel
    {
        private TimeSpan _startTime;
        public GraphData(TimeSpan startTime)
        {
            _startTime = startTime;
            this.Title = "Wit data";
            LinearAccelereation = Create(OxyColor.FromRgb(0xff, 0, 0));
            AngularVelocity = Create(OxyColor.FromRgb(0, 0xff, 0));
            Angle = Create(OxyColor.FromRgb(0, 0, 0xff));

            this.Series.Add(LinearAccelereation);
            this.Series.Add(AngularVelocity);
            this.Series.Add(Angle);

            //this.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));
        }

        public LineSeries LinearAccelereation { get; }
        public LineSeries AngularVelocity { get; }
        public LineSeries Angle { get; }

        public void Push(WitFrame witFrame, DataSelection dataSelection)
        {
            var delta = (_startTime - witFrame.Clock).TotalMilliseconds;
            if (dataSelection == DataSelection.GroupByX)
            {
                LinearAccelereation.Points.Add(new DataPoint(delta, witFrame.LinearAcceleration.X));
                AngularVelocity.Points.Add(new DataPoint(delta, witFrame.AngularVelocity.X));
                Angle.Points.Add(new DataPoint(delta, witFrame.Angle.X));
            }
            else if (dataSelection == DataSelection.GroupByY)
            {
                LinearAccelereation.Points.Add(new DataPoint(delta, witFrame.LinearAcceleration.Y));
                AngularVelocity.Points.Add(new DataPoint(delta, witFrame.AngularVelocity.Y));
                Angle.Points.Add(new DataPoint(delta, witFrame.Angle.Y));
            }
            else if (dataSelection == DataSelection.GroupByZ)
            {
                LinearAccelereation.Points.Add(new DataPoint(delta, witFrame.LinearAcceleration.Z));
                AngularVelocity.Points.Add(new DataPoint(delta, witFrame.AngularVelocity.Z));
                Angle.Points.Add(new DataPoint(delta, witFrame.Angle.Z));
            }
        }

        private LineSeries Create(OxyColor color)
        {
            return new LineSeries()
            {
                LineStyle = LineStyle.Solid,
                Color = color,
            };
        }

    }
}
