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

    public void Start()
    {
        try
        {
            udpClient = new UdpClient();
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(ServerAddress), ServerPort);

            SendMessage(MessageType.Hello, "Hello from the client");

            SendMessage(MessageType.RequestData, "hamlet.txt");

            ReceiveMessages();
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

    private void SendMessage(MessageType messageType, string content)
    {
        Message message = new Message { Type = messageType, Content = content };
        byte[] data = Serialize(message);
        udpClient.Send(data, data.Length, ServerAddress, ServerPort);
    }

    private void ReceiveMessages()
    {
        try
        {
            while (true)
            {
                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpClient.Receive(ref serverEndpoint);
                Message receivedMessage = Deserialize(receivedData);

                if (receivedMessage.Type == MessageType.Welcome)
                {
                    Console.WriteLine($"Received from server ({serverEndpoint}): {receivedMessage.Content}");
                }
                else if (receivedMessage.Type == MessageType.Data)
                {
                    string base64String = receivedMessage.Content;
                    base64String = base64String.PadRight(base64String.Length + (4 - base64String.Length % 4) % 4, '=');
                    byte[] fileData = Convert.FromBase64String(base64String);
                    string fileContent = Encoding.UTF8.GetString(fileData);
                    Console.WriteLine("File received successfully. Content:");
                    Console.WriteLine(fileContent);
                }
                else if (receivedMessage.Type == MessageType.End)
                {
                    Console.WriteLine("End of file transmission.");
                    break;
                }
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException occurred: {ex.Message}");
        }
    }

    private static byte[] Serialize(Message message)
    {
        string messageString = $"{message.Type}|{message.Content}";
        return Encoding.UTF8.GetBytes(messageString);
    }

    private static Message Deserialize(byte[] data)
    {
        try
        {
            string messageString = Encoding.UTF8.GetString(data);
            string[] parts = messageString.Split('|');

            if (parts.Length != 2 || !Enum.TryParse(parts[0], out MessageType messageType))
            {
                throw new ArgumentException("Invalid message format");
            }

            return new Message { Type = messageType, Content = parts[1] };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error decoding message", ex);
        }
    }
}
