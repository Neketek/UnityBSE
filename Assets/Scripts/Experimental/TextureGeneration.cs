using UnityEngine;
using System.Collections;
using DEngine.Utils;
public class TextureGeneration : MonoBehaviour {

	public Material material = null;// Use this for initialization
	public Color first,second;
	public int textureSize = 2048;
	public FilterMode filterMode = FilterMode.Bilinear;
	void Start () {
		RandomColorGenerator rColor = new RandomColorGenerator(1);
		Texture2D texture = new Texture2D(textureSize,textureSize);
		for(int y = 0;y<texture.height;y++){
			for(int x = 0;x<texture.width;x++){
				texture.SetPixel(x,y,rColor.Next());
			}
		}
		texture.filterMode = FilterMode.Trilinear;
		texture.Apply();
		material.mainTexture = texture;
		//Debug.Log(texture.texelSize);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
