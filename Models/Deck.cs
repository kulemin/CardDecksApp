using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using static CardDecksApp.Resources.Enums;

namespace CardDecksApp.Models
{
    public class Deck
    {
        public Deck()
        {
            
        }
        public Deck(int capacity, string name)
        {
            Cards = new ObservableCollection<Card>();
            for (int i = capacity == 36 ? 5 : 1; i <= 52 / 4; i++)
            {
                for(int j = 1; j <= 4; j++)
                {
                    Cards.Add(new Card((EnumCardRanks)i, (EnumCardSuits)j));
                }
            }
            if(capacity == 54)
            {
                Cards.Add(new Card(EnumCardRanks.Joker, EnumCardSuits.Worms));
                Cards.Add(new Card(EnumCardRanks.Joker, EnumCardSuits.Peaks));
            }
            Name = name;
            Capacity = capacity;
        }
        public Deck(int capacity, string name, int id, ObservableCollection<Card> cards)
        {
            Cards = cards;
            Id = id;
            Name = name;
            Capacity = capacity;
        }

        public int Id { get; set; }
        public int Capacity { get; set; }
        public ObservableCollection<Card> Cards { get; set; }
        public string Name { get; set; }
        public Avalonia.Media.Imaging.Bitmap Poster { get; set; }
    }
}
