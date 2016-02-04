using DGPE.Math.FixedPoint.Geometry3D;
using DGPE.Math.FixedPoint;
using UnityEngine;
namespace DEngine.Utils.MeshGeneration{
	public class FixedGridXZ3DMeshGenerator{
		private const int VERTICES_IN_TRIANGLE_COUNT = 3;
		private const int TRIANGLE_VERTICES_INDEXES_IN_CELL_COUNT = 6;
		private static Vector3 FixedVector3ToUnityVector(FixedVector3 fv){
			return new Vector3((float)fv.x,(float)fv.y,(float)fv.z);
		}
		private static Vector3[] CreateVerticesArray(FixedGridXZ3D grid){
			int vertexCount = grid.GetVertexCount();
			Vector3[] vertices = new Vector3[grid.GetVertexCount()];
			for(int i = 0;i<vertexCount;i++)
				vertices[i] = FixedVector3ToUnityVector(grid.GetVertexCoordinates(i));
			return vertices;
		}
		private static int[] CreateTrianglesArray(FixedGridXZ3D grid){
			int triangleIndexesBegin = 0;
			int[] triangles = new int[grid.GetTriangleCount()*TRIANGLE_VERTICES_INDEXES_IN_CELL_COUNT];
			for(int z = 0;z<grid.height;z++){
				for(int x = 0;x<grid.width;x++){
					grid.PutCellTriangleIndexesToArray(triangleIndexesBegin,triangles,x,z);
					triangleIndexesBegin+=TRIANGLE_VERTICES_INDEXES_IN_CELL_COUNT;
				}
			}
			return triangles;
		}
		private static Vector2[] CreateUVArray(FixedGridXZ3D grid){
			//Debug.Log("creating uv");
			float uWidth = 1f/(grid.width);
			float uHeight = 1f/(grid.height);
			float u = 0;
			float v = 0;
			Vector2[]uv = new Vector2[grid.GetVertexCount()];
			for(int z = 0;z<grid.height+1;z++){
				for(int x = 0;x<grid.width+1;x++){
					u = x*uWidth;
					v = z*uHeight;
					if(u>1)
						u = 1;
					if(v>1)
						v = 1;
					uv[z*(grid.width+1)+x]=new Vector2(u,v);
				}
			}
			return uv;
		}
		private static void UpdateYCoordinate(FixedGridXZ3D grid,Mesh mesh){
			int vertexCount = grid.GetVertexCount();
			for(int i = 0;i<vertexCount;i++){
				mesh.vertices[i].y  = (float)grid.GetVertexCoordinates(i).y;
			}
		}
		public static Mesh GenerateMesh(FixedGridXZ3D grid){
			Mesh mesh = new Mesh();
			mesh.vertices = CreateVerticesArray(grid);
			mesh.triangles = CreateTrianglesArray(grid);
			mesh.uv = CreateUVArray(grid);
			mesh.RecalculateNormals();
			mesh.Optimize();//it should be used always
			return mesh;
		}
		public static void UpdateGeneratedMesh(FixedGridXZ3D grid,Mesh mesh){
			UpdateYCoordinate(grid,mesh);
			mesh.RecalculateNormals();
			mesh.Optimize();
		}
	}
}
