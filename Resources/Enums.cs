using System;
using System.Collections.Generic;
using System.Text;

namespace CardDecksApp.Resources
{
    public class Enums
    {
        public enum EnumCardRanks
        {
            Two = 1,
            Three = 2, 
            Four = 3, 
            Five = 4, 
            Six = 5,
            Seven = 6,
            Eight = 7,
            Nine = 8,
            Ten = 9,
            Jack = 10,
            Queen = 11,
            King = 12,
            Ace = 13,
            Joker = 14
        };

        public enum EnumCardSuits
        {
            Worms = 1,
            Peaks = 2,
            Diamonds = 3,
            Clubs = 4
        }
        public enum EnumShuffleMethod
        {
            Random = 1,
            HalfHearted = 2
        }
    }
}
