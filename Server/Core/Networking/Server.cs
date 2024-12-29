using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Game_Maker_Studio_2_Auth_Server.Core.Packets;

namespace Game_Maker_Studio_2_Auth_Server.Core.Networking
{
    internal class Server
    {
        // Initial variables
        public List<SocketHelper> Clients;
        public List<SocketHelper> SearchingClients;
        private CancellationTokenSource _cancellationTokenSource;
        public TcpListener TCPListener = null;
        private const int BufferAlignment = 2;
        private const int BufferSize = 256;

        /// <summary>
        ///  Start the server
        /// </summary>
        public void StartServer(string ip, int port)
        {
            // Clients list
            Clients = new List<SocketHelper>();
            SearchingClients = new List<SocketHelper>();

            // CancellationTokenSource rule for threads
            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            // Running listen for new connections task
            _ = Task.Run(() => Listen(ip, port, token), token);
            Console.WriteLine("System > TCP listener task started.");

            // Running server ping task
            _ = Task.Run(() => Ping(token), token);
            Console.WriteLine("System > Ping packets task started.");
        }

        /// <summary>
        /// Stop the server and cleans all the tasks
        /// </summary>
        public void StopServer()
        {
            _cancellationTokenSource.Cancel();

            if (TCPListener != null) TCPListener.Stop();
            foreach (SocketHelper client in Clients)
            {
                client.gmkClient.GetStream().Close();
                client.gmkClient.Close();
            }

            Clients.Clear();
            Console.WriteLine("System > Server has been stoped and all the tasks has been wiped out.");
        }

        /// <summary>
        /// Pings the game maker client every 1 seconds
        /// </summary>
        private async Task Ping(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(1200, token);

                    BufferStream buffer = new BufferStream(BufferSize, BufferAlignment);
                    buffer.Seek(0);
                    ushort constant_out = 1007;
                    buffer.Write(constant_out);

                    SendToAllClients(buffer);
                }
            }
            catch (TaskCanceledException) { Console.WriteLine("System > Ping task has been aborted."); }
        }

        /// <summary>
        /// Sends a message to all the clients connected
        /// </summary>
        public void SendToAllClients(BufferStream buffer) { foreach (SocketHelper client in Clients) client.SendMessage(buffer); }

        /// <summary>
        /// Start listening for the clients and starts it's threads.
        /// </summary>
        private async Task Listen(string ip, int port, CancellationToken token)
        {
            try
            {
                TCPListener = new TcpListener(IPAddress.Parse(ip), port);
                TCPListener.Start();

                while (!token.IsCancellationRequested)
                {
                    TcpClient clientTCP = await TCPListener.AcceptTcpClientAsync(token);
                    Console.WriteLine("System > Receiving new connection request...");
                    var cli = new SocketHelper();
                    cli.StartClient(clientTCP, this);
                    Clients.Add(cli);
                }
            }
            catch (OperationCanceledException) { Console.WriteLine("System > Listen task was closed"); }
            catch (Exception ex) { Console.WriteLine($"System > Error while listening: {ex.Message}"); }
        }
    }
}
