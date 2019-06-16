using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Generates a voronoi partitioning with a nodegraph based on the points generated in the attached RandomPointGenerator.cs
/// </summary>
[RequireComponent(typeof(RandomPointGenerator))]
public partial class VoronoiGenerator : MonoBehaviour
{

	public delegate void GenerationCompleteHandlder();
	public event GenerationCompleteHandlder OnGenerationComplete;
	[SerializeField]
	private int _maxPolyVertices = 11;
	[SerializeField]
	private bool _update = false;
	[SerializeField]
	private bool _drawDebug = true;
	[SerializeField]
	private int _surrounding = 2;
	private RandomPointGenerator _pointMap;
	private List<Polygon> _polygons = new List<Polygon>();
	private List<VoronoiVertex> _voronoiVertices = new List<VoronoiVertex>();
	private int _xBind;
	private int _yBind;
	private Node<Polygon> _nodeGraph;
	[SerializeField]
	private bool _drawNodeGraph;

	/// <summary>
	/// arbitrary start of the nodegraph
	/// </summary>
	internal Node<Polygon> NodeGraph
	{
		get => _nodeGraph;
		set => _nodeGraph = value;
	}
	/// <summary>
	/// generated polygons
	/// </summary>
	internal List<Polygon> Polygons
	{
		get => _polygons;

		set => _polygons = value;
	}

	private void Start()
	{
		_pointMap = GetComponent<RandomPointGenerator>();
	}

	public void ScaleAllPolies(Vector2 scale)
	{
		foreach (var poly in _polygons)
		{
			poly.Centre = new Vector2(scale.x * poly.Centre.x, scale.y * poly.Centre.y);
		}
	}

	public void Generate()
	{
		var startTime = DateTime.Now;
		// generate the partitioning
		if (!_pointMap)
		{
			_pointMap = GetComponent<RandomPointGenerator>();
		}

		_pointMap.Generate();
		GenerateVoronoi(_pointMap.Points, _pointMap.PointWidth, _pointMap.PointHeight);
		Debug.Log("map generated in : " + (DateTime.Now - startTime).TotalMilliseconds);
		startTime = DateTime.Now;

		OnGenerationComplete?.Invoke();
		Debug.Log("polygons generated in : " + (DateTime.Now - startTime).TotalMilliseconds);

	}

	private void OnValidate()
	{
		//HACK: fake update button used to regenerate the voronoi map
		if (_update)
		{
			_update = false;
			if (_pointMap)
			{
				Generate();
			}

		}
	}

	/// <summary>
	/// Generates polygons and nodegraphs using custom algorithm 
	/// </summary>
	/// <param name="points"></param>
	/// <param name="pointWidth"></param>
	/// <param name="pointHeight"></param>
	private void GenerateVoronoi(List<Vector2> points, int pointWidth, int pointHeight)
	{
		var startTime = DateTime.Now;
		Polygons = new List<Polygon>();
		_voronoiVertices = new List<VoronoiVertex>();
		// assigns the max width and height of the "grid" of points used
		SetGraphLimits(pointWidth, pointHeight);

		List<Node<Polygon>> nodes = new List<Node<Polygon>>();
		// generate an empty node for each polygon
		foreach (var point in points)
		{
			nodes.Add(new Node<Polygon>(new Polygon() { Centre = point }));
		}
		// go trough all "sites" 
		for (int pointIndex = 0; pointIndex < points.Count; pointIndex++)
		{
			Polygon poly = nodes[pointIndex].Data;
			int mapX, mapY;
			// grid position of the polygon 
			mapX = pointIndex % pointWidth;
			mapY = pointIndex / pointHeight;

			// the grid point surrounding the current point
			List<int> surroundingPoints = GetSurrouding(mapX, mapY);

			// check all the surrounding grid points in sets of 3
			for (int point0i = 0; point0i < surroundingPoints.Count; point0i++)
			{
				for (int point1i = 0; point1i < surroundingPoints.Count; point1i++)
				{
					if (point0i == point1i)
					{
						continue;
					}
					// we skip polygons with more vertices than our limit
					if (poly.Vertices.Count > _maxPolyVertices)
					{
						// "double break"
						goto end;
					}

					Vector2 vertex;
					// does a circumcircle check
					if (CheckCircle(pointIndex, surroundingPoints[point0i], surroundingPoints[point1i], ref surroundingPoints, ref points, out vertex))
					{
						// our vertex indicates a shared edge with two other sites, add them to the nodegraph connections 
						//	float angle = (Mathf.Atan2(vertex.y - poly.Centre.y, vertex.x - poly.Centre.x) * Mathf.Rad2Deg + 405f) % 360f;
						float angle = (Mathf.Atan2(vertex.y - poly.Centre.y, vertex.x - poly.Centre.x) * Mathf.Rad2Deg - 180f);
						if (!nodes[pointIndex].ConnectionAngles.ContainsKey(angle))
						{
							if (!nodes[pointIndex].ConnectionAngles.ContainsValue(nodes[surroundingPoints[point0i]]))
							{
								nodes[pointIndex].ConnectionAngles.Add(angle, nodes[surroundingPoints[point0i]]);
							}
							else
							{
								nodes[pointIndex].ConnectionAngles.Add(angle, nodes[surroundingPoints[point1i]]);
							}
						}
						// our vertex is added to the polygon
						poly.AddVertex(vertex);
					}
				}
			}
		end:
			// orders the vertices of the polygon clockwise
			poly.Node = nodes[pointIndex];
			poly.Order();
			Polygons.Add(poly);
		}
		// ensures there are no double connections in the nodelist

		_polygons = _polygons.Distinct().ToList();


		// removes OOB polygons
		var oobPolies = _polygons.Where(
			(Polygon p) => !p.Vertices.TrueForAll((Vector2 vert) => _pointMap.InBound(vert)) || p.Vertices.Count < 3
			);
		foreach (var oobpoly in oobPolies)
		{
			oobpoly.Node.ConnectionAngles.Values.ToList().ForEach(i => i.Data.IsBackGround = false);
		}
		oobPolies.ToList().ForEach((poly) =>
		{
			poly.Node.ConnectionAngles.Values.ToList().ForEach(
				(connectedPoly) =>
				{
					float key = connectedPoly.ConnectionAngles.FirstOrDefault(x => x.Value.Data.Centre == poly.Centre).Key;
					connectedPoly.ConnectionAngles.Remove(key);
				}
					);
		});
		_polygons = _polygons.Except(oobPolies).ToList();
		// arbitrary start to our nodegraph
		_nodeGraph = nodes[0];
	}

	/// <summary>
	/// Creates a triangle between 3 vertices, Checks the circumcircle of the triangle if any 
	/// neighbouring points (surrounding) are inside it. Outputs the centre of the circumcircle
	/// if one could be created.
	/// </summary>
	/// <param name="pointIndex0">index in the pointList of the first point to check</param>
	/// <param name="pointIndex1">index in the pointList of the second point to check</param>
	/// <param name="pointIndex2">index in the pointList of the third point to check</param>
	/// <param name="surrounding">indices in the pointlist of the surrounding points</param>
	/// <param name="pointList">the pointlist</param>
	/// <param name="vertex">centre of the circumcircle if one is found</param>
	/// <returns>if a circumcircle could be generated that didn't contain any neighbouring points</returns>
	private bool CheckCircle(int pointIndex0, int pointIndex1, int pointIndex2, ref List<int> surrounding, ref List<Vector2> pointList, out Vector2 vertex)
	{
		vertex = new Vector2();

		// circumcircle around the triangle point0, point1, point2
		var circle = Circle.CircumCircle(pointList[pointIndex1], pointList[pointIndex2], pointList[pointIndex0]);

		// ensure none of the surrounding points exists withing the circumcircle
		for (int i = 0; i < surrounding.Count; i++)
		{
			if (surrounding[i] == pointIndex1 || surrounding[i] == pointIndex2 || surrounding[i] == pointIndex0)
			{
				continue;
			}

			if (circle.Contains(pointList[surrounding[i]]))
			{
				return false;
			}
		}
		vertex = circle.Pos;
		return true;
	}

	private void SetGraphLimits(int pointWidth, int pointHeight)
	{
		_xBind = pointWidth;
		_yBind = pointHeight;
	}

	/// <summary>
	/// returns a list of the surrounding grid points (search depth _surrounding) 
	/// </summary>
	/// <param name="mapX">grid point X</param>
	/// <param name="mapY">grid point Y</param>
	/// <returns></returns>
	private List<int> GetSurrouding(int mapX, int mapY)
	{
		var outp = new List<int>();

		for (int x = -_surrounding; x <= _surrounding; x++)
		{
			for (int y = -_surrounding; y <= _surrounding; y++)
			{
				if (x == 0 && y == 0)
				{
					continue;
				}

				// the surrounding point falls off the grid bounds
				int surround = CheckBounds(mapX + x, mapY + y);

				if (surround != -1)
				{
					outp.Add(surround);
				}
			}
		}
		return outp;
	}
	/// <summary>
	/// return the 1D array location of coordinates x and y, returns -1 if OOB
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	private int CheckBounds(int x, int y)
	{
		if (x < 0 || y < 0 || x >= _xBind || y >= _yBind)
		{
			return -1;
		}
		else
		{
			return x + y * _yBind;
		}
	}

	private void Update()
	{
		if (_drawDebug)
		{
			foreach (var poly in Polygons)
			{
				poly.Draw();
			}
		}
		if (_drawNodeGraph)
		{
			DrawNodeGraph();
		}
		//Debug.Break();

	}
	public float angle;

	internal static void DrawNodeGraphLine(Node<Polygon> root, float angle, ref List<Node<Polygon>> visited, bool CheckWalls = true)
	{
		visited.Add(root);
		if (root.ConnectionAngles.Count == 0)
		{
			return;
		}
		/// true  true  = I
		/// false true  = I
		/// false false = I
		/// true false  = 0
		/// 
		///	 1 == 2 || 2

		foreach (var item in root.ConnectionAngles)
		{
			bool wall = (CheckWalls == item.Value.Data.IsBackGround) || item.Value.Data.IsBackGround;
			if (visited.Contains(item.Value) || !wall || Mathf.DeltaAngle(angle, item.Key) > 90)
			{
				continue;
			}
			if (item.Key > angle)
			{
				DrawNodeGraphLine(item.Value, angle, ref visited, CheckWalls);
				root.Data.Draw();
				DrawArrow(root.Data.Centre, item.Value.Data.Centre, Color.green);
				return;
			}
		}


		var finalItem = root.ConnectionAngles.First();
		bool finalwall = (CheckWalls == finalItem.Value.Data.IsBackGround) || finalItem.Value.Data.IsBackGround;

		if (!visited.Contains(finalItem.Value) && finalwall && Mathf.DeltaAngle(angle, finalItem.Key) < 90)
		{
			root.Data.Draw();
			DrawArrow(root.Data.Centre, finalItem.Value.Data.Centre, Color.green);
			DrawNodeGraphLine(finalItem.Value, angle, ref visited, CheckWalls);
		}

	}


	/// <summary>
	/// debug draw the node graph connections 
	/// </summary>
	private void DrawNodeGraph()
	{
		var randnodes = new List<Node<Polygon>> { };
		for (int i = 0; i < 10; i++)
		{
			var poly = _polygons[UnityEngine.Random.Range(0, _polygons.Count)];
			randnodes.Add(poly.Node);
		}
		foreach (var item in randnodes)
		{
			var visited = new List<Node<Polygon>> { };

			DrawNodeGraphLine(item, angle, ref visited);
		}
		return;
		// 		var visited = new List<Node<Polygon>>();
		// 		var open = new List<Node<Polygon>>()
		// 		{ _nodeGraph };
		// 		VisitAndDrawNode(ref open, ref visited);
	}


	/// <summary>
	/// draws node graph recusively
	/// </summary>
	/// <param name="root">node</param>
	/// <param name="close">visited nodes</param>
	private void VisitAndDrawNode(Node<Polygon> root, ref List<Node<Polygon>> close)
	{
		close.Add(root);
		float c = 0;
		foreach (var node in root.ConnectionAngles)
		{
			c++;
			DrawArrow(root.Data.Centre, node.Value.Data.Centre, new Color(c / root.ConnectionAngles.Count, 0, 0));
			if (!close.Any((i) => i.Data.Centre == node.Value.Data.Centre))
			{
				close.Add(node.Value);
				VisitAndDrawNode(node.Value, ref close);
			}

		}
	}

	private void VisitAndDrawNode(ref List<Node<Polygon>> open, ref List<Node<Polygon>> close)
	{
		if (open.Count == 0)
		{
			return;
		}

		var root = open[0];
		open.RemoveAt(0);
		close.Add(root);
		foreach (var node in root.ConnectionAngles)
		{
			var color = Color.cyan;
			if (node.Value.ConnectionAngles.Values.Any((i) => i.Data.Centre == root.Data.Centre))
			{
				color = Color.gray;
			}

			if (!_pointMap.InBound(root.Data.Centre))
			{
				root.Data.Draw();
				Debug.Log(_polygons.Contains(root.Data));
			}
			DrawArrow(root.Data.Centre, node.Value.Data.Centre, color);
			if (!close.Any((i) => i.Data.Centre == node.Value.Data.Centre))
			{
				open.Add(node.Value);
				//close.Add(node.Value);
				VisitAndDrawNode(ref open, ref close);
			}

		}
	}


	private static void DrawArrow(Vector2 pos0, Vector2 pos1, Color pcolor)
	{

		var dist = pos0 - pos1;
		dist = dist.normalized * 0.1f;
		Debug.DrawLine(pos0, pos1, pcolor, 10f);
		//Gizmos.DrawSphere(pos0,0.1f);
		Debug.DrawLine(pos1, pos1 + dist + new Vector2(-dist.y, dist.x), pcolor, 10f);
		Debug.DrawLine(pos1, pos1 + dist + new Vector2(dist.y, -dist.x), pcolor, 10f);
	}
}
