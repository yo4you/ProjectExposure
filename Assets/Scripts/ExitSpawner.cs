﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExitSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject _exitPrefab;
	private LevelMaterialFixer _levelFixer;
	private float _angle = 90f;
	private List<Node<Polygon>> _path;
	bool _spawned = true;
	private void Start()
	{
		_levelFixer = FindObjectOfType<LevelMaterialFixer>();
		//_levelFixer.OnGenerationComplete += Spawn;
	}

	internal void Spawn()
	{
		Debug.Log("spawning");
		Polygon start = FindObjectOfType<SetPlayerSpawnPos>().SpawningPoly;
		var path = new List<Node<Polygon>>();
		var exit = Instantiate(_exitPrefab);

	start:
		VoronoiGenerator.DrawNodeGraphLine(start.Node, _angle, ref path);

		var exitSite = path.Last();
		//var exitSite = path[(int)(path.Count * 0.8f)];
		if (!exitSite.Data.IsWall && exitSite.ConnectionAngles.Any((i) => i.Value.Data.IsWall))
		{
			exitSite = exitSite.ConnectionAngles.First((i) => i.Value.Data.IsWall).Value;
		}

		if (!exitSite.Data.IsWall && _angle < 180f)
		{
			//_angle += 10f;
			//goto start;
		};
		var pos = exitSite.Data.Centre;
		exit.transform.position = new Vector3(pos.x, pos.y, -7);
		_spawned = false;
		_path = path;
	}

	private void Update()
	{
		if (!_spawned)
		{
			_spawned = true;
			FindObjectOfType<GuideSpawner>()?.Spawn(_path);

		}
	}
}