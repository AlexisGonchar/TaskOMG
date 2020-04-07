using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс для смены уровней.
public class NextLvl : MonoBehaviour
{
	//Количество уровней.
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
