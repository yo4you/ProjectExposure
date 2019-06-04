using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaryCentricTest : MonoBehaviour {

	[SerializeField]
	int _sides;
	Polygon _poly;
	Polygon _poly2;
	Polygon _polyInside;
	Polygon _polyInside2;

	void Start () {
		
	}


	private void OnValidate()
	{
		var seq = RandomBaryCentricSequence(3);
		Debug.Log(seq[0] + seq[1] + seq[2]);

		_poly = new Polygon();
		_poly2 = new Polygon();

		for (int i = 0; i < _sides; i++)
		{
			_poly.Vertices.Add(UnityEngine.Random.insideUnitCircle);
			_poly2.Vertices.Add(new Vector2(10,0)+ UnityEngine.Random.insideUnitCircle);
		}

		_poly.Order();
		_poly2.Order();

		_polyInside = new Polygon();
		for (int i = 0; i < _sides; i++)
		{
			
			var barycentrics = RandomBaryCentricSequence(_sides);
			Vector2 vert = BaryToVec(_poly, barycentrics);
			_polyInside.AddVertex(vert);
		}
		_polyInside.Order();


// 		_polyInside2 = new Polygon();
// 		for (int i = 0; i < _sides; i++)
// 		{
// 			var barycentrics = RandomBaryCentricSequence(_sides);
// 			Vector2 vert = BaryToVec(_poly2, barycentrics);
// 			_polyInside2.AddVertex(vert);
// 		}
// 		_polyInside2.Order();

	}

	private Vector2 BaryToVec(Polygon poly, float[] barycentrics)
	{
		Vector2 vert = new Vector2();
		for (int side = 0; side < _poly.Vertices.Count; side++)
		{
			vert += _poly.Vertices[side] * barycentrics[side];
		}
		return vert;
	}

	static float[] RandomBaryCentricSequence(int count)
	{
		var outp = new float[count];
		List<float> seq = new List<float>();
		seq.Add(0f);
		for (int i = 0; i < count-1; i++)
		{
			seq.Add(UnityEngine.Random.Range(0, 1f));
		}
		seq.Add(1f);
		for (int i = 0; i < count; i++)
		{
			outp[i] = seq[i + 1] - seq[i]; 
		}
		return outp;
	}

	void Update () {

		_poly.Draw();
		_poly2.Draw();
		_polyInside.Draw();
	}
}
