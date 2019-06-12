using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDrawer : MonoBehaviour {
	private LineRenderer _lineRenderer;
	private List<Vector3> _positions = new List<Vector3>();
	[SerializeField]
	[Range(0,10)]	
	private float _lineSmoothing;

    public delegate void PathStart();
    public static event PathStart OnPathStarted;

    //[SerializeField] float zDistance = -15.0f;

	void Start () {
		_lineRenderer = GetComponentInChildren<LineRenderer>();	
	}
	
	void Update () {
	}

	internal void StartPath(Vector3 position)
	{
		_lineRenderer.enabled = true;
		_positions = new List<Vector3>();
		PushPath(position);

        OnPathStarted?.Invoke();
        
	}

	internal void PushPath(Vector3 position)
	{

        Vector3 mousePosition = new Vector3(position.x, position.y, -Camera.main.transform.position.z);
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);


        //position.x -= 0.5f* Camera.main.pixelWidth;
        //position.y -= 0.5f* Camera.main.pixelHeight;
        //mousePosition.z = zDistance;
        _positions.Add(mousePosition);
		_lineRenderer.positionCount = _positions.Count;

		_lineRenderer.SetPositions(_positions.ToArray());
		_lineRenderer.Simplify(_lineSmoothing);

	}

	internal void ClearPath()
	{
		_lineRenderer.enabled = false;

	}

}
