using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Input;

namespace Basic.Framework
{
	public static class Draw
	{
		private static GraphicsContext graphics;
		private static ShaderProgram textureShaderProgram; //fragment?
		private static ShaderProgram colorShaderProgram; //vertex?
		
		private static Matrix4 projectionMatrix;
		private static Matrix4 viewMatrix;
		private static VertexBuffer rectVertices;
		private static VertexBuffer circleVertices;
		private static Dictionary<string,Sprite> spriteDict;
		private static int spriteNamelessCount;
		private static Font defaultFont;
		private static Font currentFont;
		
		public static bool Init (GraphicsContext graphicsContext, Stopwatch swInit = null)
		{
			graphics = graphicsContext;
			
			textureShaderProgram = createSimpleTextureShader();
			colorShaderProgram = createSimpleColorShader();
			
			projectionMatrix = Matrix4.Ortho (0, Width,
			                                 0, Height,
			                                 0.0f, 32768.0f); //thats a bit of a random number, look it up
			
			viewMatrix = Matrix4.LookAt (new Vector3 (0, Height, 0),
			                            new Vector3 (0, Height, 0),
			                            new Vector3 (0, -1, 0));
			rectVertices = new VertexBuffer (4, VertexFormat.Float3);
			rectVertices.SetVertices (0, new float[]{0,0,0,1,0,0,1,1,0,0,1,0});
			
			circleVertices = new VertexBuffer (36, VertexFormat.Float3);
			float[] circleVertex = new float[3 * 36];
			for (int i = 0; i < 36; i++) {
				float radian = ((i * 10) / 180.0f * FMath.PI);
				circleVertex [3 * 1 + 0] = FMath.Cos (radian);
				circleVertex [3 * 1 + 1] = FMath.Sin (radian);
				circleVertex [3 * 1 + 2] = 0.0f;
			}
			circleVertices.SetVertices (0, circleVertex);
			
			defaultFont = new Font (FontAlias.System, 24, FontStyle.Regular);
			//SetDefaultFont();
			
			spriteDict = new Dictionary<string, Sprite> ();
			
			spriteNamelessCount = 0;
			return true;
			
		}

		public static void Terminate ()
		{
			ClearSprite ();
			
			defaultFont.Dispose ();
			circleVertices.Dispose ();
			rectVertices.Dispose ();
			colorShaderProgram.Dispose ();
			textureShaderProgram.Dispose ();
			graphics = null; // why does this get set to null instead of Dispose() being called?
			
		}
		
		public static int Width {
			get{ return graphics.GetFrameBuffer ().Width;}
		}

		public static int Height {
			get{ return graphics.GetFrameBuffer ().Height; }
		}
		
		public static int TouchPixelX (TouchData touchData)
		{
			return (int)((touchData.X + 0.5f) * Width);
		}

		public static int TouchPixelY (TouchData touchData)
		{
			return (int)((touchData.Y + 0.5f) * Height);
		}
		
		public static void AddSprite (Sprite sprite)
		{
			AddSprite ("[nameless]:" + spriteNamelessCount, sprite);
			spriteNamelessCount++;
		}

		public static void AddSprite (string key, Sprite sprite)
		{
			if (!spriteDict.ContainsKey (key)) {
				spriteDict.Add (key, sprite);
			}
		}

		public static void AddSprite (string key, string text, uint argb, Font font, int positionX, int positionY)
		{
			if (!spriteDict.ContainsKey (text)) {
				//	AddSprite(key, new Sprite(text, argb, font, positionX, positionY));
			}
		}

		public static void AddSprite (string text, uint argb, int positionX, int positionY)
		{
			if (!spriteDict.ContainsKey (text)) {
				//	AddSprite(key, new SampleSprite(text, argb, font, positionX, positionY));
			}	
		}

		public static void RemoveSprite (string key)
		{
			if (spriteDict.ContainsKey (key)) {
				spriteDict [key].Dispose ();
				spriteDict.Remove (key);
			}
		}

		public static void ClearSprite ()
		{
			foreach (string key in spriteDict.Keys) {
				spriteDict [key].Dispose ();
			}
			spriteDict.Clear ();
			spriteNamelessCount = 0;
		}

		public static void Update ()
		{
			int keyNo = 0;
			
			string[] removeKey = new string [spriteDict.Count];
			
			foreach (var key in spriteDict.Keys) {
				if (!spriteDict [key].CheckLifeTimer ()) {
					removeKey [keyNo] = key;
					keyNo++;
				}
			}
			for (int i = 0; i < keyNo; i++) {
				if (removeKey [i] != null) {
					RemoveSprite (removeKey [i]);
				}
			}
			
		}

		public static void DrawSprite (string key)
		{
			if (spriteDict.ContainsKey (key)) {
				DrawSprite (spriteDict [key]);
			}
		}

		public static void DrawSprite (Sprite sprite)
		{
			Matrix4 modelMatrix = sprite.CreateModelMatrix ();
			Matrix4 worldViewProj = projectionMatrix * viewMatrix * modelMatrix;
			
			textureShaderProgram.SetUniformValue (0, ref worldViewProj);
			
			graphics.SetShaderProgram (textureShaderProgram);
			graphics.SetVertexBuffer (0, sprite.Vertices);
			graphics.SetTexture (0, sprite.Texture);
			
			graphics.Enable (EnableMode.Blend);
			graphics.SetBlendFunc (BlendFuncMode.Add, BlendFuncFactor.SrcAlpha, BlendFuncFactor.OneMinusSrcAlpha);
			
			graphics.DrawArrays (DrawMode.TriangleFan, 0, 4); //Why fan instead of triangle strip?
			
			sprite.SetLifeTimer ();
		}

		public static Font CurrentFont {
			get{ return currentFont;}
		}

		public static Font SetDefaultFont ()
		{
			return SetFont (defaultFont);
		}

		public static Font SetFont (Font font)
		{
			Font previousFont = currentFont;
			currentFont = font;
			return previousFont;
		}

		public static void DrawText (string text, uint argb, Font font, int positionX, int positionY)
		{
			AddSprite (text, text, argb, font, positionX, positionY);
			DrawSprite (text);
			
		}

		public static void DrawText (string text, uint argb, int positionX, int positionY)
		{
			AddSprite (text, text, argb, currentFont, positionX, positionY);
			DrawSprite (text);
			
		}

		public static void FillRect (uint argb, int positionX, int positionY, int rectW, int rectH)
		{
			FillVertices (rectVertices, argb, positionX, positionY, rectW, rectH);
		}

		public static void FillCircle (uint argb, int positionX, int positionY, int radius)
		{
			FillVertices (circleVertices, argb, positionX, positionY, radius, radius);
		}
		
		public static void FillVertices (VertexBuffer vertices, uint argb, float positionX, float positionY, float scaleX, float scaleY)
		{
			Matrix4 transMatrix = Matrix4.Translation (new Vector3 (positionX, positionY, 0.0f));
			Matrix4 scaleMatrix = Matrix4.Scale (new Vector3 (scaleX, scaleY, 1.0f));
			Matrix4 modelMatrix = transMatrix * scaleMatrix;
			Matrix4 worldViewProj = projectionMatrix * viewMatrix * modelMatrix;
			
			colorShaderProgram.SetUniformValue (0, ref worldViewProj);
			
			Vector4 color = new Vector4 ((float)((argb >> 16) & 0xff) / 0xff,
			                            (float)((argb >> 8) & 0xff) / 0xff,
			                            (float)((argb >> 0) & 0xff) / 0xff,
			                            (float)((argb >> 24) & 0xff) / 0xff);
			colorShaderProgram.SetUniformValue (colorShaderProgram.FindUniform ("MaterialColor"), ref color);
			
			graphics.SetShaderProgram (colorShaderProgram);
			graphics.SetVertexBuffer (0, vertices);
			
			graphics.DrawArrays (DrawMode.TriangleFan, 0, vertices.VertexCount);
			
		}
		
		private static ShaderProgram createSimpleTextureShader ()
		{
			string ResourceName = "Basic.Framework.shaders.Texture.cgx";
			
			Assembly resourcesAssembly = Assembly.GetExecutingAssembly ();
			if (resourcesAssembly.GetManifestResourceInfo (ResourceName) == null) {
				throw new FileNotFoundException ("File not found.", ResourceName); 
			}
			
			Stream fileStreamVertex = resourcesAssembly.GetManifestResourceStream (ResourceName);
			Byte[] dataBufferVertex = new Byte[fileStreamVertex.Length];
			fileStreamVertex.Read (dataBufferVertex, 0, dataBufferVertex.Length);
			
			ShaderProgram shaderProgram = new ShaderProgram (dataBufferVertex);
			
			shaderProgram.SetAttributeBinding (0, "a_Position");
			shaderProgram.SetAttributeBinding (1, "a_TexCoord");
			
			shaderProgram.SetUniformBinding (0, "WorldViewProj");
			
			return shaderProgram;
			
		}

		private static ShaderProgram createSimpleColorShader ()
		{
			string ResourceName = "Basic.Framework.shaders.Simple.cgx";
            
			Assembly resourceAssembly = Assembly.GetExecutingAssembly ();
			if (resourceAssembly.GetManifestResourceInfo (ResourceName) == null) {
				throw new FileNotFoundException ("File not found.", ResourceName);
			}

			Stream fileStreamVertex = resourceAssembly.GetManifestResourceStream (ResourceName);
			Byte[] dataBufferVertex = new Byte[fileStreamVertex.Length];
			fileStreamVertex.Read (dataBufferVertex, 0, dataBufferVertex.Length);
            
			var shaderProgram = new ShaderProgram (dataBufferVertex);
			// var shaderProgram = new ShaderProgram("/Application/shaders/Simple.cgx");

			shaderProgram.SetAttributeBinding (0, "a_Position");
			//shaderProgram.SetAttributeBinding(1, "a_Color0");

			shaderProgram.SetUniformBinding (0, "WorldViewProj");

			return shaderProgram;
		}
	}
	
		
}


