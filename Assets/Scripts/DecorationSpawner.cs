using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecorationSpawner : MonoBehaviour
{

	// 	[SerializeField]
	// 	private List<GameObject> _floorObjects;
	// 
	// 	[SerializeField]
	// 	private List<GameObject> _backObjects;
	// 
	// 	[SerializeField]
	// 	private List<GameObject> _ceilingObjects;
	[SerializeField]
	private LayerMask _mask;

	[SerializeField]
	private float _chanceToSpawn;

	[SerializeField]
	private List<Decoration> _decorations;

	private void Start()
	{
		FindObjectOfType<LevelMaterialFixer>().OnFinishMatFix += DecorationSpawner_OnGenerationComplete;
	}

	private void DecorationSpawner_OnGenerationComplete()
	{


		foreach (var poly in FindObjectOfType<VoronoiGenerator>().Polygons)
		{
			if (!poly.IsBackGround || UnityEngine.Random.Range(0, 100) > _chanceToSpawn)
			{
				continue;
			}

			// 			var spawnPool = new List<List<GameObject>>() { _backObjects };
			// 			if (IsCeil(poly))
			// 			{
			// 				spawnPool.Add(_ceilingObjects);
			// 			}
			// 			if (IsFloor(poly))
			// 			{
			// 				spawnPool.Add(_floorObjects);
			// 			}
			// 
			// 			var pool = spawnPool[UnityEngine.Random.Range(0, spawnPool.Count)];
			var pool = GetSpawnableDecorations(IsFloor(poly), IsCeil(poly), poly.Biome);
			if (pool.Count == 0)
			{
				continue;
			}

			var prefab = pool[UnityEngine.Random.Range(0, pool.Count)];
			var clone = Instantiate(prefab.Prefab, transform);
			PlaceOn(clone, poly);

		}
	}

	private List<Decoration> GetSpawnableDecorations(bool floor, bool ceil, BIOMES biome)
	{
		var outp = new List<Decoration>();

		foreach (var decor in _decorations)
		{
			bool backGround = !(floor || ceil);
			if (decor.ValidBiome(biome) && decor.Placable(floor, ceil, backGround))
			{
				outp.Add(decor);
			}
		}
		return outp;
	}

	private void PlaceOn(GameObject clone, Polygon poly)
	{
		Vector3 pos = poly.Centre;
		pos.z = -70;
		clone.layer = 12;
		Physics.Raycast(pos, Vector3.forward * 1000f, out RaycastHit hit, _mask);
		pos.z = hit.point.z;
		//clone.transform.Rotate(-90, 0, 0);
		clone.transform.position = pos;
	}




	private bool IsFloor(Polygon poly)
	{
		return poly.Node.ConnectionAngles.Where(i => !i.Value.Data.IsBackGround).Any(i => i.Key > 270f);
	}

	private bool IsCeil(Polygon poly)
	{
		return poly.Node.ConnectionAngles.Where(i => !i.Value.Data.IsBackGround).Any(i => i.Key > 45 && i.Key < 135f);

	}

}
