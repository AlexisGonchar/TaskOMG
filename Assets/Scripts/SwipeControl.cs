using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Направления.
public enum Direction
{
	Up,
	Down,
	Right,
	Left,
	None
}

//Класс для определения направления свайпа по экрану.
public class SwipeControl : MonoBehaviour {

	//Направление свайпа.
	public static Direction SwipeDirection;

	Vector2 startPos;

	void FixedUpdate()
	{
		SwipeDirection = Swipe();
		SwipeDirection = SwipeMouse();
		if(SwipeDirection != Direction.None)
		{
			print(SwipeDirection.ToString());
		}
	}

	//Свайп тачем.
	Direction Swipe()
	{
		if (Input.touches.Length > 0)
		{
			Touch t = Input.GetTouch(0);
			if (t.phase == TouchPhase.Began)
			{
				startPos = new Vector2(t.position.x / (float)Screen.width, t.position.y / (float)Screen.width);
			}
			if (t.phase == TouchPhase.Ended)
			{
				return CalculateDirection(t.position);
			}
		}
		return Direction.None;
	}

	//Свайп мышью.
	Direction SwipeMouse()
	{
		if (Input.GetMouseButtonDown(0))
		{
			startPos = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.width);
		}
		if (Input.GetMouseButtonUp(0))
		{
			return CalculateDirection(Input.mousePosition);
		}
		return Direction.None;
	}

	//Определение направления свайпа.
	Direction CalculateDirection(Vector2 touchPos)
	{
		Vector2 endPos = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.width);

		Vector2 swipe = new Vector2(endPos.x - startPos.x, endPos.y - startPos.y);

		if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
		{
			//Горизонтальный свайп.
			if (swipe.x > 0)
			{
				return Direction.Right;
			}
			else
			{
				return Direction.Left;
			}
		}
		else
		{
			//Вертикальный свайп.
			if (swipe.y > 0)
			{
				return Direction.Up;
			}
			else
			{
				return Direction.Down;
			}
		}
	}
}
