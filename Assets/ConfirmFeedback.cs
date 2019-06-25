using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmFeedback : MonoBehaviour
{
	public void Confirm()
	{
		var feedback = new FeedbackManager();
		feedback.Submit(new Feedback(PlayerPrefs.GetInt("feedback0"), PlayerPrefs.GetInt("feedback1")));
	}
}
