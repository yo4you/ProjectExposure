using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
	public void RestartNow()
	{
		PlayerPrefs.SetInt("level", 0);
		SceneManager.LoadScene(0);

	}
	private void Update()
	{
	//	if (Input.GetMouseButton(0)) RestartNow();
	}
}
