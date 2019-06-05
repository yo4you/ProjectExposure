using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRadars : MonoBehaviour
{


    float _rayCastDistance = 50;
    [SerializeField] float scannerRadius = 5;
    [SerializeField] float scannerScalingSpeed = 5;
    [SerializeField] float scannerDepth = 5;
    [SerializeField] GameObject sobelLightSource;
    [SerializeField] GameObject circleObject;
    [SerializeField] GameObject bigcircleObject;
    [SerializeField] GameObject playerObject;
    [SerializeField] GameObject particleTappingObject;

    bool lightSpawned = false;
    bool started = false;

    public delegate void ScannerSpawned();
    public static event ScannerSpawned OnScannerPlaced;

    CanvasMouseTracker _mousetracker;
    // Use this for initialization
    void Start()
    {
        started = true;
        _mousetracker = FindObjectOfType<CanvasMouseTracker>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        //if mouse button (left hand side) pressed instantiate a raycast
        //if (Input.GetMouseButtonDown(0))
        if (!_mousetracker)
            _mousetracker = FindObjectOfType<CanvasMouseTracker>();
        else if (Input.GetMouseButtonDown(0) && !_mousetracker.RayCastHitPlayer())
        {

            //create a ray cast and set it to the mouses cursor position in game
            Vector3 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _rayCastDistance))
            {
                OnScannerPlaced?.Invoke();

                //spawn UI for big circle
                GameObject bigcircleUI = Instantiate(bigcircleObject, Camera.main.WorldToScreenPoint(new Vector3(
                    GameObject.FindGameObjectWithTag("Player").gameObject.transform.position.x,
                    GameObject.FindGameObjectWithTag("Player").gameObject.transform.position.y,
                    0)),
                    Quaternion.identity);
                bigcircleUI.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_SS").transform, true);

                float dist = CalculateDistance(mousePos);

                Vector3 mousePositionDepth = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z + scannerDepth);
                mousePositionDepth = Camera.main.ScreenToWorldPoint(mousePositionDepth);

                if (!lightSpawned)
                {
                    GameObject light = Instantiate(sobelLightSource, mousePositionDepth, Quaternion.identity);
                    StartCoroutine(ScaleSobelLigthRange(light, scannerRadius));
                    lightSpawned = true;
                }


                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                GameObject circleUI = Instantiate(particleTappingObject, mousePosition, Quaternion.identity);
                //circleUI.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_SS").transform, true);
                circleUI.GetComponent<ParticleSystem>()?.Play();
            }

            lightSpawned = false;

        }

    }


    float CalculateDistance(Vector3 mousePos)
    {
        Vector2 playerPos = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y);
        Vector2 clickPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
        return Vector2.Distance(playerPos, clickPos);

    }

    IEnumerator ScaleSobelLigthRange(GameObject light, float Radius)
    {

        light.GetComponent<Light>().range = 0;// = new Vector3(0, outerIntersectorVolume.transform.localScale.y, 0);

        float timer = 0;

        while (true)
        {

            while (Radius > light.GetComponent<Light>().range)
            {
                timer += Time.deltaTime;
                light.GetComponent<Light>().range += Time.deltaTime * scannerScalingSpeed;
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            timer = 0;


        }
    }

}