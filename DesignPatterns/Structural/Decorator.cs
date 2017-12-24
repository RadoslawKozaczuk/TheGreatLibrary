using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
		// return builder.(.+)$
		// builder.$1\nreturn this;
		// format document
		public class CodeBuilder
		{
			readonly StringBuilder _builder = new StringBuilder();
			int _indentation;

			public override string ToString()
			{
				return _builder.ToString();
			}

			// these two methods implements Adapter pattern
			public static implicit operator CodeBuilder(string s)
			{
				var codeBuilder = new CodeBuilder();
				if (s == "{")
					codeBuilder._indentation += 2;
				else if (s == "{")
					codeBuilder._indentation -= 2;
				codeBuilder._builder.Append(s);
				return codeBuilder;
			}

			public static CodeBuilder operator +(CodeBuilder codeBuilder, string s)
			{
				codeBuilder.Append(s);
				return codeBuilder;
			}
			
			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				((ISerializable)_builder).GetObjectData(info, context);
			}

			public int EnsureCapacity(int capacity)
			{
				return _builder.EnsureCapacity(capacity);
			}

			public string ToString(int startIndex, int length)
			{
				return _builder.ToString(startIndex, length);
			}

			public CodeBuilder Clear()
			{
				_builder.Clear();
				return this;
			}

			public CodeBuilder Append(char value, int repeatCount)
			{
				_builder.Append(value, repeatCount);
				return this;
			}

			public CodeBuilder Append(char[] value, int startIndex, int charCount)
			{
				_builder.Append(value, startIndex, charCount);
				return this;
			}

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
				IndentationCheck(value);
				return this;
			}

			public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
			{
				_builder.CopyTo(sourceIndex, destination, destinationIndex, count);
			}

			public CodeBuilder Insert(int index, string value, int count)
			{
				_builder.Insert(index, value, count);
				return this;
			}

			public CodeBuilder Remove(int startIndex, int length)
			{
				_builder.Remove(startIndex, length);
				return this;
			}

			public CodeBuilder Append(bool value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(sbyte value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(byte value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(char value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(short value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(int value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(long value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(float value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(double value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(decimal value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(ushort value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(uint value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(ulong value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(object value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Append(char[] value)
			{
				_builder.Append(value);
				return this;
			}

			public CodeBuilder Insert(int index, string value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, bool value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, sbyte value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, byte value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, short value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, char value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, char[] value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, char[] value, int startIndex, int charCount)
			{
				_builder.Insert(index, value, startIndex, charCount);
				return this;
			}

			public CodeBuilder Insert(int index, int value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, long value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, float value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, double value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, decimal value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, ushort value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, uint value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, ulong value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder Insert(int index, object value)
			{
				_builder.Insert(index, value);
				return this;
			}

			public CodeBuilder AppendFormat(string format, object arg0)
			{
				_builder.AppendFormat(format, arg0);
				return this;
			}

			public CodeBuilder AppendFormat(string format, object arg0, object arg1)
			{
				_builder.AppendFormat(format, arg0, arg1);
				return this;
			}

			public CodeBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
			{
				_builder.AppendFormat(format, arg0, arg1, arg2);
				return this;
			}

			public CodeBuilder AppendFormat(string format, params object[] args)
			{
				_builder.AppendFormat(format, args);
				return this;
			}

			public CodeBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
			{
				_builder.AppendFormat(provider, format, args);
				return this;
			}

			public CodeBuilder Replace(string oldValue, string newValue)
			{
				_builder.Replace(oldValue, newValue);
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
				_builder.Replace(oldChar, newChar);				return this;
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
		
		public static void Demo()
		{
			var cb = new CodeBuilder();
			cb.AppendLine("class Foo")
				.AppendLine("{")
				.AppendLine("int i = 0;")
				.AppendLine("}");
			WriteLine(cb);
		}

		public static void MultiInheritanceDemo()
		{

		}

		#region Dynamic Decorators
		public abstract class Shape
		{
			public virtual string AsString() => string.Empty;
		}

		public class Circle : Shape
		{
			float radius;

			public Circle() : this(0)
			{

			}

			public Circle(float radius)
			{
				this.radius = radius;
			}

			public void Resize(float factor) => radius *= factor;	

			public override string AsString() => $"A circle of radius {radius}";
		}

		public class Square : Shape
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
		public class ColoredShape : Shape
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

		public class TransparentShape : Shape
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
		public class ColoredShape<T> : Shape where T : Shape, new()
		{
			string color;
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

		public class TransparentShape<T> : Shape where T : Shape, new()
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
