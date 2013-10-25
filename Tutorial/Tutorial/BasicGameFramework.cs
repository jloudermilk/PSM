using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

using Basic.Framework;


namespace Tutorial
{
	public class BasicGameFramework : GameFramework
	{
		public ImageRect rectScreen;
		public Random rand = new Random(123);
		
		Texture2D texturePlayer, textureStar;
		
		Int32 counter = 0;
		
		
		
		public BasicGameFramework ()
		{
		}
	}
}

