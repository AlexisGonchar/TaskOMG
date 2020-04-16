using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for changing levels.
public class NextLvl : MonoBehaviour
{
	//The number of levels.
	public static readonly int COUNT_OF_LVL = 3;

	void OnMouseUp()
	{
		SetNextLvl();
	}

	public static void SetNextLvl()
	{
		if (LevelControl.LvlIndex < COUNT_OF_LVL)
			LevelControl.LvlIndex++;
		else
			LevelControl.LvlIndex = 1;
		Application.LoadLevel("SwipeElements");
	}
}
