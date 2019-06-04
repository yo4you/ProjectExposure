using System.Collections.Generic;

internal class MeshVertexIdentifiers
{

	public const int INNER_EDGE_OFFSET = 100;
	public enum MeshVertex
	{
		CENTRE,
		TOP_EDGE,
		INNER_EDGE0 = INNER_EDGE_OFFSET * 1,
		INNER_EDGE1 = INNER_EDGE_OFFSET * 2,
		INNER_EDGE2 = INNER_EDGE_OFFSET * 3,
		BOTTOM_EDGE = 1000
	}

	private List<MeshVertex> _identifiers = new List<MeshVertex>();

	internal List<MeshVertex> Identifiers
	{
		get
		{
			return _identifiers;
		}

		set
		{
			_identifiers = value;
		}
	}
}