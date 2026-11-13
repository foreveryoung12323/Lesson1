using System;
using System.Collections.Generic;
using System.Text;

namespace Lesson1
{
    public class Block
    {
        // Нумерация блока по порядку его создания
        public int Index { get; set; }
        //Хеш текущего блока
        public string Hash { get; set; }
        //Данные блока
        public string Data { get; set; }
        //Хеш предыдущего блока
        public string PrevHash { get; set; }

        public string Author { get; set; } 
        public DateTime TimeStamp { get; set; }

        public long Nonce { get; set; } // Число, используемое для майнинга блока

    }
}
