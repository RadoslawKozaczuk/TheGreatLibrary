using System;
using System.Text;
using static System.Console;

namespace DesignPatterns.Structural
{
	/*
		Motivation:
		Want to augment an object with additional functionality.
		Do not want to rewrite or alter existing code (Open-Closed Principle).
		Want to keep new functionality separate (Single Responsibility Principle).
		Need to be able to interact with existing structures.
		In such case we have to options:
			- Inherit from required object if possible; sometimes not possible due to object being sealed
			- Build a Decorator, which simply references the decorated object(s)

		Definition:
		Facilitates the addition of behaviors to individual objects without inheriting from them.
	*/
	class Decorator
	{
		#region StringBuilder decorator (with a bit of Adapter)
		// return builder.(.+)$
		// builder.$1\nreturn this;
		// format document
		//
		// A decorator keeps the reference to the decorated object(s)
		// A decorator may or may not replicate the API of the original object 
		// you cannot cast the decorated to original object because there is no cast
		// so we don't need to override all the members
		//	there is no inheritance
		// Exists in a static variation
		//	X<Y<Foo>>
		class CodeBuilder
		{
			readonly StringBuilder _builder = new StringBuilder();
			int _indentation;

			public override string ToString() => _builder.ToString();

			// these two methods implements Adapter pattern
			public static implicit operator CodeBuilder(string s)
			{
				var codeBuilder = new CodeBuilder();
				codeBuilder._builder.Append(s);
				return codeBuilder;
			}

			public static CodeBuilder operator +(CodeBuilder codeBuilder, string s)
			{
				codeBuilder.Append(s);
				return codeBuilder;
			}
			
			public string ToString(int startIndex, int length) => _builder.ToString(startIndex, length);

			public CodeBuilder Clear()
			{
				_builder.Clear();
				return this;
			}

			// its up to us which methods we are going to implement

			public CodeBuilder Append(string value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(string value, int startIndex, int count)
			{
				_builder.Append(value, startIndex, count);
				return this;
			}

			public CodeBuilder AppendLine()
			{
				_builder.AppendLine();
				return this;
			}

			public CodeBuilder AppendLine(string value)
			{
				_builder.AppendLine(value);
				return this;
			}
			
			public CodeBuilder Insert(int index, string value, int count)
			{
				_builder.Insert(index, value, count);
				return this;
			}
			
			public CodeBuilder Append(int value)
			{
				_builder.Append(value);
				return this;
			}
			
			public CodeBuilder Insert(int index, string value)
			{
				_builder.Insert(index, value);
				return this;
			}
			
			public CodeBuilder Insert(int index, int value)
			{
				_builder.Insert(index, value);
				return this;
			}
			
			public bool Equals(CodeBuilder sb)
			{
				return _builder.Equals(sb);
			}

			public CodeBuilder Replace(string oldValue, string newValue, int startIndex, int count)
			{
				_builder.Replace(oldValue, newValue, startIndex, count);
				return this;
			}

			public CodeBuilder Replace(char oldChar, char newChar)
			{
				_builder.Replace(oldChar, newChar); return this;
			}

			public CodeBuilder Replace(char oldChar, char newChar, int startIndex, int count)
			{
				_builder.Replace(oldChar, newChar, startIndex, count);
				return this;
			}

			public int Capacity
			{
				get => _builder.Capacity;
				set => _builder.Capacity = value;
			}

			public int MaxCapacity => _builder.MaxCapacity;

			public int Length
			{
				get => _builder.Length;
				set => _builder.Length = value;
			}

			public char this[int index]
			{
				get => _builder[index];
				set => _builder[index] = value;
			}

			void IndentationCheck(string s)
			{
				if (s == "{")
					_indentation += 2;
				else if (s == "{")
					_indentation -= 2;
			}
		}
		#endregion

		// StringBuilder decorated with additional functionality (indentation - although it doesn't work)
		// Also adapter pattern used to add additional interface
		public static void Demo()
		{
			CodeBuilder cbbb = "Adapter functionality test, ";
			cbbb += "so much adapter everywhere";
			WriteLine(cbbb);

			var cb = new CodeBuilder();
			cb.AppendLine("class Foo")
				.AppendLine("{")
				.AppendLine("int i = 0;")
				.AppendLine("}");
			WriteLine(cb);
		}

		#region Multiple Inheritance
		interface IBird
		{
			int Weight { get; set; }
			void Fly();
		}

		interface ILizard
		{
			int Weight { get; set; }
			void Crawl();
		}

		class Bird : IBird
		{
			public int Weight { get; set; }

			public void Fly() => WriteLine($"Soaring in the sky with weight {Weight}");
		}

		class Lizard : ILizard
		{
			public int Weight { get; set; }

			public void Crawl() => WriteLine($"Crawling in the dirt with weight {Weight}");
		}

		class Dragon
		{
			public int Weight
			{ 
				get { return _weight; }
				set
				{
					bird.Weight = value;
					lizard.Weight = value;
					_weight = value;
				}
			}
			int _weight;

			Bird bird = new Bird();
			Lizard lizard = new Lizard();

			public Dragon(int weigth)
			{
				Weight = weigth;
			}
			
			public void Crawl() => lizard.Crawl();

			public void Fly() => bird.Fly();
		}
		#endregion

		// by using Decorator we can somehow walk around multiple inheritance limitation 
		public static void MultipleInheritanceDemo()
		{
			var d = new Dragon(100);
			d.Fly();
			d.Crawl();
		}

		#region Dynamic Decorators
		abstract class Shape
		{
			public virtual string AsString() => string.Empty;
		}

		class Circle : Shape
		{
			float radius;

			public Circle() : this(0)
			{
			}

			public Circle(float radius) => this.radius = radius;

			public void Resize(float factor) => radius *= factor;	

			public override string AsString() => $"A circle of radius {radius}";
		}

		class Square : Shape
		{
			float side;

			public Square() : this(0)
			{
			}

			public Square(float side)
			{
				this.side = side;
			}

			public override string AsString() => $"A square with side {side}";
		}

		// dynamic
		class ColoredShape : Shape
		{
			Shape shape;
			string color;

			public ColoredShape(Shape shape, string color)
			{
				this.shape = shape ?? throw new ArgumentNullException(paramName: nameof(shape));
				this.color = color ?? throw new ArgumentNullException(paramName: nameof(color));
			}

			public override string AsString() => $"{shape.AsString()} has the color {color}";
		}

		class TransparentShape : Shape
		{
			Shape shape;
			float transparency;

			public TransparentShape(Shape shape, float transparency)
			{
				this.shape = shape ?? throw new ArgumentNullException(paramName: nameof(shape));
				this.transparency = transparency;
			}

			public override string AsString() => $"{shape.AsString()} has {transparency * 100.0f} transparency";
		}

		// CRTP cannot be done
		//public class ColoredShape2<T> : T where T : Shape { }
		class ColoredShape<T> : Shape where T : Shape, new()
		{
			// we have to use aggregation
			string color;

			// 
			T shape = new T();

			public ColoredShape() : this("black")
			{

			}

			public ColoredShape(string color) // no constructor forwarding
			{
				this.color = color ?? throw new ArgumentNullException(paramName: nameof(color));
			}

			public override string AsString() => $"{shape.AsString()} has the color {color}";
		}

		class TransparentShape<T> : Shape where T : Shape, new()
		{
			float transparency;
			T shape = new T();

			public TransparentShape(float transparency)
			{
				this.transparency = transparency;
			}

			public override string AsString() => $"{shape.AsString()} has transparency {transparency * 100.0f}";
		}
		#endregion

		public static void DynamicDecoratorsDemo()
		{
			// nothing prevents us from applying one decorator over another
			// this approach works and we can apply new decorators at runtime
			var square = new Square(1.23f);
			WriteLine(square.AsString());

			var redSquare = new ColoredShape(square, "red");
			WriteLine(redSquare.AsString());

			var redHalfTransparentSquare = new TransparentShape(redSquare, 0.5f);
			WriteLine(redHalfTransparentSquare.AsString());

			// static - although here we lose possibility to change the radius/side
			var blueCircle = new ColoredShape<Circle>("blue");
			WriteLine(blueCircle.AsString());
			
			var blackHalfSquare = new TransparentShape<ColoredShape<Square>>(0.4f);
			WriteLine(blackHalfSquare.AsString());
		}
	}
}
