using System;
using System.Collections.Generic;
using static System.Console;

namespace DesignPatterns.Behavioral
{
	/*
		Motivation:
		Consider an ordinary telephone.
		What you do with it depends on the state of the phone as well as the line.
			If you try calling someone, and it's busy, you put the handset down.
		Changes in state can be explicit or in response to event

		Definition:
		A pattern in which the object's behavior is determined by its state. An object transitions 
		from one state to another (something needs to trigger a transition).
		A formalized construct which manages state and transitions is called a state machine.
	*/
	class State
    {
	    enum PhoneState { OffHook, Connecting, Connected, OnHold }
	    enum Trigger { CallDialed, HungUp, CallConnected, PlacedOnHold, TakenOffHold, LeftMessage }

		// all possible transitions
	    static readonly Dictionary<PhoneState, List<(Trigger, PhoneState)>> _rules = new Dictionary<PhoneState, List<(Trigger, PhoneState)>>
		{
			[PhoneState.OffHook] = new List<(Trigger, PhoneState)>
			{
				(Trigger.CallDialed, PhoneState.Connecting)
			},
			[PhoneState.Connecting] = new List<(Trigger, PhoneState)>
			{
				(Trigger.HungUp, PhoneState.OffHook),
				(Trigger.CallConnected, PhoneState.Connected)
			},
			[PhoneState.Connected] = new List<(Trigger, PhoneState)>
			{
				(Trigger.LeftMessage, PhoneState.OffHook),
				(Trigger.HungUp, PhoneState.OffHook),
				(Trigger.PlacedOnHold, PhoneState.OnHold)
			},
			[PhoneState.OnHold] = new List<(Trigger, PhoneState)>
			{
				(Trigger.TakenOffHold, PhoneState.Connected),
				(Trigger.HungUp, PhoneState.OffHook)
			}
		};

		// Typically state machine are implemented by using external libraries,
		// but for educational purposes we are going to make our own.
		// Using state machines, given sufficient complexity, pays to formally define possible states and events/triggers.
		// State machines can define:
		//	state entry/exit behaviors
		//  action when a particular even causes a transition
		//  guard conditions enabling/disabling a transition
		//  default action when no transitions are found for an event
	    public static void Demo()
		{
            WriteLine("Type numbers to input commands - typing anything not allowed ends the program");

            var state = PhoneState.OffHook;
			while (true)
			{
                WriteLine("");
                WriteLine($"The phone is currently {state}");
				WriteLine("Select command:");

				// foreach to for
				for (var i = 0; i < _rules[state].Count; i++)
				{
					var (t, _) = _rules[state][i];
					WriteLine($"{i}. {t}");
				}
                
                PhoneState s;
				try
				{
					var input = int.Parse(ReadLine());
                    s = _rules[state][input].Item2;
				}
				catch (Exception)
				{
					WriteLine($"The phone is currently {state}");
					return;
				}

				state = s;
            }
		}
    }
}
