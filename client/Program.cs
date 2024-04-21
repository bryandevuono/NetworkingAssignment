using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MessageNS;

class Program
{
    static void Main(string[] args)
    {
        ClientUDP udpClient = new ClientUDP();
        udpClient.Start();
    }
}
class ClientUDP
{
    private const string ServerAddress = "127.0.0.1"; // Server IP address
    private const int ServerPort = 32000;
    private UdpClient udpClient;
    //TODO: implement all necessary logic to create sockets and handle incoming messages
    // Do not put all the logic into one method. Create multiple methods to handle different tasks.
    public void Start()
    {
        Message helloMessage = new Message();
        helloMessage.Type = MessageType.Hello;
        helloMessage.Content = "Hello from the client";
        SendMessage(helloMessage);
        Message Request = new Message();
        Request.Type = MessageType.RequestData;
        Request.Content = "hamlet.txt";
        SendMessage(Request);
    }
    //TODO: create all needed objects for your sockets 


    //TODO: [Send Hello message]
    private void SendMessage(Message message)
    {
        udpClient = new UdpClient();

        try
        {
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(ServerAddress), ServerPort);
            byte[] data = Serialize(message);
    
            
            udpClient.Send(data, data.Length, serverEndpoint);

            IPEndPoint receiveEndpoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receivedData = udpClient.Receive(ref receiveEndpoint);
            Message receivedMessage = Deserialize(receivedData);

            if (receivedMessage.Type == MessageType.Welcome)
            {
                Console.WriteLine($"Received from server ({receiveEndpoint}): {receivedMessage.Content}");
            }
            if(receivedMessage.Type == MessageType.Data)
            {
                Console.WriteLine($"Received from server ({receiveEndpoint}): {receivedMessage.Content}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        finally
        {
            if (udpClient != null)
            {
                udpClient.Close();
            }
        }
    }

    static byte[] Serialize(Message message)
    {
        string messageString = $"{message.Type}|{message.Content}";
        return Encoding.UTF8.GetBytes(messageString);
    }

    static Message Deserialize(byte[] data)
    {
        try
        {
            string messageString = Encoding.UTF8.GetString(data);
            string[] parts = messageString.Split('|');

            if (parts.Length != 2 || !Enum.TryParse(parts[0], out MessageType messageType))
            {
                throw new ArgumentException("Invalid message format");
            }

            Message decodedMessage = new Message();
            decodedMessage.Type = messageType;
            decodedMessage.Content = parts[1];

            return decodedMessage;
        }   
        catch (Exception ex)
        {
            throw new ArgumentException("Error decoding message", ex);
        }
    }
}
    //TODO: [Receive Welcome]

    //TODO: [Send RequestData]

    //TODO: [Receive Data]

    //TODO: [Send RequestData]

    //TODO: [Send End]

    //TODO: [Handle Errors]

