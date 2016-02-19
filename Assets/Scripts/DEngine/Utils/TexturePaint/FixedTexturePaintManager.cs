using UnityEngine;
using DGPE.Math.FixedPoint.Geometry2D;
namespace DEngine.Utils.TexturePaint{
	public class FixedTexturePaintManager{
		public delegate Color PixelMixingRule(Color current,Color mixer);
		public delegate void PaintAction(int x,int y,Color value);
		private Texture2D currentTexture;
		private PixelMixingRule currentPixelMixingRule;
		public FixedTexturePaintManager(Texture2D texture){
			CurrentTexture = texture;
			CurrentPixelMixingRule = MixingMultiplicative;
		}
		public void Clear(Color value){
			DrawPixels(0,currentTexture.height-1,0,currentTexture.width-1,value);
		}
		public void MixPixel(int x,int y,Color mixer){
			currentTexture.SetPixel(x,y,currentPixelMixingRule(currentTexture.GetPixel(x,y),mixer));
		}
		public void DrawPixel(int x,int y,Color color){
			currentTexture.SetPixel(x,y,color);
		}
		public void MixPixels(int bottom,int top,int left,int right,Color mixer){
			UsePaintActionOnArea(bottom,top,left,right,mixer,MixPixel,null);	
		}
		public void DrawPixels(int bottom,int top,int left,int right,Color value){
			UsePaintActionOnArea(bottom,top,left,right,value,DrawPixel,null);	
		}
		public void DrawPixels(FixedShape2D shape, Color value){
			UsePaintActionOnArea((int)shape.GetBoundingBoxMinY()-1,
			                     (int)shape.GetBoundingBoxMaxY()+1,
			                     (int)shape.GetBoundingBoxMinX()-1,
			                     (int)shape.GetBoundingBoxMaxX()+1,
			                     value,DrawPixel,shape);
		}
		public void MixPixels(FixedShape2D shape,Color mixer){
			UsePaintActionOnArea((int)shape.GetBoundingBoxMinY()-1,
			                     (int)shape.GetBoundingBoxMaxY()+1,
			                     (int)shape.GetBoundingBoxMinX()-1,
			                     (int)shape.GetBoundingBoxMaxX()+1,
			                     mixer,MixPixel,shape);
		}
		public Color MixingMultiplicative(Color current,Color mixer){
			current.r = current.r*mixer.r;
			current.g = current.g*mixer.g;
			current.b = current.b*mixer.b;
			return current;
		}
		public Color MixingAdditive(Color current,Color mixer){
			current.r = current.r+mixer.r;
			current.g = current.g+mixer.g;
			current.b = current.b+mixer.b;
			return current;
		}
		Texture2D CurrentTexture {
			get {
				return this.currentTexture;
			}
			set {
				if(value == null)
					throw new System.ArgumentNullException("CurrentTexture");
				currentTexture = value;
			}
		}

		PixelMixingRule CurrentPixelMixingRule {
			get {
				return this.currentPixelMixingRule;
			}
			set {
				if(value == null)
					throw new System.ArgumentNullException("CurrentPixelMixingRule");
				currentPixelMixingRule = value;
			}
		}
		private bool CheckBounds(int x,int y){
			return x>=0&&x<currentTexture.width&&y>=0&&y<currentTexture.height;
		}
		private void UsePaintActionOnArea(int bottom,int top,int left,int right,Color value,PaintAction action,FixedShape2D shape){
			for(int y = bottom;y<=top;y++){
				for(int x = left;x<=right;x++){
					if(CheckBounds(x,y)){
						if(shape!=null){
							if(shape.Contains(x,y)){
								action(x,y,value);
								//Debug.Log("Draw");
							}
						}else{
							action(x,y,value);
						}
					}
				}
			}
		}
	}
}
