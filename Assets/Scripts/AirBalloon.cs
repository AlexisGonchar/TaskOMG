using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс для описания поведения воздушного шара на сцене.
public class AirBalloon : MonoBehaviour
{
	//Направление движения воздушного шара.
	private int direction;
	//Скорость движения воздушного шара.
	private float speed;
	//Начальная позиция воздушного шара.
	private Vector3 pos;

	void Start()
	{
		//Задание направления движение воздушного шара.
		direction = transform.position.x < 0 ? 1 : -1;
		pos = transform.position;
		//Генерация случайной скорости.
		speed = Random.Range(0.5f, 1.5f);
	}

	void Update()
	{
		float edge = AirBalloonGenerate.screenEdge.x;
		//Уничтожение воздушного шара после прохождения сцены.
		if (pos.x > edge || pos.x < -edge)
		{
			Destroy(this.gameObject);
			AirBalloonGenerate.balloonCount--;
		}
		else
		{
			//Перемещение по горизонтали.
			pos += transform.right * Time.deltaTime * speed * direction;
			//Движение по синусоиде.
			transform.position = pos + transform.up * Mathf.Sin(Time.time * 2) * 0.5f;
		}
	}
}
