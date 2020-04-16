using UnityEngine;

//Game block class.
public class Tile : MonoBehaviour {

	[System.NonSerialized]
	public TilesType type;
	[System.NonSerialized]
	public Animator animator;
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
	private Tile objNB;
	//Позиция, на которую текущий объект должен переместиться. 
	[System.NonSerialized]
	public Vector2 targetPos;
	//Позиция, на которую блок-сосед должен переместиться.
	private Vector2 targetPosNB;
	//Компонент Renderer текущего объекта.
	[System.NonSerialized]
	public Renderer renderer;
	[System.NonSerialized]
	public Tile tileObject;

	void Start () {
		//Инициализация переменных.
		//tile = GetComponent<Tile>();
		//animator = GetComponent<Animator>();
		VectorBias = GetComponent<BoxCollider2D>().size;
		//renderer = GetComponent<Renderer>();
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

	//Check for the possibility of a fall.
	public bool FallCheck()
	{
		Vector2 currentPos;
		int currentX, currentY;
		Tile[,] tiles = LevelControl.Tiles;
		currentPos = GetCurrentPosition(tiles);
		currentX = (int)currentPos.x;
		currentY = (int)currentPos.y;
		int lenX = tiles.GetLength(0);
		int lenY = tiles.GetLength(1);

		if (currentY != 0)
		{
			int i = currentY - 1;
			while (i >= 0 && tiles[currentX, i] == null)
			{
				i--;
			}
			if (i != currentY - 1)
			{
				tiles[currentX, currentY] = null;
				tiles[currentX, i + 1] = tileObject;
				targetPos = new Vector3(transform.position.x, transform.position.y - (currentY - (i + 1)) * VectorBias.y);
				renderer.sortingOrder -= (currentY - (i + 1)) * tiles.GetLength(0);
				return true;
			}
		}
		return false;
	}

	//Comparison of vectors with a given error.
	bool Vector3Compare(Vector3 v1, Vector3 v2, double tolerance)
	{
		if (Mathf.Abs(transform.position.x - targetPos.x) < tolerance &&
			Mathf.Abs(transform.position.y - targetPos.y) < tolerance)
		{
			return true;
		}
		return false;
	}

	// Determining the possibility of displacement, miscalculation of the target position of the shift.
	// Returns true if motion is possible, false if motion is not possible.
	bool SwapTiles()
	{
		Vector2 currentPos;
		int currentX, currentY;
		Tile[,] tiles = LevelControl.Tiles;
		currentPos = GetCurrentPosition(tiles);
		currentX = (int)currentPos.x;
		currentY = (int)currentPos.y;
		int lenX = tiles.GetLength(0);
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
					tiles[currentX - 1, currentY] = tileObject;
					break;
				case Direction.Right:
					targetPos.x += VectorBias.x;
					layerSwap = 1;
					objNB = tiles[currentX + 1, currentY];
					tiles[currentX + 1, currentY] = tileObject;
					break;
				case Direction.Up:
					targetPos.y += VectorBias.y;
					layerSwap = lenX;
					objNB = tiles[currentX, currentY + 1];
					tiles[currentX, currentY + 1] = tileObject;
					break;
				case Direction.Down:
					targetPos.y -= VectorBias.y;
					layerSwap = -lenX;
					objNB = tiles[currentX, currentY - 1];
					tiles[currentX, currentY - 1] = tileObject;
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

	//Getting the current position of the object on the playing field.
	Vector2 GetCurrentPosition(Tile[,] tiles)
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

	//Determination of the boundaries of the screen.
	//Returns true if movement is possible, false if movement is not possible.
	bool CheckBorder(Tile[,] tiles, int currentX, int currentY)
	{
		int lenX = tiles.GetLength(0);
		int lenY = tiles.GetLength(1);
		Direction curDir = SwipeControl.SwipeDirection;
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
