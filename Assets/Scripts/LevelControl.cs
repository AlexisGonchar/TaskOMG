using UnityEngine;

public class LevelControl : MonoBehaviour {

	//Игровые объекты блоков.
	public GameObject watter;
	public GameObject fire;
	//Позиция первого блока (нижнего левого) на сцене.
	public static Vector2 InitialPosition;
	//Вектор смещения блоков относительно Осей X и Y.
	public static Vector2 VectorBias;
	//Двумерная матрица обЪектов, где элемент[0,0] - левый нижний край. 
	public static Tile[,] Tiles;
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
		Init();
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

	//Initialization.
	private void Init()
	{
		PoolManager.Init(transform);
		BeginGlobalFallCheck = false;
		IsDestruction = false;
		IsCheckGroups = false;
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
			//If you win, load the level.
			if (CheckWin())
			{
				NextLvl.SetNextLvl();
			}
			GlobalFallCheck();
			BeginGlobalFallCheck = false;
		}
	}

	//Drop check after destruction of all blocks.
	void GlobalFallCheck()
	{
		bool[] checkArray = new bool[lenX];
		//An array in which data is stored, whether there are gaps in the columns.
		for (int i = 0; i < lenX; i++)
		{
			checkArray[i] = false;
		}
		for(int y = 0; y < lenY; y++)
		{
			for(int x = 0; x < lenX; x++)
			{
				if (Tiles[x, y] == null)
					checkArray[x] = true;
				//Only blocks with gaps in their columns are checked.
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

	//Checking for a win.
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

	//Destruction of blocks.
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

	//Construction of a matrix of coincidences.
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

					//Horizontally.
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
					//Vertically.
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

	//Finding matches in direction.
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

	//Generation of a matrix of the current field state.
	int[,] GenerateArrayTiles()
	{
		int[,] tiles = new int[lenX, lenY];
		for (int x = 0; x < lenX; x++)
		{
			for (int y = 0; y < lenY; y++)
			{
				if (Tiles[x, y] == null)
					tiles[x, y] = 0;
				else if (Tiles[x, y].type == TilesType.Water)
					tiles[x, y] = 1;
				else if (Tiles[x, y].type == TilesType.Fire)
					tiles[x, y] = 2;
			}
		}
		return tiles;
	}

	//Block generation on stage.
	void GenerateLevel(int[,] lvl)
	{
		Tiles = new Tile[lvl.GetLength(1), lvl.GetLength(0)];
		int layer = 100;
		GameObject obj;
		GameObject block;
		TilesType type = TilesType.None;
		Tile tile;
		string name = null;

		for (int y = lvl.GetLength(0) - 1; y >= 0; y--)
		{
			for (int x = 0; x < lvl.GetLength(1); x++)
			{
				obj = null;
				if (lvl[y, x] == 1)
				{
					obj = watter;
					type = TilesType.Water;
					name = "Watter";
				}
				else if (lvl[y, x] == 2)
				{
					obj = fire;
					type = TilesType.Fire;
					name = "Fire";
				}
				if (obj != null)
				{
					//Creating a block on stage.
					block = Instantiate(obj, new Vector2(InitialPosition.x + x * VectorBias.x, InitialPosition.y + (lvl.GetLength(0) - 1 - y) * VectorBias.y), Quaternion.identity);
					block.name = "Block" + name + (x + 1) + (lvl.GetLength(0) - y);
					tile = block.GetComponent<Tile>();
					tile.renderer = block.GetComponent<Renderer>();
					tile.animator = block.GetComponent<Animator>();
					tile.tileObject = tile;
					tile.type = type;
					tile.renderer.sortingOrder = layer;
					tile.animator.Play(block.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash, 0, Random.Range(0f, 1f));
					Tiles[x, lvl.GetLength(0) - 1 - y] = block.GetComponent<Tile>();
				}
				layer++;
			}
		}
	}
}
