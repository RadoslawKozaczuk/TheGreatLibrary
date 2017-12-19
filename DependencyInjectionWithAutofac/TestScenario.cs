﻿using Autofac;
using System;

namespace DependencyInjectionWithAutofac
{
	public interface ILog
	{
		void Write(string message);
	}

	public interface IConsole {	}

	public class ConsoleLog : ILog, IConsole
	{
		public void Write(string message) => Console.WriteLine(message);
	}

	// with built-in parameter
	public class EmailLog : ILog
	{
		private const string adminEmail = "admin@foo.com";

		public void Write(string message) => Console.WriteLine($"Email sent to {adminEmail} : {message}");
	}

	// with a parameter
	public class SMSLog : ILog
	{
		private string phoneNumber;

		public SMSLog(string phoneNumber)
		{
			this.phoneNumber = phoneNumber;
		}

		public void Write(string message) => Console.WriteLine($"SMS to {phoneNumber} : {message}");
	}

	public class Engine
	{
		private ILog log;
		private int id;

		public Engine(ILog log)
		{
			this.log = log;
			id = new Random().Next();
		}

		public Engine(ILog log, int id)
		{
			this.log = log;
			this.id = id;
		}

		public void Ahead(int power) => log.Write($"Engine [{id}] ahead {power}");
	}

	public class Car
	{
		private Engine engine;
		private ILog log;

		public Car(Engine engine)
		{
			this.engine = engine;
			this.log = new EmailLog();
		}

		public Car(Engine engine, ILog log)
		{
			this.engine = engine;
			this.log = log;
		}

		public void Go()
		{
			engine.Ahead(100);
			log.Write("Car going forward...");
		}
	}

	public class Parent
	{
		public override string ToString() => "I am your father";
	}

	public class Child
	{
		public string Name { get; set; }
		public Parent Parent { get; set; }

		public void SetParent(Parent parent)
		{
			Parent = parent;
		}
	}

	public class ParentChildModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<Parent>();
			builder.Register(c => new Child() { Parent = c.Resolve<Parent>() });
		}
	}
}
