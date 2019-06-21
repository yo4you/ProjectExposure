using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupHud : MonoBehaviour
{
	[SerializeField]
	GameObject _scorePrefab;

	public void Score(int amount, int bonus)
	{
		var score = Instantiate(_scorePrefab, transform);
		score.GetComponentsInChildren<FloatTowards>()[0].Value = amount + bonus;
		score.GetComponentsInChildren<FloatTowards>()[0].ShowValue = amount;
		score.GetComponentsInChildren<FloatTowards>()[1].Value = 0;
		score.GetComponentsInChildren<FloatTowards>()[1].ShowValue = bonus;
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
		//	Score(UnityEngine.Random.Range(0, 100), UnityEngine.Random.Range(0, 100));
		}
	}
}
