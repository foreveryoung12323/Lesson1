//using System;

//namespace Lesson1
//{
//    public class MiningService
//    {
//        private readonly HashingService _hashingService;

//        public MiningService()
//        {
//            _hashingService = new HashingService();
//        }

//        public void MineBlock(Block block, int difficulty)
//        {
//            string target = new string('0', difficulty);
//            MineBlockByPattern(block, target);
//        }

//        public void MineBlockByPattern(Block block, string targetPattern)
//        {
//            string target = targetPattern.ToLower();

//            while (true)
//            {
//                block.Hash = _hashingService.ComputeBlockHash(block);

//                if (block.Hash.StartsWith(target))
//                {
//                    Console.WriteLine($"Block mined: {block.Hash} with nonce: {block.Nonce}");
//                    break;
//                }

//                if (block.Nonce % 100000 == 0 && block.Nonce > 0)
//                {
//                    Console.WriteLine($"Mining in progress... Current nonce: {block.Nonce}, Current hash: {block.Hash}");
//                }

//                block.Nonce++;
//            }
//        }

//        // Mines block until the Base64 hash STARTS with targetPrefix (e.g., "step")
//        public void MineBlockByBase64Prefix(Block block, string targetPrefix)
//        {
//            while (true)
//            {
//                block.Hash = _hashingService.ComputeBlockHashBase64(block);

//                if (block.Hash.StartsWith(targetPrefix, StringComparison.OrdinalIgnoreCase))
//                {
//                    Console.WriteLine($"Block mined! Base64 Hash starts with '{targetPrefix}': {block.Hash} | Nonce: {block.Nonce}");
//                    break;
//                }

//                if (block.Nonce % 100000 == 0 && block.Nonce > 0)
//                {
//                    Console.WriteLine($"Mining in progress... Nonce: {block.Nonce}, Current hash: {block.Hash}");
//                }

//                block.Nonce++;
//            }
//        }
//    }
//}

using System;

namespace Lesson1
{
    public class MiningService
    {
        private readonly HashingService _hashingService;

        public MiningService()
        {
            _hashingService = new HashingService();
        }

        public long MineBlock(Block block, int difficulty)
        {
            string target = new string('0', difficulty);
            long attempts = 0;
            block.Nonce = 0;

            while (true)
            {
                block.Hash = _hashingService.ComputeBlockHash(block);
                attempts++;

                if (block.Hash.StartsWith(target))
                {
                    break;
                }

                block.Nonce++;
            }

            return attempts;
        }

        public async Task<long> MineBlockAsync(Block block, int difficulty)
        {
            return await Task.Run(() => MineBlock(block, difficulty));
        }

        public async Task<long> MineBlockParallelAsync(Block block, int difficulty)
        {
            int threadCount = Environment.ProcessorCount;
            string target = new string('0', difficulty);
            using var cts = new CancellationTokenSource();
            object lockObj = new object();
            long totalAttempts = 0;
            bool isFound = false;

            var tasks = new Task[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int threadIndex = i;
                tasks[i] = Task.Run(() =>
                {
                    long localNonce = threadIndex;
                    long localAttempts = 0;

                    while (!cts.Token.IsCancellationRequested)
                    {
                        string hash = _hashingService.ComputeBlockHash(block, localNonce);
                        localAttempts++;

                        if (hash.StartsWith(target))
                        {
                            lock (lockObj)
                            {
                                if (!isFound)
                                {
                                    isFound = true;
                                    block.Nonce = localNonce;
                                    block.Hash = hash;
                                    cts.Cancel(); 
                                }
                            }
                            break;
                        }

                        localNonce += threadCount;
                    }

                    Interlocked.Add(ref totalAttempts, localAttempts);
                });
            }

            await Task.WhenAll(tasks);
            return totalAttempts;
        }
    }
}