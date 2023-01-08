﻿// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using TSP_Shared.Algorithms;
using TSP_Shared.Models;
using TSP_Shared.Pipes;

Console.WriteLine("TSP-Thread");


CitiesSolution bestSolution = null;
object solutionLock = new object();
object progressLock = new object();
double maxDurationInMilisecs = 123;
Stopwatch watcher = new Stopwatch();
double lastTotalDuration = 0;
int FIRST_ITERATIONS = 10000;
int SECOND_ITERATIONS = 10000;
int howMany;
Client client = new Client("tsp");
var threads = new List<Thread>();


client.OnMessageReceived += (PipeMessage message) =>
{
    Console.WriteLine("Received message: " + message.Type.ToString());

    if (message.Type == MessageType.STOP)
    {
        watcher.Stop();
        watcher.Reset();
        Console.WriteLine("Cancelled");

        foreach (var thread in threads)
        {
            thread.Interrupt();
        }
        client.SendMessage(new PipeMessage { Type = MessageType.PROGRESS, Progress = maxDurationInMilisecs, SolutionCount = client.SolutionCount });
        //Environment.Exit(0);
    }
    else if (message.Type == MessageType.START)
    {
        Console.WriteLine($"Running {message.HowMany} threads");
        howMany = message.HowMany;
        maxDurationInMilisecs = message.MaxDurationInMilisecs;
        for (int i = 0; i < message.HowMany; i++)
        {
            try
            {
                Thread t = new Thread(new ThreadStart(() => DoWork(message, client)));
                t.Start();
                threads.Add(t);
            }
            catch (Exception)
            {
                Console.WriteLine("Cancelled");
            }
        }

        watcher.Start();
    }
};

while (true) { }

void DoWork(PipeMessage message, Client client)
{
    Console.WriteLine("Starting calculations...");

    for (int i = 0; i < FIRST_ITERATIONS; i++)
    {
        var solution1 = message.CitiesSolution.GetShuffledCopy();
        var solution2 = message.CitiesSolution.GetShuffledCopy();

        var mutated1 = PMX.PMXMutate(solution1, solution2);
        var mutated2 = PMX.PMXMutate(solution2, solution1);


        for (int j = 0; j < SECOND_ITERATIONS; j++)
        {
            var opted1 = ThreeOpt.Process(mutated1);
            var opted2 = ThreeOpt.Process(mutated2.GetCopy());

            var localBest = opted1.CalculateTotalDistance() < opted2.CalculateTotalDistance() ? opted1 : opted2;

            if (j % 100 == 0)
            {
                UpdateProgress();
            }

            client.IncrementSolutionCounter();

            lock (solutionLock)
            {
                if (bestSolution == null || localBest.CalculateTotalDistance() < bestSolution.CalculateTotalDistance())
                {
                    bestSolution = localBest.GetCopy();
                    Console.WriteLine($"Total Duration ms: {watcher.ElapsedMilliseconds}, " +
                        $"Max duration ms: {maxDurationInMilisecs}, " +
                        $"Solution count: {client.SolutionCount}, " +
                        $"Best solution: {bestSolution.CalculateTotalDistance()}, " +
                        $"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
                    client.SendMessage(new PipeMessage
                    {
                        ThreadId = Thread.CurrentThread.ManagedThreadId.ToString(),
                        Type = MessageType.BEST_SOLUTION,
                        CitiesSolution = bestSolution,
                        SolutionCount = client.SolutionCount,
                        Progress = watcher.ElapsedMilliseconds,
                    });
                }
            }
        }
    }

    Console.WriteLine("Finished calculations");
}

void UpdateProgress()
{
    lock (progressLock)
    {

        var totalDuration = watcher.ElapsedMilliseconds;

        if (totalDuration - lastTotalDuration > 500)
        {
            Console.WriteLine($"Total Duration ms: {totalDuration}, " +
                $"Max duration ms: {maxDurationInMilisecs}, " +
                $"Solution count: {client.SolutionCount}, " +
                $"Best solution: {bestSolution.CalculateTotalDistance()}, " +
                $"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            lastTotalDuration = totalDuration;
            client.SendMessage(new PipeMessage { Type = MessageType.PROGRESS, Progress = totalDuration, SolutionCount = client.SolutionCount });
        }
    }
}