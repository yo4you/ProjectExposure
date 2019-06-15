using UnityEngine;

public class AgeSelector : MonoBehaviour
{
	public int Age { get; set; } = 3;

	private void SetAge(int age)
	{
		Age = age;
	}
}

