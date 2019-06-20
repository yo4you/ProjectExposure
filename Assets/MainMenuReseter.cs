using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuReseter : MonoBehaviour
{
    void Start()
    {
		PlayerPrefs.SetInt("score", 0);
		PlayerPrefs.SetInt("level", 0);
    }

    void Update()
    {
        
    }
}
