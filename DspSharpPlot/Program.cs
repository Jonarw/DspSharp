using System;
using System.ServiceModel;
using DspSharpPlot.PlotServer;

namespace DspSharpPlot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceHost = new ServiceHost(typeof(PlotServer.PlotServer), new Uri("net.pipe://localhost/DspSharp"));
            var binding = new NetNamedPipeBinding() {MaxReceivedMessageSize = 50000000};
            serviceHost.AddServiceEndpoint(typeof(IPlotContract), binding, "PlotService");
            serviceHost.Open();

            Console.WriteLine("ServiceHost running. Press Return to Exit");
            foreach (var serviceEndpoint in serviceHost.Description.Endpoints)
            {
                Console.WriteLine(serviceEndpoint.ListenUri.AbsoluteUri);
            }
            Console.ReadLine();
        }
    }
}