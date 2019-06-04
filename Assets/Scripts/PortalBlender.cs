using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBlender : MonoBehaviour {


    public Texture mainCameraTexture;
    public Texture sobelCameraTexture;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //if (!mainCameraTexture || !sobelCameraTexture) return;

        GetComponent<Renderer>()?.material.SetTexture("_MainRenderTexture", mainCameraTexture);
        GetComponent<Renderer>()?.material.SetTexture("_SobelRenderTexture", sobelCameraTexture);


    }
}
