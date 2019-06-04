using UnityEngine;
using System.Linq;

public class SetPlayerSpawnPos : MonoBehaviour
{
	private VoronoiGenerator _voronoi;

	private void Start()
	{
		FindObjectOfType<LevelMaterialFixer>().OnGenerationComplete += Spawn;
	}

	private void Spawn()
	{
		_voronoi = FindObjectOfType<VoronoiGenerator>();
		// get a random poly as a starting point
		Polygon poly;
		do
		{
			var randIndex = 1058;// Random.Range(0, _voronoi.Polygons.Count);
	
			poly = _voronoi.Polygons[randIndex];
			// keep picking random polies till we find a valid spawn position
		} while (!IsValidSpawn(poly));

		transform.position = new Vector3(poly.Centre.x, poly.Centre.y, transform.position.z);
	}

	private bool IsValidSpawn(Polygon poly)
	{
		if (!poly.IsWall)
			return false;
		return !poly.Node.ConnectionAngles.Values.All((i)=> i.ConnectionAngles.Values.All((j) => !j.Data.IsWall));
		return true;
	}

	
}
