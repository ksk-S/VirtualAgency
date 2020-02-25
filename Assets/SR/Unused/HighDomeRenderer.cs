//rendering high polygon sphere

using UnityEngine;
using System.Collections;

public class HighDomeRenderer : MonoBehaviour {
    
     public float fRadius = 2.0f;
     public int PHI_STEPS = 10;
     public int THETA_STEPS = 10;
    
     float PI = 3.1415926f;
    
     Vector3[] vertices;
     int[] triangles;
     Vector2[] uvs;
    
     // Use this for initialization
     void Start () {
          //get a reference to the mesh
          MeshFilter BaseMeshFilter = transform.GetComponent("MeshFilter") as MeshFilter;
          Mesh mesh = BaseMeshFilter.mesh;
         
          mesh.Clear();
         
          float[]  fX = new float[4], fY = new float[4], fZ = new float[4];
          float    fPhi_0, fPhi_1, fTheta_0, fTheta_1;
          float    fPhiSteps = PHI_STEPS;
          float    fThetaSteps = THETA_STEPS;
         
          vertices = new Vector3[PHI_STEPS * THETA_STEPS * 4];
          triangles = new int[PHI_STEPS * THETA_STEPS * 6];
          uvs = new Vector2[PHI_STEPS * THETA_STEPS * 4];
         
          for ( int i = 0; i < fPhiSteps; i++ )
          {
               fPhi_0 = PI * (i+0)/fPhiSteps;
               fPhi_1 = PI * (i+1)/fPhiSteps;
			
			   fTheta_0 = -(( PI * 3.0f )/2.0f);
              
               fX[2] = fRadius * Mathf.Sin( fPhi_1 ) * Mathf.Cos( fTheta_0 );
               fX[3] = fRadius * Mathf.Sin( fPhi_0 ) * Mathf.Cos( fTheta_0 );

               fY[1] = fY[2] = fRadius * Mathf.Cos( fPhi_1 );
               fY[0] = fY[3] = fRadius * Mathf.Cos( fPhi_0 );

               fZ[2] = fRadius * Mathf.Sin( fPhi_1 ) * Mathf.Sin( fTheta_0 );
               fZ[3] = fRadius * Mathf.Sin( fPhi_0 ) * Mathf.Sin( fTheta_0 );

               for ( int j = 0; j < fThetaSteps; j++ )
               {    
                    fTheta_1 = ( 2.0f * PI * (j+1)/fThetaSteps ) - ( PI * 3.0f/2.0f );

                    fX[0] = fX[3];
                    fX[1] = fX[2];
                    fX[2] = fRadius * Mathf.Sin( fPhi_1 ) * Mathf.Cos( fTheta_1 );
                    fX[3] = fRadius * Mathf.Sin( fPhi_0 ) * Mathf.Cos( fTheta_1 );

                    fZ[0] = fZ[3];
                    fZ[1] = fZ[2];
                    fZ[2] = fRadius * Mathf.Sin( fPhi_1 ) * Mathf.Sin( fTheta_1 );
                    fZ[3] = fRadius * Mathf.Sin( fPhi_0 ) * Mathf.Sin( fTheta_1 );
                   
				
                    int index = j + i * THETA_STEPS;
                   
                    for(int k=0; k<4; k++){
                         vertices[index * 4  + k] =  new Vector3(fX[k], fY[k], fZ[k]);
                    }

					float fTheta_0_tmp = ( 2.0f * PI * j/fThetaSteps )  ;
					float fTheta_1_tmp = ( 2.0f * PI * (j+1)/fThetaSteps )  ;
    				
					float x_0 = fTheta_0_tmp/ 2 / PI;
					float x_1 = fTheta_1_tmp/ 2 / PI; 
					float y_0 = fPhi_0 / PI ;
					float y_1 = fPhi_1 / PI ;
					Debug.Log (x_0 + " " +x_1 );
				
                    uvs[index * 4 + 0] = new Vector2(x_0,y_0);
                    uvs[index * 4 + 1] = new Vector2(x_0,y_1 );
                    uvs[index * 4 + 2] = new Vector2(x_1,y_1 );
                    uvs[index * 4 + 3] = new Vector2(x_1,y_0);
                   
                    triangles[index * 6 + 0] = index * 4  + 2;
                    triangles[index * 6 + 1] = index * 4 + 0;
                    triangles[index * 6 + 2] = index * 4 + 1;
                    triangles[index * 6 + 3] = index * 4 + 2;
                    triangles[index * 6 + 4] = index * 4 + 3;
                    triangles[index * 6 + 5] = index * 4 + 0;
                   
               }
          }
         
         
         //copy local built in arrays back to the mesh
         mesh.vertices = vertices;
         mesh.uv = uvs;
         mesh.triangles=triangles;
         
         mesh.RecalculateNormals();
         mesh.RecalculateBounds();
		
		//invertion
		/*
		int numpolies = triangles.Length / 3;
		for(int t=0;t < numpolies;t++)
        {
        	int tribuffer = triangles[t*3];
        	triangles[t*3]=triangles[(t*3)+2];
        	triangles[(t*3)+2]=tribuffer;
        }
 
    	//readjust uv map for inner sphere projection
    	for(int uvnum = 0;uvnum < uvs.Length;uvnum++)
        {
       		uvs[uvnum]=new Vector2(1-uvs[uvnum].x,uvs[uvnum].y);
       	}
 
    	//readjust normals for inner sphere projection 
    	Vector3[] norms = mesh.normals;       
    	for(int normalsnum = 0;normalsnum < norms.Length;normalsnum++)
       	{
       		norms[normalsnum]=-norms[normalsnum];
       	}
 
    	//copy local built in arrays back to the mesh
    	mesh.uv = uvs;
    	mesh.triangles=triangles;
    	mesh.normals=norms;    
		*/
		
     }
    
     // Update is called once per frame
     void Update () {
    
     }
}