using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SharpWebServer
{
    /// <summary>
    ///     Simple web server.
    /// </summary>
    public static class Server
    {
        private static HttpListener _listener;

        private static Semaphore sem = new(MaxSimultaneousConnections,
            MaxSimultaneousConnections);

        public static int MaxSimultaneousConnections { get; } = 20;


        /// <summary>
        ///     Returns list of IP addresses assigned to localhost network devices.
        /// </summary>
        /// <returns>List Of IP addresses of localhost network devices.</returns>
        private static List<IPAddress> GetLocalHostIPs()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            return host.AddressList
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToList();
        }

        /// <summary>
        ///     Returns initialized HTTP listener
        /// </summary>
        /// <param name="localhostIpAddresses">List of IP addresses assigned to localhost network devices</param>
        /// <returns>Initialized HTTP listener</returns>
        private static HttpListener InitializeHttpListener(List<IPAddress> localhostIpAddresses)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost/");

            // Listen to IP addresses
            localhostIpAddresses.ForEach(ip =>
            {
                Console.WriteLine("Listening on IP " + "http://" + ip + "/");
                listener.Prefixes.Add("http://" + ip + "/");
            });

            return listener;
        }
    }
}