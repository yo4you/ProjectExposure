using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarScaler : MonoBehaviour {

    // Use this for initialization

    public float speed = 10.0f;
    [SerializeField] float _radius = 10.0f;

    void Start () {
        StartCoroutine("Scaling");
        StartCoroutine(FadeTo(0.0f, 1.0f));
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Scaling()
    {
        Vector3 destinationScale = new Vector3(2 * _radius, 2 * _radius, 2 * _radius);

        float timer = 0;

        while (true)
        {

            while (destinationScale.x > transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * 10;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            timer = 0;


        }
    }

    IEnumerator FadeTo( float targetOpacity, float duration)
    {

        Color color = GetComponent<Image>().color;
        float startOpacity = color.a;

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / duration);

            color.a = Mathf.Lerp(startOpacity, targetOpacity, blend);

            GetComponent<Image>().color = color;
            
            yield return null;
        }

        Destroy(gameObject);    
    }
}
