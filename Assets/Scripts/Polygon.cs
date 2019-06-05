using System;
using System.Collections.Generic;
using UnityEngine;

class Polygon : IEquatable<Polygon>
{
	public List<Vector2> Vertices { get; set; } = new List<Vector2>();

	public Vector2 Centre { get; set; }
	public bool IsWall { get; set; } = false;

	internal Node<Polygon> Node { get; set; }

	public void AddVertex(Vector2 vert)
	{
		if (!Vertices.Contains(vert))
		{
			Vertices.Add(vert);
		}
	}

	public void Draw()
	{								   
		for (int i = 0; i < Vertices.Count; i++)
		{
			Debug.DrawLine(Vertices[i], Vertices[(i + 1) % Vertices.Count], Color.red);
		}
	}

	public bool Equals(Polygon other)
	{
		return Centre == other.Centre;
	}

	internal void Order()
	{
		Centre = new Vector2();
		foreach (var vert in Vertices)
		{
			Centre += vert;
		}
		Centre /= Vertices.Count;

		Vertices.Sort((v0,v1) => 
			Vector2.SignedAngle(Vector2.up, v0 - Centre).CompareTo(Vector2.SignedAngle(Vector2.up, v1 - Centre))
		);
	}
}
