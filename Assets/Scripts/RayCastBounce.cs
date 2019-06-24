using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastBounce : MonoBehaviour
{

    [SerializeField] private int _maxIterations = 3;
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private int _raysCount = 6;
    [SerializeField] private float _lightScannerDepth = -10;
    [SerializeField] private float _minimumDistanceInBetweenRipples = 3.0f;
    [SerializeField] private int _wavePoolCap = 5;

    //[SerializeField] bool _drawRays = false;

    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject line;
    [SerializeField] private GameObject trailObject;
    [SerializeField] private GameObject particleAroundOrigin;
    [SerializeField] private GameObject particleBounce;
    [SerializeField] private GameObject rippleBackgroundQuad;
    [SerializeField] private GameObject sobelLightSource;
    [SerializeField] private GameObject crossClickObject;
    [SerializeField] private GameObject pointerClickObject;
    [SerializeField]
    private LayerMask _layerMask;

    public delegate void BouncedOffWalls(Vector3 position);
    public static event BouncedOffWalls OnWaveBounced;

    private bool _isActive = false;
    private List<List<int>> _lineBounces = new List<List<int>>();
    //private List<List<GameObject>> _lineRenderers = new List<List<GameObject>>();
    private List<List<GameObject>> _trails = new List<List<GameObject>>();
    private List<List<List<Vector3>>> _trailsPoints = new List<List<List<Vector3>>>();
    private List<List<Vector3>> _middlePoints = new List<List<Vector3>>();
    private List<List<Coroutine>> _throwCoroutines = new List<List<Coroutine>>();
    private Vector3 _directionV = Vector3.zero;
    private Vector3 _orthoLine = Vector3.zero;
    private Vector3 _clickedPos = Vector3.zero;
    private ShrimpController _shrimp;
    private CanvasMouseTracker _mousetracker;

    private int _currentFlyingWaveIndex = 0;

    List<List<bool>> trailsIdle = new List<List<bool>>();
    bool veryStart = true;
    bool startedMovement = false;
    private void Start()
    {
        _shrimp = FindObjectOfType<ShrimpController>();
        _shrimp.OnBubblePop += _shrimp_OnBubblePop;
        _mousetracker = FindObjectOfType<CanvasMouseTracker>();

        for (int i = 0; i < _raysCount; i++)
        {

            _lineBounces.Add(new List<int>());
            //_lineRenderers.Add(new List<GameObject>());

            _trailsPoints.Add(new List<List<Vector3>>());
            _middlePoints.Add(new List<Vector3>());
            _trails.Add(new List<GameObject>());
            trailsIdle.Add(new List<bool>());
            _throwCoroutines.Add(new List<Coroutine>());
            for (int j = 0; j < _wavePoolCap; j++)
            {
                _trails[i].Add(Instantiate(trailObject));
                _trails[i][j].transform.position = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, rippleBackgroundQuad.transform.position.z);
                _trailsPoints[i].Add(new List<Vector3>());
                _trailsPoints[i][j] = new List<Vector3>();
                _middlePoints[i].Add(new Vector3());
                _lineBounces[i].Add(0);
                //_lineRenderers[i].Add(Instantiate(line));
                trailsIdle[i].Add(true);
                _throwCoroutines[i].Add(null);
            }


        }
    }

    private void _shrimp_OnBubblePop(Vector3 clickPos)
    {
        if (veryStart)
        {
            veryStart = false;
            return;
        }
        //if (_currentFlyingWaveIndex > _wavePoolCap) _currentFlyingWaveIndex = 0;
        //_currentFlyingWaveIndex++;

        //clickPos = new Vector3(clickPos.x, clickPos.y, rippleBackgroundQuad.transform.position.z);

        Vector3 originToTarget = new Vector3(
            targetObject.transform.position.x - clickPos.x,
            targetObject.transform.position.y - clickPos.y,
            0//rippleBackgroundQuad.transform.position.z
            );

        Vector3 orthoLine = Quaternion.AngleAxis(-90, Vector3.forward) * originToTarget;
        _orthoLine = new Vector3(orthoLine.x, orthoLine.y, 0);
        _clickedPos = clickPos;

        //StopAllCoroutines();

        for (int i = 0; i < _raysCount; i++)
        {
            //StartCoroutine(ResetTrailRenderer(_trails[i][_currentFlyingWaveIndex].GetComponent<TrailRenderer>()));
            _trails[i][_currentFlyingWaveIndex].transform.position = new Vector3(clickPos.x, clickPos.y, rippleBackgroundQuad.transform.position.z);
            //_trailsPoints[i][_currentFlyingWaveIndex].Clear();
        }

        GameObject particleBig = Instantiate(particleAroundOrigin);
        particleBig.transform.position = clickPos;

        ThrowLineRays(_orthoLine, originToTarget);


        for (int i = 0; i < _raysCount; i++)
        {
            //for (int j = 0; j < _wavePoolCap; j++)
            //{
            //    if (trailsIdle[i][j]) StartCoroutine(ThrowTrails(i, j));
            //
            //}

            if (!trailsIdle[i][_currentFlyingWaveIndex])
            {
                print("started throwing");
                _trails[i][_currentFlyingWaveIndex].GetComponent<TrailRenderer>().enabled = true;
                _throwCoroutines[i][_currentFlyingWaveIndex] = StartCoroutine(ThrowTrails(i, _currentFlyingWaveIndex));
            }
            //_trails[i].GetComponent<MoveEcho>().StartSpreading(_trailsPoints[i]);
            //SpawnParticlesOnBounce(i);
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
            Vector3 clickPos = Vector3.zero;
            RaycastHit hit;

            

            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, _layerMask))
            {
                clickPos = hit.point;
                if (hit.point.z < rippleBackgroundQuad.transform.position.z)
                {
                    GameObject crossMisclick = Instantiate(crossClickObject);
                    crossMisclick.transform.position = new Vector3(clickPos.x, clickPos.y, hit.point.z);

                    return;
                }
                else
                {
                    GameObject crossClick = Instantiate(pointerClickObject);
                    crossClick.transform.position = new Vector3(clickPos.x, clickPos.y, hit.point.z);
                }

                startedMovement = false;

                for (int i = 0; i < _trails.Count; i++)
                {
                    //_trails[i] = Instantiate(trailObject);

                    _currentFlyingWaveIndex++;
                    if (_currentFlyingWaveIndex > _wavePoolCap - 1) _currentFlyingWaveIndex = 0;

                    if (!trailsIdle[i][_currentFlyingWaveIndex])
                    {
                        StopCoroutine(_throwCoroutines[i][_currentFlyingWaveIndex]);
                        //_trailsPoints[i][_currentFlyingWaveIndex].Clear();
                        print("force stop");

                        //_trails[i][_currentFlyingWaveIndex]
                        _trails[i][_currentFlyingWaveIndex].GetComponent<TrailRenderer>().enabled = false;
                        _trails[i][_currentFlyingWaveIndex].transform.position = new Vector3(clickPos.x, clickPos.y, rippleBackgroundQuad.transform.position.z);
                        //_trails[i][_currentFlyingWaveIndex].GetComponent<TrailRenderer>().enabled = true;
                        //trailsIdle[i][_currentFlyingWaveIndex] = true;
                    }
                    else
                    {

                        trailsIdle[i][_currentFlyingWaveIndex] = false;
                        _trails[i][_currentFlyingWaveIndex].GetComponent<TrailRenderer>().enabled = false;
                        _trails[i][_currentFlyingWaveIndex].transform.position = new Vector3(clickPos.x, clickPos.y, rippleBackgroundQuad.transform.position.z);
                        //_trails[i][_currentFlyingWaveIndex].GetComponent<TrailRenderer>().enabled = true;

                    }

                }
            }
            _shrimp.MoveTo(clickPos);


        }

        for (int i = 0; i < _raysCount; i++)
        {
            for (int j = 0; j < _wavePoolCap; j++)
            {
                SpawnParticlesOnBounce(i, j);
                CheckTrailsInactivity(i, j);
            }
        }
    }

    private void ThrowLineRays(Vector3 orthogonalLine, Vector3 normal)
    {
        for (int i = 0; i < _raysCount; i++)
        {
            float angle = 180.0f / (_raysCount + 1) * (i + 1);
            Vector3 directionVector = Quaternion.AngleAxis(angle, Vector3.forward) * orthogonalLine;

            _directionV = directionVector;
            GenerateRays(directionVector, i);

        }
    }

    private void CheckTrailsInactivity(int trailIndex, int poolIndex)
    {
        if (trailsIdle[trailIndex][poolIndex])
        {
            if (_throwCoroutines[trailIndex][poolIndex]!=null) StopCoroutine(_throwCoroutines[trailIndex][poolIndex]);
            //_trails[trailIndex][poolIndex].transform.position = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, rippleBackgroundQuad.transform.position.z);
        }
    }
    private void GenerateRays(Vector3 dir, int index)
    {
        
        //for (int i = 0; i < _wavePoolCap; i++)
       
            _lineBounces[index][_currentFlyingWaveIndex] = 0;
            _trailsPoints[index][_currentFlyingWaveIndex].Clear();
            bool hasCaughtSmth = RayCast(new Ray(new Vector3(_clickedPos.x, _clickedPos.y, rippleBackgroundQuad.transform.position.z), dir), index, _currentFlyingWaveIndex);

       

        //if (!_drawRays) return;
        //_lineRenderers[index][_currentFlyingWaveIndex].GetComponent<LineRenderer>().positionCount = (1);
        //_lineRenderers[index][_currentFlyingWaveIndex].GetComponent<LineRenderer>().SetPosition(0, new Vector3(_clickedPos.x, _clickedPos.y, rippleBackgroundQuad.transform.position.z));
        //_lineRenderers[index][_currentFlyingWaveIndex].GetComponent<LineRenderer>().enabled = hasCaughtSmth;

    }

    private void GenerateMiddlePoint(int trailIndex, int pointIndex, int indexOfPooledElement)
    {
        Vector3 origin = _trails[trailIndex][indexOfPooledElement].transform.position;
        Vector3 dest = _trailsPoints[trailIndex][indexOfPooledElement][pointIndex];

        _middlePoints[trailIndex][indexOfPooledElement] = new Vector3((dest.x + origin.x) / 2, (dest.y + origin.y) / 2, rippleBackgroundQuad.transform.position.z);
        //print("Generated middle point for: " + _trails[trailIndex][indexOfPooledElement].transform.position + " to " + _trailsPoints[trailIndex][indexOfPooledElement][pointIndex] + " in " + _middlePoints[trailIndex][indexOfPooledElement]);

    }

    private void SpawnParticlesOnBounce(int rayIndex, int trailPoolIndex)
    {
        if (_trails[rayIndex][trailPoolIndex] == null || trailsIdle[rayIndex][trailPoolIndex]) return;


        for (int pointIndex = 0; pointIndex < _trailsPoints[rayIndex][trailPoolIndex].Count - 1; pointIndex++)
        {

            if (!startedMovement)
            {
                GenerateMiddlePoint(rayIndex, pointIndex, trailPoolIndex);
                //print("Generated middle from start");
                startedMovement = true;
            }

            //print("checking " + _trails[index].transform.position + "with " + _middlePoints[index]);
            //spawn when reached the middle
            if (Vector3.Distance(_trailsPoints[rayIndex][trailPoolIndex][pointIndex + 1], _middlePoints[rayIndex][trailPoolIndex]) > _minimumDistanceInBetweenRipples
                && Vector3.Distance(_trails[rayIndex][trailPoolIndex].transform.position, _middlePoints[rayIndex][trailPoolIndex]) < 1f)
            {
                GameObject particleOnBounce = Instantiate(particleBounce);
                particleOnBounce.transform.position = _trails[rayIndex][trailPoolIndex].transform.position;

                //print("on middle pos");

                GenerateMiddlePoint(rayIndex, pointIndex + 1, trailPoolIndex);
            }
        }
        

        for (int pointIndex = 0; pointIndex < _trailsPoints[rayIndex][trailPoolIndex].Count - 1; pointIndex++)
        { 
            //spawn when reached the destination
            if (Vector3.Distance(_trails[rayIndex][trailPoolIndex].transform.position, _trailsPoints[rayIndex][trailPoolIndex][pointIndex]) < 0.01f)
            {

                GameObject particleOnBounce = Instantiate(particleBounce);
                particleOnBounce.transform.position = _trails[rayIndex][trailPoolIndex].transform.position;

                OnWaveBounced?.Invoke(_trails[rayIndex][trailPoolIndex].transform.position);

                if (pointIndex == _trailsPoints[rayIndex][trailPoolIndex].Count - 2)
                {
                    trailsIdle[rayIndex][trailPoolIndex] = true;
                    //print("trail started idling " + trailPoolIndex);
                }
            }
         }



	}

	private IEnumerator ScaleSobelLigthRange(GameObject light, float Radius)
	{

		light.GetComponent<Light>().range = 0;// = new Vector3(0, outerIntersectorVolume.transform.localScale.y, 0);

		float timer = 0;

		bool isFinished = false;

		while (true && !isFinished)
		{

			while (Radius > light.GetComponent<Light>().range)
			{
				timer += Time.deltaTime;
				light.GetComponent<Light>().range += Time.deltaTime * 3.0f; //TODO change speed to var
				yield return null;
			}
			yield return new WaitForSeconds(0.5f);
			timer = 0;
			while (0 < light.GetComponent<Light>().range)
			{
				timer += Time.deltaTime;
				light.GetComponent<Light>().range -= Time.deltaTime * 3.0f; //TODO change speed to var
				yield return null;
			}
			yield return new WaitForSeconds(0.5f);
			timer = 0;

			isFinished = true;
		}

		if (isFinished)
		{
			Destroy(light);
		}
	}

	private bool RayCast(Ray ray, int index, int trailOfPoolIndex)
	{
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, _maxDistance) && _lineBounces[index][trailOfPoolIndex] <= _maxIterations - 1)
		{
			_lineBounces[index][trailOfPoolIndex]++;

			// GameObject particleOnBounce = Instantiate(particleBounce);
			// particleBounce.transform.position = hit.point;

			var reflectAngle = Vector3.Reflect(ray.direction, hit.normal);
			//_lineRenderers[index][trailOfPoolIndex].GetComponent<LineRenderer>().positionCount = (_lineBounces[index][trailOfPoolIndex] + 1);
			//_lineRenderers[index][trailOfPoolIndex].GetComponent<LineRenderer>().SetPosition(_lineBounces[index][trailOfPoolIndex], hit.point);

			_trailsPoints[index][trailOfPoolIndex].Add(new Vector3(hit.point.x, hit.point.y, rippleBackgroundQuad.transform.position.z));
			RayCast(new Ray(new Vector3(hit.point.x, hit.point.y, rippleBackgroundQuad.transform.position.z), reflectAngle), index, trailOfPoolIndex);
			return true;
		}
        //_lineRenderers[index][trailOfPoolIndex].GetComponent<LineRenderer>().positionCount = (_lineBounces[index][trailOfPoolIndex] + 2);
        //_lineRenderers[index][trailOfPoolIndex].GetComponent<LineRenderer>().SetPosition(_lineBounces[index][trailOfPoolIndex] + 1, ray.GetPoint(_maxDistance));

		return false;
	}

	private IEnumerator ResetTrailRenderer(TrailRenderer tr)
	{
		float trailTime = tr.time;
		tr.time = 0;
		yield return null;
		tr.time = trailTime;
	}

    IEnumerator ResetTrailDist(TrailRenderer trailRenderer)
    {
        yield return new WaitForSeconds(.1f);
        trailRenderer.time = 1;

    }
    private IEnumerator ThrowTrails(int trailIndex, int trailInPoolIndex)
	{
        for (int pointIndex = 0; pointIndex < _trailsPoints[trailIndex][trailInPoolIndex].Count - 1; pointIndex++)
        {

            while (Vector3.Distance(_trails[trailIndex][trailInPoolIndex].transform.position, _trailsPoints[trailIndex][trailInPoolIndex][pointIndex]) > 0.005f)
            {
                _trails[trailIndex][trailInPoolIndex].transform.position = Vector3.MoveTowards(_trails[trailIndex][trailInPoolIndex].transform.position, _trailsPoints[trailIndex][trailInPoolIndex][pointIndex], 4.0f * Time.deltaTime);
                //print("moving trail " + trailInPoolIndex);
                yield return null;
            }

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
