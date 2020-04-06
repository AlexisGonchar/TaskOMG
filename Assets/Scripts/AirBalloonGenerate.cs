using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс для генерации воздушных шаров на сцене.
public class AirBalloonGenerate : MonoBehaviour
{
	//Игровые объекты воздушных шаров.
	public GameObject AirBalloon1;
	public GameObject AirBalloon2;
	//Количество воздушных шаров на сцене.
	public static int balloonCount;
	//Правый верхний край экрана.
	public static Vector2 ScreenEdge;

	void Start()
	{
		//Вычисление края экрана.
		ScreenEdge = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		ScreenEdge.x += AirBalloon1.GetComponent<BoxCollider2D>().size.x / 2f;
		ScreenEdge.y -= AirBalloon1.GetComponent<BoxCollider2D>().size.y;
		//Инициализация.
		balloonCount = 0;
	}

	void Update()
	{
		//Генерация нового воздушного шара, если таковых меньше трёх на сцене.
		if (balloonCount < 3)
		{
			GenerateAirBalloon();
			balloonCount++;
		}
	}

	//Метод генерации воздушных шаров.
	void GenerateAirBalloon()
	{
		GameObject obj;
		//Определение вида воздушного гара случайным образом.
		obj = Random.Range(0, 2) > 0 ? AirBalloon1 : AirBalloon2;
		//Определение стороны генерации воздушного шара на сцене случайным образом.
		float x = Random.Range(0, 2) > 0 ? ScreenEdge.x : -ScreenEdge.x;
		//Определение высоты генерации воздушного шара случайным образом.
		float y = Random.Range(ScreenEdge.y , (-ScreenEdge.y + ScreenEdge.y * 0.5f));
		//Создание объекта.
		Instantiate(obj, new Vector2(x, y), Quaternion.identity);
	}
}
