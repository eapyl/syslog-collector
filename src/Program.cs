using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
                try
                {
                    listener.Start();
                    Console.WriteLine("Started TCP Syslog server");
                    while (true)
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        try
                        {
                            using (var bufferedStream = new BufferedStream(client.GetStream()))
                            using (StreamReader streamReader = new StreamReader(bufferedStream))
                            {
                                while (!streamReader.EndOfStream)
                                {
                                    var data = await streamReader.ReadLineAsync();
                                    logger.Information(data + "\n");
                                }
                            }
                        }
                        finally
                        {
                            client.Dispose();
                        }
                    }
                }
                finally
                {
                    listener.Stop();
                }
            }).Wait();
        }
    }
}
