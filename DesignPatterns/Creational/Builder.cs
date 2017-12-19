using System.Collections.Generic;
using System.Text;
using static System.Console;

namespace DesignPatterns.Creational
{
	/*
		Some objects are simple and can be created in a single constructor call.
		Other require a lot of ceremony to create.
		Having an object with 10 constructor arguments is not productive.
		Instead, opt for piecewise construction.
		Builder provides an API for constructing an object step-by-step.

		Definition of the Builder pattern:
		When piecewise object construction is complicated, provide an API for doing it succinctly.
	*/
	internal class HtmlElement
	{
		public string Name, Text;
		public List<HtmlElement> Elements = new List<HtmlElement>();
		private const int IndentSize = 2;

		public HtmlElement()
		{

		}

		public HtmlElement(string name, string text)
		{
			Name = name;
			Text = text;
		}

		private string ToStringImpl(int indent)
		{
			var sb = new StringBuilder();
			var i = new string(' ', IndentSize * indent);
			sb.Append($"{i}<{Name}>\n");
			if (!string.IsNullOrWhiteSpace(Text))
			{
				sb.Append(new string(' ', IndentSize * (indent + 1)));
				sb.Append(Text);
				sb.Append("\n");
			}

			foreach (var e in Elements)
				sb.Append(e.ToStringImpl(indent + 1));

			sb.Append($"{i}</{Name}>\n");
			return sb.ToString();
		}

		public override string ToString() => ToStringImpl(0);
	}

	internal class HtmlBuilder
	{
		private readonly string _rootName;
		private HtmlElement _root = new HtmlElement();

		public HtmlBuilder(string rootName)
		{
			_rootName = rootName;
			_root.Name = rootName;
		}

		// not fluent
		public void AddChild(string childName, string childText) 
			=> _root.Elements.Add(new HtmlElement(childName, childText));

		// we return HtmlBuilder here to allow fluent interface (call several calls like Append().Append()).
		public HtmlBuilder AddChildFluent(string childName, string childText)
		{
			_root.Elements.Add(new HtmlElement(childName, childText));
			return this;
		}

		public override string ToString() => _root.ToString();

		public void Clear() => _root = new HtmlElement {Name = _rootName};
	}

	public class Builder
	{
		public static void Demo()
		{
			const string hello = "hello";
			const string world = "world";

			var sb = new StringBuilder();
			var words = new[] {hello, world};
			sb.Append("<ul>");

			foreach (var word in words)
				sb.AppendFormat("<li>{0}</li>", word);

			sb.Append("</ul>");
			WriteLine(sb);

			// fluent builder
			var builder = new HtmlBuilder("ul");
			builder.AddChildFluent("li", hello).AddChildFluent("li", world);
			WriteLine(builder);
		}
	}
}
