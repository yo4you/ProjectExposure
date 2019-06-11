using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPathFollower : MonoBehaviour
{
	private enum CollisionType
	{
		NONE,
		BOTTOM,
		TOP,
		BOTH
	}

	public delegate void OnPathResolveHandle();
	public event OnPathResolveHandle OnPathResolve;
	[SerializeField]
	private float _speed;
	private Queue<Vector2> _reversePath = new Queue<Vector2>();
	[SerializeField]
	private LayerMask _layerMask;
	[SerializeField]
	private float _playerZ;
	private bool _colliding = false;
	private bool _stopNextStep;
	private bool _reversing = false;
	[SerializeField]
	private float _ignorePathMargin = 0.1f;
	[SerializeField]
	private int _rotationSmoothing;
	private Quaternion _baseRotation;
	[SerializeField]
	private float _angleApproximationBase = 0.8f;
	[SerializeField]
	private float _rotationSpeed = 10f;
	[SerializeField]
	private float _manoeuvreAngle = 45f;
	private float _colliderRadius;
	[SerializeField]
	private float _avoidanceDistance;
	private int _pathParts;

	public float PlayerZ => _playerZ;

	private void Start()
	{
		//_colliderRadius = GetComponent<SphereCollider>().radius;
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (_colliding)
		{
			return;
		}

		_colliding = true;
		//Debug.Log("collisionEnter");
		StopAllCoroutines();
		// 		var shortReversePath = new Queue<Vector2>();
		// 		for (int i = 0; i < _reverseSteps; i++)
		// 		{
		// 			shortReversePath.Enqueue(_reversePath.Dequeue());
		// 		}
		// 		_reversePath.Dequeue();
		_reversing = true;
		EngagePath(new Queue<Vector2>(_reversePath.Reverse()));
	}

	private void OnCollisionExit(Collision collision)
	{
		//	Debug.Log("collisionExit");

		_colliding = false;
		//StopAllCoroutines();
		StopNextStep();

		//OnPathResolve?.Invoke();
	}

	private void StopNextStep()
	{
		_stopNextStep = true;
	}

	internal void EngagePath(Queue<Vector2> path)
	{
		GetComponent<NodeTransverser>().LineToPath(path.ToList());
		GetComponent<NodeTransverser>().EngagePath();
		//StartCoroutine(StartPath(path));

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

	private IEnumerator StartPath(Queue<Vector2> path)
	{
		if (path.Count == 0)
		{
			yield break;
		}
		_reversePath = new Queue<Vector2>();
		Vector2 start = transform.position;
		float frameTime = Time.deltaTime;
		_reversePath.Enqueue(path.Dequeue());
		_reversePath.Enqueue(path.Dequeue());
		while (path.Count > 1 && Vector2.Distance(transform.position, path.Peek()) < _ignorePathMargin)
		{
			_reversePath.Enqueue(path.Dequeue());
		}
		if (path.Count == 0)
		{
			OnPathResolve?.Invoke();
			yield break;
		}
		while (path.Count > 0)
		{
			if (_stopNextStep && _reversing)
			{
				break;
			}
			_stopNextStep = false;
			float timeTillCurrentFrame = Time.deltaTime;
		start:
			float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), path.Peek());
			if (dist < timeTillCurrentFrame * _speed)
			{
				timeTillCurrentFrame -= dist / _speed;
				_reversePath.Enqueue(path.Peek());

				Vector3 target = path.Dequeue();
				target.z = transform.position.z;
				_baseRotation = transform.GetChild(0).rotation;
				target.z = transform.position.z;
				transform.position = target;

				if (path.Count != 0)
				{
					goto start;
				}
			}
			else
			{
				// 				Debug.DrawLine(
				// 					transform.position,
				// 					(Vector2)transform.position + DirectionApproxiomate(transform.position, path.ToList()) * 1000000f,
				// 					Color.red);

				Vector3 target = path.Peek();
				target.z = transform.position.z;

				//avoidanceDistance = 2f;
				Vector2 futureTarget = transform.position;
				float storeAvoidance = _avoidanceDistance;
				var pathlist = path.ToList();
				_pathParts = 0;
				for (int i = 0; i < pathlist.Count && _avoidanceDistance > 0; i++)
				{
					_avoidanceDistance -= Vector2.Distance(futureTarget, pathlist[i]);
					futureTarget += pathlist[i] - futureTarget;
					_pathParts++;
				}
				_avoidanceDistance = storeAvoidance;
				Debug.DrawLine(transform.position, new Vector3(futureTarget.x, futureTarget.y, transform.position.z), Color.cyan, 1f);

				var rayCollision = RayCheck(futureTarget);
				if (rayCollision != CollisionType.NONE)
				{
					RecalcPath(ref path, futureTarget, rayCollision);
				}


				var pos = (Vector2)transform.position + ((path.Peek() - (Vector2)transform.position).normalized * timeTillCurrentFrame * _speed);
				Vector3 lookTarget = DirectionApproxiomate(transform.position, path.ToList());
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

				transform.position = new Vector3(pos.x, pos.y, transform.position.z);
				yield return null;
			}
		}
		_reversing = false;
		OnPathResolve?.Invoke();

	}
// 
// 	private void Update()
// 	{
// 		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
// 		Physics.Raycast(ray, out RaycastHit hit);
// 		var mousepos = hit.point;
// 		mousepos.z = transform.position.z;
// 
// 		var angle = Mathf.Atan2(mousepos.y - transform.position.y, mousepos.x - transform.position.x) * Mathf.Rad2Deg + 180f;
// 		Debug.Log(angle);
// 		Debug.DrawRay(transform.position, mousepos - transform.position,
// 			Physics.Raycast(transform.position, mousepos - transform.position, Vector3.Distance(mousepos, transform.position)) ? Color.magenta : Color.blue);
// 	}

	private CollisionType RayCheck(Vector3 target)
	{
		target.z = transform.position.z;
		var forward = ((Vector2)(transform.GetChild(0).worldToLocalMatrix * (Vector3.up))).normalized;
		var perp = new Vector3(-forward.y, forward.x, 0) * _colliderRadius * 0.5f;
		Vector3 colliderTop = transform.position + perp;
		Vector3 colliderBottom = transform.position - perp;

		var dist = Vector3.Distance(transform.position, target);

		Debug.DrawRay(colliderTop, target - transform.position, Physics.Raycast(colliderTop, (target + perp) - transform.position, _layerMask) ? Color.red : Color.green, 1);

		Debug.DrawRay(colliderBottom, target - transform.position, Physics.Raycast(colliderBottom, (target - perp) - transform.position, _layerMask) ? Color.red : Color.green, 1);

		int outp = 0;
		if (Physics.Raycast(colliderTop, (target) - transform.position, _layerMask))
		{
			outp += (int)CollisionType.BOTTOM;
		}
		if(Physics.Raycast(colliderBottom, (target) - transform.position, _layerMask))
		{
			outp += (int)CollisionType.TOP;
		}

		return (CollisionType)outp;
			
	}

	private void RecalcPath(ref Queue<Vector2> path, Vector3 target, CollisionType collisionType)
	{
		Debug.Log("boop");
		var pathList = path.ToList();
		//Vector3 target = pathList[0];
		target.z = transform.position.z;
		var dist = target - transform.position;
		var angle = _manoeuvreAngle;

		if (collisionType == CollisionType.TOP)
		{
			angle *= -1f;
		}
		if (collisionType != CollisionType.BOTH)
		{
			var avoidTry = Vector2Extension.Rotate(dist * 1f, angle);
			if (RayCheck(transform.position + (Vector3)avoidTry) == CollisionType.NONE)
			{
				for (int i = 0; i < _pathParts && pathList.Count !=0; i++)
				{
					pathList.RemoveAt(0);

				}
				pathList.Insert(0, (Vector2)transform.position + avoidTry);
				Debug.DrawLine(transform.position, transform.position + (Vector3)avoidTry, Color.yellow, 2f);
			}
		}
		

		// 		if (RayCheck(transform.position + (Vector3)avoidTry) == CollisionType.NONE)
		// 		{
		// 			Debug.DrawLine(transform.position, transform.position + (Vector3)avoidTry, Color.yellow, 2f);
		// 
		// 			pathList.Insert(0, (Vector2)transform.position + avoidTry);
		// 		}
		// 		else
		// 		{
		// 			avoidTry = Vector2Extension.Rotate(dist * 0.5f, -_manoeuvreAngle);
		// 
		// 			if (RayCheck(transform.position + (Vector3)avoidTry) == CollisionType.NONE)
		// 			{
		// 				Debug.DrawLine(transform.position, transform.position + (Vector3)avoidTry, Color.yellow, 2f);
		// 
		// 				pathList.Insert(0, (Vector2)transform.position + avoidTry);
		// 			}
		// 		}


		path = new Queue<Vector2>(pathList);
	}
}
