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
        List<GameType> highscoreOptions;                       //List of GameType objects to hold high score menu options
        int selectedIndex;                                     //An int to hold the index of the listbox item selected by the user

        //Constructor
        public HighScoresMenu()
        {
            this.InitializeComponent();
            createGameOptionList();                                         //Populate list with high score menu options
            highscoreOptionsList.ItemsSource = highscoreOptions;            //Make list of GameTypes the item source for the list
        }//end HighScoresMenu constructor

        //Create list of high score options for the user
        private void createGameOptionList()
        {
            GameType option;                                        //Declare GameType object

            if (highscoreOptions == null)                       //Instantiate list of GameTypes if it hasnt been already
            {
                highscoreOptions = new List<GameType>();            //Instantiate done here
            }
            
            //We then instantiate the single GameType object, give its instance variable
            //(which is the variable we are binding) a value and add the 
            //object to the list
            option = new GameType();
            option.userChoice = " 4 Letter High Scores";
            highscoreOptions.Add(option);

            //Repeat the process for each high score option
            option = new GameType();
            option.userChoice = " 5 Letter High Scores";
            highscoreOptions.Add(option);

            option = new GameType();
            option.userChoice = " 6 Letter High Scores";
            highscoreOptions.Add(option);

            option = new GameType();
            option.userChoice = " 7 Letter High Scores";
            highscoreOptions.Add(option);
        }//end createGameOptionList()

        //Tap event for the listbox
        private void listItemTap(object sender, TappedRoutedEventArgs e)
        {
            //first get the index of the listbox item tapped by the user
            selectedIndex = Convert.ToInt32(highscoreOptionsList.SelectedIndex);

            //Go to HighScores.xaml and pass the users choice using the DataPasser.cs class
            Frame.Navigate(typeof(HighScores), new DataPasser { data = selectedIndex });
        }
    }
}
