using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevel : MonoBehaviour
{
	private Coroutine _levelEnding;

	void Start()
    {
        
    }

    void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			if (_levelEnding != null)
			{
				StopCoroutine(_levelEnding);
				_levelEnding = null;
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			_levelEnding =  StartCoroutine(EndLevel());
		}
    }

	private IEnumerator EndLevel()
	{
		yield return new WaitForSeconds(10);
		SceneManager.LoadScene(0);
	}
}
