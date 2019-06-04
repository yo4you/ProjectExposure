using System.Collections.Generic;
using UnityEngine;

internal class SiteEvent : VornoiEvent
{
	private Vector2 _pos;
	public SiteEvent(Vector2 pos)
	{
		_pos = pos;
		_order = pos.y;
	}

	internal override List<VornoiEvent> Resolve(BreechLine breachLine)
	{
		return breachLine.AddParabola(focus: _pos);
	}
}
