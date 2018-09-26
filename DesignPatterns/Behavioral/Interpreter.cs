using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		Textual input needs to be processed
		Examples: 
			programming language compilers, interpreters and IDEs
			HTML, XML and similar
			Numeric expressions (3+4/5)
			Regular expressions (language in a language)
		Turning strings into Object Oriented Programming based structures in a complicated process
		
		Definition:
		A component that processes structured text data. Does so by turning it into separate lexical tokens (lexing)
		and then interpreting sequences of said tokens (parsing)
	*/
	class Interpreter
    {
		interface IElement
		{
			int Value { get; }
		}

		class Integer : IElement
		{
			public Integer(int value)
			{
				Value = value;
			}

			public int Value { get; }
		}

		class BinaryOperation : IElement
		{
			public enum Type { Addition, Subtraction }

			public Type OperationType;
			public IElement Left, Right;

			public int Value
			{
				get
				{
					switch (OperationType)
					{
						case Type.Addition:
							return Left.Value + Right.Value;
						case Type.Subtraction:
							return Left.Value - Right.Value;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
			}
		}

		// mainly an enumeration of kind of tokens we have
		class Token
		{
			public enum Type
			{
				Integer, Plus, Minus, Lparen, Rparen
			}

			public readonly Type TokenType;
			public readonly string Text;

			public Token(Type type, string text)
			{
				TokenType = type;
				Text = text ?? throw new ArgumentNullException(nameof(text));
			}

			public override string ToString() => $"`{Text}`";
		}

		// lexing part of the interpretation (creating tokens)
		static List<Token> Lexer(string input)
		{
			var result = new List<Token>();

			for (int i = 0; i < input.Length; i++)
			{
				switch (input[i])
				{
					case '+':
						result.Add(new Token(Token.Type.Plus, "+"));
						break;
					case '-':
						result.Add(new Token(Token.Type.Minus, "-"));
						break;
					case '(':
						result.Add(new Token(Token.Type.Lparen, "("));
						break;
					case ')':
						result.Add(new Token(Token.Type.Rparen, ")"));
						break;
					default:
						var sb = new StringBuilder(input[i].ToString());
						for (int j = i + 1; j < input.Length; ++j)
						{
							// is the next one a digit
							if (char.IsDigit(input[j]))
							{
								sb.Append(input[j]);
								++i;
							}
							else
							{
								result.Add(new Token(Token.Type.Integer, sb.ToString()));
								break;
							}
						}
						break;
				}
			}

			return result;
		}

		static IElement Parse(IReadOnlyList<Token> tokens)
		{
			var result = new BinaryOperation();
			var haveLHS = false; // whether the left hand side was initiated or not
			for (int i = 0; i < tokens.Count; i++)
			{
				var token = tokens[i];

				// look at the type of token
				switch (token.TokenType)
				{
					case Token.Type.Integer:
						var integer = new Integer(int.Parse(token.Text));
						if (!haveLHS)
						{
							result.Left = integer;
							haveLHS = true;
						}
						else
						{
							result.Right = integer;
						}
						break;
					case Token.Type.Plus:
						result.OperationType = BinaryOperation.Type.Addition;
						break;
					case Token.Type.Minus:
						result.OperationType = BinaryOperation.Type.Subtraction;
						break;
					case Token.Type.Lparen:
						int j = i;
						for (; j < tokens.Count; ++j)
							if (tokens[j].TokenType == Token.Type.Rparen)
								break; // process subexpression w/o opening (
						var subexpression = tokens.Skip(i + 1).Take(j - i - 1).ToList();
						var element = Parse(subexpression);
						if (!haveLHS)
						{
							result.Left = element;
							haveLHS = true;
						}
						else result.Right = element;
						i = j; // advance 
						break;
					// right parenthesis was omitted because in the left parenthesis we find the right parenthesis and we handle it
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			return result;
		}

		// Barring simple cases, an interpreter acts in two stages
		// Lexing turns text into a set of tokens
		// Parsing tokens into meaningful constructs
		// Parsed data can then be traversed
		public static void Demo()
		{
			var input = "(13+4)-(12+1)";
			var tokens = Lexer(input);
			WriteLine(string.Join("  ", tokens));

			var parsed = Parse(tokens);
			WriteLine($"{input} = {parsed.Value}");
		}
	}
}
