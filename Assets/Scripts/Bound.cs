using UnityEngine;



public partial class GenerateGeometry
{
	/// <summary>
	/// 2D square bounds used for subdividing terrain 
	/// </summary>
	private class Bound
	{
		private Vector2 _corner0;
		private Vector2 _corner1;

		public Bound(Vector2 corner0, Vector2 corner1)
		{
			_corner0 = corner0;
			_corner1 = corner1;
		}

		/// <summary>
		/// does the point fall between the corners of the bound
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public bool Contains(Vector3 pos)
		{
			return 
				pos.x > _corner0.x && 
				pos.y > _corner0.y && 
				pos.x < _corner1.x && 
				pos.y < _corner1.y;
		}
		/// <summary>
		/// returns 4 smaller bounds, bottomleft-centre, center-topright, leftcentre-topcentre, bottomcentre-rightcentre
		/// </summary>
		/// <returns></returns>
		public Bound[] Subdivide()
		{
			Vector3 centre = (_corner0 + _corner1) * 0.5f;
			return new Bound[4]
			{
				new Bound(_corner0,centre),
				new Bound(centre,_corner1),
				new Bound(new Vector2(_corner0.x,centre.y),new Vector2(centre.x,_corner1.y)),
				new Bound(new Vector2(centre.x,_corner0.y),new Vector2(_corner1.x,centre.y))
			};
		}

	}

}

