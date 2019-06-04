using System.Collections.Generic;

internal class EdgeEvent : VornoiEvent
{
	private Parabola _parabola;
	public EdgeEvent(Parabola parabola)
	{
		_parabola = parabola;
	}

	internal override List<VornoiEvent> Resolve(BreechLine breachLine)
	{
		return breachLine.RemoveParabola(_parabola);
	}
}