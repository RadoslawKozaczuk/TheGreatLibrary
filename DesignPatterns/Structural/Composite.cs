using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using static System.Console;

namespace DesignPatterns.Structural
{
	// we cannot use a base class 
	public static class ExtensionMethods
	{
		internal static void ConnectTo(this IEnumerable<Composite.Neuron> self, IEnumerable<Composite.Neuron> other)
		{
			if (ReferenceEquals(self, other)) return;

			foreach (var from in self)
				foreach (var to in other)
				{
					from.Out.Add(to);
					to.In.Add(from);
				}
		}

		internal static int Sum(this List<Composite.IValueContainer> containers)
		{
			int result = 0;
			foreach (var c in containers)
				foreach (var i in c)
					result += i;
			return result;
		}
	}

	/*
		Motivation:
		Objects use other objects' fields/properties/members through inheritance and composition.
		Composite lets us make compound objects e.g. a group of shapes that consist other shapes.
		Composite design pattern is used to treat both single (scalar) and composite objects uniformly.

		Definition:
		A mechanism for treating individual (scalar) objects and compositions of objects in a uniform manner.
	*/
	class Composite
    {
		#region "Common interface"
		// in order to make one method that handles all operations we need to have something in common
		// Collection<Neuron> implements IEnumerable so we can use this
		public class Neuron : IEnumerable<Neuron>
	    {
		    public List<Neuron> In = new List<Neuron>(), Out = new List<Neuron>();

			public IEnumerator<Neuron> GetEnumerator()
		    {
			    yield return this;
		    }

		    IEnumerator IEnumerable.GetEnumerator()
		    {
			    yield return this;
		    }
	    }

	    class NeuronLayer : Collection<Neuron>
		{
	    }
		#endregion

		public static void CommonInterfaceDemo()
	    {
		    var neuron1 = new Neuron();
			var neuron2 = new Neuron();
			var layer1 = new NeuronLayer();
			var layer2 = new NeuronLayer();

			neuron1.ConnectTo(neuron2);
			neuron1.ConnectTo(layer1);
			layer1.ConnectTo(layer2);

			WriteLine("This example does not provide any output, please check the code.");
	    }

		#region "Object Hierarchy"
		// this object contains a collection of self
		// collection is type of Lazy to save some operation time
		class GraphicObject
	    {
		    public virtual string Name { get; set; } = "Group";
		    public string Color;
		    public List<GraphicObject> Children => _children.Value;
		    readonly Lazy<List<GraphicObject>> _children = new Lazy<List<GraphicObject>>();
			
		    public override string ToString()
		    {
			    var sb = new StringBuilder();
			    Print(sb, 0);
			    return sb.ToString();
		    }

		    void Print(StringBuilder sb, int depth)
		    {
			    sb.Append(new string('*', depth))
				    .Append(string.IsNullOrWhiteSpace(Color) ? string.Empty : $"{Color} ")
				    .AppendLine($"{Name}");
			    foreach (var child in Children)
				    child.Print(sb, depth + 1);
		    }
		}

	    class Circle : GraphicObject
	    {
		    public override string Name => "Circle";
	    }

	    class Square : GraphicObject
	    {
		    public override string Name => "Square";
	    }
		#endregion

		public static void BeautifulHierarchyDemo()
		{
			var drawing = new GraphicObject { Name = "My Drawing" };
			drawing.Children.Add(new Square { Color = "Red" });
			drawing.Children.Add(new Circle { Color = "Yellow" });

			var group = new GraphicObject();
			group.Children.Add(new Circle { Color = "Blue" });
			group.Children.Add(new Square { Color = "Blue" });
			drawing.Children.Add(group);

			Write(drawing);
		}

		#region "The experiment number three"

		public  interface IValueContainer : IEnumerable<int>
		{
		}

		class SingleValue : IValueContainer
		{
			public int Value;
			public IEnumerator<int> GetEnumerator()
			{
				yield return Value;
			}

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		}

		public class ManyValues : List<int>, IValueContainer
		{

		}
		#endregion

		// Objects can use other objects via inheritance/composition
		// Some composed and singular objects need similar/identical behaviors
		// Composite design pattern lets us treat both types of objects uniformly
		// C# has special support for the enumeration concept
		// A single object can masquerade as a collection with yield return this;
		public static void DemoNumberThree()
		{
			var singleValue = new SingleValue { Value = 11 };
			var otherValues = new ManyValues { 22, 33 };
			WriteLine(new List<IValueContainer> { singleValue, otherValues }.Sum() == 66 ? "Yey it works!" : "Error 404");
		}
	}
}