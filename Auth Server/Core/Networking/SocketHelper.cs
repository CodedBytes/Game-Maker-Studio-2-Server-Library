using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Game_Maker_Studio_2_Auth_Server.Core.Packets;

namespace Game_Maker_Studio_2_Auth_Server.Core.Networking
{
    internal class SocketHelper
    {
        private Queue<BufferStream> BufferWriteQueue = new Queue<BufferStream>();
        private CancellationTokenSource _cancellationTokenSource;
        public TcpClient gmkClient;
        public Server authServer;
        public string ClientIPAddress;
        private const int BufferSize = 256;
        private const int BufferAlign = 2;

        /// <summary>
        /// Starts all client related threads and informations
        /// </summary>
        public void StartClient(TcpClient client, Server server)
        {
            // Starting variables
            gmkClient = client;
            gmkClient.SendBufferSize = BufferSize;
            gmkClient.ReceiveBufferSize = BufferSize;
            authServer = server;
            _cancellationTokenSource = new CancellationTokenSource();

            // Write
            var readThread = new Thread(() => Read(client, _cancellationTokenSource.Token));
            readThread.Start();
            Console.WriteLine("System > Read thread started for the new player");

            // Read
            var writeThread = new Thread(() => Write(client, _cancellationTokenSource.Token));
            writeThread.Start();
            Console.WriteLine("System > Write thread started for the new player");
        }

        /// <summary>
        /// Sends a packet for the game maker client in order, ensuring that every packet is sent to the client.
        /// </summary>
        public void SendMessage(BufferStream buffer) { BufferWriteQueue.Enqueue(buffer); }

        /// <summary>
        /// Disconnects the client and its threads.
        /// </summary>
        public void DisconnectClient()
        {
            // LOG
            Console.WriteLine("System > Disconnecting: " + ClientIPAddress);

            // Removes the client from the server
            authServer.Clients.Remove(this);

            // Closing the data stream
            gmkClient.Close();

            // Request the threads abortion.
            _cancellationTokenSource.Cancel();
            Console.WriteLine("System > All threads for this player is finished.");
        }

        /// <summary>
        /// Writes the packets to game maker client
        /// </summary>
        public void Write(TcpClient client, CancellationToken token)
        {
            var HexPacket = "";
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (BufferWriteQueue.Count > 0)
                    {
                        BufferStream buffer = BufferWriteQueue.Dequeue();
                        var stream = client.GetStream();
                        stream.Write(buffer.Memory, 0, buffer.Iterator);
                        HexPacket = BitConverter.ToString(buffer.Memory, 0, buffer.Iterator).Replace("-", " ");
                        stream.Flush();
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException)
            {
                Console.WriteLine();
                Console.WriteLine($"// -------------------------------------------------------------------- //");
                Console.WriteLine($"// -- System > Client disconnected while sending back a packet.       --//");
                Console.WriteLine($"// ----------------------------- PACKET ------------------------------- //");
                Console.WriteLine($"// --> {HexPacket}");
                Console.WriteLine($"// -------------------------------------------------------------------- //");
                Console.WriteLine();

                // Disconnecting the client
                DisconnectClient();
            }
        }

        /// <summary>
        /// Reads the packet that came from game maker
        /// </summary>
        public void Read(TcpClient client, CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Thread.Sleep(10);
                    BufferStream readBuffer = new BufferStream(BufferSize, BufferAlign);
                    NetworkStream stream = client.GetStream();
                    int bytesRead = stream.Read(readBuffer.Memory, 0, BufferSize);

                    if (bytesRead > 0)
                    {

                        // Process the packet header uint16 id
                        ushort packet_header;
                        readBuffer.Read(out packet_header);

                        // Switching the header
                        switch (packet_header)
                        {
                            case 2000:
                                // New Connection packet handler
                                readBuffer.Read(out string ip);
                                ClientIPAddress = ip;

                                // Hex packet log
                                string ConnHexData = BitConverter.ToString(readBuffer.Memory, 0, bytesRead).Replace("-", " ");
                                Console.WriteLine();
                                Console.WriteLine($"// ----------------------------- PACKET ------------------------------- //");
                                Console.WriteLine($"// --> Packet from {ClientIPAddress}");
                                Console.WriteLine($"// --> {ConnHexData}");
                                Console.WriteLine($"// -------------------------------------------------------------------- //");
                                Console.WriteLine();

                                // -
                                Console.WriteLine($"System > {ip} connected. {authServer.Clients.Count} client(s) online.");
                                break;

                            case 2004:
                                // Ping from client
                                BufferStream buffer = new BufferStream(BufferSize, BufferAlign);
                                buffer.Seek(0);
                                buffer.Write((ushort)1050);
                                SendMessage(buffer);
                                break;

                            case 2005:
                                // Already treated by the client.
                                break;

                            default:
                                // Detects any unknown or lost packet coming from game maker
                                string UnknownHexData = BitConverter.ToString(readBuffer.Memory, 0, bytesRead).Replace("-", " ");
                                Console.WriteLine();
                                Console.WriteLine($"// ------------------------ Unknown PACKET ---------------------------- //");
                                Console.WriteLine($"// --> Packet from {ClientIPAddress}");
                                Console.WriteLine($"// --> Packet id {packet_header}");
                                Console.WriteLine($"// --> {UnknownHexData}");
                                Console.WriteLine($"// -------------------------------------------------------------------- //");
                                Console.WriteLine();
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException)
            {
                // If any exception hits the server, heres the log.
                Console.WriteLine();
                Console.WriteLine($"// ---------------------------- ERROR --------------------------------- //");
                Console.WriteLine($"// -- System > Client {ClientIPAddress} disconnected By Exception: {ex}.");
                Console.WriteLine($"// -------------------------------------------------------------------- //");
                DisconnectClient();
            }
        }
    }
}
