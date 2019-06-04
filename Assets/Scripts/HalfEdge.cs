using UnityEngine;

internal class HalfEdge
{
	public HalfEdge(Vector2 pos, Vector2 dir)
	{
		Pos = pos;
		Dir = dir;
	}

	public Vector2 Pos { get; }
	public Vector2 Dir { get; }

	public Vector2 IntersectionPoint(HalfEdge other)
	{
		// line intersection: ax + c = bx + d 
		float a = (this.Dir.y / this.Dir.x) - this.Pos.x;
		float b = other.Dir.y / other.Dir.x - other.Pos.x;
		float c = this.Pos.x;
		float d = other.Pos.x;
		return new Vector2
		(
			 (a - c) / (a - b),
			 (a * d - b * c) / (a - b)
		);
	}


}