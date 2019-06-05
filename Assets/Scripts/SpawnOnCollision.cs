using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnCollision : MonoBehaviour
{
	[SerializeField]
	GameObject _toSpawn;

	bool _spawned;
	private void OnTriggerEnter(Collider other)
	{
		if (!other.gameObject.GetComponent<SetPlayerSpawnPos>())
			return;
		if (!_spawned)
		{
			Instantiate(_toSpawn);
		}
		_spawned = true;

	}
	
}
