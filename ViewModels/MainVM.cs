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
        public string? PerLetterTime {  get; set; }
        public string? Errors { get; set; }
        public List<string> Lessons { get; set; }

        public ICommand StopComand { get; }

        public int SelectedLesson
        {
            get { return _selectedLesson; }
            set
            {
                if (_selectedLesson != value) {
                    _selectedLesson = value;
                    OnSeelectedLessonChanged(_selectedLesson);
                }
            }
        } 

        private TrainModel _trainModel;
        private DateTime _start;
        private DateTime _startLetter;
        private TimeSpan _timePerLetter = TimeSpan.FromMilliseconds(0);
        private int _count = 0;
        private int _pause = 0;
        private int _letterCount = 0;

        private System.Timers.Timer _timer;
        private int _countletters = 0;
        private string _entered = "";
        private int _selectedLesson = 0;
        private bool _isInProcess = false;
        private LangHelper.Languages _lang;


        private readonly Dictionary<string, SolidColorBrush> _errcolors = new Dictionary<string, SolidColorBrush>()
        {
            {"white", new SolidColorBrush(Colors.White) },
            { "green", new SolidColorBrush(Colors.Green)},
            {"red", new SolidColorBrush(Colors.Red) } };
        private Dictionary<string, int> _errors= new Dictionary<string, int>();

        private Lock changeLetter = new Lock();

        public void Tick(object? sender, EventArgs e)
        {
            changeLetter.Enter();
            if (!_isInProcess)
            {
                this.Letter = _trainModel.NextLetter().ToString();
                Debug.WriteLine("Letter: " +  this.Letter);
                this.OnPropertyChanged("Letter");
                _isInProcess = true;
                this._countletters++;
            }
            this.LetterColor = this._errcolors["white"];
            OnPropertyChanged("LetterColor");
            //this.LetterColor.Freeze();
            this.OnPropertyChanged("LetterColor");
            this.KeyError = _errcolors["green"];
            OnPropertyChanged("KeyError");

            this._startLetter = DateTime.Now;
            this._letterCount = 0;
            var stop = DateTime.Now;
            TimeSpan elapsed = stop - _start;
            ElapsedTime = "Время: " + elapsed.ToString(@"hh\:mm\:ss");
            OnPropertyChanged("ElapsedTime");
            changeLetter.Exit();
        }

        public void OnKeyDown(object sender, TextCompositionEventArgs args)
        {
            Debug.WriteLine("KeyDown: " + args.Text);
            //var equal = (this.Letter.ToLower() == args.Text.ToLower());//this.Letter.ToUpper().Equals(keyEventArgs.Key);
            //Debug.WriteLine(@"Key equal: ", equal);
            if (this.Letter.Equals(args.Text))
            {
                _isInProcess = false;
                this._timePerLetter += (DateTime.Now - _startLetter);

                KeyError = _errcolors["green"];//new SolidColorBrush(Colors.Green);
                this._entered += args.Text;
                if (_entered.Length > 150)
                {
                    _entered = _entered.Substring(_entered.Length - 5);
                }
                Entered = _entered;
                OnPropertyChanged("Entered");
                this.Tick(null,null);
                
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
                    _isInProcess = false;
                    LetterColor = _errcolors["red"]; //new SolidColorBrush(Colors.Red);
                }
                this.OnPropertyChanged("KeyError");
            }
            //changeLetter.Exit();
        }

            

            private void Stop() {
            if (this._timer.Enabled)
            {
                this._timer.Enabled = false;
            }
            else
            {
                _timer.Enabled = true;
            }
            this.PerLetterTime =  _countletters == 0 ? "0" : (_timePerLetter / _countletters).TotalSeconds.ToString("0.00");
            OnPropertyChanged("PerLetterTime");
            this.Errors = _errors.Sum(e => e.Value).ToString() + "/" + _countletters.ToString() + " : " + Math.Floor((Math.Ceiling((double)(_errors.Sum(e => e.Value))) / _countletters) * 100).ToString() + "%";
            OnPropertyChanged("Errors");

            }

        private void OnSeelectedLessonChanged(int index)
        {
            if (_timer.Enabled)
            {
                _timer.Enabled = false;
            }
            string lesson = Lessons[index];
            _trainModel = new TrainModel();
            _trainModel.LoadLesson(lesson, _lang);

        }
        

        //public TrainVM TrainModel { get; set; }
        public MainVM()
        {
            this._count = 0;
            this._countletters = 0;
            this._letterCount = 0;
            _trainModel = new TrainModel();
            this.KeyError = _errcolors["green"];
            var lang = LangHelper.GetKeyboardLayout();
            if (lang == ((ushort)LangHelper.Languages.English))
            {
                _lang = LangHelper.Languages.English;
                Debug.WriteLine("Lang:English");
                this._trainModel.LoadLesson("Random", LangHelper.Languages.English);

            }
            else
            {
                _lang = LangHelper.Languages.Russian;
                Debug.WriteLine("Lang: Russian");
                this._trainModel.LoadLesson("Random", LangHelper.Languages.Russian);
                
            }
            this.Title = "Random";
            this.OnPropertyChanged("Title");
            _pause = 5;// int.Parse(ConfigurationManager.AppSettings["Pause"]);
            this._count = 3;// int.Parse(ConfigurationManager.AppSettings["Repeat"]);
            
            this.StopComand = new RelayCommand(Stop);
            this.LetterColor = _errcolors["white"];
            //this.LetterColor.Freeze();
            
            OnPropertyChanged("LetterColor");
            _start = DateTime.Now;

            OnPropertyChanged("TrainModel");
            /*lessons*/
            Lessons = new List<string>();
            LessonsConfigurationSection lessons = (LessonsConfigurationSection)ConfigurationManager.GetSection("lessonsSection");
            Debug.WriteLine("Lessons: " + lessons == null ? "Null": lessons);

            foreach (LessonElement lesson in lessons.LessonItems)
            {
                Debug.WriteLine("Lesson: " + lesson.LessonType + ", " + lesson.Path);
                Lessons.Add(lesson.Path);
            }

            OnPropertyChanged("Lessons");
            /*Timer*/
            this._timer = new System.Timers.Timer(_pause * 1000);

            this._timer.Elapsed += Tick;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }
    }
}
