using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A class for reading and parsing levels from files.
public class LevelParser : MonoBehaviour
{
	//A method of reading and parsing levels from files.
	public static int[,] LoadLevel(int index)
	{
		string content = ReadFile(index);
		return Parser(content);
	}

	//Method for reading a file from game resources.
	private static string ReadFile(int index)
	{
		TextAsset txt = (TextAsset)Resources.Load("lvl_" + index, typeof(TextAsset));
		return txt.text;
	}

	//Level string parsing.
	//Return in the form of a two-dimensional matrix, where the element [0,0] is the upper left edge.
	private static int[,] Parser(string content)
	{
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
