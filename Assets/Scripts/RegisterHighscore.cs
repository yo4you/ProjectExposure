using System;
using UnityEngine;

public class RegisterHighscore : MonoBehaviour
{

	public void Register()
	{
		var playerAccount = new PlayerAccount(
			name: PlayerPrefs.GetString("Name"),
			score: PlayerPrefs.GetInt("score"),
			male: PlayerPrefs.GetInt("Male") == 1,
			age: PlayerPrefs.GetInt("Age"),
			day: DateTime.Now
			);

		var highScoreManager = new HighScoreManager();
		highScoreManager.Add(playerAccount);
	}
}
