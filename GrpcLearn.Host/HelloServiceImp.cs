using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using HelloMessage;
using HelloMessageService;

namespace GrpcLearn.Host
{
    public class HelloServiceImp:HelloService.HelloServiceBase
    {
        public override Task<HelloMessageResponse> SayHello(HelloMessageRequest request, ServerCallContext context)
        {
            var res = new HelloMessageResponse()
            {
                Message = request.Name + "Hello"
            };
            return Task.FromResult(res);
        }
        public override async Task<HelloMessageResponse> SayHelloSingleReverse(IAsyncStreamReader<HelloMessageRequest> requestStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext())
            {
                var message = requestStream.Current;
                Console.WriteLine("Recevied Message" + message);
            }
            return new HelloMessageResponse() { Message = "Complete recevied" };
        }
        public override async Task SayHelloSingle(HelloMessageRequest request, IServerStreamWriter<HelloMessageResponse> responseStream, ServerCallContext context)
        {
            int sendCount = 0;
            while(true)
            {
                if(sendCount > 50)
                {
                    break;
                }
                await responseStream.WriteAsync(new HelloMessageResponse() { Message = request.Name + "\t" + sendCount.ToString() });
                await Task.Delay(1000);
                sendCount++;
            }
        }
        public override async Task SayHelloStreamVersion(IAsyncStreamReader<HelloMessageRequest> requestStream, IServerStreamWriter<HelloMessageResponse> responseStream, ServerCallContext context)
        {
            var connectReply = new HelloMessageResponse() { Message = "Connect success" };
            await responseStream.WriteAsync(connectReply);
            try
            {
                while (await requestStream.MoveNext())
                {
                    var  requestMessage = requestStream.Current;
                    Console.WriteLine("Recevied Message" + requestMessage.Name);
                    var message = "Hello\t" + requestMessage.Name;
                    var reply = new HelloMessageResponse()
                    {
                        Message = message
                    };
                    await responseStream.WriteAsync(reply);
                    Console.WriteLine("send message" + message);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("close connection");
            }
        }
    }
}
