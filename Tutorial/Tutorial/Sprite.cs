using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Imaging;


namespace Tutorial
{
	public class Sprite: IDisposable
	{
		private VertexBuffer vertices;
		private Vector2 position;
		private Vector2 center;
		private float degree;
		private Vector2 scale;
		private Texture2D texture;
		
		public Sprite (Texture2D texture,float posX,float posY)
		{
			SetTexture(texture);
			
		}
		public void Dispose()
		{
			vertices.Dispose();
			texture.Dispose();
		}
		public float PositionX
		{
			get {return position.X;}
			set {position.X = value;}
		}
		public float PositionY
		{
			get {return position.Y;}
			set {position.Y = value;}
		}
		public Vector2 Position
		{
			get {return position;}
			set {position = value;}
		}
		public float CenterX
		{
			get {return center.X;}
			set {center.X = value;}
		}
		public float CenterY
		{
			get {return center.Y;}
			set {center.Y = value;}
		}
		public Vector2 Center
		{
			get {return center;}
			set {center = value;}
		}
		public float Degree
		{
        get {return degree;}
        set {degree = value;}
		}
		public float ScaleX
		{
			get {return scale.X;}
			set {scale.X = value;}
		}
		public float ScaleY
		{
			get {return scale.Y;}
			set {scale.Y = value;}
		}
		public Vector2 Scale
		{
			get {return scale;}
			set {scale = value;}
		}
		
	public VertexBuffer Vertices
		{
			get {return vertices;}
		}
		public Texture2D Texture
		{
			get {return texture; }
		}
		
		
		public void SetTexture(Texture2D texture){

			this.texture = texture.ShallowClone() as Texture2D;
			float l = 0;
			float t = 0;
			float r = texture.Width;
			float b = texture.Height;
			vertices = new VertexBuffer(4, VertexFormat.Float3, VertexFormat.Float2);
			vertices.SetVertices(0, new float[]{l, t, 0,r, t, 0,r, b, 0,l, b, 0});
			vertices.SetVertices(1, new float[]{0.0f, 0.0f,1.0f, 0.0f,1.0f, 1.0f,0.0f, 1.0f});
		}
		public Matrix4 CreateModelMatrix(){
			Matrix4 centerMatrix = Matrix4.Translation(new Vector3(-center.X, -center.Y, 0.0f));
			Matrix4 transMatrix = Matrix4.Translation(new Vector3(position.X + center.X, position.Y + center.Y, 0.0f));
			Matrix4 rotMatrix = Matrix4.RotationZ((float)(degree / 180.0f * FMath.PI));
			Matrix4 scaleMatrix = Matrix4.Scale(new Vector3(scale.X, scale.Y, 1.0f));
			
			return transMatrix * rotMatrix * scaleMatrix * centerMatrix;
}
	}
}


