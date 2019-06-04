using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleSystem : MonoBehaviour {


    [SerializeField] GameObject rippleSprite;
    [SerializeField] GameObject finalPoint;
    [SerializeField] float speed;

    bool spawned = false;
    List<GameObject> rips = new List<GameObject>();

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 15; i++)
        {
            rips.Add(GameObject.Instantiate(rippleSprite));
        }
	}
	
	// Update is called once per frame
	void Update () {

        //if (spawned) return;

        float step = speed * Time.deltaTime;

        for (int i = 0; i < 15; i++)
        {
            rips[i].transform.localScale += new Vector3(step, step, step)*0.05f;
            rips[i].transform.position = Vector3.MoveTowards(rips[i].transform.position, finalPoint.transform.position, step*5.0f/i);
        }
	}
}
