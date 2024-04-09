using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MessageNS;

// SendTo();
class Program
{
    static void Main(string[] args)
    {
        ClientUDP cUDP = new ClientUDP();
        cUDP.start();
    }
}

class ClientUDP
{

    private Socket _socket;
    private IPEndPoint _serverEndPoint;

    //TODO: implement all necessary logic to create sockets and handle incoming messages
    // Do not put all the logic into one method. Create multiple methods to handle different tasks.
    public void start()
    {
        _serverEndPoint = GetServerEndPoint();
        _socket = CreateSocket();
        SendHelloMessage();
    }

    private Socket CreateSocket()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        return socket;
    }

    private IPEndPoint GetServerEndPoint()
    {
        // TODO: Implement the logic to get the server end point.
        // Replace the following line with your implementation.
        return new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);
    }

    //TODO: create all needed objects for your sockets 
    private void SendHelloMessage()
    {
        string message = "Hello";
        byte[] buffer = Encoding.UTF8.GetBytes(message + Environment.NewLine);
        _socket.SendTo(buffer, _serverEndPoint);
    }
    //TODO: [Send Hello message]

    //TODO: [Receive Welcome]

    //TODO: [Send RequestData]

    //TODO: [Receive Data]

    //TODO: [Send RequestData]

    //TODO: [Send End]

    //TODO: [Handle Errors]


}