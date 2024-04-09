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
        ServerUDP sUDP = new ServerUDP();
        sUDP.start();
    }
}

class ServerUDP
{

    //TODO: implement all necessary logic to create sockets and handle incoming messages
    // Do not put all the logic into one method. Create multiple methods to handle different tasks.
    private Socket _socket;

    public void start()
    {
        _socket = CreateSocket();
        BindSocket(_socket);
        ReceiveHelloMessage();
    }

    //TODO: create all needed objects for your sockets 
    private Socket CreateSocket()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        return socket;
    }
    //TODO: keep receiving messages from clients
    // you can call a dedicated method to handle each received type of messages
    private void BindSocket(Socket socket)
    {
        int portNumber = 1234; // Replace 1234 with the actual port number you want to use
        socket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
    }
    //TODO: [Receive Hello]
    private void ReceiveHelloMessage()
    {
        byte[] buffer = new byte[1024];
        int bytesReceived = _socket.Receive(buffer);
        string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);

        if (message.Trim() == "Hello")
        {
            Console.WriteLine("Client says: Hello");
        }
        else
        {
            Console.WriteLine("Unexpected message: " + message);
        }
    }
    // Which should print hello!
    //TODO: [Send Welcome]

    //TODO: [Receive RequestData]

    //TODO: [Send Data]

    //TODO: [Implement your slow-start algorithm considering the threshold] 

    //TODO: [End sending data to client]

    //TODO: [Handle Errors]

    //TODO: create all needed methods to handle incoming messages


}