using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuReseter : MonoBehaviour
{
    void Start()
    {
		PlayerPrefs.SetInt("score", 0);
		PlayerPrefs.SetInt("level", 0);
		PlayerPrefs.SetInt("feedback0", 4);
		PlayerPrefs.SetInt("feedback1", 4);
	}

	void Update()
    {
        
    }
}
