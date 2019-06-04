using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindUberShaderValues : MonoBehaviour {


    GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");

	}
	
	// Update is called once per frame
	void Update () {

        //var screenPos = this.GetComponent<Camera>().WorldToScreenPoint(player.transform.position);
        var screenPos = this.GetComponent<Camera>().WorldToScreenPoint(new Vector3(0,0,0));


        //bind players position
        this.GetComponent<CustomImageEffect>().material?.SetFloat("_SourcePos_X", screenPos.x);
        this.GetComponent<CustomImageEffect>().material?.SetFloat("_SourcePos_Y", screenPos.y);

        //bind screen resolution
        var screenResolution = new Vector2(Screen.width, Screen.height);
        this.GetComponent<CustomImageEffect>().material?.SetFloat("_ScreenUV_X", screenResolution.x);
        this.GetComponent<CustomImageEffect>().material?.SetFloat("_ScreenUV_Y", screenResolution.y);
    }
}
