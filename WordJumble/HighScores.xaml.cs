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
        private int rank;

        public HighScores()
        {
            this.InitializeComponent();
            copyDatabase();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<HighScore> tasks;
            con = new SQLiteConnection("GameDatabase.db");
            con.CreateTable<HighScore>();

            dataHolder = e.Parameter as DataPasser;
            getListOfScores(dataHolder.data, out tasks);
          
            for (rank = 0; rank < tasks.Count(); ++rank)
            {
                HighScore currentInList = tasks.ElementAt(rank);
                currentInList.Id = (rank + 1);
                tasks.RemoveAt(rank);
                tasks.Insert(rank, currentInList);
            }

            gameHighScoreList.ItemsSource = tasks;
        }

        private void getListOfScores(int statementSelect, out List<HighScore> tasks)
        {
            tasks = null;

            switch (statementSelect)
            {
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
