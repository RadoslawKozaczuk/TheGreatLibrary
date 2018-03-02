using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PerformanceOptimization
{
	class Matrix : IEnumerable
	{
		public int Rows;
		public int Columns;

		private readonly int[] _data;

		public int this[int row, int col]
		{
			get => _data[Columns * row + col];
			set => _data[Columns * row + col] = value;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		public Matrix(int[,] value)
		{
			Rows = value.GetLength(0);
			Columns = value.GetLength(1);
			_data = new int[Rows * Columns];

			//flattening
			for (int i = 0; i < Rows; i++)
				for (int j = 0; j < Columns; j++)
					_data[Columns * i + j] = value[i, j];

			/* Just in case I would forget which one is column again...
			   C1 C2 C3
			R1 00 01 02
			R2 10 11 12
			
			after flatterning
			00 01 02 10 11 12 // old indexes
			0  1  2  3  4  5  // indexes
			
			Overall equation:
			2-dim[row, col] = flat[Columns * row + col]
			 */
		}
	}

	public static class Exercises
	{
		static Dictionary<char, int> GetCharacterCount(string name)
		{
			string lowerName = name.Trim().ToLower();
			var result = new Dictionary<char, int>();
			foreach (char c in lowerName)
			{
				if (!char.IsLetter(c)) continue;

				if (result.ContainsKey(c))
					result[c]++;
				else
					result[c] = 1;
			}
			return result;
		}

		// in: "4 score and 7 years ago, 8 men had the same PIN code: 6571"
		// expected out: "four score and seven years ago, eight men had the same PIN code: six five seven one"
		static string ReplaceDigits(string sentence)
		{
			string[] words = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

			var sb = new StringBuilder();
			string[] chunks = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < chunks.Length; i++)
			{
				string currentStr = chunks[i];

				if (currentStr.Length == 1)
				{
					sb.Append(int.TryParse(currentStr, out var res)
						? words[res]
						: currentStr);
				}
				else
				{
					if (int.TryParse(currentStr, out _))
					{
						// additional split needed
						for (int j = 0; j < currentStr.Length; j++)
						{
							int parsedInt = int.Parse(currentStr.Substring(j, 1));
							sb.Append(words[parsedInt]);

							// skip last run
							if (j < currentStr.Length - 1)
								sb.Append(" ");
						}
					}
					else
					{
						// just add it
						sb.Append(currentStr);
					}
				}

				// skip last run
				if (i < chunks.Length - 1)
					sb.Append(" ");
			}

			return sb.ToString();
		}

		static Matrix Multiply(Matrix a, Matrix b)
		{
			var result = new Matrix(new int[a.Rows, b.Columns]);
			for (int i = 0; i < result.Rows; i++)
			{
				for (int j = 0; j < result.Columns; j++)
				{
					result[i, j] = 0;
					for (int k = 0; k < a.Columns; k++)
						result[i, j] += a[i, k] * b[k, j];
				}
			}
			return result;
		}

		public static void RunExercises()
		{
			//var res1 = Exercise.GetCharacterCount("John Doe");
			//var res2 = Exercise.ReplaceDigits("4 score and 7 years ago, 8 men had the same PIN code: 6571");

			var a = new Matrix(new[,]
			{
				{1, 2, 3},
				{4, 5, 6}
			});
			var b = new Matrix(new[,]
			{
				{7, 8},
				{9, 10},
				{11, 12}
			});

			var res3 = Multiply(a, b);
			Console.ReadKey();
		}
	}
}
