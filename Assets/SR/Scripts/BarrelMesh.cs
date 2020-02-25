using UnityEngine;
using System.Collections;

public class BarrelMesh : MonoBehaviour {

	public float k1 =  1.1f;
	public float k2 = -1.87f;
	public float k3 =  2.0f;
	public float k4 =   0.0f;

	public int resX = 50; // 2 minimum
	public int resZ = 50;

	public bool updateEveryFrame = false;

	MeshFilter filter;

	// Use this for initialization
	void Start () {
		filter = gameObject.AddComponent< MeshFilter >();
		CreateMesh();
		ReMesh();
	}
	
	// Update is called once per frame
	void Update () {
		if(updateEveryFrame){
			CreateMesh();
			ReMesh();
		}
	}

	public void Reset(){
		CreateMesh();
		ReMesh();
	}

	void CreateMesh(){
		Mesh mesh = filter.mesh;
		mesh.Clear();
		
		float length = 10f;
		float width = 10f;
		
		#region Vertices		
		Vector3[] vertices = new Vector3[ resX * resZ ];
		for(int z = 0; z < resZ; z++)
		{
			// [ -length / 2, length / 2 ]
			float zPos = ((float)z / (resZ - 1) - .5f) * length;
			for(int x = 0; x < resX; x++)
			{
				// [ -width / 2, width / 2 ]
				float xPos = ((float)x / (resX - 1) - .5f) * width;
				vertices[ x + z * resX ] = new Vector3( xPos, 0f, zPos );
			}
		}
		#endregion
		
		#region Normales
		Vector3[] normales = new Vector3[ vertices.Length ];
		for( int n = 0; n < normales.Length; n++ )
			normales[n] = Vector3.up;
		#endregion
		
		#region UVs		
		Vector2[] uvs = new Vector2[ vertices.Length ];
		for(int v = 0; v < resZ; v++)
		{
			for(int u = 0; u < resX; u++)
			{
				uvs[ u + v * resX ] = new Vector2( (float)u / (resX - 1), (float)v / (resZ - 1) );
			}
		}
		#endregion
		
		#region Triangles
		int nbFaces = (resX - 1) * (resZ - 1);
		int[] triangles = new int[ nbFaces * 6 ];
		int t = 0;
		for(int face = 0; face < nbFaces; face++ )
		{
			// Retrieve lower left corner from face ind
			int i = face % (resX - 1) + (face / (resZ - 1) * resX);
			
			triangles[t++] = i + resX;
			triangles[t++] = i + 1;
			triangles[t++] = i;
			
			triangles[t++] = i + resX;	
			triangles[t++] = i + resX + 1;
			triangles[t++] = i + 1; 
		}
		#endregion
		
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		;

	}

	void ReMesh(){
		//get a reference to the mesh
		MeshFilter BaseMeshFilter = transform.GetComponent("MeshFilter") as MeshFilter;
		Mesh mesh = BaseMeshFilter.mesh;
		
		//readjust uv map for inner sphere projection
		Vector2[] uvs = mesh.uv;
		for(int uvnum = 0;uvnum < uvs.Length;uvnum++)
		{
			float x = uvs[uvnum].x - 0.5f;
			float y = uvs[uvnum].y - 0.5f;
			
			float r = Mathf.Sqrt(x * x + y * y);
			float theta =Mathf.Atan2(y, x);
			
			float r2 = r*r;
			float r4 = r2*r2;
			float r6 = r2*r2*r2;
			r = r * (k1 + k2*r2 + k3*r4 + k4 * r6); 
			
			Vector2 new_cordinate = new Vector2(Mathf.Cos(theta) * r + 0.5f, Mathf.Sin(theta) * r + 0.5f);
			
			uvs[uvnum] = new_cordinate;
		}
		
		//copy local built in arrays back to the mesh
		mesh.uv = uvs;

	}


	
}
