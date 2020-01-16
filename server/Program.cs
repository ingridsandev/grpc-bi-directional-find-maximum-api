using FindMax;
using Google.Protobuf.Reflection;
using Grpc.Core;
using Grpc.Reflection;
using Grpc.Reflection.V1Alpha;
using System;
using System.Collections.Generic;
using System.IO;

namespace server
{
    class Program
    {
        const int Port = 50052;

        static void Main(string[] args)
        {
            Server server = null;

            try
            {
                server = new Server()
                {
                    Services = 
                    { 
                        FindMaxService.BindService(new FindMaxServiceImplementation()),
                        ServerReflection.BindService(new ReflectionServiceImpl(new List<ServiceDescriptor>() { FindMaxService.Descriptor }))
                    },
                    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
                };

                server.Start();
                Console.WriteLine($"The server is listening on the port: {Port}");
                Console.ReadKey();
            }
            catch (IOException e)
            {
                Console.WriteLine($"Something went wrong - {e.Message}");
                throw;
            }
            finally
            {
                if (server != null)
                    server.ShutdownAsync().Wait();
            }
        }
    }
}
