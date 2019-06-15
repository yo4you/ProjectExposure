using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenderSelector : MonoBehaviour
{
	public bool Male { get; set; } = true;
	void SetMale(bool male)
	{
		Male = male;
	}
}
