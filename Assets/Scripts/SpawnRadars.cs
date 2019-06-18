using System.Collections;
using UnityEngine;

public class SpawnRadars : MonoBehaviour
{
	private float _rayCastDistance = 50;
	[SerializeField] private float scannerRadius = 5;
	[SerializeField] private float scannerScalingSpeed = 5;
	[SerializeField] private float scannerDepth = 5;
	// [SerializeField] GameObject sobelLightSource;
	[SerializeField] private GameObject circleObject;
	[SerializeField] private GameObject bigcircleObject;
	[SerializeField] private GameObject playerObject;
	[SerializeField] private GameObject particleTappingObject;
	private bool lightSpawned = false;
	private bool started = false;

	public delegate void ScannerSpawned();
	public static event ScannerSpawned OnScannerPlaced;

	private CanvasMouseTracker _mousetracker;

	// Use this for initialization
	private void Start()
	{
		FindObjectOfType<ShrimpController>().OnBubblePop += SpawnRadars_OnBubblePop;
		started = true;
		_mousetracker = FindObjectOfType<CanvasMouseTracker>();
	}

   

    private void SpawnRadars_OnBubblePop(Vector3 pos)
	{

		OnScannerPlaced?.Invoke();

		//spawn UI for big circle
		//GameObject bigcircleUI = Instantiate(bigcircleObject, Camera.main.WorldToScreenPoint(new Vector3(
		//	pos.x,
		//	pos.y,
		//	0)),
		//	Quaternion.identity);
		//bigcircleUI.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas_SS").transform, true);



		//Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
		//mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //
		//GameObject circleUI = Instantiate(particleTappingObject, mousePosition, Quaternion.identity);
		//circleUI.GetComponent<ParticleSystem>()?.Play();

		lightSpawned = false;

	}

	private float CalculateDistance(Vector3 mousePos)
	{
		Vector2 playerPos = new Vector2(playerObject.transform.position.x, playerObject.transform.position.y);
		Vector2 clickPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -Camera.main.transform.position.z));
		return Vector2.Distance(playerPos, clickPos);

	}

	private IEnumerator ScaleSobelLigthRange(GameObject light, float Radius)
	{
		//Vector3 destinationScale = new Vector3(2 * Radius, 2 * Radius, 2 * Radius);

		Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z + scannerDepth);
		mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

		//         GameObject light = Instantiate(sobelLightSource, mousePosition,
		//             Quaternion.identity);
		light.GetComponent<Light>().range = 0;// = new Vector3(0, outerIntersectorVolume.transform.localScale.y, 0);

		float timer = 0;

		while (true)
		{

			while (Radius > light.GetComponent<Light>().range)
			{
				timer += Time.deltaTime;
				light.GetComponent<Light>().range += Time.deltaTime * scannerScalingSpeed;
				yield return null;
			}
			yield return new WaitForSeconds(0.5f);
			timer = 0;


		}
	}

}
