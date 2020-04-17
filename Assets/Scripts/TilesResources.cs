using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum TilesType
{
    None,
    Water,
    Fire, 
}

public class TilesResources 
{
    public static Dictionary<TilesType, GameObject> tileObjects;

    public static void OpenTileFiles()
    {
        tileObjects = new Dictionary<TilesType, GameObject>();
        var values = Enum.GetValues(typeof(TilesType)).Cast<TilesType>();
        foreach (var tile in values){
            if(tile != TilesType.None)
            {
                GameObject myPrefab = Resources.Load(tile.ToString()) as GameObject;
                tileObjects.Add(tile, myPrefab);
            }
        }
    }
}