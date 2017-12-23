using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	}

	/*
		Motivation:
		Objects use other objects' fields/properties/members through inheritance and composition.
		Composite lets us make compound objects e.g. a group of shapes that consist other shapes.
		Composite design pattern is used to treat both single (scalar) and composite objects uniformly.

		Definition:
		A mechanism for treating individual (scalar) objects and compositions of objects in a uniform manner.
	*/
	internal class Composite
    {
	    public class Neuron : IEnumerable<Neuron>
	    {
		    public float Value;
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

	    public class NeuronLayer : Collection<Neuron>
	    {

	    }
		
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
    }
}