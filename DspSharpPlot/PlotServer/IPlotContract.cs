using System.ServiceModel;

namespace DspSharpPlot.PlotServer
{
    [ServiceContract]
    public interface IPlotContract
    {
        [OperationContract]
        void Plot(double[] x, double[] y);
    }
}