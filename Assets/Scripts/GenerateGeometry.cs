using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MeshVertexIdentifiers;

#if UNITY_EDITOR
using UnityEditor;

#endif

[RequireComponent(typeof(VoronoiGenerator))]
public partial class GenerateGeometry : MonoBehaviour
{
	
	[SerializeField]
	private List<MultiObject> _backGroundGeometry;
	[SerializeField]
	private List<MultiObject> _foreGroundGeometry;
	private Dictionary<Mesh, MeshVertexIdentifiers> _meshData = new Dictionary<Mesh, MeshVertexIdentifiers>();
	private VoronoiGenerator _generator;
	[SerializeField]
	private int _detailLevels;
	private Vector2 _randMapOffset;
	[SerializeField]
	private Vector2 _mapFocus;
	[SerializeField]
	private float _randRange = 2.0f;
	[SerializeField]
	private float _lineWidth;
	[SerializeField]
	private float _perlinPower;
	private Vector3 _corner0;
	private Vector3 _corner1;
	private Vector3 _mapSize;
	[SerializeField]
	private float _heightMapIntensity;
	[SerializeField]
	private float _polyHeightIntensity;
	[SerializeField]
	[Range(0f, 1f)]
	private float _cutoff;
	[SerializeField]
	private float _mapScale;
	[SerializeField]
	private bool _save = false;
	[SerializeField]
	private float _wallHeight;
	[SerializeField]
	private Material _material;
	[SerializeField]
	private int _chunkSize = 10;
	private bool _generatedVertexOrder = false;
	[SerializeField]
	private float _innerEdgeHeigthDiffrence = 0.8f;
	[SerializeField]
	bool _divideintoChunks =  true;

	AssignBiomes _biomes;

	private void Start()
	{
		_biomes = FindObjectOfType<AssignBiomes>();
		FindObjectOfType<VoronoiGenerator>().OnGenerationComplete += Generate;
		if (!_generatedVertexOrder)
		{
			GenerateVertexOrder();
		}
	}

	private void GenerateVertexOrder()
	{
		AssignVertexOrderByColor(_foreGroundGeometry);
		AssignVertexOrderByColor(_backGroundGeometry);
		_corner0 = GetComponent<RandomPointGenerator>().Corner0;
		_corner1 = GetComponent<RandomPointGenerator>().Corner1;
		_mapSize = GetComponent<RandomPointGenerator>().MapSize;
	}

	private void Save()
	{

#if UNITY_EDITOR
		for (int i = 0; i < transform.childCount; i++)
		{
			var mesh = transform.GetChild(i).GetComponent<MeshFilter>().mesh;
			MeshUtility.Optimize(mesh);
			AssetDatabase.CreateAsset(mesh, "Assets/generatedLevel" + DateTime.UtcNow.ToUniversalTime().ToString("yyyyMMddTHHmmss_x" + i.ToString()));
			AssetDatabase.SaveAssets();
		}
#endif
	}

	private float MapHeigthPattern(float x, float y)
	{

		float xy = (Bezier((x - _corner0.x) / _mapSize.x, (y - _corner0.y) / _mapSize.y) * _lineWidth) * Mathf.Pow(Mathf.PerlinNoise(_mapScale * x + _randMapOffset.x, _mapScale * y + _randMapOffset.y), _perlinPower);
		//float xy = (Bezier((x - _corner0.x) / _mapSize.x, (y - _corner0.y) / _mapSize.y) * _lineWidth) * Mathf.Pow(Mathf.PerlinNoise(x + _randMapOffset.x, y + _randMapOffset.y), _perlinPower);

		return Mathf.Sin(Mathf.Abs(Mathf.Sin(xy * 3.1415f)));
	}

	private float Bezier(float x, float y)
	{
		return (

			(1f - y) + y * (2 * y * _mapFocus.y * (1f - _mapFocus.y)) -
			((1f - x) * ((1 - x) + x * _mapFocus.x) + x * _mapFocus.x)
			);
	}

	private void OnValidate()
	{
		if (_save)
		{
			_save = false;
			Save();

		}

		foreach (Transform item in transform)
		{
			Destroy(item.gameObject);
		}
		Generate();
	}

	internal GameObject GenerateMesh(List<Polygon> polygons)
	{

		GameObject go = new GameObject();

		var meshcomp = go.AddComponent<MeshFilter>();
		var mesh = new Mesh();
		meshcomp.mesh = mesh;
		go.AddComponent<MeshRenderer>().material = _material;
		List<Vector3> meshVerts = new List<Vector3>(65000);
		List<int> meshTris = new List<int>(196608);
		List<Vector2> meshUvs = new List<Vector2>(65000);

		foreach (var poly in polygons)
		{
			MeshFilter geometry;

			if (poly.IsBackGround)
			{
				poly.IsBackGround = !IsBackGround(poly.Centre);
			}
			if (!poly.IsBackGround)
			{
				geometry = _foreGroundGeometry[poly.Vertices.Count - 3].RandomObject().GetComponent<MeshFilter>();
			}
			else
			{
				//poly.IsWall = true;

				geometry = _backGroundGeometry[poly.Vertices.Count - 3].RandomObject().GetComponent<MeshFilter>();
			}

			List<MeshVertex> vertIdentities = _meshData[geometry.sharedMesh].Identifiers;
			int vertCount = geometry.sharedMesh.vertices.Length;
			var newVerts = new List<Vector3>(vertCount);
			var newUVs = new List<Vector2>(vertCount);
			var uvIdentities = _meshData[geometry.sharedMesh].UVIdentifiers;
			for (int index = 0; index < vertCount; index++)
			{
				Vector3 newPos = new Vector3();
				var identity = vertIdentities[index];
				var uv = geometry.sharedMesh.uv[index];
				uv.x /= 3f;
 				var biome = (int)poly.Biome;
				if (biome > _biomes.BiomeSets.Count - 1 || biome < 0)
					Debug.Log(biome);
				
				uv += Vector2.right * _biomes.BiomeSets[biome].Uv /3f;
				

				newUVs.Add(uv);
				//newUVs.Add(uvIdentities[(int)identity]);

				if (identity >= MeshVertex.BOTTOM_EDGE)
				{
					newPos = poly.Vertices[identity - MeshVertex.BOTTOM_EDGE] - poly.Centre;
					newPos.z = _heightMapIntensity * MapHeigthPattern(poly.Centre.x + newPos.x, poly.Centre.y + newPos.y) + _wallHeight;

				}
				else if (identity >= MeshVertex.INNER_EDGE2)
				{
					;
				}
				else if (identity >= MeshVertex.INNER_EDGE1)
				{
					;
				}
				else if (identity >= MeshVertex.INNER_EDGE0)
				{
					newPos = (poly.Centre + poly.Vertices[identity - MeshVertex.INNER_EDGE0]) * 0.5f - poly.Centre;
					newPos.z -= _polyHeightIntensity * 1f;

				}
				else if (identity >= MeshVertex.TOP_EDGE)
				{
					newPos = poly.Vertices[identity - MeshVertex.TOP_EDGE] - poly.Centre;
					newPos.z -= _polyHeightIntensity * _innerEdgeHeigthDiffrence;

				}
				else if (identity == MeshVertex.CENTRE)
				{
					newPos.z -= _polyHeightIntensity;

				}
				newPos.z -= _heightMapIntensity * MapHeigthPattern(poly.Centre.x + newPos.x, poly.Centre.y + newPos.y);
				newPos.z -= poly.IsBackGround ? 0 : _polyHeightIntensity;

				newVerts.Add(newPos + (Vector3)poly.Centre);

			}
			int totalVertCount = meshVerts.Count;
			meshVerts.AddRange(newVerts);
			meshUvs.AddRange(newUVs);
			meshTris.AddRange(geometry.sharedMesh.triangles.Select(i => i += totalVertCount));

		}
		mesh.SetVertices(meshVerts);
		mesh.SetTriangles(meshTris, 0);
		mesh.RecalculateNormals();
		mesh.SetUVs(0, meshUvs);

		return go;
	}

	public void Generate()
	{
		if (!_generatedVertexOrder)
		{
			GenerateVertexOrder();
		}
		_generator = GetComponent<VoronoiGenerator>();

		if (_generator == null || _generator.Polygons.Count == 0)
		{
			return;
		}
		// 3 is the least amount of sides for a polygon
		var polies = _generator.Polygons.Where((p) => p.Vertices.Count >= 3).ToList();

		var corner0 = (polies.First().Centre);
		var corner1 = (polies.Last().Centre);
		if (!_divideintoChunks)
		{
			var go = GenerateMesh(polies);
			go.transform.SetParent(transform);
			return;
		}

		List<Bound> bounds = new List<Bound>() {
		//	new Bound(corner0 - new Vector2(2f, 2f), corner1 + new Vector2(2, 2))
			new Bound(corner0 - new Vector2(0.2f, 0.2f), corner1 + new Vector2(0.2f, 0.2f))
		};
		int power = Mathf.CeilToInt(Mathf.Log(polies.Count / _chunkSize, 4f));
		if (polies.Count < _chunkSize)
		{
			power = 0;
		}

		for (int i = 0; i < power; i++)
		{
			// this is the snapshot the count
			int boundCount = bounds.Count;
			var newBounds = new List<Bound>();
			//for (int bound = 0; bound < boundCount; bound++)
			for (int bound = boundCount - 1; bound >= 0; bound--)
			{
				newBounds.AddRange(bounds[bound].Subdivide());
				bounds.RemoveAt(bound);
			}
			bounds.AddRange(newBounds);
		}
		foreach (var bound in bounds)
		{
			List<Polygon> poliesInChunk = new List<Polygon>();

			for (int poly = polies.Count - 1; poly >= 0; poly--)
			{
				if (bound.Contains(polies[poly].Centre))
				{
					poliesInChunk.Add(polies[poly]);
					polies.RemoveAt(poly);

				}
			}
			var go = GenerateMesh(poliesInChunk);
			go.transform.SetParent(transform);
		}
	}
	/// <summary>
	/// registers dictionary definitions for all meshes in geometries, stores the "defenition" of all
	/// contained vertex indices 
	/// </summary>
	/// <param name="geometries"></param>
	private void AssignVertexOrderByColor(List<MultiObject> geometries)
	{
		// simplest prism has 3 sides
		for (int sides = 3; sides < geometries.Count + 3; sides++)
		{
			foreach (var geometry in geometries[sides - 3].objects)
			{
				var mesh = geometry.GetComponent<MeshFilter>().sharedMesh;
				// we define the centre of the polygon as red, this is important as the first inner edge is connected to the centre
				var centre = FindColor(mesh, Color.red);
				var indices = mesh.GetIndices(0);

				// Vertex paint defines vertex position, red = centre, green = inner edge, blue = outer edge
				var innerEdgeIndices = FindColor(mesh, Color.green);
				var outerEdgeIndices = FindColor(mesh, Color.blue);

			/*	Debug.Log($"sides: {sides + 3} vertices: {mesh.vertexCount} indices : {indices.Length} colors: {mesh.colors.Length} innerEdge:{innerEdgeIndices.Count} outeredge:{outerEdgeIndices.Count} ");*/
				// triangles connected to the centre
				List<Tri> innerTris = new List<Tri>();
				for (int i = 0; i < indices.Length; i += 3)
				{
					innerTris.Add(new Tri(indices[i], indices[i + 1], indices[i + 2]));
				}
				// inner triangles share one index with the centre
				innerTris = innerTris.FindAll((tri) => tri.Indices.Intersect(centre).Count() != 0);
				// triangles connected to the inner triangles
				List<Tri> outerTris = new List<Tri>();
				for (int i = 0; i < indices.Length; i += 3)
				{
					outerTris.Add(new Tri(indices[i], indices[i + 1], indices[i + 2]));
				}
				// outer edge triangles share one index with the outer edge vertices
				outerTris = outerTris.FindAll((tri) => tri.Indices.Intersect(outerEdgeIndices).Count() != 0);

				// the order in which vertices are connected around the centre
				List<int> innerEdgeIndexOrder = new List<int>()
				{
					innerTris[0].Indices.First((i) => !centre.Contains(i)),
					innerTris[0].Indices.Last((i) => !centre.Contains(i))
				};
				List<int> edge = new List<int>();
				innerTris.Remove(innerTris[0]);

				// we determine the indices that make up the inner edge of the polygon
				while (innerEdgeIndexOrder.Count < sides)
				{
					// must be connected sequentially so we take the last index
					int toConnect = innerEdgeIndexOrder.Last();
					// then find a triangle that shares the last index, as well as the centre index
					int triIndex = innerTris.FindIndex(
						(tri) => tri.Indices.Any((v) => mesh.vertices[v] == mesh.vertices[toConnect]));
					innerEdgeIndexOrder.Add(innerTris[triIndex].Indices.First(
						(v) => !centre.Contains(v) &&
						mesh.vertices[v] != mesh.vertices[toConnect]));
					// remove the tris and keep looking until we fill the sides
					innerTris.RemoveAt(triIndex);
				}

				// determine the outer edge indices 
				for (int outerEdgeIndex = 0; outerEdgeIndex < innerEdgeIndexOrder.Count; outerEdgeIndex++)
				{
					// parse trough the inner edge and get all the triangles that connect to each point on the edge, there should be 3
					var tris = outerTris.Where(
					   (tri) => tri.Indices.Any((ind) => mesh.vertices[ind] == mesh.vertices[innerEdgeIndexOrder[outerEdgeIndex]])
				   ).ToList();

					// the first triangles has 2 connections to the outer edge and one to the inner edge
					Tri tri0 = tris.First((tri) => tri.Indices.Intersect(outerEdgeIndices).Count() == 2);
					// the next triangle is connected to the next vertex on the inner edge and should have one vertex on the outer edge 
					Tri tri1 = tris.First((tri) => tri.Indices.Any(
						(ind) => mesh.vertices[ind] == mesh.vertices[innerEdgeIndexOrder[(outerEdgeIndex + 1) % innerEdgeIndexOrder.Count]]));
					// the index that is contained in the first and not the second triangle is the one outer edge equivilant of the inner edge triangle
					edge.Add(tri0.Indices.Except(tri1.Indices).First());

				}
				innerEdgeIndexOrder.Insert(0, innerEdgeIndexOrder.Last());
				innerEdgeIndexOrder = innerEdgeIndexOrder.GetRange(0, innerEdgeIndexOrder.Count - 1);

				// assign the correct vertex definitions
				var vertData = new MeshVertexIdentifiers();
				for (int i = 0; i < mesh.vertexCount; i++)
				{
					vertData.Identifiers.Add(MeshVertex.CENTRE);
					//vertData.UVIdentifiers.Add(Vector2.zero);
				}

				Vector2[] uv  = mesh.uv;
				List<int> outp = new List<int>();
				for (int index = 0; index < mesh.vertices.Length; index++)
				{
					if (centre.Contains(index))
					{
						vertData.Identifiers[index] = (MeshVertex.CENTRE);
						vertData.UVIdentifiers[(int)MeshVertex.CENTRE] = mesh.uv[centre.First()];
					}

					for (int edgeIndex = 0; edgeIndex < edge.Count(); edgeIndex++)
					{
						if (mesh.vertices[edge[edgeIndex]] == mesh.vertices[index])
						{
							vertData.Identifiers[index] = MeshVertex.TOP_EDGE + edgeIndex;
							vertData.UVIdentifiers[(int)MeshVertex.TOP_EDGE + edgeIndex] = mesh.uv[edge[edgeIndex]];

						}
						else if (Mathf.Approximately(mesh.vertices[edge[edgeIndex]].x, mesh.vertices[index].x) && Mathf.Approximately(mesh.vertices[edge[edgeIndex]].z, mesh.vertices[index].z))
						{
							vertData.Identifiers[index] = MeshVertex.BOTTOM_EDGE + edgeIndex;
							vertData.UVIdentifiers[(int)MeshVertex.BOTTOM_EDGE + edgeIndex] = mesh.uv[index];
						}
					}
					for (int edgeIndex = 0; edgeIndex < innerEdgeIndexOrder.Count(); edgeIndex++)
					{
						if (mesh.vertices[innerEdgeIndexOrder[edgeIndex]] == mesh.vertices[index])
						{
							vertData.Identifiers[index] = MeshVertex.INNER_EDGE0 + edgeIndex;
							vertData.UVIdentifiers[(int)MeshVertex.INNER_EDGE0 + edgeIndex] = mesh.uv[innerEdgeIndexOrder[edgeIndex]];

						}
					}
				}

				if (!_meshData.ContainsKey(geometry.GetComponent<MeshFilter>().sharedMesh))
				{
					_meshData.Add(geometry.GetComponent<MeshFilter>().sharedMesh, vertData);
				}
			}
		}
	}

	/// <summary>
	/// returns a list of vertices that match the vertexcolor color
	/// </summary>
	/// <param name="mesh"></param>
	/// <param name="color"></param>
	/// <returns></returns>
	private List<int> FindColor(Mesh mesh, Color color)
	{
		var outp = new List<int>();
		for (int i = 0; i < mesh.colors.Length; i++)
		{
			if (mesh.colors[i] == color)
			{
				outp.Add(i);
			}
		}
		return outp;
	}

	/// <summary>
	/// is this polygon part of the background geometry or foreground
	/// </summary>
	/// <param name="centre"></param>
	/// <returns></returns>
	private bool IsBackGround(Vector2 centre)
	{
		return _cutoff < MapHeigthPattern(centre.x, centre.y);
	}

	private float[] GetBaryCentricCoords(List<Vector2> poly, Vector2 pos)
	{
		//M. S. Floater, Mean value coordinates, CAGD 20 (2003), 19–27.

		Func<int, float> alphaI = (i) =>
			Vector2.Angle(poly[i % poly.Count()] - pos, poly[i + 1 % poly.Count()] - pos) * Mathf.Deg2Rad;
		float W = 0;

		Func<int, Vector2, float> wi = (i, x) =>
		{
			return (1f / Vector2.Distance(x, poly[i])) *
			Mathf.Tan(alphaI((int)Mathf.Repeat(i - 1, poly.Count())) / 2f) +
			Mathf.Tan(alphaI(i) / 2f);
		};

		for (int i = 0; i < poly.Count; i++)
		{
			W += wi(i, pos);
		}

		var outp = new float[poly.Count];

		for (int index = 0; index < poly.Count; index++)
		{
			outp[index] = wi(index, pos) / W;
		}

		return outp;
	}

}

