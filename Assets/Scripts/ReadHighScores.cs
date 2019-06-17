using System;
using System.Collections.Generic;
using UnityEngine;

public class ReadHighScores : MonoBehaviour
{

	[SerializeField]
	private GameObject _accountPrefab;
	[SerializeField]
	private float _distance;
	[SerializeField]
	private bool _daily;
	[SerializeField]
	private int _accounts;

	private void Start()
	{
		var accounts = new List<PlayerAccount>();
		(new HighScoreManager()).Load(ref accounts);
		int listings = 0;
		for (int i = 0; i < accounts.Count; i++)
		{
			PlayerAccount acc = accounts[i];

			if (listings > _accounts)
			{
				break;
			}
			if (DateTime.Today.Year != acc.Day.Date.Year )
			{
				continue;
			}
			if (_daily && (DateTime.Today != acc.Day.Date))
			{
				continue;
			}


			listings++;
			var display = Instantiate(_accountPrefab, transform);
			display.GetComponent<HighScoreAccount>().Display(acc);
			display.transform.position += Vector3.down * _distance * i;
		}

	}

	private void Update()
	{

	}
}
