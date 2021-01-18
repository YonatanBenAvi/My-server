using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;


namespace MyServer
{
   

    public class SynchronousSocketListener
    {

        // Incoming data from the client.  
        public static string data = null;

        public static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and
            // listen for incoming connections.  
            
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                HandleClients(listener);


                /*
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);

                    Console.WriteLine("please enter message: \n");
                    String newData = Console.ReadLine();
                    



                    // Echo the data back to the client.  
                    byte[] msg = Encoding.ASCII.GetBytes(newData);

                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
                */

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void HandleClients(Socket listener)
        {
            bool ServerDone = false;
            while (!ServerDone)
            {
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();
                HandleSingleClient(handler);
            }
        }


        public static void HandleSingleClient(Socket ClientSocket)
        {
            bool ClientDone = false;
            while (!ClientDone)
            {
                Console.WriteLine("Please enter command: ");
                String newData = Console.ReadLine() + " ";

                String msgLen = newData.Length.ToString();
                
                // Echo the data back to the client.  
                byte[] msg = Encoding.ASCII.GetBytes(newData);

                ClientSocket.Send(msg);
                if (newData.Equals("done "))
                {
                    ClientDone = true;
                }
                if (newData.Equals("file "))
                {
                    
                    byte[] bytes = new Byte[1024];
                    Console.WriteLine("ab");
                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = ClientSocket.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            break;
                        }
                    }
                    File.WriteAllText("C:\\Users\\david\\Desktop\\test\\test2.txt", data);
                }
            }
        }


        public static int Main(String[] args)
        {
            //fuck ari stein
            StartListening();
            return 0;
        }
    }
}
