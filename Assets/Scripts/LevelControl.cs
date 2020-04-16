using UnityEngine;

public class LevelControl : MonoBehaviour {

	
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
	private static int lenX, lenY;

	void Awake () {
		Init();
		TilesResources.OpenTileFiles();
		Tiles = LevelGeneration.GenerateLevel(LvlIndex);
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
			int[,] tiles = TilesMatch.Check(Tiles);
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
}
