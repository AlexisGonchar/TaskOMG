using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class for generating balloons on stage.
public class AirBalloonGenerate : MonoBehaviour
{
	//Game objects of balloons.
	public GameObject AirBalloon1;
	public GameObject AirBalloon2;
	//The number of balloons on the stage.
	public static int balloonCount;
	//The upper right edge of the screen.
	public static Vector2 ScreenEdge;
	//The maximum number of balloons on scene.
	public int BalloonsMaxCount;

	void Start()
	{
		//Calculation of the edge of the screen.
		ScreenEdge = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		ScreenEdge.x += AirBalloon1.GetComponent<BoxCollider2D>().size.x / 2f;
		ScreenEdge.y -= AirBalloon1.GetComponent<BoxCollider2D>().size.y;
		//Initialization.
		balloonCount = 0;
	}

	void Update()
	{
		//Generation of a new balloon, if there are less than three on the stage.
		if (balloonCount < BalloonsMaxCount)
		{
			GenerateAirBalloon();
			balloonCount++;
		}
	}

	//Balloon Generation Method.
	void GenerateAirBalloon()
	{
		GameObject obj;
		obj = Random.Range(0, 2) > 0 ? AirBalloon1 : AirBalloon2;
		float x = Random.Range(0, 2) > 0 ? ScreenEdge.x : -ScreenEdge.x;
		float y = Random.Range(ScreenEdge.y, (-ScreenEdge.y + ScreenEdge.y * 0.5f));
		obj = PoolManager.GetGameObjectFromPool(obj);
		obj.transform.position = new Vector2(x, y);
	}
}
