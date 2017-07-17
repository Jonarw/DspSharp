namespace DspSharpPlot.PlotServer
{
    public class PlotServer : IPlotContract
    {
        public void Plot(double[] x, double[] y)
        {
             DspSharpPlotExtensions.Plot(y, x);
        }
    }
}