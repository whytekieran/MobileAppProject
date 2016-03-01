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
    public sealed partial class MainPage : Page
    {
        List<GameType> gameOptions;                                 //List of GameType objects
        int selectedIndex;                                          //Holds the index of the item user selects from listbox
        private SimpleOrientationSensor _simpleorientation;

        //Constructor
        public MainPage()
        {
            this.InitializeComponent();
            createGameOptionList();                                 //Populate the list of GameTypes.
            gameOptionsList.ItemsSource = gameOptions;              //Pass the GameType list as the listboxes item source

            //Add event listener for the back hardware button
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            _simpleorientation = SimpleOrientationSensor.GetDefault();      //Get a defualt version of an orientation sensor.

            // Assign an event handler for the sensor orientation-changed event 
            if (_simpleorientation != null)
            {
                _simpleorientation.OrientationChanged += new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>(OrientationChanged);
            } 

            //this.NavigationCacheMode = NavigationCacheMode.Required;
        }//end MainPage() constructor

        //Event listener for the back hardware button
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            //if we can go back then go back
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }//end HardwareButtons_BackPressed()

        //On navigated to we make sure the page is displayed in portrait(triggered when page is navigated to (opens))
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        //This event handler is triggered when the orientation of the phone changes, because the method uses the
        //async keyword it will happen asynchronously. Hence allowing the application to continue with other tasks while this
        //method is being executed in a seperate thread.
        //On the main page we want the orientation to remain in portrait no matter what direction the phone has been flipped in
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
