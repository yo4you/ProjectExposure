using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeTransverser : MonoBehaviour
{
	public delegate void OnPathResolveHandle();
	public event OnPathResolveHandle OnPathResolve;

	public event OnPathResolveHandle OnMove;

	[SerializeField]
	private float _maxLineSegment = 0.2f;
	[SerializeField]
	private float _speed;
	[SerializeField]
	private int _rotationSmoothing;
	private Queue<Node<Polygon>> _path = new Queue<Node<Polygon>>();
	internal Queue<Node<Polygon>> Path { get => _path; set => _path = value; }
	public Queue<Vector2> SmoothedPath { get; set; }
	public float PlayerZ { get => _playerZ; set => _playerZ = value; }

	private NodeGraphActor _actor;
	//[SerializeField]
	private float _playerZ;
	private VoronoiGenerator _generator;
	[SerializeField]
	private float _rotationSpeed;
	[SerializeField]
	private float _cornerSmoothing = 0.9f;

	//public float PlayerZ => _playerZ;

	private Quaternion _baseRotationRight;
	private Quaternion _baseRotationLeft;
	private Animator _animation;
	private Coroutine _resetRoutine;
	[SerializeField]
	private float _resetRotationSpeed = 5f;
	private bool _pathComplete = true;

	private void Start()
	{
		_baseRotationRight = transform.GetChild(0).transform.rotation;
		_generator = FindObjectOfType<VoronoiGenerator>();
		_actor = FindObjectOfType<NodeGraphActor>();
		_baseRotationLeft = _baseRotationRight * Quaternion.Euler(180, 0, 0);
		_animation = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
	}

	public void EngagePath()
	{
		_animation.SetBool("Swim", true);
		_animation.SetBool("Idle", false);

		if (_resetRoutine != null)
		{
			StopCoroutine(_resetRoutine);
		}

		OptimizePath();
		SmoothPath();
		//StartCoroutine(StartEngagePath());
		StartCoroutine(StartEngageSmoothPath());
		OnPathResolve += ResetRotation;
	}

	private void ResetRotation()
	{
		_resetRoutine = StartCoroutine(StartResetRotation());
	}

	private IEnumerator StartResetRotation()
	{
		yield return new WaitForSeconds(0.5f);
		var rotation = _baseRotationLeft;

		if (transform.GetChild(0).TransformVector(Vector3.up).x > 0)
		{
			rotation = _baseRotationRight;
		}

		var startRot = transform.GetChild(0).transform.rotation;
		for (float t = 0; t < 1f; t += Time.deltaTime * _resetRotationSpeed)
		{
			transform.GetChild(0).transform.rotation = Quaternion.Slerp(startRot, rotation, t);
			yield return new WaitForEndOfFrame();

		}
	}

	private void SmoothPath()
	{
		var pathlist = Path.ToList();
		SmoothedPath = new Queue<Vector2>();
		SmoothedPath.Enqueue(pathlist[0].Data.Centre);
		for (int i = 1; i < pathlist.Count - 1; i++)
		{
			if (!pathlist[i - 1].ConnectionAngles.Values.Contains(pathlist[i]))
			{
				_pathComplete = false;
				return;
			}
			var displacement0 = pathlist[i].Data.Centre - pathlist[i - 1].Data.Centre;
			SmoothedPath.Enqueue(pathlist[i - 1].Data.Centre + displacement0 * _cornerSmoothing);
			var displacement1 = pathlist[i + 1].Data.Centre - pathlist[i].Data.Centre;
			if (!pathlist[i].ConnectionAngles.Values.Contains(pathlist[i + 1]))
			{
				_pathComplete = false;
				return;
			}
			SmoothedPath.Enqueue(pathlist[i].Data.Centre + displacement1 * (1f - _cornerSmoothing));
		}
		SmoothedPath.Enqueue(pathlist[pathlist.Count - 1].Data.Centre);
		_pathComplete = true;
	}

	private void OptimizePath()
	{
		if (Path.Count == 0)
		{
			return;
		}

		Queue<Node<Polygon>> newPath = new Queue<Node<Polygon>>();
		Node<Polygon> lastNode = Path.Dequeue();
		newPath.Enqueue(lastNode);
		while (Path.Count != 0)
		{
			var next = Path.Dequeue();
			if (next.Data.Centre != lastNode.Data.Centre)
			{
				newPath.Enqueue(next);
				lastNode = next;
			}
		}
		Path = newPath;
	}

	private IEnumerator StartEngageSmoothPath()
	{
		if (SmoothedPath.Count == 0)
		{
			yield break;
		}

		var start = transform.position;
		var next = SmoothedPath.Dequeue();
		float t = 0f;

	start:
		float dist = Vector2.Distance(start, next);
		if (dist == 0)
		{
			if (SmoothedPath.Count == 0)
			{
				goto end;
			}

			start = next;
			next = SmoothedPath.Dequeue();
			goto start;
		}

		t += Time.deltaTime * (_speed / dist);

		var pos = Vector3.Lerp(start, next, t);
		pos.z = PlayerZ;
		transform.position = pos;

		//rotation
		Vector3 lookTarget = DirectionApproxiomate(transform.position, SmoothedPath.ToList());
		lookTarget.z = transform.position.z;

		var targetRotation = Quaternion.LookRotation(lookTarget, Vector3.forward);
		var directionVector = targetRotation * Vector3.forward;
		if (directionVector.x > 0)
		{
			var euler = targetRotation.eulerAngles;
			euler.x += 180f;
			euler.z = 180f - euler.z;
			targetRotation.eulerAngles = euler;
		}

		transform.GetChild(0).rotation = Quaternion.RotateTowards(transform.GetChild(0).rotation, targetRotation, Time.deltaTime * _rotationSpeed);

		yield return new WaitForEndOfFrame();
		if (t > 1f)
		{
			OnMove?.Invoke();
			if (SmoothedPath.Count == 0)
			{
				goto end;
			}

			t -= 1f;
			start = next;
			next = SmoothedPath.Dequeue();

		}
		goto start;

	end:
		OnPathResolve?.Invoke();
		_animation.SetBool("Swim", false);
		_animation.SetBool("Idle", true);

		if (!_pathComplete)
		{
			_animation.Play("Hit");
		}

		yield return null;
	}


	internal void RandomPath()
	{
		_actor = GetComponent<NodeGraphActor>();

		var start = _actor.CurrentNode;
		for (int i = 0; i < 10; i++)
		{
			_path.Enqueue(start);
			start = start.ConnectionAngles.Values.ToList()[UnityEngine.Random.Range(0, _actor.CurrentNode.ConnectionAngles.Values.Count - 1)];
		}
		EngagePath();
	}

	private bool IsClosest(Node<Polygon> node, Vector2 pos, List<Node<Polygon>> visited)
	{
		//var dist =(pos - node.Data.Centre).magnitude;
		var dist = Vector2.SqrMagnitude(pos - node.Data.Centre);
		foreach (var child in node.ConnectionAngles.Values.ToList())
		{
			if (visited.Contains(child))
			{
				continue;
			}

			if (child.Data.IsBackGround && dist > Vector2.SqrMagnitude(pos - child.Data.Centre))
			//if (child.Data.IsWall && dist > (pos - child.Data.Centre).magnitude)
			{
				return false;
			}
		}
		return true;
	}

	static internal Node<Polygon> ConnectedNodeAtAngle(Node<Polygon> root, float angle, List<Node<Polygon>> visited)
	{

		if (root.ConnectionAngles.Count == 0)
		{
			return null;
		}

		foreach (var item in root.ConnectionAngles)
		{
			if (!item.Value.Data.IsBackGround || Mathf.DeltaAngle(angle, item.Key) > 90 || visited.Contains(item.Value))
			{
				continue;
			}
			if (item.Key > angle)
			{
				return item.Value;
			}
		}
		return root.ConnectionAngles.First().Value;
	}

	public void LineToPath(List<Vector2> line)
	{
		Path = new Queue<Node<Polygon>>();
		Node<Polygon> current = _actor.CurrentNode;
		Vector2 pos = transform.position;
		var voronoi = FindObjectOfType<VoronoiGenerator>();
		for (int i = 0; i < line.Count; i++)
		{
			// subdivide this line if its too long TODO
			var linePoint = line[i];
			List<Vector2> subdivPoints = new List<Vector2>();
			if (i != 0)
			{
				float lineLength = Vector2.Distance(linePoint, line[i - 1]);
				//Debug.Log(lineLength);
				if (lineLength > _maxLineSegment)
				{
					var displacement = linePoint - line[i - 1];
					float linesegments = Mathf.Ceil(lineLength / _maxLineSegment);
					for (int lineseg = 1; lineseg < linesegments - 1; lineseg++)
					{
						var interpoint = line[i - 1] + lineseg * (displacement / linesegments);
						subdivPoints.Add(interpoint);
					}
				}
			}
			subdivPoints.Add(linePoint);

			foreach (var subdibpoint in subdivPoints)
			{
				var next = voronoi.Polygons.Aggregate((min, nextItem) =>
				Vector2.SqrMagnitude(subdibpoint - min.Centre) < Vector2.SqrMagnitude(subdibpoint - nextItem.Centre) || !nextItem.IsBackGround ? min : nextItem).Node;
				Path.Enqueue(next);
			}

		}
	}

	private bool GeneratePath(Node<Polygon> current, Node<Polygon> next, out List<Node<Polygon>> path)
	{

		path = new List<Node<Polygon>>();
		if (current == next)
		{
			return true;
		}
		if (current.ConnectionAngles.Values.Contains(next))
		{
			path.Add(next);
			return true;
		}
		var angle = (Mathf.Atan2(next.Data.Centre.y - current.Data.Centre.y, next.Data.Centre.x - current.Data.Centre.y) * Mathf.Rad2Deg + 330f) % 360;
		VoronoiGenerator.DrawNodeGraphLine(current, angle, ref path);
		if (path.Contains(next))
		{
			path = path.GetRange(0, path.IndexOf(next));
			return true;
		}
		return false;
	}

	private Vector2 DirectionApproxiomate(Vector2 pos, List<Node<Polygon>> directions)
	{
		Vector2 outp = new Vector2();
		if (directions.Count == 0)
		{
			return outp.normalized;
		}

		int dirCount = Math.Min(_rotationSmoothing, directions.Count);
		float dist = 0;
		for (int i = 0; i < dirCount; i++)
		{
			//outp += (directions[i] - (Vector2)transform.position).normalized * Mathf.Pow(_angleApproximationBase,i);

			dist += Vector2.Distance(directions[i].Data.Centre, transform.position);
			outp += (directions[i].Data.Centre - (Vector2)transform.position) / dist;
		}
		return outp.normalized;
	}

	private Vector2 DirectionApproxiomate(Vector2 pos, List<Vector2> directions)
	{
		Vector2 outp = new Vector2();
		if (directions.Count == 0)
		{
			return outp.normalized;
		}

		int dirCount = Math.Min(_rotationSmoothing, directions.Count);
		float dist = 0;
		for (int i = 0; i < dirCount; i++)
		{
			//outp += (directions[i] - (Vector2)transform.position).normalized * Mathf.Pow(_angleApproximationBase,i);

			dist += Vector2.Distance(directions[i], transform.position);
			outp += (directions[i] - (Vector2)transform.position) / dist;
		}
		return outp.normalized;
	}

}
