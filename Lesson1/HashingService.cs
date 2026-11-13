using System;
using System.Security.Cryptography;
using System.Text;

namespace Lesson1
{
    public class HashingService
    {
        public string ComputeBlockHash(Block block)
        {
            return ComputeBlockHash(block, block.Nonce);
        }

        public string ComputeBlockHash(Block block, long nonce)
        {
            var blockData = $"{block.Index}{block.PrevHash}{block.TimeStamp}{block.Data}{nonce}{block.Author}";
            return ComputeHash(blockData);
        }

        public string ComputeBlockHashBase64(Block block)
        {
            var blockData = $"{block.Index}{block.PrevHash}{block.TimeStamp}{block.Data}{block.Nonce}{block.Author}";
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(blockData);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private string ComputeHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}