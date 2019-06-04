using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalScaler : MonoBehaviour {

    [SerializeField] float maxRadius;
    [SerializeField] float minRadius;
    [SerializeField] float fadeInSpeed = 1;
    [SerializeField] float fadeOutSpeed = 1;
    //[SerializeField] GameObject imageBorder;

    private bool _isFinishedScalingPortal = false;
    private bool _isFinishedScalingBorder = false;


    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
		
	}


    private void OnEnable()
    {
       SpawnRadars.OnScannerPlaced += ScalePortal;
        
    }
    private void OnDisable()
    {
       SpawnRadars.OnScannerPlaced -= ScalePortal;
    }

    void ScalePortal()
    {
        StartCoroutine("ScalePortalCircle");
        //StartCoroutine("ScalePortalBorder");
    }

    /*IEnumerator ScalePortalBorder()
    {

        //fix values
        float scale = maxRadius / minRadius;
        float originalScale = 10;// mageBorder.transform.localScale.x;
        float newScale = 10 * scale;

        float timer = 0;

        imageBorder.transform.localScale = new Vector3(originalScale, originalScale, 1);


        float difference = (scale * 10 - 10)/(maxRadius-minRadius);
        
        while (true && !_isFinishedScalingBorder)
        {

            while (newScale >= imageBorder.transform.localScale.x)
            {
                timer += Time.deltaTime;
                imageBorder.transform.localScale += new Vector3(difference, difference, 0) * Time.deltaTime * fadeInSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            timer = 0;

            while (originalScale < imageBorder.transform.localScale.x)
            {
                timer += Time.deltaTime;
                imageBorder.transform.localScale -= new Vector3(difference, difference, 0) * Time.deltaTime * fadeOutSpeed;
                yield return null;
            }
            timer = 0;
            yield return new WaitForSeconds(0.5f);

            _isFinishedScalingBorder = true;

        }

        _isFinishedScalingBorder = false;
    }
    */
    IEnumerator ScalePortalCircle()
    {
        Vector3 destinationScale = new Vector3(2 * maxRadius, 2 * maxRadius, 2 * maxRadius);
        Vector3 originalScale = new Vector3(2 * minRadius, 2 * maxRadius, 2 * minRadius);

        float timer = 0;

        transform.localScale = originalScale;

        while (true && !_isFinishedScalingPortal)
        {

            while (destinationScale.x >= transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * fadeInSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            timer = 0;

            while (originalScale.x < transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * fadeOutSpeed;
                yield return null;
            }
            timer = 0;
            yield return new WaitForSeconds(0.5f);

            _isFinishedScalingPortal = true;

        }

        _isFinishedScalingPortal = false;
    }

}
