using System;
using UnityEngine;

[Serializable]
internal class Decoration
{
	[SerializeField]
	private GameObject _prefab;
	[SerializeField]
	private bool _background;
	[SerializeField]
	private bool _roof;
	[SerializeField]
	private bool _floor;
	[SerializeField]
	private bool _normal;
	[SerializeField]
	private bool _coral;
	[SerializeField]
	private bool _trash;
	[SerializeField]
	private bool _algae;
	[SerializeField]
	int _chanceToSpawn = 0;

	public bool Background { get => _background; set => _background = value; }
	public bool Roof { get => _roof; set => _roof = value; }
	public bool Floor { get => _floor; set => _floor = value; }
	public bool Normal { get => _normal; set => _normal = value; }
	public bool Coral { get => _coral; set => _coral = value; }
	public bool Trash { get => _trash; set => _trash = value; }
	public bool Algae { get => _algae; set => _algae = value; }
	public GameObject Prefab { get => _prefab; set => _prefab = value; }

	internal bool ValidBiome(BIOMES biome)
	{
		switch (biome)
		{
			case BIOMES.NORMAL:
				return Normal;
			case BIOMES.TRASH:
				return Trash;
			case BIOMES.CORAL:
				return Coral;
			case BIOMES.ALGAE:
				return Algae;
			case BIOMES.COUNT:
				return false;
			default:
				return false;
		}
	}

	internal bool Placable(bool floor, bool ceil, bool backGround)
	{

		if (UnityEngine.Random.Range(0, 100) >= _chanceToSpawn)
		{
			return false;
		}
		if (Floor && floor)
		{
			return true;
		}

		if (Roof && ceil)
		{
			return true;
		}

		if (Background && backGround)
		{
			return true;
		}

		return false;
	}
}