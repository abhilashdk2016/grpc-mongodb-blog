using System;
using System.IO;
using Blog;
using Grpc.Core;

namespace server
{
    class MainClass
    {
        const int PORT = 50052;
        public static void Main(string[] args)
        {
            Server server = null;

            try
            {
                server = new Server()
                {
                    Services = { BlogService.BindService(new BlogServiceImpl()) },
                    Ports = { new ServerPort("localhost", PORT, ServerCredentials.Insecure) }
                };

                server.Start();
                Console.WriteLine("The server is listning on port 50052");
                Console.ReadKey();
            }
            catch (IOException ex)
            {
                Console.WriteLine("The Server failed to start : " + ex.Message);
            }
            finally
            {
                if (server != null)
                    server.ShutdownAsync().Wait();
            }
        }
    }
}
