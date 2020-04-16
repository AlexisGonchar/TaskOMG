using UnityEngine;

public class TilesMatch
{
	private static int lenX, lenY;
	public static int[,] Check(Tile[,] Tiles)
    {
		lenX = Tiles.GetLength(0);
		lenY = Tiles.GetLength(1);

		int[,] tiles = GenerateArrayTiles(Tiles);
		return CheckGroups(tiles);
	}

	//Construction of a matrix of coincidences.
	private static int[,] CheckGroups(int[,] tiles)
	{
		
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

	//Generation of a matrix of the current field state.
	private static int[,] GenerateArrayTiles(Tile[,] Tiles)
	{
		int[,] tiles = new int[lenX, lenY];
		for (int x = 0; x < lenX; x++)
		{
			for (int y = 0; y < lenY; y++)
			{
				if (Tiles[x, y] == null)
					tiles[x, y] = 0;
				else
					tiles[x, y] = (int)Tiles[x, y].type;
			}
		}
		return tiles;
	}

	//Finding matches in direction.
	private static int FindMatches(int[,] tiles, int x, int y, Direction dir)
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
}
