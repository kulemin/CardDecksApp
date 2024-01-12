using Avalonia.Controls;
using CardDecksApp.Models;
using CardDecksApp.ViewModels;
using Nito.AsyncEx;

namespace CardDecksApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListBox_Tapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var model = (MainWindowViewModel)this.DataContext;
            if (model.Deck == null) return;
            model.Deck = (Deck)((Avalonia.Controls.ListBox)sender).SelectedItem;
            model.Cards = model.Deck.Cards;
            model.InitializationNotifier = NotifyTaskCompletion.Create(model.LoadData());

        }
    }
}
