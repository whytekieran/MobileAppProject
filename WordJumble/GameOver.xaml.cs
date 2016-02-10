using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace WordJumble
{
    public sealed partial class GameOver : Page
    {
        private SQLiteConnection con;                         //Declare an SQLite connection
        private ScoreInformationPasser scoreHolder;           //Used to pass and receive score and the type of game from page to page
        private int gameType;                                 //Int representing the type of game being played
        private int id;                                       //Id representing the PK of the highscore table for an insert
        private string name;                                  //Represents user name for a high score table insert
        private int score;                                    //Represents score for a high score table insert

        //Constructor for GameOver.xaml
        public GameOver()
        {
            this.InitializeComponent();
            copyDatabase();                                    //Copy the database so it can be found locally
        }//end constructor

        //When the page has been navigated to...
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //first make sure page is displayed in portrait
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            con = new SQLiteConnection("GameDatabase.db");                  //Instantiate an SQLite connection for the Game database
            con.CreateTable<HighScore>();                                   //Create a table for the connection using HighScore objects
            scoreHolder = e.Parameter as ScoreInformationPasser;       //Receive information (score, game being played) from Game.xaml
            score = scoreHolder.score;                                 //Get score information from scoreholder object
            gameType = scoreHolder.gameType;                           //Get score information from scoreholder object
            id = getrowCount(gameType);                         //Use gametype to decide which query to excute, returns amount of rows
                                                                //in table for the highscores for the game
            txtScore.Text = score.ToString();                   //output users score to the screen
        }

        //Copying the database so it can be found locally
        private async void copyDatabase()
        {
            bool isDatabaseExisting = false;   //Set true if the database exists

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

        //Gets how many rows are in a particular table takes one parameter represnting which game is being played, four, five, six or
        //seven letter word game
        private int getrowCount(int statementSelect)
        {
            int count = -1; 
            
            //Depending on which game is being played
            switch(statementSelect)
            {
                //get how many rows are in the highscore table
                case 0:
                    count = con.Query<HighScore>("select * from FourLetterHighScores").Count;
                    break;
                case 1:
                    count = con.Query<HighScore>("select * from FiveLetterHighScores").Count;
                    break;
                case 2:
                    count = con.Query<HighScore>("select * from SixLetterHighScores").Count;
                    break;
                case 3:
                    count = con.Query<HighScore>("select * from SevenLetterHighScores").Count;
                    break;
            }

            return count; //return the row count
         }

        //Adds a new user high score to the highscore table
        private void btnSaveScore_Click(object sender, RoutedEventArgs e)
        {
            String query = "";              //string to hold the query
            name = txtUserName.Text;        //get user name that has been entered

            //Use a switch statement to assign a query to insert highscores depending on which game is being played
            switch(gameType)
            {
                //eg if its a four letter game we will use the FourLetterHighScores table
                case 0:
                    query = "insert into FourLetterHighScores (Id, Score, Name) VALUES (" + (id + 1) + "," + score + ",'" + name + "')";
                    break;
                case 1:
                    query = "insert into FiveLetterHighScores (Id, Score, Name) VALUES (" + (id + 1) + "," + score + ",'" + name + "')";
                    break;
                case 2:
                    query = "insert into SixLetterHighScores (Id, Score, Name) VALUES (" + (id + 1) + "," + score + ",'" + name + "')";
                    break;
                case 3:
                    query = "insert into SevenLetterHighScores (Id, Score, Name) VALUES (" + (id + 1) + "," + score + ",'" + name + "')";
                    break;
            }

            //Open a connection to the database
            using (var con = new SQLiteConnection("GameDatabase.db"))
            {
                //execute the query
                var existingScore = con.Query<HighScore>(query).FirstOrDefault();
                if (existingScore != null)
                {
                    con.RunInTransaction(() =>
                    {
                        //insert data into the database
                        con.Insert(existingScore);
                    });
                }
            }

            Frame.Navigate(typeof(MainPage)); //Navigate back to main menu, game is now over, and high score has been entered
        }
    }
}
