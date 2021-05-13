using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharpWebServer
{
    /// <summary>
    ///     Simple web server.
    /// </summary>
    public static class Server
    {
        private static readonly int _maxSimultaneousConnections = 20;

        private static readonly Semaphore _semaphore = new(_maxSimultaneousConnections,
            _maxSimultaneousConnections);


        /// <summary>
        ///     Returns list of IP addresses assigned to localhost network devices.
        /// </summary>
        /// <returns>List Of IP addresses of localhost network devices.</returns>
        private static List<IPAddress> GetLocalHostIPs()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            var ret = host.AddressList
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToList();

            return ret;
        }

        /// <summary>
        ///     Returns initialized HTTP listener
        /// </summary>
        /// <param name="localhostIpAddresses">List of IP addresses assigned to localhost network devices</param>
        /// <returns>Initialized HTTP listener</returns>
        private static HttpListener InitializeHttpListener(List<IPAddress> localhostIpAddresses)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:9898/");
            // Listen to IP addresses
            localhostIpAddresses.ForEach(ip =>
            {
                Console.WriteLine("Listening on IP " + "http://" + ip + ":9898" + "/");
                listener.Prefixes.Add("http://" + ip + ":9898" + "/");
            });

            return listener;
        }

        /// <summary>
        ///     Await connections.
        /// </summary>
        /// <param name="listener">Initialized HTTP listener</param>
        private static async void StartConnectionListener(HttpListener listener)
        {
            // Wait for a connection.
            var context = await listener.GetContextAsync();

            // Release the semaphore.
            _semaphore.Release();
            Log(context.Request);

            // Response content
            var response =
                "<html><head><meta http-equiv='content-type' content='text/html; charset=utf-8'/></head>Hello Browser!</html>";
            var encoded = Encoding.UTF8.GetBytes(response);
            context.Response.ContentLength64 = encoded.Length;
            context.Response.OutputStream.Write(encoded, 0, encoded.Length);
            context.Response.OutputStream.Close();
        }


        public static void Log(HttpListenerRequest request)
        {
            Console.WriteLine(request.RemoteEndPoint + " " + request.HttpMethod + "/" +
                              request.Url?.AbsoluteUri);
        }

        /// <summary>
        ///     Start awaiting for connections
        ///     Has to run in separate thread.
        /// </summary>
        /// <param name="listener">Initialized HTTP listener</param>
        private static void RunServer(HttpListener listener)
        {
            while (true)
            {
                _semaphore.WaitOne();
                StartConnectionListener(listener);
            }
        }

        /// <summary>
        ///     Listens to connections
        /// </summary>
        /// <param name="listener" Initialized HTTP listener></param>
        private static void Start(HttpListener listener)
        {
            listener.Start();
            Task.Run(() => RunServer(listener));
        }


        /// <summary>
        ///     Starts the web server.
        /// </summary>
        public static void Start()
        {
            var localhostIpAddresses = GetLocalHostIPs();
            var listener = InitializeHttpListener(localhostIpAddresses);
            Start(listener);
        }
    }
}