using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		Components may go in and out of a system at any time
			Chat room participants
			Players in a MMORPG game
		It makes no sense for them to have direct references to one another
			Those references may go dead
		The solution is to have them all refer to some central component that facilitates communication
		
		Definition:
		A component that facilitates communication between other components without them 
		necessarily being aware of each other or having direct (reference) access to each other.
	*/
	class Mediator
    {
	    public class Person
	    {
		    public string Name;
		    public ChatRoom Room;

			// all the messages this person received
		    readonly List<string> _chatLog = new List<string>();

		    public Person(string name)
		    {
			    Name = name;
		    }

			// receive a message
		    public void Receive(string sender, string message)
		    {
			    string s = $"{sender}: '{message}'";
			    WriteLine($"[{Name}'s chat session] {s}");
			    _chatLog.Add(s);
		    }

			// broadcast the message to the room
		    public void Say(string message) => Room.Broadcast(Name, message);

			// send a private message
		    public void PrivateMessage(string who, string message) => Room.Message(Name, who, message);
	    }

	    public class ChatRoom
	    {
		    readonly List<Person> _people = new List<Person>();

		    public void Broadcast(string source, string message)
		    {
			    foreach (var p in _people)
				    if (p.Name != source)
					    p.Receive(source, message);
		    }

		    public void Join(Person p)
		    {
			    string joinMsg = $"{p.Name} joins the chat";
			    Broadcast("room", joinMsg);

			    p.Room = this;
			    _people.Add(p);
		    }

			// private message
		    public void Message(string source, string destination, string message)
		    {
				// find the person for whom the message is addressed
			    _people.FirstOrDefault(p => p.Name == destination)
					?.Receive(source, message);
		    }
	    }

		// a classic example of Mediator is chat room
		// people don't need to have references one to another
	    public static void Demo()
	    {
			var room = new ChatRoom();

			var john = new Person("John");
			var jane = new Person("Jane");

			room.Join(john);
			room.Join(jane);

			john.Say("hi room");
			jane.Say("oh, hey john");

			var joi = new Person("Joi");
			room.Join(joi);
			joi.Say("hi everyone!");

			jane.PrivateMessage("Joi", "glad you could join us!");
	    }
	}
}
