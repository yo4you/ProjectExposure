using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideSpawner : MonoBehaviour {
	private LevelMaterialFixer _levelFixer;
	[SerializeField]
	int _tilesPerNode = 4;
	[SerializeField]
	int _guidingSpokes;
	[SerializeField]
	GameObject _guideSpawnPrefab;
	[SerializeField]
	GameObject _pickupSpawnPrefab;
	float _playerZ;
	[SerializeField]
	int _pickupSpawnChance = 10;
	[SerializeField]
	int _exitTileMargin = 10;
	[SerializeField]
	private int _minBranchDistance;
	[SerializeField]
	LayerMask _mask;

	private void Start()
	{
		_levelFixer = FindObjectOfType<LevelMaterialFixer>();
		_playerZ = FindObjectOfType<NodeTransverser>().PlayerZ;
		
	}

	internal void Spawn(Node<Polygon> node)
	{
		for (int i = 0; i < _guidingSpokes; i++)
		{
			var line = new List<Node<Polygon>>();
			VoronoiGenerator.DrawNodeGraphLine(node,i * 360f/_guidingSpokes, ref line,false);
			Spawn(line, node.Data.Centre);
		}
	}

	internal void Spawn(List<Node<Polygon>> nodes, Vector2 centre)
	{

		for (int i = _exitTileMargin; i < nodes.Count; i+= _tilesPerNode)
		{
			var spawn = Instantiate(_guideSpawnPrefab,transform);
			var pos = nodes[i].Data.Centre;// * _levelFixer.Scale.x;
			if (UnityEngine.Random.Range(0, 100) < _pickupSpawnChance)
			{
				SpawnPickup(nodes[i]);
			}
			Physics.Raycast(new Vector3(pos.x, pos.y, -90), Vector3.forward * 900f, out RaycastHit hit);
			spawn.transform.position = new Vector3(pos.x,pos.y,hit.point.z - 0.2f);
			spawn.transform.Rotate( Vector3.forward, 270f + Mathf.Rad2Deg * Mathf.Atan2(centre.y - pos.y, centre.x - pos.x));
		}
	}

	private void SpawnPickup(Node<Polygon> node)
	{
		
		var line = new List<Node<Polygon>>();
		VoronoiGenerator.DrawNodeGraphLine(node,  UnityEngine.Random.Range(0f,360f), ref line, true);
		if (line.Count > _minBranchDistance)
		{
			var spawn = Instantiate(_pickupSpawnPrefab,transform);
			var pos = line[_minBranchDistance-1].Data.Centre;
			spawn.transform.position = new Vector3(pos.x, pos.y, _playerZ); ;
		}
	}
}
