﻿using UnityEngine;

public class LevelControl : MonoBehaviour {

	//Игровые объекты блоков.
	public GameObject watter;
	public GameObject fire;
	//Позиция первого блока (нижнего левого) на сцене.
	public static Vector2 InitialPosition;
	//Вектор смещения блоков относительно Осей X и Y.
	public static Vector2 VectorBias;
	//Двумерная матрица обЪектов, где элемент[0,0] - левый нижний край. 
	public static GameObject[,] Tiles;
	//Индекс текущего уровня.
	public static int LvlIndex = 1;
	//Начало проверки совпадений.
	public static bool IsCheckGroups;
	//Во время уничтожения блоков находится в состоянии true.
	public static bool IsDestruction;
	//После уничтожения блоков находится в состоянии true.
	public static bool BeginGlobalFallCheck;
	//Количество блоков, которые нужно уничтожить.
	public static int CountForDestroy;
	//Длина матрицы в двух измерениях.
	private int lenX, lenY;

	void Awake () {
		//Инициалиазация.
		PoolManager.Init(transform);
		BeginGlobalFallCheck = false;
		IsDestruction = false;
		IsCheckGroups = false;
		//Матрица уровня.
		int[,] lvl = LevelParser.LoadLevel(LvlIndex);
		//Вектор смещения.
		VectorBias = watter.GetComponent<BoxCollider2D>().size;
		//Граница экрана.
		Vector2 screenEdge = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		//Позиция первого блока.
		InitialPosition = new Vector2();
		//Центрирование по горизонтали.
		InitialPosition.x = -screenEdge.x + ((screenEdge.x * 2) - (VectorBias.x * lvl.GetLength(1)))/2f + VectorBias.x/2f;
		InitialPosition.y = -screenEdge.y + screenEdge.y*0.45f;
		//Генерация блоков.
		GenerateLevel(lvl);
		//Задание длины матрицы.
		lenX = Tiles.GetLength(0);
		lenY = Tiles.GetLength(1);
	}

	void LateUpdate()
	{
		if(IsCheckGroups && !Tile.MovingTile)
		{
			print("Match!");
			int[,] tiles = CheckGroups();
			CountForDestroy = DestroyTiles(tiles);
			if (CountForDestroy > 0)
			{
				IsDestruction = true;
			}
			IsCheckGroups = false;
		}
		if (BeginGlobalFallCheck)
		{
			//При победе загрузка уровня.
			if (CheckWin())
			{
				NextLvl.SetNextLvl();
			}
			GlobalFallCheck();
			BeginGlobalFallCheck = false;
		}
	}

	//Проверка на падение после уничтожения всех блоков.
	void GlobalFallCheck()
	{
		bool[] checkArray = new bool[lenX];
		//Массив, в котором хранятся данные, есть ли в столбцах пропуски.
		for(int i = 0; i < lenX; i++)
		{
			checkArray[i] = false;
		}
		for(int y = 0; y < lenY; y++)
		{
			for(int x = 0; x < lenX; x++)
			{
				if (Tiles[x, y] == null)
					checkArray[x] = true;
				//Проверяются только те блоки, в столбцах которых есть пропуски.
				if (Tiles[x, y] != null && checkArray[x])
				{
					Tile tile = Tiles[x, y].GetComponent<Tile>();
					if (tile.FallCheck())
					{
						tile.IsMove = true;
						Tile.MovingTile = true;
					}
				}
			}
		}
	}

	//Провекрка на выигрыш.
	bool CheckWin()
	{
		for (int y = 0; y < lenY; y++)
		{
			for (int x = 0; x < lenX; x++)
			{
				if (Tiles[x, y] != null)
					return false;
			}
		}
		return true;
	}

	//Уничтожение блоков.
	int DestroyTiles(int[,] tiles)
	{
		int count = 0;
		for (int y = 0; y < lenY; y++)
		{
			for (int x = 0; x < lenX; x++)
			{
				if (tiles[x, y] < 0)
				{
					Tiles[x, y].GetComponent<Animator>().SetBool("Destroy", true);
					count++;
				}
			}
		}
		return count;
	}

	//Построение матрицы совпадений, все совпадения < 0, то есть отмечены противоположными знаками.
	int[,] CheckGroups()
	{
		int[,] tiles = GenerateArrayTiles();
		for (int x = 0; x < lenX; x++)
		{
			for (int y = 0; y < lenY; y++)
			{
				if (tiles[x, y] > 0)
				{
					int left = FindMatches(tiles, x, y, Direction.Left);
					int right = FindMatches(tiles, x, y, Direction.Right);
					int up = FindMatches(tiles, x, y, Direction.Up);
					int down = FindMatches(tiles, x, y, Direction.Down);

					//По горизонтали.
					if ((left + right) > 1)
					{
						while (left > -1)
						{
							if (tiles[x - left, y] > 0)
							{
								tiles[x - left, y] = -tiles[x - left, y];
							}
							left--;
						}
						while (right > -1)
						{
							if (tiles[x + right, y] > 0)
							{
								tiles[x + right, y] = -tiles[x + right, y];
							}
							right--;
						}

					}
					//По вертикали.
					if ((up + down) > 1)
					{
						while (down > -1)
						{
							if (tiles[x, y - down] > 0)
							{
								tiles[x, y - down] = -tiles[x, y - down];
							}
							down--;
						}
						while (up > -1)
						{
							if (tiles[x, y + up] > 0)
							{
								tiles[x, y + up] = -tiles[x, y + up];
							}
							up--;
						}
					}
				}
			}
		}
		return tiles;
	}

	//Нахождение совпадений по направлению.
	int FindMatches(int[,] tiles, int x, int y, Direction dir)
	{
		int count = 0;
		int num = Mathf.Abs(tiles[x, y]);
		Vector2 vec = new Vector2(0, 0);
		switch (dir)
		{
			case Direction.Left:
				if (x > 0)
					vec.x -= 1;
				break;
			case Direction.Right:
				if (x < tiles.GetLength(0) - 1)
					vec.x += 1;
				break;
			case Direction.Up:
				if (y < tiles.GetLength(1) - 1)
					vec.y += 1;
				break;
			case Direction.Down:
				if (y > 0)
					vec.y -= 1;
				break;
		}
		if ((int)vec.x != 0 || (int)vec.y != 0)
		{
			if (Mathf.Abs(tiles[x + (int)vec.x, y + (int)vec.y]) == num)
			{
				count++;
				count += FindMatches(tiles, x + (int)vec.x, y + (int)vec.y, dir);
			}
		}
		return count;
	}

	//Генерация матрицы текущего состояния поля.
	int[,] GenerateArrayTiles()
	{
		int[,] tiles = new int[lenX, lenY];
		for (int x = 0; x < lenX; x++)
		{
			for (int y = 0; y < lenY; y++)
			{
				if (Tiles[x, y] == null)
					tiles[x, y] = 0;
				else if (Tiles[x, y].name.Contains("Watter"))
					tiles[x, y] = 1;
				else if (Tiles[x, y].name.Contains("Fire"))
					tiles[x, y] = 2;
			}
		}
		return tiles;
	}

	//Генерация блоков на сцене.
	void GenerateLevel(int[,] lvl)
	{
		Tiles = new GameObject[lvl.GetLength(1), lvl.GetLength(0)];
		//Задание начального уровня слоя.
		int layer = 100;
		GameObject obj;
		GameObject block;
		string name = null;
		//Прогон по массиву уровня.
		for (int y = lvl.GetLength(0) - 1; y >= 0; y--)
		{
			for (int x = 0; x < lvl.GetLength(1); x++)
			{
				obj = null;
				//1 - водный блок.
				if (lvl[y, x] == 1)
				{
					obj = watter;
					name = "Watter";
				}
				//2 - огненный блок.
				else if (lvl[y, x] == 2)
				{
					obj = fire;
					name = "Fire";
				}
				if (obj != null)
				{
					//Создание блока на сцене.
					block = Instantiate(obj, new Vector2(InitialPosition.x + x * VectorBias.x, InitialPosition.y + (lvl.GetLength(0) - 1 - y) * VectorBias.y), Quaternion.identity);
					//Замена уровня слоя.
					block.GetComponent<Renderer>().sortingOrder = layer;
					//Начало анимации со случайного кадра.
					block.GetComponent<Animator>().Play(block.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash, 0, Random.Range(0f, 1f));
					block.name = "Block" + name + (x + 1) + (lvl.GetLength(0) - y);
					Tiles[x, lvl.GetLength(0) - 1 - y] = block;
				}
				layer++;
			}
		}
	}
}
