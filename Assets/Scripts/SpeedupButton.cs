using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeedupButton : MonoBehaviour
{
	[SerializeField]
	float _speed;

	public void SpeedUp()
	{
		Time.timeScale = _speed;
		SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
	}

	private void SceneManager_sceneUnloaded(Scene arg0)
	{
		Time.timeScale = 1f;
	}
}
