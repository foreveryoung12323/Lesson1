using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Lesson1
{
    public class BlockChain
    {
        public List<Block> Chain { get; set; }

        private readonly HashingService _hashingService;
        private readonly MiningService _miningService;
        public int Difficulty { get; private set; } = 3;

        public BlockChain(int difficulty)
        {
            Chain = new List<Block>();
            _hashingService = new HashingService();
            _miningService = new MiningService();
            Difficulty = difficulty;
            AddGenesisBlock();
        }

        private void AddGenesisBlock()
        {
            var genesisBlock = new Block
            {
                Index = 0,
                Data = "Genesis Block",
                Author = "System",
                PrevHash = "0",
                TimeStamp = DateTime.Parse("2023-06-01")
            };

            _miningService.MineBlock(genesisBlock, 1);
            Chain.Add(genesisBlock);
        }

        public void AddBlock(string data, string author = "System")
        {
            var lastBlock = Chain[Chain.Count - 1];
            var newBlock = new Block
            {
                Index = lastBlock.Index + 1,
                Data = data,
                Author = author,
                PrevHash = lastBlock.Hash,
                TimeStamp = DateTime.Now
            };

            _miningService.MineBlock(newBlock, Difficulty);
            Chain.Add(newBlock);
        }

        public async Task<long> AddBlockAsync(string data, string author = "System")
        {
            var lastBlock = Chain[Chain.Count - 1];
            var newBlock = new Block
            {
                Index = lastBlock.Index + 1,
                Data = data,
                Author = author,
                PrevHash = lastBlock.Hash,
                TimeStamp = DateTime.Now
            };

            long attempts = await _miningService.MineBlockAsync(newBlock, Difficulty);
            Chain.Add(newBlock);
            return attempts;
        }

        public async Task<long> AddBlockParallelAsync(string data, string author = "System")
        {
            var lastBlock = Chain[Chain.Count - 1];
            var newBlock = new Block
            {
                Index = lastBlock.Index + 1,
                Data = data,
                Author = author,
                PrevHash = lastBlock.Hash,
                TimeStamp = DateTime.Now
            };

            long attempts = await _miningService.MineBlockParallelAsync(newBlock, Difficulty);
            Chain.Add(newBlock);
            return attempts;
        }

        public void RepairChain(int startIndex)
        {
            if (startIndex < 0 || startIndex >= Chain.Count)
            {
                Console.WriteLine($"Invalid start index for repair: {startIndex}");
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            int reminedBlocksCount = 0;
            long totalAttempts = 0;

            for (int i = startIndex; i < Chain.Count; i++)
            {
                var currentBlock = Chain[i];

                if (i == 0)
                {
                    currentBlock.PrevHash = "0";
                }
                else
                {
                    currentBlock.PrevHash = Chain[i - 1].Hash;
                }

                long attempts = _miningService.MineBlock(currentBlock, Difficulty);
                totalAttempts += attempts;
                reminedBlocksCount++;

                Console.WriteLine($"[Repair] Block #{currentBlock.Index} re-mined! Hash: {currentBlock.Hash} | Attempts: {attempts}");
            }

            stopwatch.Stop();
            Console.WriteLine($"Re-mined Blocks Count: {reminedBlocksCount}");
            Console.WriteLine($"Total Mining Attempts: {totalAttempts:N0}");
            Console.WriteLine($"Elapsed Time:          {stopwatch.Elapsed.TotalSeconds:F3} seconds ({stopwatch.ElapsedMilliseconds} ms)\n");
        }

        public bool IsChainValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                var currentBlock = Chain[i];
                var previousBlock = Chain[i - 1];

                if (currentBlock.Hash != _hashingService.ComputeBlockHash(currentBlock))
                {
                    return false;
                }

                if (currentBlock.PrevHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }

        public Block? GetBlockByIndex(int index)
        {
            return Chain.FirstOrDefault(b => b.Index == index);
        }

        public List<Block> SearchByData(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<Block>();
            }

            return Chain.Where(b => b.Data != null && b.Data.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}