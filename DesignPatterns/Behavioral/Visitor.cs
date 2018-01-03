using System;
using System.Collections.Generic;
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
		#region "Intrusive Visitor"
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
		#endregion

		public static void IntrusiveVisitorDemo()
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

		#region "Reflective Visitor"
		// in this case lets assume Expression DoubleExpression and AdditionExpression does not have Print method
		// and we have to add it
		abstract class ExpressionV2
		{
		}

		class DoubleExpressionV2 : ExpressionV2
		{
			public readonly double Value;

			public DoubleExpressionV2(double value) => Value = value;
		}

		class AdditionExpressionV2 : ExpressionV2
		{
			public readonly ExpressionV2 Left;
			public readonly ExpressionV2 Right;

			public AdditionExpressionV2(ExpressionV2 left, ExpressionV2 right)
			{
				Left = left ?? throw new ArgumentNullException(nameof(left));
				Right = right ?? throw new ArgumentNullException(nameof(right));
			}
		}
		
		static class ExpressionPrinter
		{
			// we map every single type to lambda that does everything we want to do
			static readonly Dictionary<Type, Action<ExpressionV2, StringBuilder>> _actions 
				= new Dictionary<Type, Action<ExpressionV2, StringBuilder>>
			{
				[typeof(DoubleExpressionV2)] = (e, sb) =>
				{
					var de = (DoubleExpressionV2)e;
					sb.Append(de.Value);
				},
				[typeof(AdditionExpressionV2)] = (e, sb) =>
				{
					var ae = (AdditionExpressionV2)e;
					sb.Append("(");
					Print(ae.Left, sb);
					sb.Append("+");
					Print(ae.Right, sb);
					sb.Append(")");
				}
			};
			
			// one of the idea is to make a separate component that use the reflection 
			// to figure out how the particular object should be printed
			public static void Print(ExpressionV2 e, StringBuilder sb)
			{
				// but the problem is that it break open-close principle
				// each time we add new object we have to edit this method
				// additionally it will work incorrectly on missing types
				if (e is DoubleExpressionV2 de)
				{
					sb.Append(de.Value);
				}
				else if (e is AdditionExpressionV2 ae)
				{
					sb.Append("(");
					Print(ae.Left, sb);
					sb.Append("+");
					Print(ae.Right, sb);
					sb.Append(")");
				}
			}

			// another idea is to make a table we still use reflection tho
			// in case of type that is missing we will get an exception
			public static void Print2(ExpressionV2 e, StringBuilder sb) => _actions[e.GetType()](e, sb);
		}
		#endregion

		public static void ReflectiveVisitorDemo()
		{
			var e = new AdditionExpressionV2(
			  left: new DoubleExpressionV2(1),
			  right: new AdditionExpressionV2(
				left: new DoubleExpressionV2(2),
				right: new DoubleExpressionV2(3)));
			var sb = new StringBuilder();
			ExpressionPrinter.Print2(e, sb);
			WriteLine(sb);
		}

		#region "Classic Visitor"
		abstract class ExpressionV3
		{
			// here we define a method that accepts a visitor
			public abstract void Accept(IExpressionVisitor visitor);
		}

		class DoubleExpressionV3 : ExpressionV3
		{
			public readonly double Value;

			public DoubleExpressionV3(double value) => Value = value;

			public override void Accept(IExpressionVisitor visitor) => visitor.Visit(this);
		}

		class AdditionExpressionV3 : ExpressionV3
		{
			public readonly ExpressionV3 Left;
			public readonly ExpressionV3 Right;

			public AdditionExpressionV3(ExpressionV3 left, ExpressionV3 right)
			{
				Left = left ?? throw new ArgumentNullException(nameof(left));
				Right = right ?? throw new ArgumentNullException(nameof(right));
			}

			// this is double dispatch trick - visitor gets the information of the type
			public override void Accept(IExpressionVisitor visitor) => visitor.Visit(this);
		}

		// this interface need to implemented by all visitors that are going to visit expressions
		interface IExpressionVisitor
		{
			void Visit(DoubleExpressionV3 de);
			void Visit(AdditionExpressionV3 ae);
		}

		class ExpressionPrinterV3 : IExpressionVisitor
		{
			readonly StringBuilder _sb = new StringBuilder();

			public void Visit(DoubleExpressionV3 de) => _sb.Append(de.Value);

			public void Visit(AdditionExpressionV3 ae)
			{
				_sb.Append("(");
				ae.Left.Accept(this);
				_sb.Append("+");
				ae.Right.Accept(this);
				_sb.Append(")");
			}

			public override string ToString() => _sb.ToString();
		}

		// there are some limitation of this approach
		class ExpressionCalculator : IExpressionVisitor
		{
			public double Result;

			// we would like to have int result but we can't
			// therefore we have to use additional field
			public void Visit(DoubleExpressionV3 de) => Result = de.Value;

			// things gets even nastier in case of calculation left and right expression
			public void Visit(AdditionExpressionV3 ae)
			{
				ae.Left.Accept(this);
				var a = Result;
				ae.Right.Accept(this);
				var b = Result;
				Result = a + b;
			}
		}

		// this is the first real Visitor
		// previous approaches worked but were not real Visitor patterns
		// Double Dispatch approach
		public static void ClassicVisitorDemo()
		{
			var e = new AdditionExpressionV3(
				left: new DoubleExpressionV3(1),
				right: new AdditionExpressionV3(
				left: new DoubleExpressionV3(2),
				right: new DoubleExpressionV3(3)));
			var ep = new ExpressionPrinterV3();
			ep.Visit(e);
			WriteLine(ep.ToString());

			var calc = new ExpressionCalculator();
			calc.Visit(e);
			WriteLine($"{ep} = {calc.Result}");
		}
		#endregion
	}
}
