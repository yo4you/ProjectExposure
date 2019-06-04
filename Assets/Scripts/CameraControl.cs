using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    // Use this for initialization

    [SerializeField] GameObject _player;

    Camera _camera;

	void Start () {
        _camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
        _camera.transform.position = new Vector3(_player.transform.position.x, _player.transform.position.y, transform.position.z);
                
            
	}
}
