using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWar.Models
{
    public class CardModel
    {
        public string Type { get; set; }

        public string CardType { get; set; }

        public string Name { get; set; }

        public int Mana { get; set; }

        public int Attack { get; set; }

        public int Health { get; set; }

        public string CardGFX { get; set; }

        public string Quote { get; set; }
    }
}
