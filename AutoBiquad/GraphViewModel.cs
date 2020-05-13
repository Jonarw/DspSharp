using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using UmtUtilities.ViewModel;

namespace AutoBiquad
{
    public class GraphViewModel : ViewModelBase<LineSeries>
    {
        public GraphViewModel(IReadOnlyList<double> x, IReadOnlyList<double> y, string title) : base(CreateGraph(x, y))
        {
            this.X = x;
            this.Y = y;
            this.Model.Title = title;
        }

        public IReadOnlyList<double> X { get; }
        public IReadOnlyList<double> Y { get; }

        public static LineSeries CreateGraph(IEnumerable<double> x, IEnumerable<double> y)
        {
            var ret = new LineSeries();
            ret.Points.AddRange(x.Zip(y, (d, d1) => new DataPoint(d, d1)));
            return ret;
        }
    }
}