#define DOTNETCORE_LEGACY_COMPATIBILITY
#if DOTNETCORE_LEGACY_COMPATIBILITY

// Looks like Timers are missing in .NetCore so here is a German Panzer replacement found somewhere on ze Internetz.
using System;

namespace ReactiveExtensions
{
	public class ElapsedEventArgs : EventArgs
	{
		protected DateTime m_signalTime;

		public ElapsedEventArgs() => m_signalTime = DateTime.Now;
		public DateTime SignalTime { get { return m_signalTime; } }
	}

	public delegate void ElapsedEventHandler(object sender, ElapsedEventArgs e);
	
	public class Timer : IDisposable
	{
		protected System.Threading.Timer m_TaskTimer;

		public Timer() : this(0, true)
		{ }
		
		public Timer(double interval) : this(interval, true)
		{ }
		
		public Timer(double interval, bool autoReset)
		{
			Interval = interval;
			AutoReset = autoReset;
		}
		
		protected void internal_elapsed(object sender)
		{
			if (!AutoReset)
				Stop();

			ElapsedEventArgs eeargs = new ElapsedEventArgs();
			Elapsed(sender, eeargs);
		}
		
		public event ElapsedEventHandler Elapsed;

		public bool AutoReset { get; set; }
		
		public bool m_Enabled;

		public bool Enabled
		{
			get	{ return m_Enabled;	}

			set
			{
				if (value == m_Enabled)	return;
				
				if (m_Enabled) 
					Stop();
				else 
					Start();

				m_Enabled = value;
			}
		}

		protected double m_interval;

		public double Interval
		{
			get	{ return m_interval; }
			set
			{
				m_interval = value;

				if (m_TaskTimer == null) return;

				int mils = (int)Interval;
				TimeSpan ts = new TimeSpan(0, 0, 0, 0, mils);
				m_TaskTimer.Change(new TimeSpan(0), ts);
			}
		}

		public void Start()
		{
			int mils = (int)Interval;
			TimeSpan ts = new TimeSpan(0, 0, 0, 0, mils);

			if (m_TaskTimer == null)
				m_TaskTimer = new System.Threading.Timer(new System.Threading.TimerCallback(internal_elapsed), null, ts, ts);
			else
				m_TaskTimer.Change(ts, ts);
		}

		public void Stop()
		{
			if (m_TaskTimer == null) return;

			m_TaskTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
		}
		
		public void Dispose() => Dispose(true);

		void IDisposable.Dispose() => Dispose(true);

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				Stop();
				if (m_TaskTimer != null)
					m_TaskTimer.Dispose();

				m_TaskTimer = null;
				Elapsed = null;
			}
		}

		public void Close() => Dispose(true);
	}
}

#endif