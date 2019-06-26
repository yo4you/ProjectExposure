using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddPoints : MonoBehaviour
{

	public int Points { get; set; }
	public int PointsTarget { get; set; }

	[SerializeField]
	float _time;

	Text _text;

	private void Start()
	{
		_text = GetComponent<Text>();

		Points = ScoreSystem.CurrentScore;
		PointsTarget = Points;
	}
	public void Add(int p)
	{
		PointsTarget += p;
		StartCoroutine("Increment");
		ScoreSystem.CurrentScore += p;
	}

	IEnumerator Increment()
	{
		int start = Points;
		int diff = (PointsTarget - Points);
		for (float i = 0; i < _time; i += Time.deltaTime)
		{
			yield return new WaitForSeconds(Time.deltaTime);
			Points = (int)(start + (diff * (i / _time)));
		}
		Points = PointsTarget;
		//ScoreSystem.CurrentScore = Points;
	}

	void Update()
	{
		_text.text = Points.ToString();
	}
}