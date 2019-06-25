using System;
internal class Feedback
{
	private int v1;
	private int v2;

	public Feedback(int v1, int v2)
	{
		this.v1 = v1;
		this.v2 = v2;
	}

	internal string ToCSV()
	{
		return $"{v1},{v2}";
	}

}