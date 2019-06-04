using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomImageEffect : MonoBehaviour {


    public Material material;
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] RenderTexture renderTexture2;

    // Use this for initialization
    void Start () {
       // this.GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //[ExecuteInEditMode]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material!=null) Graphics.Blit(source, destination, material);
        if (renderTexture!=null) Graphics.Blit(destination, renderTexture);
        if (renderTexture2!=null) Graphics.Blit(source, renderTexture2);
    }
}
