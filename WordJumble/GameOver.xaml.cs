using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        private SQLiteConnection con;
        private ScoreInformationPasser scoreHolder;
        private int gameType;
        private int id;
        private string name;
        private int score;

        public GameOver()
        {
            this.InitializeComponent();
            copyDatabase();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            con = new SQLiteConnection("GameDatabase.db");
            con.CreateTable<HighScore>();
            scoreHolder = e.Parameter as ScoreInformationPasser;
            score = scoreHolder.score;
            gameType = scoreHolder.gameType;
            id = getrowCount(gameType);
            txtScore.Text = score.ToString();
        }

        private async void copyDatabase()
        {
            bool isDatabaseExisting = false;

            try
            {
                StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync("GameDatabase.db");
                isDatabaseExisting = true;
            }
            catch
            {
                isDatabaseExisting = false;
            }

            if (isDatabaseExisting)
            {
                StorageFile databaseFile = await Package.Current.InstalledLocation.GetFileAsync("GameDatabase.db");
                await databaseFile.CopyAsync(ApplicationData.Current.LocalFolder);
            }
        }

        private int getrowCount(int statementSelect)
        {
            int count = -1; 
            
            switch(statementSelect)
            {
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

            return count;
         }

        private void btnSaveScore_Click(object sender, RoutedEventArgs e)
        {
            String query = "";
            name = txtUserName.Text;

            switch(gameType)
            {
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

            
            using (var con = new SQLiteConnection("GameDatabase.db"))
            {
                var existingScore = con.Query<HighScore>(query).FirstOrDefault();
                if (existingScore != null)
                {
                    con.RunInTransaction(() =>
                    {
                        con.Insert(existingScore);
                    });
                }
            }

            Frame.Navigate(typeof(MainPage));
        }
    }
}
