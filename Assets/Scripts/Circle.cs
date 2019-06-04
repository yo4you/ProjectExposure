using UnityEngine;


public class Circle
{
	public Circle(Vector2 pos, float radius)
	{
		Pos = pos;
		Radius = radius;
	}

	public Vector2 Pos;
	public float Radius;

	public bool Contains(Vector2 pos)
	{
		return Vector2.SqrMagnitude(pos - Pos) < Radius;
		//return Vector2.Distance(pos, Pos) < Radius;
	}

	//https://www.ics.uci.edu/~eppstein/junkyard/circumcenter.html
	public static Circle CircumCircle(Vector2 Ai, Vector2 Bi, Vector2 Ci)
	{
		float D = (Ai.x - Ci.x) * (Bi.y - Ci.y) - (Bi.x - Ci.x) * (Ai.y - Ci.y);
		Vector2 pos =
		new Vector2(
			(((Ai.x - Ci.x) * (Ai.x + Ci.x) + (Ai.y - Ci.y) * (Ai.y + Ci.y)) / 2 * (Bi.y - Ci.y)
			- ((Bi.x - Ci.x) * (Bi.x + Ci.x) + (Bi.y - Ci.y) * (Bi.y + Ci.y)) / 2 * (Ai.y - Ci.y))
			/ D,
			(((Bi.x - Ci.x) * (Bi.x + Ci.x) + (Bi.y - Ci.y) * (Bi.y + Ci.y)) / 2 * (Ai.x - Ci.x)
			- ((Ai.x - Ci.x) * (Ai.x + Ci.x) + (Ai.y - Ci.y) * (Ai.y + Ci.y)) / 2 * (Bi.x - Ci.x))
			/ D
			);

		return new Circle(pos, ((Ci.x - pos.x) * (Ci.x - pos.x) + (Ci.y - pos.y) * (Ci.y - pos.y)));

	}

}

