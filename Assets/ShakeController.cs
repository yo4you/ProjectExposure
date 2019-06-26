using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeController : MonoBehaviour
{
	CameraControl[] _cameras;
	[SerializeField]
	private float _shakeTime;
	Animator _anim;
	void Start()
    {
		_anim = GetComponent<Animator>();
		_cameras = FindObjectsOfType<CameraControl>();
		FindObjectOfType<ShrimpController>().OnBubblePop += ShakeController_OnBubblePop;
	}

	private void ShakeController_OnBubblePop(Vector3 pos)
	{
		StartCoroutine(Shake());

	}

	IEnumerator Shake()
	{

		_anim.SetBool("SoundWaves", true);

		for (float t = 0; t < _shakeTime; t+=Time.deltaTime)
		{
			var pos = UnityEngine.Random.insideUnitSphere * 0.05f;
			pos.z = 0;
			transform.position += pos;
			yield return new WaitForEndOfFrame();
		}
		_anim.SetBool("SoundWaves", false);
		transform.position = new Vector3();
	}

	void Update()
    {
		foreach (var item in _cameras)
		{
			item.Shake(transform.localPosition);
		}
    }
}
