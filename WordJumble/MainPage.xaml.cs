using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
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
        List<GameType> gameOptions;                         //List of GameType objects
        int selectedIndex;                                  //Holds the index of the item user selects from listbox

        //Constructor
        public MainPage()
        {
            this.InitializeComponent();
            createGameOptionList();                                 //Populate the list of GameTypes.
            gameOptionsList.ItemsSource = gameOptions;              //Pass the GameType list as the listboxes item source

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }//end MainPage() constructor

        //On navigated to we make sure the page is displayed in portrait
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        //Create list of game options for the user
        private void createGameOptionList()
        {
            GameType option;                                                //Declare GameType object

            if (gameOptions == null)               //Instantiate list of GameTypes if it hasnt been already
            {
                gameOptions = new List<GameType>();
            }

            option = new GameType();                                //We then instantiate the GameType object, give its instance variable
            option.userChoice = " 4 Letter Word Jumble";            //(which is the variable we are binding) a value and add the 
            gameOptions.Add(option);                                //object to the list

            option = new GameType();
            option.userChoice = " 5 Letter Word Jumble";            //repeat this process for each game option
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

        //Tap event for the listbox
        private void listItemTap(object sender, TappedRoutedEventArgs e)
        {
            //first get the index of the listbox item tapped by the user
            selectedIndex = Convert.ToInt32(gameOptionsList.SelectedIndex);

            if(selectedIndex >= 0 && selectedIndex <= 3)  //If the index is between 0-3 (including 0 and 3)
            {
                //Go to Game.xaml and pass the users choice using the DataPasser.cs class
                Frame.Navigate(typeof(Game), new DataPasser { data = selectedIndex }); 
            }
            else if(selectedIndex == 4)//otherwise
            {
                Frame.Navigate(typeof(HighScoresMenu));   //Go to the high score menu page
            }
        }
    }
}
