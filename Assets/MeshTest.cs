using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTest : MonoBehaviour {


	int colNum = 95;
	int rowNum = 37;
	float size = 0.5f;

	public float radius = 1f;

	// Use this for initialization
	void Start () {

		this.InitTriangle ();
		
	}
	
	// Update is called once per frame
	void Update () {


		if (Input.GetMouseButtonDown (0)) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				boom (hit.point, radius);
			}

		}

	
		
	}

	public void InitTriangle ()
	{
		MeshFilter filter = gameObject.AddComponent<MeshFilter> ();

		// 构建三角形的三个顶点，并赋值给Mesh.vertices
		Mesh mesh = new Mesh ();
		filter.sharedMesh = mesh;

		int pointNum = colNum * rowNum* 4;



		Vector3[] points = new Vector3[pointNum];
		int counter = 0;

		for (int i = 0; i < colNum; i++) {
		
			for (int j = 0; j < rowNum; j++) {
				points [counter++] = new Vector3 (i*size, j * size, 0f);
				points [counter++] = new Vector3 (i * size, (j + 1) * size, 0f);
				points [counter++] = new Vector3 ((i+1) * size, j* size, 0f);
				points [counter++] = new Vector3 ((i+1) * size, (j+1)* size, 0f);
			}
		}



		mesh.vertices = points;


		int triNum = (pointNum / 4) * 2 * 3;
	
		int[] triangles = new int[triNum];
		int mcounter = 0;
		int indexCounter = 0;
		while (mcounter < triangles.Length ) {
		
			int tmpCounter = indexCounter;
			triangles [mcounter++] = tmpCounter;
			triangles [mcounter++] = tmpCounter + 1;
			triangles [mcounter++] = tmpCounter + 2;
			triangles [mcounter++] = tmpCounter + 2;
			triangles [mcounter++] = tmpCounter + 1;
			triangles [mcounter++] = tmpCounter + 3;
			indexCounter += 4;

		
		}



		mesh.triangles = triangles;
//			new int[]{0,1,2,3,4,5};

		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();

		// 使用Shader构建一个材质，并设置材质的颜色。
		Material material = new Material (Shader.Find ("Diffuse"));
		material.SetColor ("_Color", Color.black);

		// 构建一个MeshRender并把上面创建的材质赋值给它，
		// 然后使其把上面构造的Mesh渲染到屏幕上。
		MeshRenderer renderer = gameObject.AddComponent<MeshRenderer> ();
		renderer.sharedMaterial = material;

		gameObject.GetComponent<MeshCollider> ().sharedMesh = filter.sharedMesh;

	}


	void OnGUI()
	{
		//Example of polling state 

		GUILayout.BeginArea(new Rect(50,100,120,40));

		if( GUILayout.Button("desory"))
		{
			Mesh msh = this.GetComponent<MeshFilter> ().mesh;

			int x = 5;
			int[] tris = msh.triangles;
			tris [(x - 1) * 3] = 0; ;
			tris [(x - 1) * 3+1] = 0; 
			tris [(x - 1) * 3+2] = 0;
			msh.triangles = tris;

			msh.RecalculateNormals ();
			msh.RecalculateBounds ();
			MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
			//filter.sharedMesh = msh;

		}

		GUILayout.EndArea();
	}


	public void boom(Vector3 pos,float r){
	
		Vector3 rpos = pos - this.transform.position;
		MeshFilter filter = gameObject.GetComponent<MeshFilter> ();
		Mesh mesh = filter.sharedMesh;

		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector3 p1, p2, p3;

		Vector3 tpos1 = rpos;
		tpos1.x -= r;
		tpos1.y -= r;



		int startCol = (int)(tpos1.x / size);
		int startRow = (int)(tpos1.y / size);


		Vector3 tpos2 = rpos;
		tpos2.x += r;
		tpos2.y += r;

		int endCol =(int)(tpos2.x / size);
		int endRow =(int)(tpos2.y / size);


		startCol = Mathf.Clamp (startCol, 0, startCol);
		startRow = Mathf.Clamp (startRow, 0, startRow);
		endCol = Mathf.Clamp (endCol, 0, endCol);
		endRow = Mathf.Clamp (endRow, 0, endRow);


	
		List<int> triangIndexNeedToCheck = new List<int> ();
		int triIndex;

		for (int m = startCol; m <= endCol; m++) {
			for (int n = startRow; n <= endRow; n++) {
				triIndex = (m-1) * rowNum * 2 + n * 2;
				triIndex = Mathf.Clamp (triIndex, 0, rowNum*colNum*2);
				triangIndexNeedToCheck.Add (triIndex);
				triangIndexNeedToCheck.Add (triIndex + 1);
			}
		}





		foreach  (int index in triangIndexNeedToCheck) {


			int pointIndex = index * 3;
			if (pointIndex > 0 && pointIndex < triangles.Length-2) {

				p1 = mesh.vertices [triangles [pointIndex]];
				p2 = mesh.vertices [triangles [pointIndex + 1]];
				p3 = mesh.vertices [triangles [pointIndex + 2]];

				if (isTriangleInCirle (rpos, r, p1, p2, p3)) {
					triangles [pointIndex] = 0;
					triangles [pointIndex + 1] = 0;
					triangles [pointIndex + 2] = 0;
				}
			}
		}

		mesh.triangles = triangles;

	
	}


	private bool isTriangleInCirle(Vector3 center,float r,Vector3 p1,Vector3 p2,Vector3 p3){




		float rr = r * r;
		int i = 0;

		if ((p1 - center).sqrMagnitude < rr) {
			i++;
		}
		if ((p2 - center).sqrMagnitude < rr) {
			i++;
		}
		if ((p3 - center).sqrMagnitude < rr) {
			i++;
		}

		if (i > 1)
			return true;
		return false;
	}

	void OnDrawGizmosSelected() {
		
		Gizmos.color = Color.black;
		var mesh = new Mesh ();

		int colNum = 95;
		int rowNum = 37;
		float size = 0.5f;
		int zIndex = -5;

		mesh.vertices = new Vector3[] {

			new Vector3 (0, 0, zIndex)+this.transform.position,
			new Vector3 (0, rowNum * size, zIndex)+this.transform.position,
			new Vector3 (colNum * size, 0, zIndex)+this.transform.position,
			new Vector3 (colNum * size, rowNum * size, zIndex)+this.transform.position
		};

		mesh.triangles = new int[]{ 0, 1, 2, 2, 1, 3 };
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();

		Gizmos.DrawMesh(mesh);
		Debug.Log ("ddd");


	}




}
