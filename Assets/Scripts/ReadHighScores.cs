using System.Collections.Generic;
using UnityEngine;

public class ReadHighScores : MonoBehaviour
{

	[SerializeField]
	private GameObject _accountPrefab;
	[SerializeField]
	float _distance;

	private void Start()
	{
		var accounts = new List<PlayerAccount>();
		(new HighScoreManager()).Load(ref accounts);
		for (int i = 0; i < accounts.Count; i++)
		{
			PlayerAccount acc = accounts[i];
			var display = Instantiate(_accountPrefab, transform);
			display.GetComponent<HighScoreAccount>().Display(acc);
			display.transform.position += Vector3.down * _distance * i;
		}
		
	}

	private void Update()
	{

	}
}
