using UnityEngine;
using System.Collections;
using DEngine.Utils;
using DEngine.Utils.TexturePaint;
using DGPE.Math.FixedPoint.Geometry2D;
using DGPE.Math.FixedPoint;
public class TextureGeneration : MonoBehaviour {

	public Material material = null;// Use this for initialization
	public int textureSize = 2048;
	public Color clear,figure;
	public int shapeType  = 0;
	public float updateTime = 1;
	private float currentUpdateTime = 0;
	private FixedShape2D shape = null;
	FixedVector2 rotationPivot = FixedVector2.CreateZeroVector();
	FixedTexturePaintManager pm = null;
	Texture2D text;
	void Start () {
		int texSizeD2 = textureSize/2;
		int texSizeD4 = textureSize/4;
		switch(shapeType){
		case 0:
			shape = new FixedCircle2D(new FixedVertex2D(textureSize/2,textureSize/2),(Fixed)textureSize/4);
			break;
		case 1:
			FixedVector2 a = new FixedVector2(texSizeD2-texSizeD4,texSizeD2-texSizeD4);
			FixedVector2 b = new FixedVector2(texSizeD2+texSizeD4,texSizeD2-texSizeD4);
			FixedVector2 c = new FixedVector2(texSizeD2,texSizeD2+texSizeD4);
			shape = new FixedTriangle2D(a,b,c);
			break;
		case 2:
			Fixed bot = (Fixed)(texSizeD2-texSizeD4);
			Fixed l = (Fixed)(texSizeD2-texSizeD4);
			Fixed s = (Fixed)(texSizeD2);
			shape = new FixedRectangle2D(l,bot,s,s);
			break;
		default:
			shape = new FixedCircle2D(new FixedVertex2D(textureSize/2,textureSize/2),(Fixed)textureSize/4);
			break;
		}
		text = new Texture2D(textureSize,textureSize);
		pm = new FixedTexturePaintManager(text);
		pm.Clear(clear);
		pm.DrawPixels(shape,figure);
		text.Apply();
		material.mainTexture = text;
	}
	// Update is called once per frame
	void Update () {
		if(currentUpdateTime<updateTime){
			currentUpdateTime+=Time.deltaTime;
			return;
		}
		shape.RotateZAxe(1,new FixedVector2(textureSize/2,textureSize/2));
		pm.Clear(clear);
		pm.DrawPixels(shape,figure);
		text.Apply();
		material.mainTexture = text;
		currentUpdateTime = 0;
	}
}
