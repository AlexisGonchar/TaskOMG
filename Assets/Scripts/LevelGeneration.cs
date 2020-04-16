using UnityEngine;
public class LevelGeneration : MonoBehaviour
{
	//Позиция первого блока (нижнего левого) на сцене.
	public static Vector2 InitialPosition;
	//Вектор смещения блоков относительно Осей X и Y.
	public static Vector2 VectorBias;

	//Block generation on stage.
	public static Tile[,] GenerateLevel(int index)
	{
		int[,] lvl = Init(index);
		Tile[,] tiles = new Tile[lvl.GetLength(1), lvl.GetLength(0)];
		int layer = 100;
		GameObject obj;
		GameObject block;
		TilesType type = TilesType.None;
		Tile tile;

		for (int y = lvl.GetLength(0) - 1; y >= 0; y--)
		{
			for (int x = 0; x < lvl.GetLength(1); x++)
			{
				type = (TilesType)lvl[y, x];

				if (type != TilesType.None)
				{
					obj = TilesResources.tileObjects[type];
					//Creating a block on stage.
					block = Instantiate(obj, new Vector2(InitialPosition.x + x * VectorBias.x, InitialPosition.y + (lvl.GetLength(0) - 1 - y) * VectorBias.y), Quaternion.identity);
					block.name = "Block" + type.ToString() + (x + 1) + (lvl.GetLength(0) - y);
					tile = block.GetComponent<Tile>();
					tile.renderer = block.GetComponent<Renderer>();
					tile.animator = block.GetComponent<Animator>();
					tile.tileObject = tile;
					tile.type = type;
					tile.renderer.sortingOrder = layer;
					tile.animator.Play(block.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash, 0, Random.Range(0f, 1f));
					tiles[x, lvl.GetLength(0) - 1 - y] = block.GetComponent<Tile>();
				}
				layer++;
			}
		}
		return tiles;
	}

	private static int[,] Init(int index)
	{
		//Матрица уровня.
		int[,] lvl = LevelParser.LoadLevel(index);
		//Вектор смещения.
		VectorBias = TilesResources.tileObjects[(TilesType)1].GetComponent<BoxCollider2D>().size;
		//Граница экрана.
		Vector2 screenEdge = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		//Позиция первого блока.
		InitialPosition = new Vector2();
		//Центрирование по горизонтали.
		InitialPosition.x = -screenEdge.x + ((screenEdge.x * 2) - (VectorBias.x * lvl.GetLength(1))) / 2f + VectorBias.x / 2f;
		InitialPosition.y = -screenEdge.y + screenEdge.y * 0.45f;

		return lvl;
	}
}
