using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class LevelGeneratorPicker : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _generators;

	private void Awake()
	{
		var readLevel = (_generators[PlayerPrefs.GetInt("level")]);
		var comps = readLevel.GetComponents(typeof(Component));
		foreach (var item in comps)
		{
			Debug.Log(item.GetType());
		}
		var types = comps.Aggregate(new List<Type>(), (list, comp) => { list.Add(comp.GetType()); return list; });

		foreach (var type in types)
		{
			var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty);
			var compRead = readLevel.GetComponent(type);
			var compWrite = FindObjectOfType(type);
			foreach (var field in fields)
			{
				field.SetValue(compWrite, field.GetValue(compRead));
			}
		}

	}

}
