using System;
using System.Collections.Generic;
using Sce.PlayStation.Core.Input;

namespace Tutorial
{
	public class Widget : IDisposable
	{
		protected int rectX;
		protected int rectY;
		protected int rectW;
		protected int rectH;
		protected uint buttonColor;
		
		public Widget (int rectX, int rectY, int rectW, int rectH)
		{
			SetRect(rectX,rectY,rectW,rectH);
			buttonColor = 0xffffffff;
		}
		public void SetRect(int rectX, int rectY, int rectW, int rectH)
		{
			this.rectX = rectX;
			this.rectY = rectY;
			this.rectW = rectW;
			this.rectH = rectH;
		}
		
		public bool TouchDown(List<TouchData> touchDataList)
		{
			foreach(var touchData in touchDataList)
			{
				if(TouchMove(touchData))
				{
					return true;
				}
			
			}
			return false;
		}
		public bool TouchMove(TouchData touchData)
		{
			if(touchData.Status == TouchStatus.Move)
			{
				return InsideRect(Tutorial.Draw.TouchPixelX(touchData), Tutorial.Draw.TouchPixelY(touchData));
			}
			return false;
		}
		public bool InsideRect(int pixelX, int pixelY){
			if(rectX <= pixelX && rectX+ rectW >= pixelX &&
			   rectY <= pixelY && rectY +rectH >= pixelY)
			{
			return true;
			}
			return false;
		}
		public virtual void Dispose()
		{
			
		}
		public virtual void Draw()
		{
		 Tutorial.Draw.FillRect(buttonColor,rectX,rectY,rectW,rectH);
		}
		
	}
}

