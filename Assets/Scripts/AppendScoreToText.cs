using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppendScoreToText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		GetComponent<Text>().text += PlayerPrefs.GetInt("score").ToString();

	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
