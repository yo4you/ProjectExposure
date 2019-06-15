using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCounter : MonoBehaviour
{
	private void Start()
	{
		var lvl = PlayerPrefs.GetInt("level");
		SetVisible(lvl);
		if (lvl == 2)
		{
			SceneManager.LoadScene(1);
		};
	}
	private void OnApplicationQuit()
	{
		PlayerPrefs.SetInt("level",0);
	}
	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		var lvl = PlayerPrefs.GetInt("level");
		SetVisible(lvl);
	}

	void SetVisible(int toEnable)
	{
		int i = 0;
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(i == toEnable);
			i++;

		}
	}

}
