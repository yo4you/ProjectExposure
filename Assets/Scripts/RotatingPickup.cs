using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPickup : MonoBehaviour {

	[SerializeField]
	float _speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(Mathf.PerlinNoise(transform.rotation.x, transform.rotation.x), Mathf.PerlinNoise(transform.rotation.y, transform.rotation.y), Mathf.PerlinNoise(transform.rotation.z, transform.rotation.z)), Time.deltaTime * _speed);
	}
}
