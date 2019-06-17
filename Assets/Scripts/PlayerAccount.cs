using System;

internal class PlayerAccount : IComparable<PlayerAccount>
{
	string name;
	int score;
	bool male;
	int age;

	public PlayerAccount(string name, int score, bool male, int age)
	{
		this.Name = name;
		this.Score = score;
		this.Male = male;
		this.Age = age;
	}

	public string Name { get => name; set => name = value; }
	public int Score { get => score; set => score = value; }
	public bool Male { get => male; set => male = value; }
	public int Age { get => age; set => age = value; }

	public int CompareTo(PlayerAccount other)
	{
		return Score.CompareTo(other.Score);
	}

	internal string ToCSV()
	{
		return $"{Name},{Score},{Male},{Age}";
	}
	internal static PlayerAccount FromCSV(string csv)
	{
		var split = csv.Split(new char[]{ ','});
	
		var outp = new PlayerAccount(split[0],  Int32.Parse(split[1]), bool.Parse(split[2]), Int32.Parse(split[3]));
		return outp;
	}
}