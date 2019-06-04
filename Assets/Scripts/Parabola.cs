using System;
using UnityEngine;

internal class Parabola : IComparable<Parabola>
{
	private Vector2 _focus;
	bool _active = true;
	public Parabola(Vector2 focus)
	{
		this.Focus = focus;
	}

	public bool Active
	{
		get
		{
			return _active;
		}

		set
		{
			_active = value;
		}
	}

	public Vector2 Focus
	{
		get
		{
			return _focus;
		}

		set
		{
			_focus = value;
		}
	}

	public int CompareTo(Parabola other)
	{
		return other.Focus.y.CompareTo(Focus.y);
	}

	internal void Resolve()
	{
		_active = false;
	}
}