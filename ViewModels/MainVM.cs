using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        
        

        public string? Title { get; set; }


        public string? Letter { get; set; }
        public string? ErrorLetter { get; set; }

        public SolidColorBrush LetterColor { get; set; }

        public SolidColorBrush KeyError { get; set; }
        public string? ElapsedTime { get; set; }
        public string? Entered {  get; set; }

        private TrainModel _trainModel;
        private DateTime _start;
        private DateTime _startLetter;
        private TimeSpan _timePerLetter = TimeSpan.FromMilliseconds(0);
        private int _count = 0;
        private int _letterCount = 0;

        private System.Timers.Timer _timer;
        private int _countletters = 0;
        private string _entered = "";
        private readonly Dictionary<string, SolidColorBrush> _errcolors = new Dictionary<string, SolidColorBrush>()
        {
            {"white", new SolidColorBrush(Colors.White) },
            { "green", new SolidColorBrush(Colors.Green)},
            {"red", new SolidColorBrush(Colors.Red) } };
        private Dictionary<string, int> _errors= new Dictionary<string, int>();

        public void Tick(object? sender, EventArgs e)
        {
            if (KeyError == _errcolors["green"] || this._letterCount >= _count)
            {
                this.Letter = _trainModel.NextLetter().ToString();
                Debug.WriteLine("Letter: " +  this.Letter);
                this.OnPropertyChanged("Letter");
                this._countletters++;
            }
            this.LetterColor = this._errcolors["white"];
            //this.LetterColor.Freeze();
            this.OnPropertyChanged("LetterColor");
            this._startLetter = DateTime.Now;
            this._letterCount = 0;
            var stop = DateTime.Now;
            TimeSpan elapsed = stop - _start;
            ElapsedTime = "Время: " + elapsed.ToString(@"hh\:mm\:ss");
            OnPropertyChanged("ElapsedTime");
        }

        public void OnKeyDown(object sender, TextCompositionEventArgs args)
        {
            Debug.WriteLine("KeyDown: " + args.Text);
            



            var equal = (this.Letter.ToLower() == args.Text.ToLower());//this.Letter.ToUpper().Equals(keyEventArgs.Key);
            //Debug.WriteLine(@"Key equal: ", equal);
            if (this.Letter.Equals(args.Text))
            {
                this._timePerLetter += (this._startLetter - DateTime.Now);

                LetterColor = _errcolors["green"];//new SolidColorBrush(Colors.Green);
                this._entered += args.Text;
                if (_entered.Length > 150)
                {
                    _entered = _entered.Substring(_entered.Length - 5);
                }
                this.Entered = this._entered;
                OnPropertyChanged("Entered");
                KeyError = _errcolors["green"];
                OnPropertyChanged("KeyError");
                ErrorLetter = "";
                OnPropertyChanged("ErrorLetter");
                Letter = "";
                OnPropertyChanged("Letter");
            }
            else
            {
                ErrorLetter = args.Text;
                KeyError = _errcolors["red"];
                OnPropertyChanged("KeyError");
                OnPropertyChanged("ErrorLetter");
                if (this._errors.ContainsKey(this.Letter))
                {
                    this._errors[this.Letter]++;
                }
                else
                {
                    this._errors.Add(this.Letter, 1);
                }
                if (_letterCount < _count)
                {
                    _letterCount++;
                }
                else
                {
                    LetterColor = _errcolors["red"]; //new SolidColorBrush(Colors.Red);
                }
                this.OnPropertyChanged("LetterColor");
            }
        }

            public ICommand StopComand{ get;}

            private void Stop() {
                this._timer.Enabled = false;

            }
        

        //public TrainVM TrainModel { get; set; }
        public MainVM()
        {
            this._count = 0;
            this._countletters = 0;
            this._letterCount = 0;
            this.KeyError = _errcolors["green"];
            _trainModel = new TrainModel();
            this._trainModel.LoadLesson("Random");
            this.Title = "Random";
            this.OnPropertyChanged("Title");
            this.Letter = _trainModel.NextLetter().ToString();
            int pause = int.Parse(ConfigurationManager.AppSettings["Pause"]);
            this._count = int.Parse(ConfigurationManager.AppSettings["Repeat"]);
            this._timer = new System.Timers.Timer(pause * 1000);

            this._timer.Elapsed += Tick;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            this.StopComand = new RelayCommand(Stop);
            this.LetterColor = _errcolors["white"];
            //this.LetterColor.Freeze();
            
            OnPropertyChanged("LetterColor");
            _start = DateTime.Now;

            OnPropertyChanged("TrainModel");
        }
    }
}
