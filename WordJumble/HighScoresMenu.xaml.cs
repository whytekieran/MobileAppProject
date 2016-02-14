using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
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
        private SimpleOrientationSensor _simpleorientation;

        //Constructor
        public HighScoresMenu()
        {
            this.InitializeComponent();
            createGameOptionList();                                         //Populate list with high score menu options
            highscoreOptionsList.ItemsSource = highscoreOptions;            //Make list of GameTypes the item source for the list

            _simpleorientation = SimpleOrientationSensor.GetDefault();      //Get a defualt version of an orientation sensor.

            // Assign an event handler for the sensor orientation-changed event 
            if (_simpleorientation != null)
            {
                _simpleorientation.OrientationChanged += new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>(OrientationChanged);
            } 
        }//end HighScoresMenu constructor

        //On navigated to we make sure the page is displayed in portrait
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        //This event handler is triggered when the orientation of the phone changes, because the method uses the
        //async keyword it will happen asynchronously. Hence allowing the application to continue with other tasks while this
        //method is being executed in a seperate thread.
        //On the this we want the orientation to remain in portrait no matter what direction the phone has been flipped in
        private async void OrientationChanged(object sender, SimpleOrientationSensorOrientationChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SimpleOrientation orientation = e.Orientation;      //Here we retrieve the current orientation of the sensor
                switch (orientation)
                {
                    case SimpleOrientation.NotRotated:  //If the phone isnt being rotated (portrait)
                        //Portrait 
                        DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;  //Set orientation to portrait
                        break;
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:  //if rotated 90degrees to the left
                        //Landscape
                        DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait; //set orientation to portrait
                        break;
                }
            });
        }

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
