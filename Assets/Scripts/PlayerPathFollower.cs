using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPathFollower : MonoBehaviour
{


	public delegate void OnPathResolveHandle();
	public event OnPathResolveHandle OnPathResolve;
	[SerializeField]
	private float _speed;
	private Queue<Vector2> _reversePath = new Queue<Vector2>();
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

	private void Start()
	{

	}
	private void OnCollisionEnter(Collision collision)
	{
		if (_colliding)
		{
			return;
		}

		_colliding = true;
		//Debug.Log("stop");
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
		StartCoroutine(StartPath(path));

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
		//Debug.Log("start");
		_reversePath = new Queue<Vector2>();
		Vector2 start = transform.position;
		float frameTime = Time.deltaTime;
		//path.Dequeue();
		_reversePath.Enqueue(path.Dequeue());
		_reversePath.Enqueue(path.Dequeue());
		//_reversePath.Enqueue(path.Dequeue());
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
			//Vector3.RotateTowards(transform.loo)
			float dist = Vector2.Distance(new Vector2(transform.position.x, transform.position.y), path.Peek());
			if (dist < timeTillCurrentFrame * _speed)
			{
				timeTillCurrentFrame -= dist / _speed;
				_reversePath.Enqueue(path.Peek());

				Vector3 target = path.Dequeue();
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
				Debug.DrawLine(
					transform.position,
					(Vector2)transform.position + DirectionApproxiomate(transform.position, path.ToList()) * 1000000f,
					Color.red);


				var pos = (Vector2)transform.position + ((path.Peek() - (Vector2)transform.position).normalized * timeTillCurrentFrame * _speed);
				Vector3 lookTarget = DirectionApproxiomate(transform.position, path.ToList());
				lookTarget.z = transform.position.z;
			
				var targetRotation = Quaternion.LookRotation(lookTarget, Vector3.forward);
				var directionVector = targetRotation * Vector3.forward;
				if(directionVector.x > 0)
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

}
