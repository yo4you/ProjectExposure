using System;
using System.Collections.Generic;

internal abstract class VornoiEvent : IComparable<VornoiEvent>
{


	public float Order
	{
		get
		{
			return _order;
		}

		set
		{
			_order = value;
		}
	}


	protected float _order = 9999999;

	public int CompareTo(VornoiEvent other)
	{
		return -_order.CompareTo(other._order);
	}

	abstract internal List<VornoiEvent> Resolve(BreechLine breachLine);
}
