using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a set of random points withing a square by applying a random 2D offset to points placed in an even grid
/// </summary>
public class RandomPointGenerator : MonoBehaviour {

	[SerializeField]
	Vector2 _corner0;
	[SerializeField]
	Vector2 _corner1;
	[SerializeField]
	int _pointWidth;
	[SerializeField]
	int _pointHeight;
	[SerializeField]
	float _offset;
	[SerializeField]
	private bool _drawDebug;
	
	private Vector2 _mapSize;
	private Vector2 _randMapOffset;
	[SerializeField]
	private Vector2 _mapFocus;
	[SerializeField]
	private float _randRange = 2.0f;
	[SerializeField]
	private float _lineWidth;
	[SerializeField]
	private float _perlinPower;

	public List<Vector2> Points { get; set; }


	/// <summary>
	/// amount of points allong the Xaxis
	/// </summary>
	public int PointWidth
	{
		get
		{
			return _pointWidth;
		}

		set
		{
			_pointWidth = value;
		}
	}
	/// <summary>
	/// amount of points allong the Yaxis
	/// </summary>
	public int PointHeight
	{
		get
		{
			return _pointHeight;
		}

		set
		{
			_pointHeight = value;
		}
	}
	
	public Vector2 Corner0
	{
		get
		{
			return _corner0;
		}

		set
		{
			_corner0 = value;
		}
	}

	public Vector2 Corner1
	{
		get
		{
			return _corner1;
		}

		set
		{
			_corner1 = value;
		}
	}
	/// <summary>
	/// scale of the map
	/// </summary>
	public Vector2 MapSize
	{
		get
		{
			return _mapSize;
		}

		set
		{
			_mapSize = value;
		}
	}

	internal void Generate()
	{
		MapSize = Corner1 - Corner0;
		// coordinate of the map on the heightmap texture
		_randMapOffset = UnityEngine.Random.insideUnitCircle * _randRange;
		float widthPerPoint = (Corner1.x - Corner0.x) / PointWidth;
		float heigthPerPoint = (Corner1.y - Corner0.y) / PointHeight;
		Points = new List<Vector2>();
		for (int y = 0; y < _pointHeight; y++)
		{
			for (int x = 0; x < _pointWidth; x++)
			{
				//populate the points
				Points.Add(new Vector2(x * widthPerPoint, y * heigthPerPoint) +
					UnityEngine.Random.insideUnitCircle * _offset);
			}
		}
	}

	private void OnValidate()
	{
		Generate();
	}

	/// <summary>
	/// returns the intensity of a simulated perlin noise texture at X, Y
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	float MapValidityPattern(float x, float y)
	{

		float xy = (Bezier((x-Corner0.x)/MapSize.x , (y-Corner0.y) / MapSize.y) * _lineWidth) * Mathf.Pow(Mathf.PerlinNoise(x+ _randMapOffset.x, y + _randMapOffset.y), _perlinPower);
		return Mathf.Sin(Mathf.Abs(Mathf.Sin(xy * 3.1415f)));
	}

	/// <summary>
	/// returns closeness of point X Y to the used bezier curve
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	float Bezier(float x, float y)
	{
		return (
			
			(1f-y) + y*(2*y*_mapFocus.y*(1f-_mapFocus.y))-
			((1f - x) * ((1 - x) + x * _mapFocus.x) + x * _mapFocus.x)
			);
	}

	/// <summary>
	/// is the point inside the bounds of the map
	/// </summary>
	/// <param name="vert"></param>
	/// <returns></returns>
	internal bool InBound(Vector2 vert)
	{
		return vert.x > Corner0.x && vert.x < Corner1.x && vert.y > Corner0.y && vert.y < Corner1.y;
	}

	void Update () {
		if (!_drawDebug)
			return;
		// draws black and white crosses on the randomly generated points
		foreach (var point in Points)
		{
			var color = new Color(MapValidityPattern(point.x, point.y), MapValidityPattern(point.x, point.y), MapValidityPattern(point.x, point.y));
			Debug.DrawLine(point - new Vector2(0.1f, 0.1f), point + new Vector2(0.1f, 0.1f), color);
			Debug.DrawLine(point - new Vector2(0.1f, -0.1f), point + new Vector2(0.1f, -0.1f), color);
		}
	}
}
