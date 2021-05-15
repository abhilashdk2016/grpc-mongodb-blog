using System;
using System.Threading.Tasks;
using Blog;
using Grpc.Core;

namespace client
{
    class MainClass
    {
        public async static Task Main(string[] args)
        {
            Channel channel = new Channel("localhost", 50052, ChannelCredentials.Insecure);
            await channel.ConnectAsync().ContinueWith(task =>
            {
                if (task.Status == System.Threading.Tasks.TaskStatus.RanToCompletion)
                {
                    Console.WriteLine("The client connected successfully");
                }
            });

            var client = new BlogService.BlogServiceClient(channel);
            var newBlog = CreateNewBlog(client);
            //ReadBlog(client);
            //UpdateBlog(client, newBlog);
            //DeleteBlog(client, newBlog);
            await ListBlog(client);
            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }

        private static Blog.Blog CreateNewBlog(BlogService.BlogServiceClient client)
        {
            var response = client.CreatBlog(new CreateBlogRequest()
            {
                Blog = new Blog.Blog()
                {
                    AuthorId = "Abhilash",
                    Content = "First Blog",
                    Title = "New First Blog"
                }
            });

            Console.WriteLine("The blog " + response.Blog.Id + " was created");
            return response.Blog;
        }

        private static void ReadBlog(BlogService.BlogServiceClient client)
        {
            try
            {
                var response = client.ReadBlog(new ReadBlogRequest()
                {
                    BlogId = "609f98080201f0caaa80e5"
                });
                Console.WriteLine(response.Blog.ToString());
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);
            }
        }

        private static void UpdateBlog(BlogService.BlogServiceClient client, Blog.Blog blog)
        {
            try
            {
                blog.Title = "Updated Title";
                var response = client.UpdateBlog(new UpdateBlogRequest()
                {
                    Blog = blog
                });
                Console.WriteLine(response.Blog.ToString());
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);
            }
        }

        private static void DeleteBlog(BlogService.BlogServiceClient client, Blog.Blog blog)
        {
            try
            {
                var response = client.DeleteBlog(new DeleteBlogrequest() { BlogId = blog.Id });
                Console.WriteLine("The blog id " + blog.Id + " is deleted");
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);
            }
        }

        private static async Task ListBlog(BlogService.BlogServiceClient client)
        {
            try
            {
                var response = client.ListBlog(new ListBlogRequest() { });
                while(await response.ResponseStream.MoveNext())
                {
                    Console.WriteLine(response.ResponseStream.Current.Blog.ToString());
                }
            }
            catch (RpcException ex)
            {
                Console.WriteLine(ex.Status.Detail);
            }
        }
    }
}
