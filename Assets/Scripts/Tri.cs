using System;
using System.Collections.Generic;

internal class Tri : IEquatable<Tri>
{
	private int[] _indices;
	public Tri(int i0, int i1, int i2)
	{
		Indices = new int[] { i0, i1, i2 };
	}
	public int[] Indices
	{
		get
		{
			return _indices;
		}

		set
		{
			_indices = value;
		}
	}

	public bool Equals(Tri other)
	{
		return
			other.Indices[0] == _indices[0] &&
			other.Indices[1] == _indices[1] &&
			other.Indices[2] == _indices[2];
	}

	public override int GetHashCode()
	{
		return -2143259509 + EqualityComparer<int[]>.Default.GetHashCode(_indices);
	}
}
