using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingPan : MonoBehaviour
{
	private const float _camHeight = 5f;

	private void Start()
	{
		StartCoroutine(StartPan());
	}


	private static float CameraCurve(float x)
	{
		return -((2 * x - 1) * (2 * x - 1)) + 1;
	}

	private IEnumerator StartPan()
	{
		yield return new WaitForEndOfFrame();

		List<MonoBehaviour> disabled = new List<MonoBehaviour>();


		var cameras = FindObjectsOfType<CameraControl>();
		foreach (var camera in cameras)
		{
			disabled.Add(camera);
			camera.enabled = false;
		}
		var ripple = FindObjectOfType<RayCastBounce>();
		ripple.enabled = false;
		disabled.Add(ripple);

		var exit = FindObjectOfType<Exit>();
		var start = new Dictionary<CameraControl, Vector3>();
		foreach (var cam in cameras)
		{
			start.Add(cam, cam.transform.position);
		}
		var target = exit.transform.position - Vector3.forward * 5;
		for (float t = 0; t < 1f; t += Time.deltaTime)
		{
			foreach (var camera in cameras)
			{

				var campos = Vector3.Lerp(start[camera], target, Mathf.SmoothStep(0f, 1f, t));
				campos.z -= CameraCurve(t)* _camHeight;
				camera.transform.position = campos;

			}
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(2f);

		for (float t = 0; t < 1f; t += Time.deltaTime)
		{
			foreach (var camera in cameras)
			{

				var campos = Vector3.Lerp(target, start[camera], Mathf.SmoothStep(0f, 1f, t));
				campos.z -= CameraCurve(t)* _camHeight;
				camera.transform.position = campos;

			}
			yield return new WaitForEndOfFrame();
		}

		foreach (var comp in disabled)
		{
			comp.enabled = true;
		}
	}

	private void Update()
	{

	}
}
