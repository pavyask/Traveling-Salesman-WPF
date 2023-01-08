using H.Pipes;
using H.Pipes.Args;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace TSP_Shared.Pipes
{
    public class Client
    {
        public Action<PipeMessage> OnMessageReceived;

        private PipeClient<PipeMessage> _client;

        private readonly object _solutionCounterLocker = new object();

        public long SolutionCount { get; private set; }

        private readonly string _pipeName;


        public Client(string pipeName)
        {
            SolutionCount = 0;
            _pipeName = pipeName;
            Task.Run(async () => await StartClient());
        }

        public async Task StartClient()
        {
            await using var clientTemp = new PipeClient<PipeMessage>(_pipeName);
            _client = clientTemp;
            _client.MessageReceived += (o, args) => OnMessageReceived.Invoke(args.Message); ;
            _client.Connected += (o, args) => Console.WriteLine("Connected to server");
            _client.Disconnected += (o, args) => Console.WriteLine("Disconnected from server");

            await clientTemp.ConnectAsync();
            await Task.Delay(Timeout.InfiniteTimeSpan);
        }

        public void SendMessage(PipeMessage message)
        {
            _client.WriteAsync(message);
        }

        public void IncrementSolutionCounter()
        {
            lock (_solutionCounterLocker)
            {
                SolutionCount++;
            }
        }
    }
}
