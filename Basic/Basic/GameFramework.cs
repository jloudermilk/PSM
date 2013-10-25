using System;
using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Basic.Utility;

namespace Basic.Framework
{
	public class GameFramework : IDisposable
	{
		protected GraphicsContext graphics;
		public GraphicsContext Graphics
		{
			get{return graphics;}
		}
		
		GamePadData gamePadData;
		public GamePadData PadData
		{
			get{ return gamePadData;}
		}
		
		protected bool loop = true;
		protected bool drawDebugString = true;
		
		Stopwatch stopwatch;
		const int pinSize = 3;
		int[] time = new int[pinSize];
		int[] preTime= new int[pinSize];
		float[] timePercent = new float[pinSize];
		
		int frameCounter = 0;
		long preSecondTicks;
		float fps = 0;
		
		//public DebugString debugString;
		
		public void Run(string[] args)
		{
			Initialize();
			while(loop)
			{
				time[0] = (int) stopwatch.ElapsedTicks; //Start
				SystemEvents.CheckEvents();
				time[1] = (int) stopwatch.ElapsedTicks;
				Render();
			}
			
			Terminate();
		}
		virtual public void Initialize()
		{
			
		}
	}
}

