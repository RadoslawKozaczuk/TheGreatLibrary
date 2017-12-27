using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace DesignPatterns.Structural
{
	/*
		Motivation:
		Avoid redundancy when storing data.
		Plenty of users with identical first/last names - no sense in storing the same name over and over again.
		Store a list of names and pointers to them.
		.NET performs string interning, so an identical string is stored only once
		
		Definition:
		A space optimization technique that lets us use less memory by storing externally the data 
		associated with similar objects.
	*/
	class Flyweight
    {
		class FormattedText
		{
			readonly string _plainText;

			public FormattedText(string plainText)
			{
				_plainText = plainText;
				_capitalize = new bool[plainText.Length];
			}

			public void Capitalize(int start, int end)
			{
				for (int i = start; i <= end; ++i)
					_capitalize[i] = true;
			}

			readonly bool[] _capitalize;

			public override string ToString()
			{
				var sb = new StringBuilder();
				for (var i = 0; i < _plainText.Length; i++)
				{
					var c = _plainText[i];
					sb.Append(_capitalize[i] ? char.ToUpper(c) : c);
				}
				return sb.ToString();
			}
		}

		class BetterFormattedText
		{
			readonly string _plainText;
			readonly List<TextRange> _formatting = new List<TextRange>();

			public BetterFormattedText(string plainText) => _plainText = plainText;

			// here we return our Flyweight object
			public TextRange GetRange(int start, int end)
			{
				var range = new TextRange { Start = start, End = end };
				_formatting.Add(range);
				return range;
			}

			public override string ToString()
			{
				var sb = new StringBuilder();

				for (var i = 0; i < _plainText.Length; i++)
				{
					var c = _plainText[i];
					foreach (var range in _formatting)
						if (range.Covers(i) && range.Capitalize)
							c = char.ToUpperInvariant(c);
					sb.Append(c);
				}

				return sb.ToString();
			}

			// this is our Flyweight object
			public class TextRange
			{
				public int Start, End;
				public bool Capitalize, Bold, Italic;

				public bool Covers(int position) => position >= Start && position <= End;
			}
		}

		// instead of operating on single chars we say we want to capitalize a particular range
		// and that is the essence of Flyweight patter
	    public static void Demo()
	    {
		    var ft = new FormattedText("This is a brave new world");
		    ft.Capitalize(10, 15);
		    WriteLine(ft);

		    var bft = new BetterFormattedText("This is a brave new world");
		    bft.GetRange(10, 15).Capitalize = true;
		    WriteLine(bft);
		}
    }
}
