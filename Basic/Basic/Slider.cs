using System;

namespace Basic.Framework
{
	public class Slider : Widget
	{
		private uint barColor;
		private float rate;
		
		public Slider (int rectX, int rectY, int rectW, int rectH): base(rectX,rectY,rectW,rectH)
		{
			BarColor = 0xffff0000;
			Rate = 0.5f;
			
		}
		public float Rate
		{
			get{return rate;}
			    set{rate = value;}
		}
		public uint BarColor
		{
			get{return barColor;}
			set{barColor = value;}
		}
		public void Update(int pixelX)
		{
			rate = (float)(pixelX - rectX) /rectW;
		}
		public override void Draw()
		{
			base.Draw();
			Basic.Framework.Draw.FillRect(barColor,rectX,rectY,(int)(rectW * rate),rectH);
		}
	}
}

