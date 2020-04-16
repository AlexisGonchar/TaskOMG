using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class PoolManager
public class PoolManager : MonoBehaviour {

	private static Dictionary<string, LinkedList<GameObject>> poolsDictionary;
	private static Transform deactivatedObjectsParent;

	public static void Init(Transform pooledObjectsParent)
	{
		deactivatedObjectsParent = pooledObjectsParent;
		poolsDictionary = new Dictionary<string, LinkedList<GameObject>>();
	}

	//Getting an object from a pool.
	public static GameObject GetGameObjectFromPool(GameObject prefab)
	{
		if (!poolsDictionary.ContainsKey(prefab.name))
		{
			poolsDictionary[prefab.name] = new LinkedList<GameObject>();
		}

		GameObject result;

		if(poolsDictionary[prefab.name].Count > 0)
		{
			result = poolsDictionary[prefab.name].First.Value;
			poolsDictionary[prefab.name].RemoveFirst();
			result.SetActive(true);
			return result;
		}

		result = GameObject.Instantiate(prefab);
		result.name = prefab.name;

		return result;
	}

	//Putting an object to a pool.
	public static void PutGameObjectToPool(GameObject target)
	{
		poolsDictionary[target.name].AddFirst(target);
		target.transform.parent = deactivatedObjectsParent;
		target.SetActive(false);
	}
}
