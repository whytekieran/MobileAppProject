using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.Storage;
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
    public sealed partial class HighScores : Page
    {
        private SQLiteConnection con;                //Declare an SQLite connection
        private DataPasser dataHolder;               //Declare a DataPasser to receive incoming information from HighScoresMenu.xaml
        private int rank;                            //Used to assign the rank of each player
        private SimpleOrientationSensor _simpleorientation;

        //Constructor for HighScores.xaml
        public HighScores()
        {
            this.InitializeComponent();
            copyDatabase();                                     //Copys the database file so it can be found locally in the application

            //Add event listener for the back hardware button
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            _simpleorientation = SimpleOrientationSensor.GetDefault();      //Get a defualt version of an orientation sensor.

            // Assign an event handler for the sensor orientation-changed event 
            if (_simpleorientation != null)
            {
                _simpleorientation.OrientationChanged += new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>(OrientationChanged);
            } 
        }//end of constructor

        //Event listener for the back hardware button
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            //if we can go back then go back one page
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }//end HardwareButtons_BackPressed()

        //When the page is navigated to
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //firstly make sure the page is in portrait orientation
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            List<HighScore> tasks;                           //Declare list of high score objects
            con = new SQLiteConnection("GameDatabase.db");   //Instantiate a new SQLite connection to the game database
            con.CreateTable<HighScore>();                    //Create a table for the connection using HighScore objects to represent
                                                             //table entities, each row is an object.

            dataHolder = e.Parameter as DataPasser;          //Get information from HighScoresMenu stating which highscores the user wants
            getListOfScores(dataHolder.data, out tasks);     //Execute the appropriate query to get list of high scores from table
                                                             //query is ordered by score desc
          
            //We want the ranks to be in order, currently they just have ids which arent ordered by score so....

            //Start looping over the list, first element will have the highest score, second will have the second highest and so on
            for (rank = 0; rank < tasks.Count(); ++rank)    
            {
                HighScore currentInList = tasks.ElementAt(rank); //Retreive that element from the list (first iteration = highest score)
                currentInList.Id = (rank + 1);                   //Change its rank to one
                tasks.RemoveAt(rank);                            //Remove the element currently ranked in 1.
                tasks.Insert(rank, currentInList);               //Add new element with proper rank number
            }

            gameHighScoreList.ItemsSource = tasks;          //Once list is populated add it as the item source for the listbox
        }//end onNavigatedTo()

        //This event handler is triggered when the orientation of the phone changes, because the method uses the
        //async keyword it will happen asynchronously. Hence allowing the application to continue with other tasks while this
        //method is being executed in a seperate thread.
        //On this page we want the orientation to remain in portrait no matter what direction the phone has been flipped in
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

        //When the page has been navigated away from
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //Check if the connection is open and if it is
            if (con != null)
                con.Close(); // Close the database connection.   
        }

        //Gets a list of all the rows in the database parameters are an int to help choose which query to execute
        //and a list which we want to populate. using the out keyword, dont have to return the list.
        private void getListOfScores(int statementSelect, out List<HighScore> tasks)
        {
            tasks = null;

            switch (statementSelect)//Depending on which option the user choose
            {
                //Read either four, five, six, seven letter high score.
                case 0:
                    tasks = con.Query<HighScore>("select * from FourLetterHighScores order by score desc").ToList<HighScore>();
                    break;
                case 1:
                    tasks = con.Query<HighScore>("select * from FiveLetterHighScores order by score desc").ToList<HighScore>();
                    break;
                case 2:
                    tasks = con.Query<HighScore>("select * from SixLetterHighScores order by score desc").ToList<HighScore>();
                    break;
                case 3:
                    tasks = con.Query<HighScore>("select * from SevenLetterHighScores order by score desc").ToList<HighScore>();
                    break;
            }
        }

        //Copying the database so it can be found locally
        private async void copyDatabase()
        {
            bool isDatabaseExisting = false;            //Set true if the database exists

            try
            {
                //Check if we can find the database and if we can it exists
                StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync("GameDatabase.db");
                isDatabaseExisting = true;
            }
            catch
            {
                isDatabaseExisting = false;
            }

            //If the database exists
            if (isDatabaseExisting)
            {
                //Copy it locally
                StorageFile databaseFile = await Package.Current.InstalledLocation.GetFileAsync("GameDatabase.db");
                await databaseFile.CopyAsync(ApplicationData.Current.LocalFolder);
            }
        }
    }
}
