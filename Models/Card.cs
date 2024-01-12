using CardDecksApp.DataProvider;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using static CardDecksApp.Resources.Enums;

namespace CardDecksApp.Models
{
    public class Card
    {
        public Card(EnumCardRanks rank, EnumCardSuits suit)
        {
            Rank = rank;
            Suit = suit;
            Name = rank.ToString() + suit.ToString();
            //id получим из БД по имени
            Id = SQLiteDatabase.GetCardIdByName(Name);
        }

       
        public int Id { get; set; }
        public EnumCardRanks Rank { get; set; }
        public EnumCardSuits Suit { get; set; }
        public string Name { get; }

        public Avalonia.Media.Imaging.Bitmap Poster { get; set; }
    }
}
