using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CanvasMouseTracker : MonoBehaviour {

	ControlStates _controlState = ControlStates.START;
	PathDrawer _pathDrawer;
	NodeTransverser _pathFollower;
	Queue<Vector2> _path = new Queue<Vector2>();
	[SerializeField]
	LayerMask _layerMask;

	private ControlStates ControlState
	{
		get
		{
			return _controlState;
		}

		set
		{
			_controlState = value;
		}
	}

	enum ControlStates
	{
		START,
		DRAGGING,
		MOVING
	}

	void Start ()
	{
		_pathDrawer = FindObjectOfType<PathDrawer>();
	}
	
	void Update ()
	{
		switch (ControlState)
		{
			case ControlStates.START:
				CheckClick();
				break;
			case ControlStates.DRAGGING:
				Drag();
				break;
			case ControlStates.MOVING:
				break;
			default:
				break;
		}

		
	}


	private void Reset()
	{
		if (_pathFollower)
			_pathFollower.OnPathResolve -= Reset;
		ControlState = ControlStates.START;
	}

	private void Drag()
	{
		if (Input.GetMouseButtonUp(0))
		{
			ControlState = ControlStates.MOVING;

			_pathFollower.LineToPath(_path.ToList());
			_pathFollower.EngagePath();
			_pathDrawer.ClearPath();

			return;
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(ray, out RaycastHit hit, _layerMask);
		var mousepos = hit.point;
		mousepos.z = 2;

		_path.Enqueue(mousepos);
		_pathDrawer.PushPath(Input.mousePosition);
	}

	public bool RayCastHitPlayer()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			//var hitObj = hit.collider.gameObject.transform.parent.GetComponent<PlayerPathFollower>();
			var hitObj = hit.collider.GetComponentInParent<PlayerPathFollower>();
			return hitObj != null;
		}
		return false;
	}

	private void CheckClick()
	{

		if (!Input.GetMouseButton(0))
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			//var hitObj = hit.collider.gameObject.transform.parent.GetComponent<PlayerPathFollower>();
			var hitObj = hit.collider.GetComponentInParent<NodeTransverser>();
			
			if (hitObj)
			{
				_path = new Queue<Vector2>();
				_path.Enqueue(hitObj.transform.position);

				_pathFollower = hitObj;
				_pathFollower.OnPathResolve += Reset;
				ControlState = ControlStates.DRAGGING;
				_pathDrawer.StartPath(Input.mousePosition);
			}
		}
	}
}
