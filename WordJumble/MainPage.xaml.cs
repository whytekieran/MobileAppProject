using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WordJumble
{
    public sealed partial class MainPage : Page
    {
        List<GameType> gameOptions;
        int selectedIndex;

        //Constructor
        public MainPage()
        {
            this.InitializeComponent();
            createGameOptionList();
            gameOptionsList.ItemsSource = gameOptions;

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        //Create list of options for the user
        private void createGameOptionList()
        {
            GameType option;

            if (gameOptions == null)
            {
                gameOptions = new List<GameType>();
            }

            option = new GameType();
            option.userChoice = " 4 Letter Word Jumble";
            gameOptions.Add(option);

            option = new GameType();
            option.userChoice = " 5 Letter Word Jumble";
            gameOptions.Add(option);

            option = new GameType();
            option.userChoice = " 6 Letter Word Jumble";
            gameOptions.Add(option);

            option = new GameType();
            option.userChoice = " 7 Letter Word Jumble";
            gameOptions.Add(option);

            option = new GameType();
            option.userChoice = " High Scores Menu";
            gameOptions.Add(option);

        }//end createGameOptionList

        private void listItemTap(object sender, TappedRoutedEventArgs e)
        {
            selectedIndex = Convert.ToInt32(gameOptionsList.SelectedIndex);

            if(selectedIndex >= 0 && selectedIndex <= 3)
            {
                Frame.Navigate(typeof(Game), new DataPasser { data = selectedIndex });
            }
            else if(selectedIndex == 4)
            {
                Frame.Navigate(typeof(HighScoresMenu));
            }
        }
    }
}
