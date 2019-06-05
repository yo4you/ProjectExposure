using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaterialFixer : MonoBehaviour {
	public delegate void GenerationCompleteHandler();
	public event GenerationCompleteHandler OnGenerationComplete;

	[SerializeField]
	GameObject _levelGenerator;
	[SerializeField]
	Material _material0;
	[SerializeField]
	Material _material1;
	[SerializeField]
	Material _material2;
	[SerializeField]
	private Vector3 _scale;
	private VoronoiGenerator _voronoiGen;

	public Vector3 Scale
	{
		get
		{
			return _scale;
		}

		set
		{
			_scale = value;
		}
	}

	void Start () {
		_voronoiGen = _levelGenerator.GetComponent<VoronoiGenerator>();
		_voronoiGen.Generate();
		_voronoiGen.ScaleAllPolies(Scale);
		OnGenerationComplete?.Invoke();
	}

	void Update () {
		var models = new List<Transform>();

		foreach (Transform child in _levelGenerator.transform)
		{
			models.Add(child);
			child.SetParent(gameObject.transform);
			child.localScale = new Vector3(1, 1, 1);
			child.GetComponent<MeshRenderer>().material = _material0;
			child.gameObject.layer = 9;
			child.gameObject.AddComponent<MeshCollider>();

			var child1 = Instantiate(child);
			child1.SetParent(transform);
			child1.GetComponent<MeshRenderer>().material = _material1;
			child1.gameObject.layer = 10;
			child1.localScale = new Vector3(1, 1, 1);


			var child2 = Instantiate(child);
			child2.SetParent(transform);
			child2.GetComponent<MeshRenderer>().material = _material2;
			child2.gameObject.layer = 13;
			child2.localScale = new Vector3(1, 1, 1);


		}
		transform.localScale = Scale;
	}
}
