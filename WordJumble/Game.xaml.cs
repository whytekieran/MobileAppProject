using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WordJumble
{
    public sealed partial class Game : Page
    {
        private SQLiteConnection con;                           //Declare an SQLite connection
        private int wordsID;                                    //Decides which word is retreived from the database
        private String jumbledWord;                             //Holds the jumbled version of the word
        private String unJumbledWord;                           //Holds the unjumbled version of the word
        private string userWord;                                //Holds the word entered by the user
        private DataPasser dataHolder;                          //Used to pass and receive information from and to other pages
        private int score = 0;                                  //The users score
        Random randomNum = new Random();                        //Generate random number to select random word
        DispatcherTimer timer;                                  //Timer is used to launch a tick event
        Stopwatch stopWatch;                                    //Stopwatch works by ticks specified by timer
        private long mins;                                      //Holds the minutes of the timer
        private long secs;                                      //Holds the seconds of the timer
        private SimpleOrientationSensor _simpleorientation; 
   
        //Constructor for Game.xaml
        public Game()
        {
            this.InitializeComponent();
            copyDatabase();                                     //Copy the database so it can be found locally

            //Add event listener for the back hardware button
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            _simpleorientation = SimpleOrientationSensor.GetDefault();      //Get a defualt version of an orientation sensor.

            // Assign an event handler for the sensor orientation-changed event 
            if (_simpleorientation != null)
            {
                _simpleorientation.OrientationChanged += new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>(OrientationChanged);
            } 
        }//end constructor
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

        //This event handler is triggered when the orientation of the phone changes, because the method uses the
        //async keyword it will happen asynchronously. Hence allowing the application to continue with other tasks while this
        //method is being executed in a seperate to the UI thread.
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
                        VisualStateManager.GoToState(this, "Portrait", true);                       //use portrait visual state
                        break;
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:  //if rotated 90degrees to the left
                        //Landscape
                        DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape; //set orientation to landscape
                        VisualStateManager.GoToState(this, "Landscape", true);                      //use the landscape visual state
                        break;
                }
            });
        }

        //When the game page has been navigated to.....
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            timer = new DispatcherTimer();              //Instantiate a timer
            stopWatch = new Stopwatch();                //Instantiate a stop watch

            secs = 60;                                              //seconds for timer is set to 59
            mins = 2;                                               //minutes for timer is set to 2
            con = new SQLiteConnection("GameDatabase.db");          //Instantiate an SQLite connection object for the Games database
            con.CreateTable<Words>();

           txtScore.Text = score.ToString();                     //Output user score, zero to begin with
           dataHolder = e.Parameter as DataPasser;               //Get data passed from MainPage.xaml specifying which game the user wants
           retreiveAndOutputWord(dataHolder.data);              //Use the data to retrieve word from certain table, jumble and output it

           timer.Interval = new TimeSpan(0, 0, 0, 1, 0);        //Set the timer interval to one second
           timer.Tick += timerTick;                             //Add event to timers tick event
           timer.Start();                                       //Start the timer
           stopWatch.Start();                                   //Start the stopwatch
        }//end OnNavigatedTo()

        //Event for timer.Tick, timers interval has been set for one second so this event fires once a second
        //Counting down from 3 minutes
        private void timerTick(object sender, object e)
        {
            --secs; //Every tick removes one second

            //Set the textblocks holding score and jumbled word every tick, this is used to correct a bug detected earlier,
            //when the phone orientation changes the values are not maintained in the textblocks, this insures that they are
            //repeatedly updated
            if (jumbledWord != null)
            {
                txtJumbledWord.Text = jumbledWord;
                txtScore.Text = score.ToString();
            }

            txtTimeDisplay.Text = mins.ToString() + ":" + secs.ToString(); //Output mins and seconds to the screen

            if(secs == 0)//If seconds  is zero
            {
               secs = 59; //reset it
               --mins;    //take away a minute

               //Time must still be outputted even if seconds is zero
               txtTimeDisplay.Text = mins.ToString() + ":" + secs.ToString();

               //if mins is less than zero and seconds is zero to (outer if)
               if(mins < 0)
               {
                  txtTimeDisplay.Text = "0" + ":" + "00";               //Output zero minutes and seconds
                  MessageBoxDisplay();                                  //Output a message box stating the games over and the score
                  
                   //Navigate to the GameOver.xaml page so the user can enter his/her high score. Use ScoreInformationPasser.cs
                  //to pass the score and which game (4letter, 5letter etc) is being played
                  Frame.Navigate(typeof(GameOver), new ScoreInformationPasser { score = score, gameType = dataHolder.data });
               }   
            }
        }//end timerTick()

        //Displays a game over message box with the users score
        private async void MessageBoxDisplay()
        {
            //Creating instance of MessageDialog and calling its show method to show a message box
            MessageDialog msgbox = new MessageDialog("Game Over, Score: "+score);    
            await msgbox.ShowAsync();   
        }

        //When the page has been navigated away from
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            timer.Stop();                                         //Stop the timer
            stopWatch.Stop();     
            //Check if the connection is open and if it is
            if (con != null)
                con.Close(); // Close the database connection.   
        }

        //Copying the database so it can be found locally
        private async void copyDatabase()
        {
            bool isDatabaseExisting = false;        //Set true if the database exists

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

        //Retreives a word from the database, jumbles it, and outputs it, takes one parameter which decides what query to execute
        private void retreiveAndOutputWord(int statementSelect)
        {
            wordsID = randomNum.Next(1, 200);                          //Generate random number between 1-200 amount of words in table
            retrieveWord(wordsID, statementSelect, out unJumbledWord);       //Execute query to get a word (unjumbled word)
            jumbledWord = new string(unJumbledWord.OrderBy(r => randomNum.Next()).ToArray());   //Shuffle the word
            txtJumbledWord.Text = jumbledWord;          //make unjumbled word text block equal to unjumbled word
        }

        //receive word from the database
        private void retrieveWord(int id, int statementSelect, out string unJumbledWord)
        {
            //Retrieving Data
            Words result;
            switch(statementSelect)//depending on which game the user choose to play
            {
                 //Use a particular statement
                 case 0:
                    result = con.Query<Words>("select * from FourLetterWords where id =" + id).FirstOrDefault();
                    //Then assign the word to unjumbled word passed from method using out keyword
                    assignUnJumbledWord(result, out unJumbledWord); 
                    break;
                case 1:
                     result = con.Query<Words>("select * from FiveLetterWords where id =" + id).FirstOrDefault();
                     assignUnJumbledWord(result, out unJumbledWord);
                    break;
                case 2:
                     result = con.Query<Words>("select * from SixLetterWords where id =" + id).FirstOrDefault();
                     assignUnJumbledWord(result, out unJumbledWord);
                    break;
                case 3:
                     result = con.Query<Words>("select * from SevenLetterWords where id =" + id).FirstOrDefault();
                     assignUnJumbledWord(result, out unJumbledWord);
                    break;
                default:
                    unJumbledWord = "No Information Found";
                    break;
            }
        }

        //Assigns variable unjumbled word the word retreived from the database
        private void assignUnJumbledWord(Words result, out string unJumbledWord)
        {
            if (result == null)
            {
                unJumbledWord = "No Information Found";
            }
            else
            {
                //No need to return the word as its being passed from method to method
                unJumbledWord = result.Word;
            }
        }

        //Button click for when user wants to submit a word
        private void enterWordClick(object sender, RoutedEventArgs e)
        {
            //Get the word entered by the user and remove any whitespace before or after the word
            userWord = txtEnteredWord.Text;
            userWord = userWord.Trim();
           
            //Check if it is equal to the unjumbled version of the word and if it is..
            if(userWord.Equals(unJumbledWord, StringComparison.OrdinalIgnoreCase))
            {
                //Match, correct guess give the user another word
                txtResult.Foreground = new SolidColorBrush(Colors.Green);  //Set result textblock foreground to green
                txtResult.Text = "Correct";                                //Output the word "Correct" in green
                retreiveAndOutputWord(dataHolder.data);                    //Call method the retrieves, jumbles and outputs another word
                txtEnteredWord.Text = "";                                  //reset textbox for entered user words
                score += 4;                                                //increment the score
                txtScore.Text = score.ToString();                          //output the new score
            }
            else
            {
                //No match, incorrect guess user must try again
                txtResult.Foreground = new SolidColorBrush(Colors.Red);   //Set result textblock foreground to red
                txtResult.Text = "Incorrect";                             //Output the word "Incorrect" in red
                txtEnteredWord.Text = "";                                 //reset textbox for entered user words
            }
        }

        //When the text box for entering word is in focus
        private void whenInUse(object sender, RoutedEventArgs e)
        {
            //the txtblock for outputting correct or incorrect should not show anything
            txtResult.Text = "";
        }

        //Detects when the enter key has been pressed on the keyboard
        private void enterKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            //when the enter key has been pressed
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                enterWordClick(sender, e);//Call the same method that is used for the enter word button
            }
        }
    }
}
