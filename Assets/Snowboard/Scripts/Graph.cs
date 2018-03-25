using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {
	public int resolution;
	public Transform pointPosition;

	public GrabData.GrabData front;
	public	GrabData.GrabData back;

	public float lag;
	//for gyro 120, 2000
	public int xMulti = 100;
	public int yDiv = 4000;

	public LineRenderer linefront;
	public LineRenderer lineback;



	// Use this for initialization
	void Start () {
		//get the data
		front.LoadData ();
		front.CreateDataSet ();

		back.LoadData ();
		back.CreateDataSet ();


		//make sure we use the smaller one so that both have same num of points
		resolution = Mathf.Min(front.myData.Count,back.myData.Count);
		//positioning on X based on num of points
		float factor = (2f / resolution) *xMulti;
		//set the point number
		linefront.positionCount = resolution;
		lineback.positionCount = resolution;
		//position points
		for (int i = 0; i < resolution; i++) {
			//GRYO y
			linefront.SetPosition (i, new Vector3 ((i + 0.5f) * factor - 1f, front.myData [i].gyroC.y/yDiv, 0));
			lineback.SetPosition (i, new Vector3 (((i + 0.5f) * factor - 1f)-lag, back.myData [i].gyroC.y/yDiv, 0));

			//linefront.SetPosition (i, new Vector3 ((i + 0.5f) * factor - 1f, front.myData [i].accelC.x/yDiv, 0));
			//linefront2.SetPosition (i, new Vector3 ((i + 0.5f) * factor - 1f, front.myData [i].accelC.y/(yDiv), 0));
			//linefront3.SetPosition (i, new Vector3 ((i + 0.5f) * factor - 1f, front.myData [i].accelC.z/yDiv - 100, 0));


			//lineback.SetPosition (i, new Vector3 (((i + 0.5f) * factor - 1f)-lag, back.myData [i].accelC.x/yDiv, 0));

		}
		//print ("RESOLUTION" + resolution);
		//back.LoadData();
		//back.CreateDataSet();
		/*
		//cube style
		float factor = (2f / resolution) * 50;
		Vector3 scale = Vector3.one * factor;
		Vector3 position;
		//		position.y = 0f;
		position.z = 0f;
		for (int i = 0; i < resolution; i++) {
			Transform point = Instantiate (pointPosition);
			position.x = (i + 0.5f) *factor - 1f;
			position.y = front.myData[i].gyroC.y/5000;
			//position.y = position.x;
			point.localPosition = position;
			point.localScale = scale;
			point.SetParent(this.transform);
		}

		*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
