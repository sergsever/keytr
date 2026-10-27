using System.IO;
using System.Text;

public interface NextLetterStrategy
{
    char NextLetter();
}

public class NextLetterFromString : NextLetterStrategy
{
    private  int _pos;
    private string _text;
    public char NextLetter() {
        return _text[_pos++];
    }
    public NextLetterFromString(string path)
    {
        _text = File.ReadAllText(path);
    }
}

public class NextLetterRandom : NextLetterStrategy
{
    private readonly string letters = "aliops;mxz";
    public char NextLetter()
    {
        var random = new Random();
        return letters[random.Next(letters.Length)];

    }
    public class Lesson
    {
        public string Title { get; set; }
        public int Pause { get; set; }
        public int Repeate { get; set; }

    }
}

