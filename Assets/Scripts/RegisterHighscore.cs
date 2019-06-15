using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterHighscore : MonoBehaviour
{
	public void Register()
	{
		var playerAccount = new PlayerAccount(
			name : FindObjectOfType<ShowVirtualKeyboard>().Text,
			score: PlayerPrefs.GetInt("score"),
			male: FindObjectOfType<GenderSelector>().Male,
			age: FindObjectOfType<AgeSelector>().Age
			);

		var highScoreManager = new HighScoreManager();
		highScoreManager.Add(playerAccount);
	}
}
