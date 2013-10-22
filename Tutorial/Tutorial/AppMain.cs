/*
 * A run through the PSM Tutorials
 * 
 * by Justin Loudermilk
 * */

using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.Core.Input;

namespace Tutorial
{
	public class AppMain
	{
		private static GraphicsContext graphics;
		public static GamePadData gamePadData;
		//tutorial 1: clearing the screen and fade color
		static float colorValue = 0;
		static Vector4 clearColor;
		
		public enum COLOR
		{
			WHITE,
			BLACK,
			RED,
			BLUE,
			GREEN
			
		}
		
		//tutorial 2: display a sprite
		static ShaderProgram shaderProgram;
		static Texture2D texture;
		static float[] vertices = new float[12];
		static float [] texcoords = 
		{
			0.0f, 0.0f,	// 0 top left.
			0.0f, 1.0f,	// 1 bottom left.
			1.0f, 0.0f,	// 2 top right.
			1.0f, 1.0f,	// 3 bottom right.
			
		};
				static float[] colors = {
			1.0f,	1.0f,	1.0f,	1.0f,	// 0 top left.
			1.0f,	1.0f,	1.0f,	1.0f,	// 1 bottom left.
			1.0f,	1.0f,	1.0f,	1.0f,	// 2 top right.
			1.0f,	1.0f,	1.0f,	1.0f,	// 3 bottom right.
		};
		
		const int indexSize = 4;
		
		static ushort[] indices;
		
		static VertexBuffer vertexBuffer;
		
		static Matrix4 screenMatrix;
		
		
		public static void Main (string[] args)
		{
			Initialize ();

			while (true) {
				SystemEvents.CheckEvents ();
				Update ();
				Render ();
			}
		}

		public static void Initialize ()
		{
			// Set up the graphics system
			graphics = new GraphicsContext ();
			
			ImageRect rectScreen = graphics.Screen.Rectangle;
			
			texture = new Texture2D("/Application/resources/Player.png", false);
			shaderProgram = new ShaderProgram("/Application/shaders/Sprite.cgx");
			shaderProgram.SetUniformBinding(0, "u_ScreenMatrix");

			vertices[0]=0.0f;	// x0
			vertices[1]=0.0f;	// y0
			vertices[2]=0.0f;	// z0
			
			vertices[3]=0.0f;	// x1
			vertices[4]=texture.Height;	// y1
			vertices[5]=0.0f;	// z1
			
			vertices[6]=texture.Width;	// x2
			vertices[7]=0.0f;	// y2
			vertices[8]=0.0f;	// z2
			
			vertices[9]=texture.Width;	// x3
			vertices[10]=texture.Height;	// y3
			vertices[11]=0.0f;	// z3
			

			indices = new ushort[indexSize];
			indices[0] = 0;
			indices[1] = 1;
			indices[2] = 2;
			indices[3] = 3;
			
			//												vertex pos,               texture,       color
			vertexBuffer = new VertexBuffer(4, indexSize, VertexFormat.Float3, VertexFormat.Float2, VertexFormat.Float4);
			

			vertexBuffer.SetVertices(0, vertices);
			vertexBuffer.SetVertices(1, texcoords);
			vertexBuffer.SetVertices(2, colors);
			
			vertexBuffer.SetIndices(indices);
			graphics.SetVertexBuffer(0, vertexBuffer);

			screenMatrix = new Matrix4(
				 2.0f/rectScreen.Width,	0.0f,	    0.0f, 0.0f,
				 0.0f,   -2.0f/rectScreen.Height,	0.0f, 0.0f,
				 0.0f,   0.0f, 1.0f, 0.0f,
				 -1.0f,  1.0f, 0.0f, 1.0f
			);
		}

		public static void Update ()
		{
			
			//tutorial 1 stuff
			//fadeTo (COLOR.RED);
			// Query gamepad for current state
			gamePadData = GamePad.GetData (0);
		}

		public static void Render ()
		{
			// Clear the screen
		
			graphics.Clear ();
			
			graphics.SetShaderProgram(shaderProgram);
			graphics.SetTexture(0,texture);
			shaderProgram.SetUniformValue(0, ref screenMatrix);
			
			graphics.DrawArrays(DrawMode.TriangleStrip,0,indexSize);
			
			// Present the screen
			graphics.SwapBuffers ();
		}

		public static bool fadeTo (COLOR c, float a_fSpeed = .01f)
		{
			clearColor = graphics.GetClearColor ();
			switch (c) {
			case COLOR.WHITE:
				if (colorValue > 1)
					break;
				clearColor += a_fSpeed;
				colorValue += a_fSpeed;
				graphics.SetClearColor (clearColor);
				return true;
			case COLOR.BLACK:
				if (colorValue < 0)
					break;
				clearColor -= a_fSpeed;
				colorValue -= a_fSpeed;
				graphics.SetClearColor (clearColor);
				return true;
			case COLOR.RED:
				if (colorValue > 1)
					break;
				clearColor -= a_fSpeed;
				clearColor.R += a_fSpeed * 2;
				colorValue += a_fSpeed;
				graphics.SetClearColor (clearColor);
				return true;
			case COLOR.GREEN:
				if (colorValue > 1)
					break;
				clearColor -= a_fSpeed;
				clearColor.G += a_fSpeed * 2;
				colorValue += a_fSpeed;
				graphics.SetClearColor (clearColor);
				return true;
			case COLOR.BLUE:
				if (colorValue > 1)
					break;
				clearColor -= a_fSpeed;
				clearColor.B += a_fSpeed * 2;
				colorValue += a_fSpeed;
				graphics.SetClearColor (clearColor);
				return true;

			default:
				return false;
			}
			return false;
			
		}

		
	}
}
