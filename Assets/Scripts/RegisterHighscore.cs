using UnityEngine;

public class RegisterHighscore : MonoBehaviour
{
	public void Register()
	{
		var playerAccount = new PlayerAccount(
			name: FindObjectOfType<ShowVirtualKeyboard>().Text,
			score: PlayerPrefs.GetInt("score"),
			male: PlayerPrefs.GetInt("Male") == 1,
			age: PlayerPrefs.GetInt("Age")
			);

		var highScoreManager = new HighScoreManager();
		highScoreManager.Add(playerAccount);
	}
}
