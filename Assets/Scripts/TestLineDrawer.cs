using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLineDrawer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float rayDistance;
            if (plane.Raycast(ray, out rayDistance))
            {
                this.transform.position = ray.GetPoint(rayDistance);
            }
        }
	}
}
