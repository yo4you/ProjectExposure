using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
class MultiObject
{
	[SerializeField]
	public List<GameObject> objects = new List<GameObject>();

	internal GameObject RandomObject()
	{
		return objects[UnityEngine.Random.Range(0,objects.Count)];
	}
}