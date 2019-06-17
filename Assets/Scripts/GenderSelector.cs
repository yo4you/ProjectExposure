using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenderSelector : MonoBehaviour
{
	private void Start()
	{
		PlayerPrefs.SetInt("Male", 1);

	}
	public void SetMale(bool male)
	{
		PlayerPrefs.SetInt("Male", male ?  1 : 0);
	}
}
