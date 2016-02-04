using UnityEngine;
using System.Collections;
using DGPE.Math.FixedPoint;
using DGPE.Math.FixedPoint.Geometry2D;
using DGPE.Math.FixedPoint.Geometry3D;
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
		System.Random rand = new System.Random();
		Fixed cellSize = (Fixed)size;
		grid = new FixedGridXZ3D(width,height,cellSize);
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		mesh.name = grid.ToString();
		for(int z = 0;z<grid.height+1;z++){
			for(int x = 0;x<grid.width+1;x++){
				grid.SetYOfVertex(x,z,cellSize/rand.Next(1,10));
			}
		}
		Vector3[]vertices = new Vector3[grid.GetVertexCount()];
		Vector2[]uv = GenerateUV(grid.width+1,grid.height+1);
		int[] triangles = new int[grid.GetTriangleCount()*3];
		for(int i = 0;i<grid.GetVertexCount();i++){
			vertices[i] = FixedVector3ToUnityVector(grid.GetVertexCoordinates(i));
		}
		int triangleOffset = 0;
		for(int x = 0;x<grid.width;x++){
			for(int z = 0;z<grid.height;z++){
				grid.PutCellTriangleIndexesToArray(triangleOffset,triangles,x,z);
				triangleOffset+=6;
			}
		}
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
	}
	// Update is called once per frame
	void Update ()
	{
	
	}
}

