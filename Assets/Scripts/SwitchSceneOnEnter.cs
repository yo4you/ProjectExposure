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
		SceneManager.LoadScene(0);
	}
	void Update()
    {
        
    }
}
