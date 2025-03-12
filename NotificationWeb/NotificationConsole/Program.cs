using RestSharp;

namespace WhatsAppNotificationConsolePOC;

public class Program
{
    public static async Task Main(string[] args)
    {
        const string instanceId = "<<instance_id>>"; // Your instance Id
        const string token = "<<instance_token>>";   // Your instance Token
        const string mobile = "<<phone_number>>";    // The phone number that sends the message
        const string message = "Hola, mundo 👋🏻 a través de WhatsApp";
        const string url = "https://api.ultramsg.com/" + instanceId + "/messages/chat";

        var client = new RestClient(url);
        var request = new RestRequest(url, Method.Post);
        request.AddHeader("content-type", "application/x-www-form-urlencoded");
        request.AddParameter("token", token);
        request.AddParameter("to", mobile);
        request.AddParameter("body", message);

        var response = await client.ExecuteAsync(request);
        var output = response.Content;

        Console.WriteLine(output);
    }
}