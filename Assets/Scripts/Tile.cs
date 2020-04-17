using UnityEngine;

//Game block class.
public class Tile : MonoBehaviour {

	[System.NonSerialized]
	public TilesType type;
	[System.NonSerialized]
	public Animator animator;
	[System.NonSerialized]
	public Transform transform;
	//The displacement vector of the blocks relative to the X and Y axes.
	private Vector2 VectorBias;
	// Static variable that indicates whether the blocks are currently moving.
	// (Block extra swipe)
	public static bool MovingTile;
	private bool mouseOn;
	//A value that indicates whether the object is moving.
	[System.NonSerialized]
	public bool IsMove;
	[System.NonSerialized]
	public bool IsFall;
	private Tile objNB;
	[System.NonSerialized]
	public Vector2 targetPos;
	private Vector2 targetPosNB;
	[System.NonSerialized]
	public Renderer renderer;

	void Start () {
		//Initialization of variables.
		VectorBias = GetComponent<BoxCollider2D>().size;
		transform = gameObject.transform;
		MovingTile = false;
		mouseOn = false;
		IsMove = false;
		IsFall = false;
		objNB = null;		
	}

	void Update () {
		if (IsMove || IsFall)
		{
			MoveTile();
		}
		//Swipe processing on the screen.
		if (mouseOn && SwipeControl.SwipeDirection != Direction.None && !MovingTile)
		{
			if (SwapTiles())
			{
				IsMove = true;
				MovingTile = true;
			}
		}
		mouseOn = false;
	}

	private void MoveTile()
	{
		if (objNB != null)
			objNB.transform.position = Vector3.MoveTowards(objNB.transform.position, targetPosNB, 0.2f);
		transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.2f);
		//Determination of the completion of the move.
		if (Vector3Compare(transform.position, targetPos, 0.01))
		{
			objNB = null;

			if (IsFall)
			{
				LevelControl.CountToFall--;
				if (LevelControl.CountToFall == 0)
					EventAggregator.Match.Publish();
				IsFall = false;
			}

			if (IsMove)
			{
				EventAggregator.Fall.Publish();
				IsMove = false;
			}
		}
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
				tiles[currentX, i + 1] = this;
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
					tiles[currentX - 1, currentY] = this;
					break;
				case Direction.Right:
					targetPos.x += VectorBias.x;
					layerSwap = 1;
					objNB = tiles[currentX + 1, currentY];
					tiles[currentX + 1, currentY] = this;
					break;
				case Direction.Up:
					targetPos.y += VectorBias.y;
					layerSwap = lenX;
					objNB = tiles[currentX, currentY + 1];
					tiles[currentX, currentY + 1] = this;
					break;
				case Direction.Down:
					targetPos.y -= VectorBias.y;
					layerSwap = -lenX;
					objNB = tiles[currentX, currentY - 1];
					tiles[currentX, currentY - 1] = this;
					break;
			}
			if(objNB != null)
			{
				targetPosNB = transform.position;
				objNB.renderer.sortingOrder -= layerSwap;
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

	void OnMouseUp()
	{
		mouseOn = true;
	}

	void Die()
	{
		LevelControl.CountToDestroy--;
		if (LevelControl.CountToDestroy == 0)
		{
			EventAggregator.Fall.Publish();
		}
		
		Destroy(gameObject);
	}
}
