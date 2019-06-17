using UnityEngine;

public class AgeSelector : MonoBehaviour
{

	private void Start()
	{
		PlayerPrefs.SetInt("Age", 3);

	}

	public void SetAge(int age)
	{
		PlayerPrefs.SetInt("Age", age);
	}
}

