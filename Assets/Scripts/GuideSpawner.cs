using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSpawner : MonoBehaviour {
	private LevelMaterialFixer _levelFixer;
	[SerializeField]
	int _tilesPerNode = 4;
	[SerializeField]
	GameObject _spawnPrefab;
	private void Start()
	{
		_levelFixer = FindObjectOfType<LevelMaterialFixer>();
	}

	internal void Spawn(List<Node<Polygon>> nodes)
	{

		for (int i = 0; i < nodes.Count; i+= _tilesPerNode)
		{
			var spawn = Instantiate(_spawnPrefab);
			var pos = nodes[i].Data.Centre;// * _levelFixer.Scale.x;

			spawn.transform.position = new Vector3(pos.x,pos.y,-7);
		}
	}
}
