using System;
using System.IO;

internal class FeedbackManager
{

	private const string _FILENAME = "\\feedback.csv";
	private string _directory;

	public FeedbackManager()
	{
		_directory = Directory.GetCurrentDirectory() + _FILENAME;
		if (!File.Exists(_directory))
		{
			File.Create(_directory).Close();

		}
	}

	internal void Submit(Feedback feedback)
	{
		File.AppendAllText(_directory, "\n" + feedback.ToCSV());
	}
}