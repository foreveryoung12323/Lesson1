using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Lesson1;

int difficulty = 6;
var blockChainService = new BlockChain(difficulty);
var miningService = new MiningService();
var displayService = new DisplayService();

Console.WriteLine($"Testing async mining (Difficulty: {difficulty}, Threads: {Environment.ProcessorCount})");

Console.Write("Mining block 1");
var task = blockChainService.AddBlockParallelAsync("User Registration: Stepan", "Stepan");

while (!task.IsCompleted)
{
    Console.Write(".");
    await Task.Delay(200);
}

long attempts1 = await task;
Console.WriteLine($"\nBlock 1 mined. Attempts: {attempts1}");

var timeNow = DateTime.Now;
var lastBlock = blockChainService.Chain[blockChainService.Chain.Count - 1];

var block1 = new Block
{
    Index = lastBlock.Index + 1,
    Data = "Transfer: Stepan -> Alex 1500",
    Author = "Stepan",
    PrevHash = lastBlock.Hash,
    TimeStamp = timeNow
};

var block2 = new Block
{
    Index = lastBlock.Index + 1,
    Data = "Transfer: Stepan -> Alex 1500",
    Author = "Stepan",
    PrevHash = lastBlock.Hash,
    TimeStamp = timeNow
};

Console.WriteLine("\nStarting single-thread mining...");
var sw = Stopwatch.StartNew();
long singleAttempts = await miningService.MineBlockAsync(block1, difficulty);
sw.Stop();
long singleMs = sw.ElapsedMilliseconds;
Console.WriteLine($"Single-thread: {singleMs} ms, attempts: {singleAttempts}, nonce: {block1.Nonce}");
Console.WriteLine($"Hash: {block1.Hash}");

Console.WriteLine("\nStarting multi-thread mining...");
sw.Restart();
long parallelAttempts = await miningService.MineBlockParallelAsync(block2, difficulty);
sw.Stop();
long parallelMs = sw.ElapsedMilliseconds;
Console.WriteLine($"Multi-thread: {parallelMs} ms, attempts: {parallelAttempts}, nonce: {block2.Nonce}");
Console.WriteLine($"Hash: {block2.Hash}");

blockChainService.Chain.Add(block2);

if (parallelMs > 0)
{
    double speedup = Math.Round((double)singleMs / parallelMs, 2);
    Console.WriteLine($"\nSpeedup: {speedup}x");
}

Console.WriteLine("Is chain valid: " + blockChainService.IsChainValid());
Console.WriteLine();
displayService.DisplayChain(blockChainService);