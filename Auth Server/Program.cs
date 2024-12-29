using System;
using Game_Maker_Studio_2_Auth_Server.Core.IniReader;
using Game_Maker_Studio_2_Auth_Server.Core.Networking;

namespace Game_Maker_Studio_2_Auth_Server
{
    internal class Program
    {
        // Variables
        private static bool isRunning = false;
        private static IniFileReader iniReader = new("config.ini");

        static void Main(string[] args)
        {
            // Initial log
            Console.WriteLine("//-----------------------------------------------------------------//");
            Console.WriteLine("// ------- Servidor Game Maker Studio 2 2024.X - 2025.X -----------//");
            Console.WriteLine("//-----------------------------------------------------------------//");
            Console.WriteLine("//- Server developed to make online games development using game  -//");
            Console.WriteLine("//- maker studio 2 much more easier.                              -//");
            Console.WriteLine("//- The main point of this server, is to provide an stable and    -//");
            Console.WriteLine("//- easy way to handle public networking between game maker       -//");
            Console.WriteLine("//- client and a dedicated server, with the lowest delay possible -//");
            Console.WriteLine("//-----------------------------------------------------------------//");
            Console.WriteLine("//- Copyrights to CodedBytes and FatalSheep.                      -//");
            Console.WriteLine("//- Major Changes to TCP Socket and Packet System: CodedBytes     -//");
            Console.WriteLine("//- Code changes to fit .NET8.0 Framework: CodedBytes             -//");
            Console.WriteLine("//-----------------------------------------------------------------//");
            Console.WriteLine("//- Type /Credits for full crediting.                             -//");
            Console.WriteLine("//-----------------------------------------------------------------//");
            Console.WriteLine();
            Console.WriteLine();

            // Starting the server
            Console.WriteLine("System > Auth server initiated.");
            var server = new Server();

            try
            {
                server.StartServer(iniReader.GetValue("Configs", "server_ip"), UInt16.Parse(iniReader.GetValue("Configs", "server_port")));
                isRunning = true;


                // Command system
                string input;
                do
                {
                    input = Console.ReadLine()?.ToLower();
                    switch (input)
                    {
                        case "/status":
                            Console.WriteLine($"System > Connected Clients: {server.Clients.Count}");
                            break;

                        case "/stop":
                            if (isRunning)
                            {
                                server.StopServer();
                                isRunning = false;
                                Console.WriteLine("System > Server has been stoped");
                            }
                            else Console.WriteLine("System > Server already stoped. try starting the server again by using the command /start.");
                            break;

                        case "/start":
                            if (!isRunning)
                            {
                                server.StartServer(iniReader.GetValue("Configs", "server_ip"), UInt16.Parse(iniReader.GetValue("Configs", "server_port")));
                                isRunning = true;
                                Console.WriteLine("System > Server started.");
                            }
                            else Console.WriteLine("System > Server is already running. Try to stop him first before starting again.");
                            break;

                        case "/clear":
                            Console.Clear();
                            Console.WriteLine("System > Console cleaned.");
                            break;

                        case "/exit":
                            Console.WriteLine("System > Finishing the server...");
                            server.StopServer();
                            break;

                        default:
                            if (!string.IsNullOrWhiteSpace(input)) Console.WriteLine($"System > Unknown Command: {input}");
                            break;
                    }
                } while (input != "/exit");

                Console.WriteLine("System > Press Enter to exit.");
                Console.ReadKey();
            }
            catch (Exception ex) { Console.WriteLine($"System > Critical error: {ex.Message}"); }
            finally { Console.WriteLine("System > Server finished."); }
        }
    }
}
