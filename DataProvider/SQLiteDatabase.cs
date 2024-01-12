using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data.SQLite;
using CardDecksApp.Models;
using System.Collections.ObjectModel;
using System.Data;
using static CardDecksApp.Resources.Enums;

namespace CardDecksApp.DataProvider
{
    public static class SQLiteDatabase
    {
        internal static String _sqliteFileName;
        internal static SQLiteConnection _sqliteConn;
        internal static SQLiteCommand _sqliteCmd;
        public static bool GetConnection()
        {
            _sqliteConn = new SQLiteConnection();
            _sqliteCmd = new SQLiteCommand();

            _sqliteFileName = "CardDecksStorage.sqlite";

            //При первом запуске, если еще не создана БД, - создаем и наполняем тестовыми данными, иначе - подключаемся к БД
            if (!File.Exists(_sqliteFileName))
            {
                CreateSQLiteDB();
                return false;
            }
            else
            {
                ConnectSQLiteDB();
                return true;
            }
        }

        private static void CreateSQLiteDB()
        {
            //Создаем файл с БД
            SQLiteConnection.CreateFile(_sqliteFileName);

            //Подключаемся к БД
            ConnectSQLiteDB();
            try
            {
                _sqliteCmd.CommandText = "CREATE TABLE IF NOT EXISTS Cards (id INTEGER PRIMARY KEY AUTOINCREMENT, rank TEXT, " +
                                        "suit TEXT, name TEXT)";
                _sqliteCmd.ExecuteNonQuery();
                _sqliteCmd.CommandText = "CREATE TABLE IF NOT EXISTS Decks (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, " +
                                        "capacity INTEGER)";
                _sqliteCmd.ExecuteNonQuery();
                _sqliteCmd.CommandText = "CREATE TABLE IF NOT EXISTS DeckCardLinks (id INTEGER PRIMARY KEY AUTOINCREMENT, cardId INTEGER, " +
                                        "deckId INTEGER, FOREIGN KEY (cardId) REFERENCES Cards(id), FOREIGN KEY (deckId) REFERENCES Decks(id))";
                _sqliteCmd.ExecuteNonQuery();
                FillCards();
            }
            catch (SQLiteException ex)
            {
                //MessageBox.Show("Error: " + ex.Message);
            }
        }

        //заполняем таблицу картами - в нашем случае они будут всегда одни и те же
        private static void FillCards()
        {
            var demoDeck = new Deck(54, "DemoDeck");
            foreach(var card in demoDeck.Cards)
            {
                _sqliteCmd.CommandText = $"INSERT INTO Cards ('rank', 'suit', 'name') values ('{card.Rank}', '{card.Suit}', '{card.Name}')";
                _sqliteCmd.ExecuteNonQuery();
            }
        }

        public static void SaveDeck(Deck deck)
        {
            var deckId = GetDeckIdByName(deck.Name);
            if (deckId == 0)
            {
                InsertDeck(deck);
                deckId = GetDeckIdByName(deck.Name);
                deck.Id = deckId;
            }
            else UpdateDeck(deck);
        }

        public static void InsertDeck(Deck deck)
        {
            _sqliteCmd.CommandText = $"INSERT INTO Decks ('name', 'capacity') values ('{deck.Name}', '{deck.Capacity}')";
            _sqliteCmd.ExecuteNonQuery();
            var deckId = GetDeckIdByName(deck.Name);
            foreach (var card in deck.Cards)
            {
                _sqliteCmd.CommandText = $"INSERT INTO DeckCardLinks ('cardId', 'deckId') values ('{card.Id}', '{deckId}')";
                _sqliteCmd.ExecuteNonQuery();
            }
        }

        public static void UpdateDeck(Deck deck)
        {
            _sqliteCmd.CommandText = $"UPDATE Decks SET capacity = '{deck.Capacity}' where id = {deck.Id}";
            _sqliteCmd.ExecuteNonQuery();
            _sqliteCmd.CommandText = $"DELETE FROM DeckCardLinks where deckId = {deck.Id}";
            _sqliteCmd.ExecuteNonQuery();
            foreach (var card in deck.Cards)
            {
                _sqliteCmd.CommandText = $"INSERT INTO DeckCardLinks ('cardId', 'deckId') values ('{card.Id}', '{deck.Id}')";
                _sqliteCmd.ExecuteNonQuery();
            }
        }

        public static void DeleteDeck(Deck deck)
        {
            _sqliteCmd.CommandText = $"DELETE FROM DeckCardLinks where deckId = {deck.Id}";
            _sqliteCmd.ExecuteNonQuery();
            _sqliteCmd.CommandText = $"DELETE FROM Decks where id = {deck.Id}";
            _sqliteCmd.ExecuteNonQuery();
        }

        public static int GetDeckIdByName(string name)
        {
            var sqlQuery = $"SELECT id, name, capacity FROM Decks WHERE name = '{name}'";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, _sqliteConn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            if (table.Rows.Count < 1) return 0;
            DataRow row = table.Rows[0];
            return Convert.ToInt32(row["id"]);
        }

        public static int GetCardIdByName(string name)
        {
            var sqlQuery = $"SELECT id, rank, suit FROM Cards WHERE name = '{name}'";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlQuery, _sqliteConn);
            DataTable table = new DataTable();
            adapter.Fill(table);
            if (table.Rows.Count < 1) return 0;
            DataRow row = table.Rows[0];
            return Convert.ToInt32(row["id"]);
        }

        public static ObservableCollection<Deck> GetDecks()
        {
            var decks = new ObservableCollection<Deck>();
            var getDeckQuery = "SELECT id, name, capacity FROM Decks";
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(getDeckQuery, _sqliteConn);
            DataTable decksTable = new DataTable();
            adapter.Fill(decksTable);
            foreach(DataRow deckRow in decksTable.Rows)
            {
                var deckId = Convert.ToInt32(deckRow["id"]);
                var getCardsQuery = "SELECT Cards.rank rank, Cards.suit suit FROM DeckCardLinks " +
                                        "INNER JOIN Cards ON Cards.id = DeckCardLinks.cardId " +
                                        $"WHERE DeckCardLinks.deckId = {deckId}";
                adapter = new SQLiteDataAdapter(getCardsQuery, _sqliteConn);
                DataTable cardsTable = new DataTable();
                adapter.Fill(cardsTable);
                var cards = new ObservableCollection<Card>();
                foreach(DataRow cardRow in cardsTable.Rows)
                {
                    cards.Add(new Card((EnumCardRanks)Enum.Parse(typeof(EnumCardRanks), cardRow["rank"].ToString()),
                        (EnumCardSuits)Enum.Parse(typeof(EnumCardSuits), cardRow["suit"].ToString())));
                }
                decks.Add(new Deck(Convert.ToInt32(deckRow["capacity"]), deckRow["name"].ToString(),
                    Convert.ToInt32(deckRow["id"]), cards));
            }
            return decks;
        }

        private static void ConnectSQLiteDB()
        {
            _sqliteConn = new SQLiteConnection("Data Source=" + _sqliteFileName + ";Version=3;");
            _sqliteConn.Open();
            _sqliteCmd.Connection = _sqliteConn;
        }
    }
}
