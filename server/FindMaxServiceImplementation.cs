using static FindMax.FindMaxService;
using FindMax;
using Grpc.Core;
using System;
using System.Threading.Tasks;

namespace server
{
    public class FindMaxServiceImplementation : FindMaxServiceBase
    {
        public override async Task FindMax(Grpc.Core.IAsyncStreamReader<FindMax.FindMaxRequest> requestStream, Grpc.Core.IServerStreamWriter<FindMax.FindMaxResponse> responseStream, Grpc.Core.ServerCallContext context)
        {
            try
            {
                var max = 0;
                Console.WriteLine($"Received:");
                while (await requestStream.MoveNext())
                {
                    Console.WriteLine($"{requestStream.Current.Number}");

                    if (max < requestStream.Current.Number)
                    {
                        max = requestStream.Current.Number;
                    }
                }

                Console.WriteLine($"Max found: {max}");
                await responseStream.WriteAsync(new FindMaxResponse() { Max = max });
            }
            catch (System.Exception e)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Something went wrong - Exception: {e}"));
            }
        }
    }
}