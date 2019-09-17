using System;
using Grpc.Core;
using HelloMessageService;

namespace GrpcLearn.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server()
            {
                Services = { HelloService.BindService(new HelloServiceImp()) },
                Ports = { new ServerPort("localhost", 8888, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Enter any key to stop");
            Console.ReadKey();
            Console.WriteLine("Hello World!");
        }
    }
}
