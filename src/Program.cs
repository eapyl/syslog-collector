using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace LogCollector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var logger = new LoggerConfiguration()
                    .WriteTo.RollingFile("logs/log-{Date}.txt", outputTemplate: "{Message}")
                    .CreateLogger();

                TcpListener listener = new TcpListener(IPAddress.Any, 5000);
                listener.Start();
                Console.WriteLine("Started");
                while (true)
                {
                    var client = await listener.AcceptTcpClientAsync();

                    var stream = client.GetStream();
                    using (var bufferedStream = new BufferedStream(stream))
                    using (StreamReader streamReader = new StreamReader(bufferedStream))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            var data = await streamReader.ReadLineAsync();
                            logger.Verbose(data+"\n");
                        }
                    }
                    stream.Dispose();
                }
            }).Wait();
        }
    }
}
