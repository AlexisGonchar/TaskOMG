using System.Collections;
using System.Collections.Generic;
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
	public static GameObject[,] Tiles;
	//Индекс текущего уровня.
	public static int LvlIndex = 2;

	void Awake () {
		//Матрица уровня.
		int[,] lvl = LevelParser.LoadLevel(LvlIndex);
		VectorBias = watter.GetComponent<BoxCollider2D>().size;
		Vector2 screenEdge = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
		print(screenEdge);
		InitialPosition = new Vector2();
		InitialPosition.x = -screenEdge.x + ((screenEdge.x * 2) - (VectorBias.x * lvl.GetLength(1)))/2f + VectorBias.x/2f;
		print(InitialPosition.x);
		InitialPosition.y = -screenEdge.y + screenEdge.y*0.4f;
		GenerateLevel(lvl);
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
					block.GetComponent<Renderer>().sortingOrder = layer;
					//block.GetComponent<Animator>().Play(block.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).fullPathHash, 0, Random.Range(0f, 1f));
					block.name = "Block" + name + (x + 1) + (lvl.GetLength(0) - y);
					Tiles[x, lvl.GetLength(0) - 1 - y] = block;
				}
				layer++;
			}
		}
	}
}
