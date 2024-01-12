using CardDecksApp.DataProvider;
using CardDecksApp.Models;
using CardDecksApp.Resources;
using CardDecksApp.Services;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace CardDecksApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private DeckService _deckService;
        private CardService _cardService;
        private List<string> _deckNames;
        
        public MainWindowViewModel()
        {
            IsZeroStep = true;
            InitializationNotifier = NotifyTaskCompletion.Create(InitializeAsync());
        }

        public INotifyTaskCompletion InitializationNotifier { get; set; }

        private async Task InitializeAsync()
        {
            _deckNames = new List<string>();
            var isDBCreated = SQLiteDatabase.GetConnection();
            if (!isDBCreated)
            {
                FillTestData();
            }
            Decks = SQLiteDatabase.GetDecks();
            foreach(var deck in Decks)
            {
                _deckNames.Add(deck.Name);
            }
            Deck = Decks[0];
            Cards = Decks[0].Cards;
            _deckService = new DeckService();
            _cardService = new CardService();
            ClearSearch();
        }

        public void FillTestData()
        {
            var newDeck = new Deck(36, "Пример 1");
            SQLiteDatabase.SaveDeck(newDeck);
            newDeck = new Deck(52, "Пример 2");
            SQLiteDatabase.SaveDeck(newDeck);
            newDeck = new Deck(54, "Пример 3");
            SQLiteDatabase.SaveDeck(newDeck);
        }

        public async Task LoadData()
        {
            if (Deck == null) return;
            var decks = await _deckService.GetDecks(_searchDecks, Deck);
            SearchDecks = new ObservableCollection<Deck>(decks);
            var cards = await _cardService.GetCards(_cards);
            Cards = new ObservableCollection<Card>(cards);

        }

        public void ShuffleDeck()
        {
            if (Deck.Capacity == 0) return;
            Deck = Shuffle.ShuffleDeck(Deck, Enums.EnumShuffleMethod.Random);
            SQLiteDatabase.SaveDeck(Deck);
            Cards = Deck.Cards;
            InitializationNotifier = NotifyTaskCompletion.Create(LoadData());
        }

        public void DeleteDeck()
        {
            SQLiteDatabase.DeleteDeck(Deck);
            _deckNames.Remove(Deck.Name);
            Decks.Remove(Deck);
            FindDeck();
            InitializationNotifier = NotifyTaskCompletion.Create(LoadData());
        }

        public void CreateDeck()
        {
            IsZeroStep = false;
            IsFirstStep = true;
        }

        public void FillName()
        {
            if (_deckNames.Contains(DeckName))
            {
                IsFaildName = true;
                IsFirstStep = false;
                IsSecondStep = false;
                IsZeroStep = false;
                return;
            }
            IsFirstStep = false;
            IsSecondStep = true;
            
        }

        public void FillDeck36()
        {
            FillDeck(36);
        }

        public void FillDeck52()
        {
            FillDeck(52);
        }

        public void FillDeck54()
        {
            FillDeck(54);
        }

        public void FillDeck(int capacity)
        {
            IsFirstStep = false;
            IsSecondStep = false;
            IsZeroStep = true;
            var newDeck = new Deck(capacity, DeckName);
            SQLiteDatabase.SaveDeck(newDeck);            
            _decks.Add(newDeck);
            _deckNames.Add(newDeck.Name);
            FindDeck();
            DeckName = "";
            InitializationNotifier = NotifyTaskCompletion.Create(LoadData());
        }

        public void CancelDeckCreation()
        {
            IsFirstStep = false;
            IsSecondStep = false;
            IsZeroStep = true;
            IsFaildName = false;
            DeckName = "";
        }

        public void FindDeck()
        {
            SearchDecks = new ObservableCollection<Deck>();
            foreach(var deck in Decks)
            {
                if (deck.Name.ToLower().Contains(SearchName) || SearchName.ToLower() == "")
                    SearchDecks.Add(deck);
            }
            Deck = SearchDecks.Count > 0 ? SearchDecks[0] : new Deck();
            Cards = SearchDecks.Count > 0 ? SearchDecks[0].Cards : new ObservableCollection<Card>();
            InitializationNotifier = NotifyTaskCompletion.Create(LoadData());
        }
        public void ClearSearch()
        {
            SearchName = "";
            FindDeck();
        }

        private ObservableCollection<Deck> _decks;
        public ObservableCollection<Deck> Decks
        {
            get => _decks;
            set
            {
                if (value != null)
                {
                    _decks = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Deck> _searchDecks;
        public ObservableCollection<Deck> SearchDecks
        {
            get => _searchDecks;
            set
            {
                if (value != null)
                {
                    _searchDecks = value;
                    OnPropertyChanged();
                }
            }
        }

        private Deck _deck;
        public Deck Deck
        {
            get => _deck;
            set
            {
                if (value != null)
                {
                    _deck = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<Card> _cards;
        public ObservableCollection<Card> Cards
        {
            get => _cards;
            set
            {
                if (value != null)
                {
                    _cards = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isZeroStep;
        public bool IsZeroStep 
        {
            get => _isZeroStep;
            set
            {
                if (value != null)
                {
                    _isZeroStep = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFirstStep;
        public bool IsFirstStep
        {
            get => _isFirstStep;
            set
            {
                if (value != null)
                {
                    _isFirstStep = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isSecondStep;
        public bool IsSecondStep
        {
            get => _isSecondStep;
            set
            {
                if (value != null)
                {
                    _isSecondStep = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFaildName;
        public bool IsFaildName
        {
            get => _isFaildName;
            set
            {
                if (value != null)
                {
                    _isFaildName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _deckName;
        public string DeckName
        {
            get => _deckName;
            set
            {
                if (value != null)
                {
                    _deckName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _searchName;
        public string SearchName
        {
            get => _searchName;
            set
            {
                if (value != null)
                {
                    _searchName = value;
                    OnPropertyChanged();
                }
            }
        }

        
    }
}

