using CardDecksApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CardDecksApp.Services
{
    public class CardService
    {
        readonly string _workingDirectory = Environment.CurrentDirectory;
        internal async Task<IEnumerable<Card>> GetCards(ObservableCollection<Card> cards)
        {
            var folderPath = $"{_workingDirectory}\\Data\\Cards\\Images";

            foreach (var card in cards)
            {
                var imagePath = Path.Combine(folderPath, $"{card.Name}.png");
                card.Poster = await GetPoster(imagePath);
            }
            return cards;
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
