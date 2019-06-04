using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// fortune algorithm breechline, stores parabolas and edges
/// </summary>
internal class BreechLine
{
	//List<object> _line;
	private LinkedList<object> _linkedList = new LinkedList<object>();

	//private List<Parabola> _parabolas = new List<Parabola>();
	//private List<Parabola> _completeParabolas = new List<Parabola>();
	//private List<HalfEdge> _halfEdges = new List<HalfEdge>();

	private float _ypos;
	private List<HalfEdge> _edges;

	public float Ypos
	{
		get
		{
			return _ypos;
		}

		set
		{
			_ypos = value;
		}
	}

	public BreechLine()
	{
	}

	internal List<VornoiEvent> AddParabola(Vector2 focus)
	{

		if (_linkedList.Count == 0)
		{

		}


		Parabola prevParabola = GetParabolaInterSec(focus);
		Parabola leftParabola, centreParabola, rightParabola;
		leftParabola = new Parabola(focus);
		centreParabola = new Parabola(prevParabola.Focus);
		rightParabola = new Parabola(prevParabola.Focus);

		HalfEdge leftEdge = new HalfEdge(leftParabola.Focus, centreParabola.Focus - leftParabola.Focus);
		HalfEdge rightEdge = new HalfEdge(centreParabola.Focus, rightParabola.Focus - centreParabola.Focus);

		var node = _linkedList.Find(prevParabola);
		_linkedList.Remove(node);

		node = _linkedList.AddBefore(node, leftParabola);
		node = _linkedList.AddBefore(node, leftEdge);
		node = _linkedList.AddBefore(node, centreParabola);
		node = _linkedList.AddBefore(node, rightEdge);
		node = _linkedList.AddBefore(node, rightParabola);

		return new List<VornoiEvent>
		{
			CalcEdgeEvents(leftParabola),
			CalcEdgeEvents(rightParabola)
		};
	}

	private VornoiEvent CalcEdgeEvents(Parabola parabola)
	{
		Parabola leftParabola = (Parabola)_linkedList.First((p) => p is Parabola && p != parabola && parabola.Focus.x > (p as Parabola).Focus.x);
		Parabola rightParabola = (Parabola)_linkedList.First((p) => p is Parabola && p != parabola && parabola.Focus.x <= (p as Parabola).Focus.x);
		var node = _linkedList.Find(parabola);
		HalfEdge leftEdge = node.Next.Value as HalfEdge;
		HalfEdge rightEdge = node.Previous.Value as HalfEdge;
		if ((leftParabola == null) || (rightParabola == null) || (leftParabola.Focus == rightParabola.Focus))
		{
			return null;
		}

		var crosspoint = leftEdge.IntersectionPoint(rightEdge);
		if (crosspoint == null)
		{
			return null;
		}

		float distance = Vector2.Distance(crosspoint, parabola.Focus);
		if (crosspoint.y + distance > Ypos)
		{
			return null;
		}

		return new EdgeEvent(parabola)
		{
			Order = crosspoint.y + distance
		};
	}
	internal List<VornoiEvent> RemoveParabola(Parabola parabola)
	{
		Parabola leftParabola = (Parabola)_linkedList.First((p) => p is Parabola && p != parabola && parabola.Focus.x > (p as Parabola).Focus.x);
		Parabola rightParabola = (Parabola)_linkedList.First((p) => p is Parabola && p != parabola && parabola.Focus.x <= (p as Parabola).Focus.x);
		//remove l and r events from queue
		var node = _linkedList.Find(parabola);
		HalfEdge leftEdge = node.Next.Value as HalfEdge;
		HalfEdge rightEdge = node.Previous.Value as HalfEdge;
		var centre = Circle.CircumCircle(leftParabola.Focus, parabola.Focus, rightParabola.Focus).Pos;
		FinishHalfEdge(leftEdge, centre);
		FinishHalfEdge(rightEdge, centre);
		node = node.Previous;
		_linkedList.Remove(node); //left edge
		_linkedList.Remove(node); //parabola
		_linkedList.Remove(node); //right edge
		_linkedList.AddAfter(node, new HalfEdge(centre, leftParabola.Focus-centre));
		_linkedList.AddAfter(node, new HalfEdge(centre, rightParabola.Focus-centre));

		return new List<VornoiEvent> {
			CalcEdgeEvents(leftParabola),
			CalcEdgeEvents(rightParabola)
		};

	}

	private void FinishHalfEdge(HalfEdge edge, Vector2 point)
	{
		_edges.Add(new HalfEdge(edge.Pos,point));
	}
	//
	internal int AddHalfEdge(HalfEdge halfEdge)
	{
		return -1;
		//_parabolas.Add(new Parabola(focus));
		//return _parabolas.Count;
	}

	// 	internal void ResolveCell(int index)
	// 	{
	// 		_completeParabolas.Add(_parabolas.[index]));
	// 		_parabolas.RemoveAt(index);
	// 
	// 	}

	internal List<Polygon> CalcPolies()
	{
		return null;
		//throw new NotImplementedException();
	}

	internal Parabola GetParabolaInterSec(Vector2 pos)
	{
		throw new NotImplementedException();
	}


}