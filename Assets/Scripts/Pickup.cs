﻿using UnityEngine;

public class Pickup : MonoBehaviour
{
	[SerializeField] private int scoreToGive;
	private GameObject player;
	private Animator _animator;
	[SerializeField]
	private float _hintRadiusBorder = 20f;

	private float _distanceToPlayer;

    public float Distance;

	private void Start()
	{
        player = FindObjectOfType<PickupCollector>().gameObject;
		//player = FindObjectOfType<NodeTransverser>().gameObject;
		_animator =  GetComponentInChildren<Animator>();
	}
	public int Score => scoreToGive;

	private void Update()
	{
		var newDistance = Vector2.Distance(transform.position, player.transform.position);
		if (_distanceToPlayer < _hintRadiusBorder && newDistance > _hintRadiusBorder)
		{
			_animator.SetBool("Shaking", true);

		}
		if (newDistance < _hintRadiusBorder)
		{
			_animator.SetBool("Shaking", false);

		}
		_distanceToPlayer = newDistance;
        //print(_distanceToPlayer);
	}


}
