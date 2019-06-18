using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    //[ExecuteInEditMode]
public class CustomImageEffect : MonoBehaviour {

    public Material material;
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] RenderTexture renderTexture2;

    public bool RT_to_DEST = false;
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
        if (material!=null && !RT_to_DEST) Graphics.Blit(source, destination, material);
        if (renderTexture!=null && !RT_to_DEST) Graphics.Blit(destination, renderTexture);
        if (renderTexture2!=null) Graphics.Blit(source, renderTexture2);

        //if (RT_to_DEST) Graphics.Blit(renderTexture, destination, material);
    }
}
