using Grpc.Core;
using Helloworld;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RPCScript : MonoBehaviour
{
    /// <summary>
    /// grpc grpc.tools 2.26.0 prorobuf 3.11.2
    /// <ItemGroup><Protobuf Include = "**/*.proto" />
    /// grpc_unity_package.2.27.0-dev.zip
    /// https://packages.grpc.io/archive/2019/12/e522302e33b2420722f866e3de815e4e0a1d9952-219973fd-1007-4db7-a78f-976ec554952d/index.xml
    /// </summary>


    void Start()
    {
        
    }
    public void Send()
    {
        try
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var client = new Greeter.GreeterClient(channel);
            string user = "Addo";

            var reply = client.SayHello(new HelloRequest { Name = user });

          Debug.Log( reply.Message);

            channel.ShutdownAsync().Wait();
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }

    }

    void startServer() {
        Server server = new Server
        {
            Services = { Greeter.BindService(new SayHelloImpl()) },
            Ports = { new ServerPort("localhost", 50051, ServerCredentials.Insecure) }
        };
        server.Start();
        Debug.Log("Server Start On Locolhost:50051");
        Debug.Log("Press any key to stop the server...");
        server.ShutdownAsync().Wait();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Send();
        }
    }
}
class SayHelloImpl : Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply { Message = "Hello " + request.Name });
    }
}