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

	public class Builder
	{
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
			readonly string _rootName;
			HtmlElement _root = new HtmlElement();

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

			public void Clear() => _root = new HtmlElement { Name = _rootName };
		}

		public static void BuilderDemo()
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


		// we want several builder to take care of the process
		// facade for address and facade for employment
		class Person
		{
			public string StreetAddress, Postcode, City;
			public string CompanyName, Position;
			public int AnnualIncome;

			public override string ToString() =>
				$"{nameof(StreetAddress)}: {StreetAddress}, " +
				$"{nameof(Postcode)}: {Postcode}, " +
				$"{nameof(City)}: {City}, " +
				$"{nameof(CompanyName)}: {CompanyName}, " +
				$"{nameof(Position)}: {Position}, " +
				$"{nameof(AnnualIncome)}: {AnnualIncome}";
		}

		// it doesn't actually build anything by himself but stores a reference
		class PersonBuilder // facade (a component that hides functionality behind it)
		{
			// the object we're going to build
			protected Person Person = new Person(); // this is a reference! So it won't work with a struct

			public PersonAddressBuilder Lives => new PersonAddressBuilder(Person); // we expose the builder to make use of its methods
			public PersonJobBuilder Works => new PersonJobBuilder(Person); // we expose the builder to make use of its methods

			// to avoid Cannot implicitly convert type 'DesignPatterns.Creational.Builder.PersonJobBuilder' to 'DesignPatterns.Creational.Builder.Person'
			public static implicit operator Person(PersonBuilder pb) => pb.Person; // this is to allow us to use Person person = new PersonBuilder();
		}

		class PersonJobBuilder : PersonBuilder
		{
			public PersonJobBuilder(Person person)
			{
				Person = person;
			}

			public PersonJobBuilder At(string companyName)
			{
				Person.CompanyName = companyName;
				return this;
			}

			public PersonJobBuilder AsA(string position)
			{
				Person.Position = position;
				return this;
			}

			public PersonJobBuilder Earning(int annualIncome)
			{
				Person.AnnualIncome = annualIncome;
				return this;
			}
		}

		class PersonAddressBuilder : PersonBuilder
		{
			// might not work with a value type!
			public PersonAddressBuilder(Person person)
			{
				Person = person;
			}

			public PersonAddressBuilder At(string streetAddress)
			{
				Person.StreetAddress = streetAddress;
				return this;
			}

			public PersonAddressBuilder WithPostcode(string postcode)
			{
				Person.Postcode = postcode;
				return this;
			}

			public PersonAddressBuilder In(string city)
			{
				Person.City = city;
				return this;
			}
		}

		public static void FacadedBuilderDemo()
		{
			var pb = new PersonBuilder();
			Person person = pb
				.Lives
				.At("123 London Road")
				.In("London")
				.WithPostcode("SW12BC")
				.Works
				.At("Fabrikam")
				.AsA("Engineer")
				.Earning(123000);

			// of course you can do like this but this is just a side effect of making things convenient
			pb.Lives.Lives.Lives.At("123 Troll Street");

			WriteLine(person);
		}
	}
}
