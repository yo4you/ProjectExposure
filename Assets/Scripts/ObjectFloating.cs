using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFloating : MonoBehaviour {


    [SerializeField] float amplitude;
    [SerializeField] float frequency;


    Vector3 _positionOffset;
    Vector3 _tempPosition;

    // Use this for initialization
    void Start () {
        _positionOffset = transform.position;	
	}
	
	// Update is called once per frame
	void Update () {
        _tempPosition = _positionOffset;
        _tempPosition.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = _tempPosition;
    }
}
