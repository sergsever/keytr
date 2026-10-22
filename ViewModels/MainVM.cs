using Keytr.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Windows.Input;

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

        public void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            Debug.WriteLine("KeyDown: " +  keyEventArgs.Key);
        }

        public string? Title { get; set; }


        public string? Letter { get; set; }

        private TrainModel _trainModel;

        private System.Timers.Timer _timer;

        public void Tick(object? sender, EventArgs e)
        {
            this.Letter = _trainModel.NextLetter().ToString();
            Debug.WriteLine("Letter: " + this.Letter);
            this.OnPropertyChanged("Letter");
        }

        public TrainVM TrainModel { get; set; }
        public MainVM()
        {
            _trainModel = new TrainModel();
            this._trainModel.LoadLesson("Random");
            this.Title = "Random";
            this.OnPropertyChanged("Title");
            int pause = int.Parse(ConfigurationManager.AppSettings["Pause"]);
            this._timer = new System.Timers.Timer(pause * 1000);
            this._timer.Elapsed += Tick;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
            OnPropertyChanged("TrainModel");
        }
    }
}
