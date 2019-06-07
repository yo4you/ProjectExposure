using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SobelLightSystem : MonoBehaviour
{
    [SerializeField] float sobelLightMaxAngle = 5;
    [SerializeField] float sobelLightScalingSpeed = 2.0f;
    [SerializeField] float sobelLightDepth = -5.0f;
    [SerializeField] GameObject sobelLightSource;

    private bool _isSpawned = false;

    private void OnEnable()
    {
        RayCastBounce.OnWaveBounced += SpawnLights;
    }

    private void OnDisable()
    {
        RayCastBounce.OnWaveBounced -= SpawnLights;
    }

    void SpawnLights(Vector3 pos)
    {
        //if (!_isSpawned)
        {
            GameObject light =
                Instantiate
                (
                    sobelLightSource,
                    new Vector3(
                        pos.x,
                        pos.y,
                        pos.z + sobelLightDepth),
                    Quaternion.identity
                );

            StartCoroutine(ScaleSobelLigthRange(light, sobelLightMaxAngle));
            //_isSpawned = true;
        }
    }

    IEnumerator ScaleSobelLigthRange(GameObject light, float Radius)
    {

        light.GetComponent<Light>().spotAngle = 0;// = new Vector3(0, outerIntersectorVolume.transform.localScale.y, 0);

        float timer = 0;

        bool isFinished = false;

        while (true && !isFinished)
        {

            while (Radius > light.GetComponent<Light>().spotAngle)
            {
                timer += Time.deltaTime;
                light.GetComponent<Light>().spotAngle += Time.deltaTime * sobelLightScalingSpeed; 
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            timer = 0;
            while (1 < light.GetComponent<Light>().spotAngle)
            {
                timer += Time.deltaTime;
                light.GetComponent<Light>().spotAngle -= Time.deltaTime * sobelLightScalingSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            timer = 0;

            isFinished = true;
        }

        if (isFinished) Destroy(light);

    }

}
