using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignBiomes : MonoBehaviour
{
	private VoronoiGenerator _voronoiMap;

	void Start()
    {
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
		return BIOMES.CORAL;
	}

}
