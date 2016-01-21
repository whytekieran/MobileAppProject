using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class HighScores : Page
    {
        private SQLiteConnection con;
        private DataPasser dataHolder;

        public HighScores()
        {
            copyDatabase();
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<HighScore> tasks;
            con = new SQLiteConnection("GameDatabase.db");
            con.CreateTable<HighScore>();

            dataHolder = e.Parameter as DataPasser;

            switch(dataHolder.data)
            {
                case 0:
                    tasks = con.Query<HighScore>("select * from FourLetterHighScores order by score").ToList<HighScore>();
                    gameHighScoreList.ItemsSource = tasks;
                    break;
                case 1:
                    tasks = con.Query<HighScore>("select * from FiveLetterHighScores order by score").ToList<HighScore>();
                    gameHighScoreList.ItemsSource = tasks;
                    break;
                case 2:
                    tasks = con.Query<HighScore>("select * from SixLetterHighScores order by score").ToList<HighScore>();
                    gameHighScoreList.ItemsSource = tasks;
                    break;
                case 3:
                    tasks = con.Query<HighScore>("select * from SevenLetterHighScores order by score").ToList<HighScore>();
                    gameHighScoreList.ItemsSource = tasks;
                    break;
            }
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
    }
}
