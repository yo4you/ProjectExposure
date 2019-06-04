using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableMovement : MonoBehaviour {


    [SerializeField] GameObject target;
    [SerializeField] float radius;
    [SerializeField] float rotationSpeed;
    [SerializeField] float radiusSpeed;

    // Use this for initialization
    void Start () {
        transform.position = (transform.position - target.transform.position).normalized * radius + target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(target.transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);

        Vector3 finalPos = (transform.position - target.transform.position).normalized * radius + target.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, finalPos, Time.deltaTime * radiusSpeed);

    }
}
