using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace GPSMap.Data
{
    public static class SendCloud2DeviceMsgs
    {
        private static readonly IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static Microsoft.Azure.Devices.IotHubServiceClient serviceClient;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        static string targetDevice = "{device id}";

        private async static Task SendCloudToDeviceMessageAsync(string msg)
        {
            var commandMessage = new
                Message(Encoding.ASCII.GetBytes(msg));
            await serviceClient.Messages.SendAsync(targetDevice, commandMessage);
        }

        public static void Setup()
        {
            string connectionString = config.GetValue<string>("HubConnectionString");
            System.Diagnostics.Debug.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = new IotHubServiceClient(connectionString);
        }

        public static async Task Invoke(string msg)
        {
            System.Diagnostics.Debug.WriteLine("Sending C2D message.");
            await SendCloudToDeviceMessageAsync(msg);
            System.Diagnostics.Debug.WriteLine("CD Message Sent");

        }
    }
}

