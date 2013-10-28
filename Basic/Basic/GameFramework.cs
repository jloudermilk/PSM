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
		private long managedMemoryUsage;
		
		Stopwatch stopwatch;
		const int pinSize = 3;
		int[] time = new int[pinSize];
		int[] preTime= new int[pinSize];
		float[] timePercent = new float[pinSize];
		
		int frameCounter = 0;
		long preSecondTicks;
		float fps = 0;
		
		public DebugString debugString;
		
		public void Run(string[] args)
		{
			Initialize();
			while(loop)
			{
				time[0] = (int) stopwatch.ElapsedTicks; //Start
				SystemEvents.CheckEvents();
				Update();
				time[1] = (int) stopwatch.ElapsedTicks;
				Render();
			}
			
			Terminate();
		}
		virtual public void Initialize()
		{
			Console.WriteLine("Initialize()");
			
			stopwatch = new Stopwatch();
			stopwatch.Start();
			preSecondTicks = stopwatch.ElapsedTicks;
			
			graphics = new GraphicsContext();
			graphics.SetClearColor(0.0f,0.0f,0.0f,0.0f);
			graphics.Enable(EnableMode.Blend);
			graphics.Enable(EnableMode.DepthTest);
			graphics.SetBlendFunc(BlendFuncMode.Add, BlendFuncFactor.SrcAlpha,BlendFuncFactor.OneMinusSrcAlpha);
			
			debugString = new DebugString(graphics);
		}
		virtual public void Terminate()
		{
			Console.WriteLine("Terminate()");
		}
		virtual public void Input()
		{
			gamePadData = GamePad.GetData(0);
		}
		virtual public void Update()
		{
			debugString.Clear();
			
			Input();
#if DEBUG
			//Terminate a program with simultaneously pressing Start button and Select button.
			if(((gamePadData.Buttons & GamePadButtons.Start) != 0) && ((gamePadData.Buttons & GamePadButtons.Select) != 0))
			{
				Console.WriteLine("exit.");
				loop = false;
				return;
			}
#endif	
			// Q key and E key
			if((gamePadData.Buttons & GamePadButtons.L) != 0 &&  (gamePadData.ButtonsDown & GamePadButtons.R) != 0)
			{
				drawDebugString = (drawDebugString == true) ? false : true;
			}
			
			CalculateProcessTime();
			
		}
		virtual public void Render()
		{
#if DEBUG
		if(drawDebugString == true)
			{
				debugString.Render();
			}		
#endif
			time[2] = (int)stopwatch.ElapsedTicks;
			
			graphics.SwapBuffers();	
			
			frameCounter++;
			
			CalculateFPS();
			
			preTime=(int[])time.Clone();
			
		}
		
		public void Dispose()
		{
			graphics.Dispose();
			debugString.Dispose();
			
		}
		
		void CalculateProcessTime()
		{
			float ticksPerFrame = Stopwatch.Frequency / 60.0f;	
			timePercent[0]=(preTime[1]-preTime[0])/ticksPerFrame;
			timePercent[1]=(preTime[2]-preTime[1])/ticksPerFrame;
			
			debugString.WriteLine(string.Format("Update={0,6:N}%", timePercent[0] * 100));
			debugString.WriteLine(string.Format("Render={0,6:N}%", timePercent[1] * 100));
			debugString.WriteLine(string.Format("FPS={0,5:N}", fps));
			
			debugString.WriteLine(string.Format("managedMemory= {0}M {1}K {2}byte", 
			                                    this.managedMemoryUsage>>20, (this.managedMemoryUsage&0xFFFFF)>>10, this.managedMemoryUsage&0x3FF));
			
		}
		void CalculateFPS()
		{
			//Update FPS counter if 1 second has elapsed.
			long elapsedTicks = stopwatch.ElapsedTicks;
			if( elapsedTicks - preSecondTicks >= Stopwatch.Frequency)
			{
				fps=(float)frameCounter*Stopwatch.Frequency/(elapsedTicks - preSecondTicks);
				frameCounter=0;
				preSecondTicks=stopwatch.ElapsedTicks;
				
				//Usage of managed memory.
				managedMemoryUsage = System.GC.GetTotalMemory(false);
			
		}
	}
}}

