using CardDecksApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static CardDecksApp.Resources.Enums;

namespace CardDecksApp.Resources
{
    public static class Shuffle
    {
        public static Deck ShuffleDeck(Deck deck, EnumShuffleMethod method)
        {
            if (deck.Capacity == 0) return deck;
            Random rnd = new Random();
            switch (method)
            {
                case EnumShuffleMethod.Random:
                    var cards = new List<Card>();
                    foreach(var card in deck.Cards)
                    {
                        cards.Add(card);
                    }
                    cards.Sort((x, y) => rnd.Next(-1, 1));
                    for(int i = 0; i < cards.Count; i++)
                    {
                        deck.Cards[i] = cards[i];
                    }
                    break;
                case EnumShuffleMethod.HalfHearted:
                    int iterationsCount = rnd.Next(9, 13);
                    ObservableCollection<Card> leftCards;
                    ObservableCollection<Card> rightCards;
                    for (int j = 0; j < iterationsCount; j++)
                    {
                        leftCards = new ObservableCollection<Card>();
                        rightCards = new ObservableCollection<Card>();
                        for (int i = 0; i < deck.Cards.Count; i++)
                        {
                            if (i < deck.Cards.Count / 2) leftCards.Add(deck.Cards[i]);
                            else rightCards.Add(deck.Cards[i]);
                        }
                        deck.Cards = new ObservableCollection<Card>();
                        for (int i = 0; i < rightCards.Count; i++)
                        {
                            deck.Cards.Add(rightCards[i]);
                            deck.Cards.Add(leftCards[i]);

                        }
                    }
                    break;
            }

            return deck;
        }
    }
}
