using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;// Required when using Event data.
using UnityEngine.UI;
using System.Linq;

public class ShowVirtualKeyboard : MonoBehaviour, ISelectHandler, IDeselectHandler// required interface when using the OnSelect method.
{
	//Do this when the selectable UI object is selected.
	private VirtualKeyboard _virtualKeyboard;
	
	private void Start()
	{
		HighScoreManager hs = new HighScoreManager();
		_virtualKeyboard = new VirtualKeyboard();
		GetComponent<InputField>().onEndEdit.AddListener((i) =>
		{
			Debug.Log(i.ToString());
			_virtualKeyboard.HideTouchKeyboard();

		});
	}

	public void OnSelect(BaseEventData eventData)
	{
		_virtualKeyboard.ShowTouchKeyboard();
	}
	public void OnDeselect(BaseEventData eventData)
	{
		_virtualKeyboard.HideTouchKeyboard();
	}

}

internal class HighScoreManager
{
	private const string fileName = "\\highscore.csv";
	private string _directory;
	//private Dictionary<string, int> _submissions = null;
	private List<HighScoreRecord> _submissions = null;

	public List<HighScoreRecord> Submissions
	{
		get
		{
			if (_submissions != null)
			{
				ParseSubmissions();
			}
			return _submissions;
		}
		set => _submissions = value;
	}

	private void ParseSubmissions()
	{
		_submissions = new List<HighScoreRecord>();
		using (var reader = new StreamReader(_directory))
		using (var csv = new CsvHelper.CsvReader(reader))
		{
			csv.Configuration.HasHeaderRecord = false;
			var records = csv.GetRecords<HighScoreRecord>();
		}

	}

	public HighScoreManager()
	{

		_directory = Directory.GetCurrentDirectory() + fileName;
		if (!File.Exists(_directory))
		{
			File.Create(_directory);
		}
	}

	private void Add(HighScoreRecord record)
	{
		if (Submissions.Any(i=>i.Name.Equals(record.Name)))
		{
			Submissions.Find(i => i.Name.Equals(record.Name)).Score = record.Score;
		}
		else
		{
			Submissions.Add(record);
			UpdateFile();
		}
		
	}

	private void UpdateFile()
	{
		var records = new List<HighScoreRecord>() { };
		using (var writer = new StreamWriter(_directory))
		using (var csv = new CsvHelper.CsvWriter(writer))
		{
			//csv.WriteRecords(record);
		}
	}
}