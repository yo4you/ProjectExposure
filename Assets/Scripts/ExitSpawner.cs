using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExitSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject _exitPrefab;
	private LevelMaterialFixer _levelFixer;
	private float _angle = 0f;
	private List<Node<Polygon>> _path;
	private bool _spawned = true;
	private Node<Polygon> _exit;
	[SerializeField]
	private float _minDist;
	private void Start()
	{
		_levelFixer = FindObjectOfType<LevelMaterialFixer>();
		//_levelFixer.OnGenerationComplete += Spawn;
	}

	internal void Spawn()
	{
		Debug.Log("spawning");
	reset:
		var _playerSpawner = FindObjectOfType<SetPlayerSpawnPos>();
		Polygon start = _playerSpawner.SpawningPoly;
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

		if (!exitSite.Data.IsWall || (Vector3.Distance(exitSite.Data.Centre, start.Node.Data.Centre) < _minDist))
		{
			_angle += 5f;
			if (_angle > 359f)
			{
				_playerSpawner.Respawn();
				_angle = 0;
				goto reset;
			}
			goto start;
		};
		_exit = exitSite;
		var pos = exitSite.Data.Centre;
		exit.transform.position = new Vector3(pos.x, pos.y, FindObjectOfType<NodeTransverser>().PlayerZ);
		_spawned = false;
		_path = path;
	}

	private void Update()
	{
		if (!_spawned)
		{
			_spawned = true;
			FindObjectOfType<GuideSpawner>()?.Spawn(_exit);
		}
	}
}
