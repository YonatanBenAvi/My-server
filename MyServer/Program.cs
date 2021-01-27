using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;



namespace MyServer
{
   

    public class SynchronousSocketListener
    {
        static List<Socket> handlers = new List<Socket>();


        // Incoming data from the client.

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


            listener.Bind(localEndPoint);
            listener.Listen(10);

            // Start listening for connections.  
            Thread thread = new Thread(() => HandleClients(listener));
            thread.Start();
            HandleSingleClient();

            /*
            
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
            */

        }

        public static void HandleClients(Socket listener)
        {
           
            bool serverDone = false;
            while (!serverDone)
            {
                //Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();
                handlers.Add(handler);
                Console.WriteLine("conection added");
            }
        }


        public static Boolean HandleSingleClient()
        {
            int clientPosition = 0;
            bool clientDone = false;
            bool serverDone = false;
            while (!clientDone)
            {
                Console.WriteLine("Please enter command: ");
                string newData = Console.ReadLine();

                clientPosition = int.Parse(newData.Substring(0, 1));

                newData = newData.Substring(1, newData.Length - 1);

                string msgLen = newData.Length.ToString().PadLeft(10, '0');
                
                // Echo the data back to the client.  
                byte[] msg = Encoding.ASCII.GetBytes(msgLen + newData);

                handlers[clientPosition].Send(msg);

                String command = newData.Split(' ')[0];
                if (command.Equals("done"))
                {
                    
                    //clientDone = true;
                }else if (command.Equals("quit"))
                {
                    clientDone = true;
                    serverDone = true;
                }
                
                if (command.Equals("send_file"))
                {
                    String filePath = @"C:\Users\shay\Desktop\server\" + newData.Split(' ')[1].Split('\\').Last();
                    Console.WriteLine(filePath);
                    // Create a temporary file, and put some data into it.
                    using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    {
                        byte[] bytes = new Byte[1024];
                        byte[] msgLenBytes = new byte[10];
                        handlers[clientPosition].Receive(msgLenBytes);
                        int sizeToRead = Int32.Parse(Encoding.ASCII.GetString(msgLenBytes));

                        Console.WriteLine("ab");
                        int bytesRec;
                        // An incoming connection needs to be processed.  
                        while (sizeToRead > 0)
                        {
                            if (sizeToRead > 1024)
                            {
                                bytesRec = handlers[clientPosition].Receive(bytes);
                            }
                            else
                            {
                                bytesRec = handlers[clientPosition].Receive(bytes, sizeToRead, 0);
                            }
                            // Add some information to the file.
                            fs.Write(bytes, 0, bytesRec);
                            sizeToRead -= bytesRec;
                        }
                    }
                }

                String clientResponse = ReciveMessageFromClient(handlers[clientPosition]);

                Console.WriteLine(clientResponse);
            }
            handlers[clientPosition].Shutdown(SocketShutdown.Both);
            handlers[clientPosition].Close();
            return serverDone;
            
        }

        public static String ReciveMessageFromClient(Socket sender)
        {
            byte[] size = new byte[10];

            // Receive the response from the remote device.  
            int sizeLen = sender.Receive(size);

            int rcvSize = Int32.Parse(Encoding.ASCII.GetString(size, 0, sizeLen));

            byte[] bytes = new byte[rcvSize];
            int bytesRec = sender.Receive(bytes);

            String msg = Encoding.ASCII.GetString(bytes, 0, bytesRec);
            return msg;
        }



        public static int Main(String[] args)
        {
            //fuck ari stein
            StartListening();
            return 0;
        }
    }
}
