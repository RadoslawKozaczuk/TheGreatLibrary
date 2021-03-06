﻿using System;
using System.ComponentModel;
using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		We need to be informed when certain things happen (property changes, event occurs etc.).
		We want to listen to events and notified when they occur.
		Built into c# with the event keyword and certain interfaces (e.g. IObservable<T>) and collections (e.g. BindingList<T>).

		Definition:
		An observer is an object that wishes to be informed about events happening in the system. 
		The entity generating the events is typically called an observable.
	*/
	class Observer
    {
		#region "Observer"
		class FallsIllEventArgs
	    {
		    public string Address;
	    }

	    class Person
	    {
            // we call the event - if there is no subscribers we will get a null exception error
            // therefore we use a safe call (null check)
            public void CatchACold() => FallsIll?.Invoke(this, new FallsIllEventArgs { Address = "123 London Road" });

            // this is how we specify an event
            public event EventHandler<FallsIllEventArgs> FallsIll;
	    }

		// whenever person gets ill we call a doctor
		// sender is who exactly generated the event
	    static void CallDoctor(object sender, FallsIllEventArgs eventArgs) => WriteLine($"A doctor has been called to {eventArgs.Address}");
		#endregion

		// .Net introduces events which is an implementation of Observer
		public static void EventDemo()
		{
			var person = new Person();
			person.FallsIll += CallDoctor;
			person.CatchACold();
			
			// unsubscribe
			person.FallsIll -= CallDoctor;
			person.CatchACold();
		}

		#region Memory Leak Demo
		class Button
	    {
		    public event EventHandler Clicked;

            public void Fire() => Clicked?.Invoke(this, EventArgs.Empty);
        }

	    class Window
	    {
		    public Window(Button button)
		    {
				// window does no store the reference but it subscribes to events
			    button.Clicked += ButtonOnClicked;
		    }

            // handling events
            void ButtonOnClicked(object sender, EventArgs eventArgs) => WriteLine("Button clicked (Window handler)");

            ~Window()
		    {
			    WriteLine("Window finalized");
		    }
	    }

		static void FireGarbageCollection()
		{
			WriteLine("Starting GC");
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			WriteLine("GC is done!");
		}
		#endregion

		// an event subscription can lead to a memory
		// leak if you hold on to it past the object's lifetime
		public static void MemoryLeakDemo()
		{
			var btn = new Button();
			var window = new Window(btn);
			var windowRef = new WeakReference(window);
			btn.Fire();

			WriteLine("Setting window to null");
			window = null;

			FireGarbageCollection();

			// windows is still alive because the button it is subscribed to is still alive
			WriteLine($"Is window alive after GC? {windowRef.IsAlive}");

			// it is still possible to access the window through WeakReference but obviously not through the variable
			WriteLine($"Window.ToString(): {windowRef.Target}");
			
			btn.Fire();

			WriteLine("Setting button to null");
			btn = null;

			FireGarbageCollection();
		}

		#region "Binding List Demo"
		public class Market
	    {
			public BindingList<float> Prices = new BindingList<float>();

		    public void AddPrice(float price)
		    {
				if(Prices.AllowNew)
					Prices.Add(price);
		    }
	    }

	    public class PriceAddedEventArgs
	    {
		    public float Price;
	    }
	    #endregion
		
		public static void BindingListDemo()
		{
			var market = new Market();
			market.Prices.ListChanged += (sender, eventArgs) => // Subscribe
			{
				if (eventArgs.ListChangedType == ListChangedType.ItemAdded)
					WriteLine($"Added price {((BindingList<float>)sender)[eventArgs.NewIndex]}");
			};
			market.AddPrice(123);

			// the BindingList has some additional flags that allows extra customization to some degree
			market.Prices.AllowNew = false;
			market.AddPrice(222);
		}
	}
}
