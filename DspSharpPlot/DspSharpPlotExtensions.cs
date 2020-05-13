using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using DspSharpPlot.Axes;
using OxyPlot;
using OxyPlot.Series;
using UmtUtilities.DialogProvider;
using UTilities.Collections;
using UTilities.Extensions;

namespace DspSharpPlot
{
    public static class DspSharpPlotExtensions
    {
        public static PlotViewModel Plot(this IReadOnlyList<double> yData, IReadOnlyList<double> xData)
        {
            var plot = CreatePlot();
            Plot(xData, yData, plot);
            return plot;
        }

        public static PlotViewModel CreatePlot(bool fastMode = false)
        {
            var sem = new SemaphoreSlim(0);
            PlotViewModel ret = null;
            var t = new Thread(() => CreatePlotWindow(sem, fastMode, out ret));
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;

            t.Start();
            if (!sem.Wait(5000))
                throw new Exception("Error initializing plot window.");

            return ret;
        }

        private static void CreatePlotWindow(SemaphoreSlim semaphore, bool fastMode, out PlotViewModel plotViewModel)
        {
            var win = new PlotWindow(fastMode);
            plotViewModel = win.Plotter;
            var dispatcher = Dispatcher.CurrentDispatcher;
            dispatcher.InvokeAsync(() => win.Show());
            semaphore.Release();
            Dispatcher.Run();
            win.Close();
            //GC.Collect(2, GCCollectionMode.Forced, true, true);
        }

        public static LineSeries Plot(this IReadOnlyList<double> yData, IReadOnlyList<double> xData, PlotViewModel plot, string displayName = null)
        {
            return Plot(xData.Zip(yData, (x, y) => new DataPoint(x, y)), plot, displayName);
        }

        public static LineSeries Plot(this IEnumerable<DataPoint> dataSource, PlotViewModel plot, string displayName = null)
        {
            var newData = new LineSeriesEx {ItemsSource = dataSource, Title = displayName, CanTrackerInterpolatePoints = false};
            plot.GraphsViewModel.Graphs.Add(newData);
            return newData;
        }

        public static PlotViewModel Plot(this IReadOnlyList<double> xData)
        {
            return xData.Plot(Enumerable.Range(0, xData.Count).Select(i => (double)i).ToReadOnlyList());
        }

        public static void Plot(this IReadOnlyList<double> xData, PlotViewModel plot)
        {
            xData.Plot(Enumerable.Range(0, xData.Count).Select(i => (double)i).ToReadOnlyList(), plot);
        }
    }
}