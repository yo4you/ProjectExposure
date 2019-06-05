using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastBounce : MonoBehaviour
{

	[SerializeField] private int _maxIterations = 3;
	[SerializeField] private float _maxDistance = 10f;
	[SerializeField] private int raysCount = 6;
	[SerializeField] private float lineWidth = 0.05f;

	[SerializeField] private GameObject targetObject;
	[SerializeField] private GameObject lineObject;
	[SerializeField] private GameObject trailObject;
	[SerializeField] private GameObject particleAroundOrigin;
	[SerializeField] private GameObject particleBounce;


	[SerializeField] private float rayDepthDistance = 5.0f;
	private bool _isActive = false;


	private List<int> _lineBounces = new List<int>();
	//private List<GameObject> _lineRenderers = new List<GameObject>();
	private List<GameObject> _trails = new List<GameObject>();
	private List<List<Vector3>> _trailsPoints = new List<List<Vector3>>();
	private Vector3 _directionV = Vector3.zero;
	private Vector3 _orthoLine = Vector3.zero;
	private Vector3 _clickedPos = Vector3.zero;
	private CanvasMouseTracker _mousetracker;

	private void Start()
	{
		_mousetracker = FindObjectOfType<CanvasMouseTracker>();

		for (int i = 0; i < raysCount; i++)
		{

			_lineBounces.Add(0);
			//_lineRenderers.Add(Instantiate(lineObject));

			_trails.Add(Instantiate(trailObject));
			_trails[i].transform.position = transform.position;
			_trailsPoints.Add(new List<Vector3>());
		}
	}

	private void ThrowLineRays(Vector3 orthogonalLine, Vector3 normal)
	{
		for (int i = 0; i < raysCount; i++)
		{
			float angle = 180.0f / (raysCount + 1) * (i + 1);
			Vector3 directionVector = Quaternion.AngleAxis(angle, Vector3.forward) * orthogonalLine;

			_directionV = directionVector;
			GenerateRays(directionVector, i);

		}
	}



	private void GenerateRays(Vector3 dir, int index)
	{

		_lineBounces[index] = 0;
		//_lineRenderers[index].GetComponent<LineRenderer>().positionCount = (1);
		//_lineRenderers[index].GetComponent<LineRenderer>().SetPosition(0, new Vector3(_clickedPos.x, _clickedPos.y, rayDepthDistance));
		//_lineRenderers[index].GetComponent<LineRenderer>().enabled = 
		RayCast(new Ray(new Vector3(_clickedPos.x, _clickedPos.y, rayDepthDistance), dir), index);
	}

	private void SpawnParticlesOnBounce(int index)
	{
		for (int i = 0; i < _trailsPoints[index].Count; i++)
		{
			if (Vector3.Distance(_trails[index].transform.position, _trailsPoints[index][i]) < 0.01f)
			{
				GameObject particleOnBounce = Instantiate(particleBounce);
				particleBounce.transform.position = _trails[index].transform.position;
			}
		}
	}

	private void Update()
	{


		if (!_mousetracker)
		{
			_mousetracker = FindObjectOfType<CanvasMouseTracker>();
		}
		else if (Input.GetMouseButtonDown(0) && !_mousetracker.RayCastHitPlayer())
		{


			Vector3 mouse = Input.mousePosition;
			Ray castPoint = Camera.main.ScreenPointToRay(mouse);
			RaycastHit hit;
			Vector3 clickPos = Vector3.zero;

			if (Physics.Raycast(castPoint, out hit, Mathf.Infinity))
			{
				clickPos = hit.point;
				//if (hit.point.z < 0) return;
			}

			clickPos = new Vector3(clickPos.x, clickPos.y, 0);

			Vector3 originToTarget = new Vector3(
				targetObject.transform.position.x - clickPos.x,
				targetObject.transform.position.y - clickPos.y,
				rayDepthDistance
				);

			Vector3 orthoLine = Quaternion.AngleAxis(-90, Vector3.forward) * originToTarget;
			_orthoLine = orthoLine;
			_clickedPos = clickPos;

			StopAllCoroutines();

			for (int i = 0; i < raysCount; i++)
			{
				StartCoroutine(ResetTrailRenderer(_trails[i].GetComponent<TrailRenderer>()));
				_trails[i].transform.position = clickPos;
				_trailsPoints[i].Clear();
			}

			GameObject particleBig = Instantiate(particleAroundOrigin);
			particleBig.transform.position = clickPos;

			ThrowLineRays(_orthoLine, originToTarget);



			for (int i = 0; i < raysCount; i++)
			{
				StartCoroutine(ThrowTrails(i));
				//_trails[i].GetComponent<MoveEcho>().StartSpreading(_trailsPoints[i]);
			}

		}


	}
	private bool RayCast(Ray ray, int index)
	{
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, _maxDistance) && _lineBounces[index] <= _maxIterations - 1)
		{
			_lineBounces[index]++;

			// GameObject particleOnBounce = Instantiate(particleBounce);
			// particleBounce.transform.position = hit.point;

			var reflectAngle = Vector3.Reflect(ray.direction, hit.normal);
			//_lineRenderers[index].GetComponent<LineRenderer>().positionCount = (_lineBounces[index] + 1);
			//_lineRenderers[index].GetComponent<LineRenderer>().SetPosition(_lineBounces[index], hit.point);

			_trailsPoints[index].Add(hit.point);
			//_trails[index].transform.position = Vector3.MoveTowards(_trails[index].transform.position, hit.point, Time.deltaTime);
			RayCast(new Ray(hit.point, reflectAngle), index);
			return true;
		}
		// _lineRenderers[index].GetComponent<LineRenderer>().positionCount = (_lineBounces[index] + 2);
		// _lineRenderers[index].GetComponent<LineRenderer>().SetPosition(_lineBounces[index] + 1, ray.GetPoint(_maxDistance));
		_trailsPoints[index].Add(ray.GetPoint(_maxDistance));

		return false;
	}

	private IEnumerator ResetTrailRenderer(TrailRenderer tr)
	{
		float trailTime = tr.time;
		tr.time = 0;
		yield return null;
		tr.time = trailTime;
	}

	private IEnumerator ThrowTrails(int i)
	{
		_trails[i].GetComponent<TrailRenderer>().Clear();
		for (int j = 0; j < _trailsPoints[i].Count; j++)
		{

			while (Vector3.Distance(_trails[i].transform.position, _trailsPoints[i][j]) > 0.005f)
			{
				_trails[i].transform.position = Vector3.MoveTowards(_trails[i].transform.position, _trailsPoints[i][j], 4.0f * Time.deltaTime);
				//_trails[i].transform.LookAt(_trailsPoints[i][j], Vector3.forward);


				//GameObject particleOnBounce = Instantiate(particleBounce);
				//particleBounce.transform.position = _trails[i].transform.position;

				yield return null;
			}

			SpawnParticlesOnBounce(i);


			//_trailsPoints[i].RemoveAt(j);

		}



	}

	private IEnumerator WaitInBetweenClicks(float time)
	{
		//_isActive = false;
		yield return new WaitForSeconds(time);
	}


	private void OnDrawGizmos()
	{
		Gizmos.DrawRay(new Ray(_clickedPos, _orthoLine));
		//if (targetObject != null) Gizmos.DrawLine(_clickedPos, targetObject.transform.position);
		Gizmos.DrawRay(new Ray(_clickedPos, _directionV));
		//Gizmos.DrawLine(transform.position, targetObject.transform.position);
	}


}
