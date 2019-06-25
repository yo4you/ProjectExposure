using UnityEngine;

public class FeebackSubmission : MonoBehaviour
{
	[SerializeField]
	private bool _learn;

	public void SubmitFeedback(int feedback)
	{
		if (_learn)
		{
			PlayerPrefs.SetInt("feedback0", feedback);
		}
		else
		{
			PlayerPrefs.SetInt("feedback1", feedback);
		}
	}


}
