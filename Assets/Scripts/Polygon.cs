using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

class Polygon : IEquatable<Polygon>
{
	public List<Vector2> Vertices { get; set; } = new List<Vector2>();

	public Vector2 Centre { get; set; }
	public bool IsBackGround { get; set; } = true;
	public BIOMES Biome { get; internal set; }
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

		// sort node angles
		Node.ConnectionAngles = Node.ConnectionAngles.OrderBy(a => a.Key).ToDictionary(x => x.Key, x => x.Value);

		var newAngles = new Dictionary<float, Node<Polygon>>();
		for (int i = 0; i < Node.ConnectionAngles.Count; i++)
		{
			var item = Node.ConnectionAngles.ElementAt(i);
			float newAngle = Mathf.Atan2(Centre.y - item.Value.Data.Centre.y, Centre.x - item.Value.Data.Centre.x) * Mathf.Rad2Deg + 180f;
			//float newAngle = Mathf.Atan2(Centre.y - Vertices[i].y, Centre.x - Vertices[i].x) * Mathf.Rad2Deg + 180f;
			if(!newAngles.ContainsKey(newAngle))
				newAngles.Add(newAngle, item.Value);
		}
		Node.ConnectionAngles = newAngles.OrderBy(a => a.Key).ToDictionary(x => x.Key, x => x.Value);

	}
}
