using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		Iteration (traversal) is a core functionality of various data structures
		An iterator is a class that facilitates the traversal
			Keeps a reference to the current element
			Knows how to move to a different element
		Iterator is an implicit construct
			.NET builds a state machine around your yield return statements
		

		Definition:
		An object (or, in .NET, a method) that facilitates the traversal of a data structure.
	*/
	class Iterator
    {
		public class Node<T>
		{
			public T Value;
			public Node<T> Left, Right;
			public Node<T> Parent;

			public Node(T value)
			{
				Value = value;
			}

			public Node(T value, Node<T> left, Node<T> right)
			{
				Value = value;
				Left = left;
				Right = right;

				left.Parent = right.Parent = this;
			}
		}

		// this can be perceived as a state machine
		// in order to make it possible to be used in foreach statements
		public class InOrderIterator<T>
		{
			public Node<T> Current { get; set; } // current state
			readonly Node<T> _root;
			bool _yieldedStart; 

			public InOrderIterator(Node<T> root)
			{
				_root = Current = root;
				while (Current.Left != null)
					Current = Current.Left;
			}
			
			// reset the iterator back to the starting position
			public void Reset()
			{
				Current = _root;
				_yieldedStart = true;
			}

			// attempts to move to the next element and return true if succeed
			// this is basically how foreach loop is implemented in .NET framework
			public bool MoveNext()
			{
				// in first call of the MoveNext we need to return current value
				// and then every following call of the MoveNext actually moves the pointer by one
				if (!_yieldedStart)
				{
					_yieldedStart = true;
					return true;
				}

				if (Current.Right != null)
				{
					Current = Current.Right;
					while (Current.Left != null)
						Current = Current.Left;
					return true;
				}
				else
				{
					var p = Current.Parent;
					while (p != null && Current == p.Right)
					{
						Current = p;
						p = p.Parent;
					}
					Current = p;
					return Current != null;
				}
			}
		}

		// Iterator as a method.
		// This is not IEnumerable<Node<T>> but we still can use it in foreach loop because we implemented GetEnumerator 
		// and InOrderIterator has a property named Current and method MoveNext that return bool.
		public class BinaryTree<T>
		{
			readonly Node<T> _root;

			public BinaryTree(Node<T> root)
			{
				_root = root;
			}

			public InOrderIterator<T> GetEnumerator() => new InOrderIterator<T>(_root);

			// thanks to returning IEnumerable we can use LINQ
			public IEnumerable<Node<T>> NaturalInOrder
			{
				get
				{
					// a method embedded in a method
					IEnumerable<Node<T>> TraverseInOrder(Node<T> current)
					{
						if (current.Left != null)
						{
							foreach (var left in TraverseInOrder(current.Left))
								yield return left;
						}
						yield return current;
						if (current.Right != null)
						{
							foreach (var right in TraverseInOrder(current.Right))
								yield return right;
						}
					}
					foreach (var node in TraverseInOrder(_root))
						yield return node;
				}
			}
		}

		// an iterator that is a separate object
		public static void Demo()
	    {
			//   1
		    //  / \
		    // 2   3

		    // in-order:  213
		    // preorder:  123
		    // postorder: 231

		    var root = new Node<int>(1,
			    new Node<int>(2), new Node<int>(3));

		    // C++ style
		    var it = new InOrderIterator<int>(root);

		    while (it.MoveNext())
		    {
			    Write(it.Current.Value);
			    Write(',');
		    }
		    WriteLine();

		    // C# style
		    var tree = new BinaryTree<int>(root);

		    WriteLine(string.Join(",", tree.NaturalInOrder.Select(x => x.Value)));
			
		    foreach (var node in tree)
			    WriteLine(node.Value);
	    }
    }
}
