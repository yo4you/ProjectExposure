using System.Linq;
using UnityEngine;

public class SetPlayerSpawnPos : MonoBehaviour
{
	private VoronoiGenerator _voronoi;

	internal Polygon SpawningPoly { get; set; }
	public delegate void OnPathResolveHandle();
	public event OnPathResolveHandle OnPathResolve;
	private bool _spawned = true;
	private int[] _levelSeeds =
		{
		5071,
1826 ,
2938 ,
2562 ,
3690 ,
5197 ,
5782 ,
3878 ,
5704 ,
4685 ,
6068

	};
	private void Start()
	{
		FindObjectOfType<LevelMaterialFixer>().OnGenerationComplete += Spawn;
	}

	private void Spawn()
	{
		_voronoi = FindObjectOfType<VoronoiGenerator>();
		// get a random poly as a starting point
		int randIndex;
		Polygon poly;
		do
		{
			//randIndex = Random.Range(0, _voronoi.Polygons.Count);
			randIndex = _levelSeeds[Random.Range(0, _levelSeeds.Length)];
			poly = _voronoi.Polygons[randIndex];
			// keep picking random polies till we find a valid spawn position
		} while (!IsValidSpawn(poly));
		Debug.Log(randIndex);
		SpawningPoly = poly;
		var actor = GetComponent<NodeGraphActor>();
		actor.CurrentNode	 = poly.Node;
		transform.position = new Vector3(poly.Centre.x, poly.Centre.y, transform.position.z);
		_spawned = false;
	}
	private void Update()
	{

		if (!_spawned)
		{
			var spawner = FindObjectOfType<ExitSpawner>();
			if (spawner)
			{
				spawner.Spawn();
				_spawned = true;
			}
		}
	}

	private bool IsValidSpawn(Polygon poly)
	{
		if (!poly.IsWall)
		{
			return false;
		}

		return !poly.Node.ConnectionAngles.Values.All((i) => i.ConnectionAngles.Values.All((j) => !j.Data.IsWall));
	}


}
