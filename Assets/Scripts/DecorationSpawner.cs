using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DecorationSpawner : MonoBehaviour
{

	[SerializeField]
	List<GameObject> _floorObjects;

	[SerializeField]
	List<GameObject> _backObjects;

	[SerializeField]
	List<GameObject> _ceilingObjects;
	[SerializeField]
	LayerMask _mask;

	[SerializeField]
	float _chanceToSpawn;

	void Start()
    {
		FindObjectOfType<LevelMaterialFixer>().OnFinishMatFix += DecorationSpawner_OnGenerationComplete;
    }

	private void DecorationSpawner_OnGenerationComplete()
	{


		foreach (var poly in FindObjectOfType<VoronoiGenerator>().Polygons)
		{
			if (!poly.IsWall || UnityEngine.Random.Range(0, 100) > _chanceToSpawn)
				continue;
			var spawnPool = new List<List<GameObject>>() { _backObjects };
			if (IsCeil(poly))
			{
				spawnPool.Add(_ceilingObjects);
			}
			if (IsFloor(poly))
			{
				spawnPool.Add(_floorObjects);
			}

			var pool = spawnPool[UnityEngine.Random.Range(0, spawnPool.Count)];
			var prefab = pool[UnityEngine.Random.Range(0, pool.Count)];
			var clone = Instantiate(prefab);
			PlaceOn(clone, poly);

		}
	}

	private void PlaceOn(GameObject clone, Polygon poly)
	{
		Vector3 pos = poly.Centre;
		pos.z = -70;
		clone.layer = 12;
		Physics.Raycast(pos, Vector3.forward*1000f,out RaycastHit hit, _mask);
		pos.z = hit.point.z;
		clone.transform.Rotate(-90, 0, 0);
		clone.transform.position = pos;
	}

	private bool IsFloor(Polygon poly)
	{
		return false;
		//return poly.Node.ConnectionAngles.se;
	}

	private bool IsCeil(Polygon poly)
	{
		return false;
	}

}
