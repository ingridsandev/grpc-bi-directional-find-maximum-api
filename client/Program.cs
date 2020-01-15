using Grpc.Core;
using System;
using System.Threading.Tasks;
using FindMax;

namespace client
{
    class Program
    {
        const string target = "127.0.0.1:50052";
        const string esc = "esc";

        static async Task Main(string[] args)
        {
            try
            {
                Channel channel = new Channel(target, ChannelCredentials.Insecure);

                await channel.ConnectAsync().ContinueWith((task) =>
                {
                    if (task.Status == TaskStatus.RanToCompletion)
                        Console.WriteLine("The client connected successfully");
                });

                var client = new FindMaxService.FindMaxServiceClient(channel);

                var stream = client.FindMax();

                var responseReaderTask = Task.Run(async () => 
                {
                    while (await stream.ResponseStream.MoveNext())
                    {
                        Console.WriteLine($"Max: {stream.ResponseStream.Current.Max}");
                    }
                });

                string key;
        
                do
                {
                    Console.WriteLine(Environment.NewLine + "Digit a number or type 'ESC' to exit");
                    key = Console.ReadLine();

                    var isNumeric = int.TryParse(key, out int number);

                    if (key.ToLower() != esc && isNumeric)
                    {
                        Console.WriteLine($"Sending: {number}");
                        await stream.RequestStream.WriteAsync(new FindMaxRequest()
                        {
                            Number = number
                        });
                    }

                } while (key.ToLower() != esc);

                await stream.RequestStream.CompleteAsync();
                await responseReaderTask;

                channel.ShutdownAsync().Wait();
                Console.ReadLine();
            }
            catch (RpcException e)
            {
                Console.WriteLine($"StatusCode: {e.Status.StatusCode} | Detail: {e.Status.Detail}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong.");
            }
        }
    }
}
