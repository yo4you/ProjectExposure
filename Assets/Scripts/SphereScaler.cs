using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereScaler : MonoBehaviour {

    public float speed = 10.0f;
    //[SerializeField] float _radius = 10.0f;

    public float Radius { get; set; }

    // Use this for initialization
    void Start () {
    }

    public void StartScaling()
    {
        StartCoroutine("Scaling");
    }

    IEnumerator Scaling()
    {
        Vector3 destinationScale = new Vector3(transform.localScale.x, 2 * Radius, 2 * Radius);

        float timer = 0;

        while (true)
        {

            while (destinationScale.y > transform.localScale.y)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(0,0, 0) * Time.deltaTime * 10;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            timer = 0;


        }
    }

    // Update is called once per frame
    void Update () {

    }
}
