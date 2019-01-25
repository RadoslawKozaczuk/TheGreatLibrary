using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		Many algorithms can be decomposed into higher and lower lever parts.
		Making tea can be decomposed into.
			The process of making a hot beverage (boil water, pour into cup).
			Tea-specific things (put teabag into water).
		The high-level algorithm can then be reused for making coffee or hot chocolate.
			Supported by beverage-specific strategies.

		Definition:
		Enables the exact behavior of a system to be selected either at run-time (dynamic)
		or compile-time (static).
		Also know as a policy (especially in the C++ world).
	*/
	class Strategy
    {
		#region "Common objects"
		enum OutputFormat
	    {
		    Markdown,
		    Html // list in HTML is <ul><li>boo</li></ul>
	    }
        
	    interface IListStrategy
	    {
		    void Start(StringBuilder sb); // opening tag
		    void End(StringBuilder sb); // closing tag
		    void AddListItem(StringBuilder sb, string item);
	    }

		// markdown does not need any opening and closing tag for a list
	    class MarkdownListStrategy : IListStrategy
	    {
		    public void Start(StringBuilder sb) { }
		    public void End(StringBuilder sb) { }
		    public void AddListItem(StringBuilder sb, string item) => sb.AppendLine($" * {item}");
	    }

	    class HtmlListStrategy : IListStrategy
	    {
		    public void Start(StringBuilder sb) => sb.AppendLine("<ul>");
		    public void End(StringBuilder sb) => sb.AppendLine("</ul>");
		    public void AddListItem(StringBuilder sb, string item) => sb.AppendLine($"  <li>{item}</li>");
	    }
		#endregion
		
		class TextProcessorDynamic
		{
			readonly StringBuilder _sb = new StringBuilder();
			IListStrategy _listStrategy;

			public void SetOutputFormat(OutputFormat format)
			{
				switch (format)
				{
					case OutputFormat.Markdown:
						_listStrategy = new MarkdownListStrategy();
						break;
					case OutputFormat.Html:
						_listStrategy = new HtmlListStrategy();
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(format), format, null);
				}
			}

			public void AppendList(IEnumerable<string> items)
			{
				_listStrategy.Start(_sb);

				foreach (var item in items)
					_listStrategy.AddListItem(_sb, item);

				_listStrategy.End(_sb);
			}

			public StringBuilder Clear() => _sb.Clear();

			public override string ToString() => _sb.ToString();
		}

		// strategy allows us to change the part of the solution by substituting a particular component
		public static void DynamicStrategyDemo()
		{
			var tp = new TextProcessorDynamic();
			tp.SetOutputFormat(OutputFormat.Markdown);
			tp.AppendList(new[] { "foo", "bar", "baz" });
			WriteLine(tp);

			tp.Clear();
			tp.SetOutputFormat(OutputFormat.Html);
			tp.AppendList(new[] { "foo", "bar", "baz" });
			WriteLine(tp);
		}

		// static means that we assume we always now the output format at the compile time
		// where LS : IListStrategy - LS is type of IListStrategy
		// new() - LS has a default constructor
		class TextProcessorStatic<LS> where LS : IListStrategy, new()
	    {
		    readonly StringBuilder _sb = new StringBuilder();
		    readonly IListStrategy _listStrategy = new LS();

		    public void AppendList(IEnumerable<string> items)
		    {
			    _listStrategy.Start(_sb);
			    foreach (var item in items)
				    _listStrategy.AddListItem(_sb, item);
			    _listStrategy.End(_sb);
		    }

		    public StringBuilder Clear() => _sb.Clear();

			public override string ToString() => _sb.ToString();
	    }

		// Define an algorithm at a high level.
		// Define the interface you expect each strategy to follow.
		// Provide for either dynamic or static composition of strategy in the overall algorithm.
	    public static void StaticStrategyDemo()
		{ 
			var tp = new TextProcessorStatic<MarkdownListStrategy>();
			tp.AppendList(new[] { "foo", "bar", "baz" });
			WriteLine(tp);

			// unfortunately we cannot assign a new one with a different ListStrategy
			// tp = new TextProcessorStatic<HtmlListStrategy>();
			// because tp is already type of TextProcessorStatic<MarkdownListStrategy>

			// also we cannot make tp type of TextProcessorStatic<IListStrategy>
			// TextProcessorStatic<IListStrategy> tp = TextProcessorStatic<MarkdownListStrategy>();
			// because that would violate the default constructor constraint

			var tp2 = new TextProcessorStatic<HtmlListStrategy>();
			tp2.AppendList(new[] { "foo", "bar", "baz" });
			WriteLine(tp2);
	    }
	}
}
