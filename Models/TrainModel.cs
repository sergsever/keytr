using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Keytr.Models
{
    public enum LessonType
    {
        Random,
        Text
    }
    public class TrainModel
    {
        private LessonType type;

        private NextLetterStrategy? nextletter; 

        public char NextLetter()
        {
            return nextletter.NextLetter();
        }
        public string Name { get; set; }
        public List<char> Letters { get; set; }
        public int Pause { get; set; }
        public int Repeat { get; set; }
        public char Letter { get; set; }

        public void LoadLesson(string name)
        {
            //var path =  ConfigurationManager.GetSection("lessons");

            if (name.Equals("Random"))
            {
                this.type = LessonType.Random;
                nextletter = new NextLetterRandom();
            }
            else
            {
                this.type = LessonType.Text;
               // this.nextletter = new NextLetterFromString();
            }
            this.Letter = nextletter.NextLetter();
        }
    }
}
