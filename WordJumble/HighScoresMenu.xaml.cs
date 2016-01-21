using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WordJumble
{
    public sealed partial class HighScoresMenu : Page
    {
        List<GameType> highscoreOptions;
        int selectedIndex;

        public HighScoresMenu()
        {
            this.InitializeComponent();
            createGameOptionList();
            highscoreOptionsList.ItemsSource = highscoreOptions;
        }

        private void createGameOptionList()
        {
            GameType option;

            if (highscoreOptions == null)
            {
                highscoreOptions = new List<GameType>();
            }

            option = new GameType();
            option.userChoice = " 4 Letter High Scores";
            highscoreOptions.Add(option);

            option = new GameType();
            option.userChoice = " 5 Letter High Scores";
            highscoreOptions.Add(option);

            option = new GameType();
            option.userChoice = " 6 Letter High Scores";
            highscoreOptions.Add(option);

            option = new GameType();
            option.userChoice = " 7 Letter High Scores";
            highscoreOptions.Add(option);
        }

        private void listItemTap(object sender, TappedRoutedEventArgs e)
        {
            selectedIndex = Convert.ToInt32(highscoreOptionsList.SelectedIndex);
            Frame.Navigate(typeof(HighScores), new DataPasser { data = selectedIndex });
        }
    }
}
