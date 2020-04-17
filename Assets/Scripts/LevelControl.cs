using UnityEngine;

//Class for level control
public class LevelControl : MonoBehaviour {


	//The two-dimensional matrix of objects, where the element [0,0] is the lower left edge.
	public static Tile[,] Tiles;
	//Index of the current level.
	public static int LvlIndex = 1;
	//The number of blocks to be destroyed.
	public static int CountToDestroy;
	//The number of blocks that should fall.
	public static int CountToFall;
	private static int lenX, lenY;

	void Awake () {
		Init();
		EventAggregator.Fall.Subscribe(OnFallEvent);
		EventAggregator.Match.Subscribe(OnMatchEvent);
		TilesResources.OpenTileFiles();
		Tiles = LevelGeneration.GenerateLevel(LvlIndex);
		lenX = Tiles.GetLength(0);
		lenY = Tiles.GetLength(1);
	}

	//Initialization.
	private void Init()
	{
		EventAggregator.Fall = new FallEvent();
		EventAggregator.Match = new MatchEvent();
		PoolManager.Init(transform);
		CountToFall = 0;
	}

	private void OnFallEvent()
	{
		//If you win, load next level.
		if (CheckWin())
		{
			NextLvl.SetNextLvl();
		}
		CountToFall = GlobalFallCheck();
		if (CountToFall == 0)
		{
			EventAggregator.Match.Publish();
			Tile.MovingTile = false;
		}
	}

	private void OnMatchEvent()
	{
		int[,] tiles = TilesMatch.Check(Tiles);
		CountToDestroy = DestroyTiles(tiles);
		if(CountToDestroy == 0)
		{
			Tile.MovingTile = false;
		}
	}

	//Drop check after destruction of all blocks.
	int GlobalFallCheck()
	{
		int count = 0;
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
					Tile tile = Tiles[x, y];
					if (tile.FallCheck())
					{
						tile.IsFall = true;
						count++;
					}
				}
			}
		}
		return count;
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
					Tiles[x, y].animator.SetBool("Destroy", true);
					Tiles[x, y] = null;
					count++;
				}
			}
		}
		return count;
	}

	public static bool CheckMove()
	{
		foreach(Tile tile in Tiles)
		{
			if (tile != null && tile.IsMove)
				return true;
		}
		return false;
	}
}
