using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCounter : MonoBehaviour
{
	[SerializeField]
	GameObject _levelEnd;
	private void Start()
	{
		var lvl = PlayerPrefs.GetInt("level");
		SetVisible(lvl);
		if (lvl == 3)
		{
			_levelEnd.SetActive(true);
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
		if (lvl == 3)
		{

		}
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
