using System;
using System.Text;
using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		Need to define a new operation on an entire class hierarchy.
		We do not want to keep modifying every class in the hierarchy.
		Need access to the non-common aspects of classes in the hierarchy.
		We want to create an external component to handle rendering but avoid type checks.

		Definition:
		A pattern where a component (visitor) is allowed to traverse the entire inheritance hierarchy. 
		Implemented by propagating a singe visit() method throughout the entire hierarchy.

		Dispatch:
		Which function to call?
		Single dispatch - depends on name of request and type of receiver.
		Double dispatch - depends on name of request and type of two receivers 
			(type of visitor and type of element being visited).

	*/
	class Visitor
    {
		abstract class Expression
		{
			// visitor is when we have to add a new operation when the hierarchy is already set and you cannot modify members
			public abstract void Print(StringBuilder sb);
		}

		class DoubleExpression : Expression
		{
			double _value;

			public DoubleExpression(double value) => _value = value;

			public override void Print(StringBuilder sb) => sb.Append(_value);
		}

		class AdditionExpression : Expression
		{
			Expression _left, _right;

			public AdditionExpression(Expression left, Expression right)
			{
				_left = left ?? throw new ArgumentNullException(paramName: nameof(left));
				_right = right ?? throw new ArgumentNullException(paramName: nameof(right));
			}

			public override void Print(StringBuilder sb)
			{
				sb.Append(value: "(");
				_left.Print(sb);
				sb.Append(value: "+");
				_right.Print(sb);
				sb.Append(value: ")");
			}
		}

		public static void Demo()
		{
			var e = new AdditionExpression(
				left: new DoubleExpression(1),
				right: new AdditionExpression(
				left: new DoubleExpression(2),
				right: new DoubleExpression(3)));
			var sb = new StringBuilder();
			e.Print(sb);
			WriteLine(sb);
		}
	}
}
