using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using HelloMessageService;
using System.IO.Pipelines;
using System.IO;

namespace GrpcLearn.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //when host in Grpc.AspNetCore.Server use this;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //when use this,it can work;
            //var channel = GrpcChannel.ForAddress("http://localhost:8888", new GrpcChannelOptions()
            //{
            //    Credentials = ChannelCredentials.Insecure
            //});
            //then host in aspnetcore with https; use this;
            //var channel = GrpcChannel.ForAddress("https://localhost:8888");
            //use this will work when service host in aspnetcore.but can not work when service host in Grpc.Core.Server
            var channel = new Channel("localhost:8888", ChannelCredentials.Insecure);
            var client = new HelloMessageService.HelloService.HelloServiceClient(channel);
            var helloRes = client.SayHello(new HelloMessage.HelloMessageRequest() { Name = "tim lv" });

            var res = await client.SayHelloAsync(new HelloMessage.HelloMessageRequest() { Name = "tim lv" });
            Console.WriteLine(res.Message);

            var call = client.SayHelloStreamVersion();
            var readTask = Task.Run(async () =>
            {
                while(await call.ResponseStream.MoveNext())
                {
                    var message = call.ResponseStream.Current;
                    Console.WriteLine(message);
                }
            });
            while (true)
            {
                var result = Console.ReadKey(intercept: true);
                if (result.Key == ConsoleKey.Escape)
                {
                    break;
                }
                await call.RequestStream.WriteAsync(new HelloMessage.HelloMessageRequest() { Name = "tim lv" });
            }
            await call.RequestStream.CompleteAsync();
            await readTask;
            Console.ReadLine();
        }




        public static async Task SingleReverse(HelloService.HelloServiceClient client)
        {
            var call = client.SayHelloSingleReverse();
            for (int i = 0; i < 50; i++)
            {
                await call.RequestStream.WriteAsync(new HelloMessage.HelloMessageRequest() { Name = "tim lv" + i.ToString() });
            }
            await call.RequestStream.CompleteAsync();
            var msg = await call.ResponseAsync;
            Console.WriteLine(msg);
        }
        public static async Task SingleHello(HelloService.HelloServiceClient client)
        {
            var call = client.SayHelloSingle(new HelloMessage.HelloMessageRequest() { Name = " tim lv" });
            await foreach (var message in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine(message);
            };
            Console.WriteLine("Success");
            Console.ReadLine();
        }

        public static async Task StartStreamHelloAsync(HelloService.HelloServiceClient client)
        {
            var call = client.SayHelloStreamVersion();
            var requestStream = call.RequestStream;
            var responseStream = call.ResponseStream;
            var resTask = ReadResponseAsync(responseStream);
            while (true)
            {
                Console.WriteLine("Enter a message");
                var message = Console.ReadLine();
                if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                await requestStream.WriteAsync(new HelloMessage.HelloMessageRequest() { Name = message });
            }
            await requestStream.CompleteAsync();
            Console.WriteLine("request complete");
            await resTask;
        }

        public static async Task WriteRequestAsync(IClientStreamWriter<HelloMessage.HelloMessageRequest> requestStream)
        {
            while (true)
            {
                await Task.Yield();
                Console.WriteLine("Enter a message");
                var message = Console.ReadLine();
                if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                await requestStream.WriteAsync(new HelloMessage.HelloMessageRequest() { Name = message });
            }
            await requestStream.CompleteAsync();
            Console.WriteLine("request complete");
        }

        public static async Task ReadResponseAsync(IAsyncStreamReader<HelloMessage.HelloMessageResponse> responseStream)
        {
            Console.WriteLine("start recevied message");
            while (await responseStream.MoveNext())
            {
                var message = responseStream.Current;
                Console.WriteLine("recevied message" + message);
                Console.WriteLine(message);
            }
            //await foreach (var message in responseStream.ReadAllAsync())
            //{
            //    Console.WriteLine("recevied message" + message);
            //    Console.WriteLine(message);
            //}
            //Console.WriteLine("complete");
        }
    }
}
