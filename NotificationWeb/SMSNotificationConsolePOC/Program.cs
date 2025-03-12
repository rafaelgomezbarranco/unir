using Azure.Communication.Sms;

namespace SMSNotificationConsolePOC;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Este código recupera la cadena de conexión de una variable de entorno.
        const string connectionString = "<<connection-string>> ";  // Your connection string
        var smsClient = new SmsClient(connectionString);

        var response = await smsClient.SendAsync(
            from: "<<phone_number_send>>", // The phone number that sends the message
            to: "<<phone_number_receive>>",       // The phone number that receives the message 
            message: """Hola, mundo 👋🏻 a través de SMS"""
        );

        var output = response.Value;

        Console.WriteLine(output);
    }
}