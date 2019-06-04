using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDepth : MonoBehaviour {

    // Use this for initialization

    private Camera _camera;
    public GameObject root;

	void Start () {
        _camera = GetComponent<Camera>();

        if (_camera != null)
            _camera.depthTextureMode = DepthTextureMode.Depth;
	}

    // Update is called once per frame
    void OnPreRender()
    {
        if (_camera == null)
            return;

        if (root == null)
            return;
        for (int i = 0; i < root.transform.childCount; i++)
        {
            GameObject obj = root.transform.GetChild(i).gameObject;
            if (obj != null || obj.GetComponent<MeshRenderer>() != null && obj.GetComponent<MeshRenderer>().sharedMaterial != null)
            {
                Matrix4x4 m = obj.GetComponent<MeshRenderer>().localToWorldMatrix;
                Matrix4x4 v = _camera.worldToCameraMatrix;
                Matrix4x4 p = GL.GetGPUProjectionMatrix(_camera.projectionMatrix, false);
                Matrix4x4 mvp = p * v * m;
                Shader.SetGlobalMatrix("_invMVP", mvp.inverse);
            }
        }
    }
}
