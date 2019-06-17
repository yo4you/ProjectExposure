using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomOrientation : MonoBehaviour
{
	[SerializeField]
	Vector3 _rotationRange = new Vector3();
    void Start()
    {
		var rotation = new Vector3(
			UnityEngine.Random.Range(-_rotationRange.x, _rotationRange.x),
			UnityEngine.Random.Range(-_rotationRange.y, _rotationRange.y),
			UnityEngine.Random.Range(-_rotationRange.z, _rotationRange.z)
			);
		transform.Rotate(rotation);
    }

    void Update()
    {
        
    }
}
