using System;
using System.Collections.Generic;
using System.Text;

namespace Lesson1
{
    public class DisplayService
    {
        public void DisplayBlock(Block? block)
        {
            if (block == null)
            {
                Console.WriteLine("Block not found");
                Console.WriteLine(new string('-', 45));
                return;
            }
            Console.WriteLine($"Index:     {block.Index}");
            Console.WriteLine($"Author:    {block.Author}");
            Console.WriteLine($"Data:      {block.Data}");
            Console.WriteLine($"Hash:      {block.Hash}");
            Console.WriteLine($"Nonce:     {block.Nonce}");
            Console.WriteLine($"PrevHash:  {block.PrevHash}");
            Console.WriteLine($"TimeStamp: {block.TimeStamp}");
            Console.WriteLine(new string('-', 45));
        }
        public void DisplayChain(BlockChain blockChain)
        {
            foreach (var block in blockChain.Chain)
            {
                DisplayBlock(block);
            }
        }
    }
    }
 
