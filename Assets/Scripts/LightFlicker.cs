using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour {

	[SerializeField]
	float _speed = 1f;
	float _time;

	Light _light;
	float _intensity;
	void Start () {
		_light = GetComponent<Light>();
		_intensity = _light.intensity;
	}
	
	void Update () {
		_time = _time + _speed * Time.deltaTime;
		_light.intensity = _intensity * 0.5f + Mathf.PerlinNoise(_time, 0.5f) * 0.5f * _intensity;
	}
}
