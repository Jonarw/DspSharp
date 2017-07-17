using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using DspSharp.Algorithms;

namespace DspSharpPlot.PlotServer
{
    public class PlotClient : ClientBase<IPlotContract>
    {
        public PlotClient() : base(new ServiceEndpoint(ContractDescription.GetContract(typeof(IPlotContract)),
            new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/DspSharp/PlotService")))
        {
        }

        public void Plot(double[] x, double[] y)
        {
            this.Channel.Plot(x, y);
        }
    }

    public static class PlotClientExtensions
    {
        private static PlotClient Client { get; } = new PlotClient();

        public static void Plot(this IEnumerable<double> y, IEnumerable<double> x)
        {
            Client.Plot(x.ToArrayOptimized(), y.ToArrayOptimized());
        }

        public static void Plot(this IEnumerable<double> y)
        {
            var yarray = y.ToArrayOptimized();
            Client.Plot(Enumerable.Range(0, yarray.Length).Select(i => (double)i).ToArray(), yarray);
        }

        public static unsafe void Plot(double* x, double* y, int length)
        {
            var xarray = Unsafe.ToManagedArray(x, length);
            var yarray = Unsafe.ToManagedArray(y, length);
            Client.Plot(xarray, yarray);
        }

        public static unsafe void Plot(double* y, int length)
        {
            var xarray = Enumerable.Range(0, length).Select(i => (double)i).ToArray();
            var yarray = Unsafe.ToManagedArray(y, length);
            Client.Plot(xarray, yarray);
        }
    }
}