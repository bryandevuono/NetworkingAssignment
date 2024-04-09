using System;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using MessageNS;

// Julina Mercera | 1055662
// Bryan de Vuono | 1061043
// Do not modify this class
class Program
{
    static void Main(string[] args)
    {
        // ServerUDP sUDP = new ServerUDP();
        // sUDP.start();
        const int port = 5000; // Port to listen on
                               //TCP socket
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Bind the socket to the port
        listener.Bind(new IPEndPoint(IPAddress.Any, port));
        //listen for incoming connections
        listener.Listen(10);

        Console.WriteLine("Server started on port: " + port);

        //Accept connection
        Socket clientSocket = listener.Accept();

        //Receive date from client
        byte[] buffer = new byte[1024];
        int bytesReceived = clientSocket.Receive(buffer);

        //Convert received bytes to string
        string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
        Console.WriteLine("Client says: " + message);

        //Send a response msg back to client
        string response = "Hello from the server";
        byte[] responseBytes = Encoding.ASCII.GetBytes(response);
        clientSocket.Send(responseBytes);

        //Close the conn
        clientSocket.Close();
        listener.Close();
    }
}

class ServerUDP
{

    //TODO: implement all necessary logic to create sockets and handle incoming messages
    // Do not put all the logic into one method. Create multiple methods to handle different tasks.

    public void start()
    {

    }

    //TODO: create all needed objects for your sockets 

    //TODO: keep receiving messages from clients
    // you can call a dedicated method to handle each received type of messages

    //TODO: [Receive Hello]
    // Which should print hello!
    //TODO: [Send Welcome]

    //TODO: [Receive RequestData]

    //TODO: [Send Data]

    //TODO: [Implement your slow-start algorithm considering the threshold] 

    //TODO: [End sending data to client]

    //TODO: [Handle Errors]

    //TODO: create all needed methods to handle incoming messages


}