﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMaterialFixer : MonoBehaviour {
	public delegate void GenerationCompleteHandler();
	public event GenerationCompleteHandler OnGenerationComplete;
	public event GenerationCompleteHandler OnFinishMatFix;

	[SerializeField]
	GameObject _levelGenerator;
	[SerializeField]
	Material _material0;
	[SerializeField]
	Material _material1;
	[SerializeField]
	Material _material2;
	//[SerializeField]
	private Vector3 _scale;
	private VoronoiGenerator _voronoiGen;
	private bool _finishMatFix;

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
		_scale = new Vector3( _voronoiGen.MapSize.x, _voronoiGen.MapSize.y, _voronoiGen.MapSize.x);

		var player = FindObjectOfType<NodeTransverser>();
		player.PlayerZ = (-_scale.x / 10f) - 0.5f;
		var pos = player.transform.position;
		pos.z = player.PlayerZ;
		player.transform.position = pos;
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
            child.GetComponent<MeshRenderer>().material = _material1;// _material0;
            child.gameObject.layer = 10;// 9;
			child.gameObject.AddComponent<MeshCollider>();

			//var child1 = Instantiate(child);
			//child1.SetParent(transform);
			//child1.GetComponent<MeshRenderer>().material = _material1;
			//child1.gameObject.layer = 10;
			//child1.localScale = new Vector3(1, 1, 1);

            
			var child2 = Instantiate(child);
			child2.SetParent(transform);
			child2.GetComponent<MeshRenderer>().material = _material2;
			child2.gameObject.layer = 13;
			child2.localScale = new Vector3(1, 1, 1);

		}
		if ((!_finishMatFix )&& _levelGenerator.transform.childCount == 0)
		{
			_finishMatFix = true;
			OnFinishMatFix?.Invoke();
		}

		transform.localScale = Scale;
	}
}
