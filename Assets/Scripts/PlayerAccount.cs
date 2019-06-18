using System;

internal class PlayerAccount : IComparable<PlayerAccount>
{
	string name;
	int score;
	bool male;
	int age;
	DateTime day;

	public PlayerAccount(string name, int score, bool male, int age, DateTime day)
	{
		this.Name = name.ToLower();
		this.Score = score;
		this.Male = male;
		this.Age = age;
		this.Day = day;
	}

	public string Name { get => name; set => name = value; }
	public int Score { get => score; set => score = value; }
	public bool Male { get => male; set => male = value; }
	public int Age { get => age; set => age = value; }
	public DateTime Day { get => day; set => day = value; }

	public int CompareTo(PlayerAccount other)
	{
		return Score.CompareTo(other.Score);
	}

	internal string ToCSV()
	{
		return $"{Name},{Score},{Male},{Age},{Day.ToString()}";
	}
	internal static PlayerAccount FromCSV(string csv)
	{
		var split = csv.Split(new char[]{ ','});
	
		var outp = new PlayerAccount(split[0],  Int32.Parse(split[1]), bool.Parse(split[2]), Int32.Parse(split[3]), DateTime.Parse(split[4]));
		return outp;
	}
}