using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatTowards : MonoBehaviour
{

	public int Value { get; set; }
	public int ShowValue { get; internal set; }

	private AddPoints _target;
	[SerializeField]
	float _speed;

	void Start()
	{
		_target = FindObjectOfType<AddPoints>();
		Text text = GetComponent<Text>();
		text.text = "+" + ShowValue.ToString();
	}

	void Update()
	{
		transform.position = Vector3.Lerp(transform.position, _target.transform.position, Time.deltaTime * _speed);
		if (Vector3.Distance(transform.position, _target.transform.position) < 0.3f)
		{
			if(Value!=0)
				_target.Add(Value);
			Destroy(gameObject);
		}
	}
}