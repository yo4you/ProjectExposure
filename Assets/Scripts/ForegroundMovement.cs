using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundMovement : MonoBehaviour
{

    [SerializeField] GameObject cameraToFollow;
    [SerializeField] float mainCameraSpeedRatio;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cameraToFollow.transform.position.x * mainCameraSpeedRatio, cameraToFollow.transform.position.y * mainCameraSpeedRatio, transform.position.z);
    }
}
