using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class VoronoiVertex : IEquatable<VoronoiVertex>
{
	public Vector2 pos;
	public List<int> conncetedVertices;

	public void Draw()
	{
		Debug.DrawLine(pos - new Vector2(0.1f, 0.1f), pos + new Vector2(0.1f, 0.1f), Color.blue);
		Debug.DrawLine(pos - new Vector2(0.1f, -0.1f), pos + new Vector2(0.1f, -0.1f), Color.blue);

	}

	public bool Equals(VoronoiVertex other)
	{
		
		return this.pos == other.pos;
	}

	public override int GetHashCode()
	{
		return 991532785 + EqualityComparer<Vector2>.Default.GetHashCode(pos);
	}
}