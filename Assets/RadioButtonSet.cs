using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioButtonSet : MonoBehaviour
{
	[SerializeField]
	List<Button> _buttons;
	Button _button;
	private void Start()
	{
		_button = GetComponent<Button>();
	}
	public void Click()
	{
		_button.interactable = false;
		foreach (var button in _buttons)
		{
			button.interactable = true;
			button.animator.Play("New State");
		}
	}
}
