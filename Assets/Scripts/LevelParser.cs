using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Класс для чтения и парсинга уровней из файлов.
public class LevelParser : MonoBehaviour
{
	//Метод чтения и парсинга уровней из файлов.
	//Возврат в виде двумерной матрицы, где элемент[0,0] - левый верхний край.
	public static int[,] LoadLevel(int index)
	{
		//Чтение файла.
		TextAsset txt = (TextAsset)Resources.Load("lvl_" + index, typeof(TextAsset));
		string content = txt.text;
		//Парсинг.
		string[] lines = content.Split('\n');
		int[,] lvl = new int[lines.Length, lines[0].Split(' ').Length];
		for (int i = 0; i < lines.Length; i++)
		{
			string[] Temp = lines[i].Split(' ');
			for (int j = 0; j < Temp.Length; j++)
			{
				Temp[j] = Temp[j].Trim(' ');
				lvl[i, j] = int.Parse(Temp[j]);
			}
		}
		return lvl;
	}
}
