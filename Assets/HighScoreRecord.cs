using System;
using CsvHelper;

internal class HighScoreRecord: IComparable<HighScoreRecord>
{
	[CsvHelper.Configuration.Attributes.Index(0)]
	public string Name { get; set; }
	[CsvHelper.Configuration.Attributes.Index(1)]
	public int Score { get; set; }

	public int CompareTo(HighScoreRecord other)
	{
		return	Score.CompareTo(other.Score);
	}
}