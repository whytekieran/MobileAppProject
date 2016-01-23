using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI;
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
        private SQLiteConnection con;
        private int wordsID;
        private String jumbledWord;
        private String unJumbledWord;
        private string userWord;
        private DataPasser dataHolder;
        private int score = 0;
        Random randomNum = new Random();
        DispatcherTimer timer;                                                         //Used to launch a tick event
        Stopwatch stopWatch;
        private long mins;
        private long secs;

        public Game()
        {
            this.InitializeComponent();
            copyDatabase();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            timer = new DispatcherTimer();
            stopWatch = new Stopwatch();

            secs = 59;
            mins = 2;
            con = new SQLiteConnection("GameDatabase.db");
            con.CreateTable<Words>();

           txtScore.Text = score.ToString();
           dataHolder = e.Parameter as DataPasser;
           retreiveAndOutputWord(dataHolder.data);

           timer = new DispatcherTimer();
           timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
           timer.Tick += timerTick;
           timer.Start();
           stopWatch.Start();
        }

        private void timerTick(object sender, object e)
        {
            --secs;

            txtTimeDisplay.Text = mins.ToString() + ":" + secs.ToString();

            if(secs == 0)
            {
               secs = 59;
               --mins;

               txtTimeDisplay.Text = mins.ToString() + ":" + secs.ToString();

               if(mins < 0)
               {
                  timer.Stop();
                  stopWatch.Stop();
                  txtTimeDisplay.Text = "0" + ":" + "00";
                  MessageBoxDisplay();
                  Frame.Navigate(typeof(GameOver), new ScoreInformationPasser { score = score, gameType = dataHolder.data });
               }   
            }
        }

        private async void MessageBoxDisplay()
        {
            //Creating instance of MessageDialog and calling its show method to show a message box
            MessageDialog msgbox = new MessageDialog("Game Over, Score: "+score);    
            await msgbox.ShowAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (con != null)
                con.Close(); // Close the database connection.   
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

        private void retreiveAndOutputWord(int statementSelect)
        {
            wordsID = randomNum.Next(1, 200);
            retrieveWord(wordsID, statementSelect, out unJumbledWord);
            jumbledWord = new string(unJumbledWord.OrderBy(r => randomNum.Next()).ToArray()); //Shuffle the word
            txtUnJumbledWord.Text = unJumbledWord;
            txtJumbledWord.Text = jumbledWord;
        }

        private void retrieveWord(int id, int statementSelect, out string unJumbledWord)
        {
            //Retrieving Data
            Words result;
            switch(statementSelect)
            {
                 case 0:
                    result = con.Query<Words>("select * from FourLetterWords where id =" + id).FirstOrDefault();
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

        private void assignUnJumbledWord(Words result, out string unJumbledWord)
        {
            if (result == null)
            {
                unJumbledWord = "No Information Found";
            }
            else
            {
                unJumbledWord = result.Word;
            }
        }

        private void enterWordClick(object sender, RoutedEventArgs e)
        {
            unJumbledWord = txtUnJumbledWord.Text;
            userWord = txtEnteredWord.Text;
           
            if(userWord.Equals(unJumbledWord, StringComparison.OrdinalIgnoreCase))
            {
                //Match, correct guess give the user another word
                txtResult.Foreground = new SolidColorBrush(Colors.Green);
                txtResult.Text = "Correct";
                retreiveAndOutputWord(dataHolder.data);
                txtEnteredWord.Text = "";
                score += 4;
                txtScore.Text = score.ToString();
            }
            else
            {
                //No match, incorrect guess user must try again
                txtResult.Foreground = new SolidColorBrush(Colors.Red);
                txtResult.Text = "Incorrect";
                txtEnteredWord.Text = "";
            }
        }

        private void whenInUse(object sender, RoutedEventArgs e)
        {
            txtResult.Text = "";
        }
    }
}
