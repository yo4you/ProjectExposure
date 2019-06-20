using System;
using System.Collections.Generic;
using UnityEngine;

public class AssignBiomes : MonoBehaviour
{
	private Vector2 _randomOffset;
	private VoronoiGenerator _voronoiMap;
	[SerializeField]
	private List<BiomeSet> _biomeSets;
	[SerializeField]
	private float _mapScale = 8000f;

	internal List<BiomeSet> BiomeSets { get => _biomeSets; set => _biomeSets = value; }

	private void Start()
	{
		_randomOffset = UnityEngine.Random.insideUnitCircle * _mapScale;
		_voronoiMap = FindObjectOfType<VoronoiGenerator>();
		_voronoiMap.OnGenerationComplete += AssignBiomes_OnGenerationComplete;
	}

	private void AssignBiomes_OnGenerationComplete()
	{
		foreach (var poly in _voronoiMap.Polygons)
		{
			poly.Biome = DetermineBiome(poly.Centre);
		}
	}

	private BIOMES DetermineBiome(Vector2 centre)
	{
		var perlin = Mathf.PerlinNoise(_randomOffset.x + centre.x * _mapScale, _randomOffset.y + centre.y * _mapScale);
		perlin = Mathf.Clamp01(perlin -0.0001f);
		return (BIOMES)(perlin * (int)BIOMES.COUNT);
	}

}

[Serializable]
internal class BiomeSet
{
	[SerializeField]
	private BIOMES _biome;
	[SerializeField]
	private int _uv;

	public int Uv { get => _uv; set => _uv = value; }
	public BIOMES Biome { get => _biome; set => _biome = value; }
}