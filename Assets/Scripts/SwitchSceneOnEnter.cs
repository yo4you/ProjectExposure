using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneOnEnter : MonoBehaviour
{
    void Start()
    {
		StartCoroutine(EndLevel());
    }
	private IEnumerator EndLevel()
	{
		yield return new WaitForSeconds(3);
		PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level")+1);

		SceneManager.LoadScene(0);
	}
	void Update()
    {
        
    }
}
