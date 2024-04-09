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
        // ClientUDP cUDP = new ClientUDP();
        // cUDP.start();
        const string serverAddress = "172.20.10.7"; // server ipaddress
        const int port = 5000; // port to connect to

        //create tcp socket

        //Conn to server

        //send msg to server
        string message = "Hello from the client";
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        Socket clientSocket = null; // Declare the clientSocket variable outside of the if statement

        IPAddress ipAddress;
        if (IPAddress.TryParse(serverAddress, out ipAddress))
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse(serverAddress), port));
        }
        else
        {
            Console.WriteLine("Invalid IP address provided. Please enter a valid IP");
        }

        if (clientSocket != null) // Check if clientSocket is not null before using it
        {
            clientSocket.Send(messageBytes);

            //REceive a response from the server
            byte[] buffer = new byte[1024];
            int bytesReceived = clientSocket.Receive(buffer);

            //convert received bytes to string
            string response = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
            Console.WriteLine("Server says: " + response);
        }

        //close conn
        clientSocket.Close();
    }
}

class ClientUDP
{

    //TODO: implement all necessary logic to create sockets and handle incoming messages
    // Do not put all the logic into one method. Create multiple methods to handle different tasks.
    public void start()
    {

    }

    // TODO: Implement the logic to get the server end point.
    // Replace the following line with your implementation.

    //TODO: create all needed objects for your sockets 

    //TODO: [Send Hello message]

    //TODO: [Receive Welcome]

    //TODO: [Send RequestData]

    //TODO: [Receive Data]

    //TODO: [Send RequestData]

    //TODO: [Send End]

    //TODO: [Handle Errors]


}