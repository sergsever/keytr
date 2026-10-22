using Keytr.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Keytr.ViewModels
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void OnKeyDown(object sender, TextCompositionEventArgs args)
        {
            Debug.WriteLine("KeyDown: " + args.Text);



            var equal = (this.Letter.ToLower() == args.Text.ToLower());//this.Letter.ToUpper().Equals(keyEventArgs.Key);
            Debug.WriteLine(@"Key equal: ", equal);
            if (this.Letter.Equals(args.Text))
            {
                
                LetterColor = new SolidColorBrush(Colors.Green);
            }
            else
            {
                LetterColor = new SolidColorBrush(Colors.Red);

            }
            this.LetterColor.Freeze();
            OnPropertyChanged("LetterColor");
        }

        public string? Title { get; set; }


        public string? Letter { get; set; }

        public SolidColorBrush LetterColor { get; set; }

        private TrainModel _trainModel;

        private System.Timers.Timer _timer;

        public void Tick(object? sender, EventArgs e)
        {
            this.Letter = _trainModel.NextLetter().ToString();
            this.OnPropertyChanged("Letter");
            this.LetterColor = new SolidColorBrush(Colors.White);
            this.LetterColor.Freeze();
            this.OnPropertyChanged("LetterColor");
        }

        public TrainVM TrainModel { get; set; }
        public MainVM()
        {
            _trainModel = new TrainModel();
            this._trainModel.LoadLesson("Random");
            this.Title = "Random";
            this.OnPropertyChanged("Title");
            this.Letter = _trainModel.NextLetter().ToString();
            int pause = int.Parse(ConfigurationManager.AppSettings["Pause"]);
            this._timer = new System.Timers.Timer(pause * 1000);
            this._timer.Elapsed += Tick;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            this.LetterColor = new SolidColorBrush(Colors.White);
            this.LetterColor.Freeze();
            OnPropertyChanged("LetterColor");
            //_timer.Start();

            OnPropertyChanged("TrainModel");
        }
    }
}
