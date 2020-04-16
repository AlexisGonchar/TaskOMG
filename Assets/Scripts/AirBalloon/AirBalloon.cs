using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for describing the behavior of a balloon on a stage.
public class AirBalloon : MonoBehaviour
{
	//The direction of the balloon.
	private int direction;
	//The speed of the balloon.
	private float speed;
	//The starting position of the balloon.
	private Vector3 pos;
	//Balloon Speed ​​Range
	public Vector2 speedRange;

	void Start()
	{
		Init();
	}

	void OnEnable()
	{
		Init();
	}

	//Initialization of object parameters.
	private void Init()
	{
		//Setting the direction of movement of the balloon.
		direction = transform.position.x < 0 ? 1 : -1;
		pos = transform.position;
		//Random speed generation.
		speed = Random.Range(speedRange.x, speedRange.y);
	}

	void Update()
	{
		float edge = AirBalloonGenerate.ScreenEdge.x;
		//Destruction of the balloon after passing the scene.
		if (pos.x > edge || pos.x < -edge)
		{
			PoolManager.PutGameObjectToPool(gameObject);
			AirBalloonGenerate.balloonCount--;
		}
		else
		{
			//Moving horizontally.
			pos += transform.right * Time.deltaTime * speed * direction;
			//Sine wave movement.
			transform.position = pos + transform.up * Mathf.Sin(Time.time * 2) * 0.5f;
		}
	}
}
