using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreAccount : MonoBehaviour
{
    void Start()
    {
        
    }

	void Display(PlayerAccount account)
	{
		var textBoxes = transform.GetComponentsInChildren<Text>();
		textBoxes[0].text = account.Name;
		textBoxes[1].text = account.Score.ToString();

	}
}
