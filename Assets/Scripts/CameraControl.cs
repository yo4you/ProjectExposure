using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    // Use this for initialization

    [SerializeField] GameObject _player;

    Camera _camera;
	private Vector3 _shake;

	void Start () {
		_player = FindObjectOfType<NodeTransverser>().gameObject;
        _camera = GetComponent<Camera>();
	}
	
	void LateUpdate () {
        _camera.transform.position = _shake + new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
                
            
	}

	internal void Shake(Vector3 position)
	{
		_shake = position;
	}
}
