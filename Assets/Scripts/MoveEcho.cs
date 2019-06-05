using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEcho : MonoBehaviour {

    // Use this for initialization

    //public List<Vector3> points;
	void Start () {
		
	}

    public void StartSpreading(List<Vector3> ps)
    {
        StartCoroutine(MoveBetweenPoints(ps));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator MoveBetweenPoints(List<Vector3> points)
    {

        for (int j = 0; j < points.Count; j++)
        {
            while (Vector3.Distance(transform.position, points[j]) > 0.005f)
            {
                transform.position = Vector3.MoveTowards(transform.position, points[j], 4.0f * Time.deltaTime);
                //_trails[i].transform.LookAt(_trailsPoints[i][j], Vector3.forward);
               // print("printing position in ray " + i + "point " + j);
                yield return null;
            }
        }
    }
}
