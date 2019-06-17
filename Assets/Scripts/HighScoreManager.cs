using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

internal class HighScoreManager
{
	private List<PlayerAccount> _accounts;
	private string _directory;
	private const int _MAXACCOUNTS = 400;
	private const string _FILENAME = "\\highscore.csv";

	public HighScoreManager()
	{
		_directory = Directory.GetCurrentDirectory() + _FILENAME;
		if (!File.Exists(_directory))
		{
			File.Create(_directory).Close();

		}
	}

	internal void Add(PlayerAccount playerAccount)
	{
		Load(ref _accounts);
		var acc = _accounts.Find(i => i.Name.Equals(playerAccount.Name));
		if (acc != null)
		{
			acc.Score = Math.Max(acc.Score, playerAccount.Score);
		}
		else
		{
			_accounts.Add(playerAccount);
		}

		_accounts.Sort();
		_accounts.Reverse();

		if (_accounts.Count > _MAXACCOUNTS)
		{
			_accounts = _accounts.GetRange(0, _MAXACCOUNTS - 1);
		}
		WriteToFile(_accounts);
	}

	private void WriteToFile(List<PlayerAccount> accounts)
	{

		StringBuilder strinBuilder = new StringBuilder();
		foreach (var account in _accounts)
		{
			strinBuilder.AppendLine(account.ToCSV());
		}

		File.WriteAllText(_directory, strinBuilder.ToString());
	}

	public void Load(ref List<PlayerAccount> accounts)
	{
		accounts = new List<PlayerAccount>();
		var content = File.ReadAllText(_directory);
		using (StringReader reader = new StringReader(content))
		{

			var line = "";
			do
			{
				line = reader.ReadLine();
				if (line == null)
					break;
				accounts.Add(PlayerAccount.FromCSV(line));
			} while (true);
		}
	}
}