using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Game block class.
public class Tile : MonoBehaviour {

	//Статическая переменная, которая указывает смещаются ли данный момент блоки.
	//(Блокировка лишних свайпов)
	public static bool MovingTile;
	//Вектор смещения блоков относительно Осей X и Y.
	private Vector2 VectorBias;
	//Нажатие на объект.
	private bool mouseOn;
	//Значение, которое указывает идёт ли процесс перемещения объекта.
	[System.NonSerialized]
	public bool IsMove;
	//Соседний блок с текущим, с которым будет происходить смена позициями.
	private GameObject objNB;
	//Позиция, на которую текущий объект должен переместиться. 
	[System.NonSerialized]
	public Vector2 targetPos;
	//Позиция, на которую блок-сосед должен переместиться.
	private Vector2 targetPosNB;
	//Компонент Renderer текущего объекта.
	private Renderer renderer;

	void Start () {
		//Инициализация переменных.
		VectorBias = GetComponent<BoxCollider2D>().size;
		renderer = GetComponent<Renderer>();
		MovingTile = false;
		mouseOn = false;
		IsMove = false;
		objNB = null;		
	}
	
	void Update () {
		if (IsMove)
		{
			if (objNB != null)
				objNB.transform.position = Vector3.MoveTowards(objNB.transform.position, targetPosNB, 0.2f);
			transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.2f);
			//Определение завершения перемещения.
			if (Vector3Compare(transform.position, targetPos, 0.01))
			{
				objNB = null;
				IsMove = FallCheck();
				MovingTile = IsMove;
				LevelControl.IsCheckGroups = !MovingTile;
				LevelControl.BeginGlobalFallCheck = true;
			}
			else
			{
				MovingTile = true;
			}
		}
		//Обработка свайпа по экрану.
		if (mouseOn && SwipeControl.SwipeDirection != Direction.None && !MovingTile && !LevelControl.IsDestruction)
		{
			if (SwapTiles())
			{
				IsMove = true;
				MovingTile = true;
			}
		}
		//Онулирование данных.
		mouseOn = false;
	}

	//Проверка на возможность падения.
	public bool FallCheck()
	{
		Vector2 currentPos;
		int currentX, currentY;
		GameObject[,] tiles = LevelControl.Tiles;
		currentPos = GetCurrentPosition(tiles);
		currentX = (int)currentPos.x;
		currentY = (int)currentPos.y;
		int lenX = tiles.GetLength(0);
		int lenY = tiles.GetLength(1);

		if (currentY != 0)
		{
			int i = currentY - 1;
			//Идёт вниз, пока не встретит землю, либо другой блок.
			while (i >= 0 && tiles[currentX, i] == null)
			{
				i--;
			}
			if (i != currentY - 1)
			{
				tiles[currentX, currentY] = null;
				tiles[currentX, i + 1] = gameObject;
				targetPos = new Vector3(transform.position.x, transform.position.y - (currentY - (i + 1)) * VectorBias.y);
				renderer.sortingOrder -= (currentY - (i + 1)) * tiles.GetLength(0);
				return true;
			}
		}
		return false;
	}

	//Сравнение векторов с заданной погрешностью.
	bool Vector3Compare(Vector3 v1, Vector3 v2, double tolerance)
	{
		if (Mathf.Abs(transform.position.x - targetPos.x) < tolerance &&
			Mathf.Abs(transform.position.y - targetPos.y) < tolerance)
		{
			return true;
		}
		return false;
	}

	//Определение возможности смещение, просчёт целевых позиций сдвига.
	//Возвращает true, если движение возможно, false, если движение невозможно.
	bool SwapTiles()
	{
		Vector2 currentPos;
		int currentX, currentY;
		GameObject[,] tiles = LevelControl.Tiles;
		currentPos = GetCurrentPosition(tiles);
		currentX = (int)currentPos.x;
		currentY = (int)currentPos.y;
		int lenX = tiles.GetLength(0);
		int lenY = tiles.GetLength(1);
		int layerSwap = 0;
		if(CheckBorder(tiles, currentX, currentY))
		{
			targetPos = transform.position;
			switch (SwipeControl.SwipeDirection)
			{
				case Direction.Left:
					targetPos.x -= VectorBias.x;
					layerSwap = -1;
					objNB = tiles[currentX - 1, currentY];
					//Перемещение объекта в матрице.
					tiles[currentX - 1, currentY] = gameObject;
					break;
				case Direction.Right:
					targetPos.x += VectorBias.x;
					layerSwap = 1;
					objNB = tiles[currentX + 1, currentY];
					tiles[currentX + 1, currentY] = gameObject;
					break;
				case Direction.Up:
					targetPos.y += VectorBias.y;
					layerSwap = lenX;
					objNB = tiles[currentX, currentY + 1];
					tiles[currentX, currentY + 1] = gameObject;
					break;
				case Direction.Down:
					targetPos.y -= VectorBias.y;
					layerSwap = -lenX;
					objNB = tiles[currentX, currentY - 1];
					tiles[currentX, currentY - 1] = gameObject;
					break;
			}
			if(objNB != null)
			{
				targetPosNB = transform.position;
				objNB.GetComponent<Renderer>().sortingOrder -= layerSwap;
			}
			tiles[currentX, currentY] = objNB;
			renderer.sortingOrder += layerSwap;
			return true;
		}
		return false;
	}

	//Получение текущей позиции объекта на игровом поле.
	Vector2 GetCurrentPosition(GameObject[,] tiles)
	{
		Vector2 currentPos = new Vector2();
		int lenX = tiles.GetLength(0);
		int lenY = tiles.GetLength(1);
		for (int x = 0; x < lenX; x++)
		{
			for (int y = 0; y < lenY; y++)
			{
				if (tiles[x, y] != null)
				{
					if (name.Equals(tiles[x, y].name))
					{
						currentPos = new Vector2(x, y);
					}
				}
			}
		}
		return currentPos;
	}

	//Определение границ экрана (определение невозможности движения в данном направлении).
	//Возвращает true, если двежение возможно, false, если движение невозможно.
	bool CheckBorder(GameObject[,] tiles, int currentX, int currentY)
	{
		int lenX = tiles.GetLength(0);
		int lenY = tiles.GetLength(1);
		//Данное направление.
		Direction curDir = SwipeControl.SwipeDirection;
		//Проверка границ экрана.
		if ((curDir == Direction.Left && currentX == 0) ||
			(curDir == Direction.Right && currentX == lenX - 1) ||
			(curDir == Direction.Up && currentY == lenY - 1) ||
			(curDir == Direction.Up && tiles[currentX, currentY + 1] == null) ||
			(curDir == Direction.Down && currentY == 0))
		{
			return false;
		}
		return true;
	}

	//Триггер, который срабатывает после свайпа на определенном блоке.
	void OnMouseUp()
	{
		mouseOn = true;
	}

	//Триггер, который срабатывает после завершения анимации destroy.
	void Die()
	{
		LevelControl.CountForDestroy--;
		if (LevelControl.CountForDestroy == 0)
		{
			LevelControl.IsDestruction = false;
			LevelControl.BeginGlobalFallCheck = true;
		}
		Vector2 curPos = GetCurrentPosition(LevelControl.Tiles);
		LevelControl.Tiles[(int)curPos.x, (int)curPos.y] = null;
		Destroy(gameObject);
	}
}
