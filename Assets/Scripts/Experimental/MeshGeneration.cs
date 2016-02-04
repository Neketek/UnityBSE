using UnityEngine;
using System.Collections;
using DGPE.Math.FixedPoint;
using DGPE.Math.FixedPoint.Geometry2D;
using DGPE.Math.FixedPoint.Geometry3D;
using DEngine.Utils;
using DEngine.Utils.MeshGeneration;
public class MeshGeneration : MonoBehaviour
{
	FixedGridXZ3D grid = null;
	public int width = 1,height = 1;
	public float size = 1;
	private static Vector3 FixedVector3ToUnityVector(FixedVector3 fv){
		return new Vector3((float)fv.x,(float)fv.y,(float)fv.z);
	}	
	Vector2[] GenerateUV(int width,int height){
		float uWidth = 1f/width;
		float uHeight = 1f/height;
		Vector2[] uv = new Vector2[width*height];
		for(int z = 0;z<height;z++){
			for(int x = 0;x<width;x++){
				uv[z*width+x]=new Vector2(x*uWidth,z*uHeight);
			}
		}
		return uv;
	}
	void Start ()
	{
		RandomFixedGenerator fixedRandom = new RandomFixedGenerator(1);
		System.Random rand = new System.Random();
		Fixed cellSize = (Fixed)size;
		grid = new FixedGridXZ3D(width,height,cellSize);
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		for(int z = 0;z<grid.height+1;z++){
			for(int x = 0;x<grid.width+1;x++){
				grid.SetYOfVertex(x,z,fixedRandom.Next());
			}
		}
		meshFilter.mesh = FixedGridXZ3DMeshGenerator.GenerateMesh(grid);
	}
	// Update is called once per frame
	void Update ()
	{
	
	}
}

