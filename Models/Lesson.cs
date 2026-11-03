using System.IO;
using System.Text;

public interface NextLetterStrategy
{
    char NextLetter(string lesson, int pos);
}

public class NextLetterFromString : NextLetterStrategy
{
    private  int _pos;
    private string _text;
    public char NextLetter() {
        return _text[_pos++];
    }

    public NextLetterFromString(string text, int pos)
    {
        this._text = text;
        this._pos = pos;
    }
    public char NextLetter(string lesson, int pos)
    {
        _pos = pos;
        if (string.IsNullOrEmpty(_text))
        {
            _text = lesson;
        }
        return _text[_pos++];

    }
}

public class NextLetterRandom : NextLetterStrategy
{
    private readonly string letters = "wolmpqxie";
    public char NextLetter(string lesson, int pos)
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

