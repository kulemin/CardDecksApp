using CardDecksApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CardDecksApp.Services
{
    public class DeckService
    {
        readonly string _workingDirectory = Environment.CurrentDirectory;
        internal async Task<IEnumerable<Deck>> GetDecks(ObservableCollection<Deck> decks, Deck selectedDeck)
        {
            var folderPath = $"{_workingDirectory}\\Data\\Cards\\Images";
            
            foreach (var deck in decks)
            {
                var deckName = deck == selectedDeck ? deck.Cards[0].Name : "Suit";
                var imagePath = Path.Combine(folderPath, $"{deckName}.png");
                deck.Poster = await GetPoster(imagePath);
            }
            return decks;
        }

        private Task<Avalonia.Media.Imaging.Bitmap> GetPoster(string posterUrl)
        {
            return Task.Run(() =>
            {
                using var fileStream = new FileStream(posterUrl, FileMode.Open, FileAccess.Read) { Position = 0 };
                var bitmap = new Avalonia.Media.Imaging.Bitmap(fileStream);
                return bitmap;
            });
        }
    }
}
