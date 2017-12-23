using static System.Console;

namespace DesignPatterns.Structural
{
	public interface IRenderer
	{
		void RenderCircle(float radius);
	}

	/*
		Motivation:
		Theoretically speaking if we wanted to create an object hierarchy in which we create a dedicated object 
		for each combination of attributes we would effectively ended up with extremely huge object hierarchy.
		Example: Car <- SportCar, Truck <- CheapSportCar, ExpensiveSportCar, CheapTruck, ExpensiveTruck and so on.

		Definition:
		A mechanism that decouples an interface (hierarchy) from an implementation (hierarchy).
	*/
	class Bridge
    {
	    public class VectorRenderer : IRenderer
	    {
		    public void RenderCircle(float radius) => WriteLine($"Rendering a circle of radius {radius} by using VectorRenderer");
	    }

	    public class RasterRenderer : IRenderer
	    {
		    public void RenderCircle(float radius) => WriteLine($"Drawing a circle of radius {radius} by using RasterRenderer");
	    }

	    public abstract class Shape
	    {
		    protected IRenderer Renderer;

		    // a bridge between the shape that's being drawn an
		    // the component which actually draws it
		    protected Shape(IRenderer renderer)
		    {
			    Renderer = renderer;
		    }

		    public abstract void Draw();
		    public abstract void Resize(float factor);
	    }

	    public class Circle : Shape
	    {
		    float _radius;

		    public Circle(IRenderer renderer, float radius) : base(renderer)
		    {
			    _radius = radius;
		    }

		    public override void Draw() => Renderer.RenderCircle(_radius);

		    public override void Resize(float factor) => _radius *= factor;
	    }

	    public static void Demo()
		{
			var circle = new Circle(new VectorRenderer(), 5);
			circle.Draw();
			circle.Resize(2);
			circle.Draw();

			circle = new Circle(new RasterRenderer(), 5);
			circle.Draw();
			circle.Resize(2);
			circle.Draw();

			// Without aggregation we would have to specify CircleVector and CircleRaster objects
			// but instead we just take one functionality and extract it out of the hierarchy.
			// This pattern is so easy that people probably use it without even being aware of it.
		}
	}
}
