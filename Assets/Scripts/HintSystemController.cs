using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintSystemController : MonoBehaviour
{
	[SerializeField]
	float _idleSecondsBeforeMoveHint;
	float _moveTimer;

	[SerializeField]
	float _idleSecondsBeforeTapTime;
	float _tapTimer;

	void Start()
    {
		FindObjectOfType<ShrimpController>().OnBubblePop += HintSystemController_OnBubblePop;
		FindObjectOfType<NodeTransverser>().OnMove += HintSystemController_OnNodeChanged; ; 
    }

	private void HintSystemController_OnNodeChanged()
	{
		_moveTimer = 0;

	}

	private void HintSystemController_OnBubblePop(Vector3 pos)
	{
		_tapTimer = 0;
	}

	void Update()
    {
		_tapTimer += Time.deltaTime;
		_moveTimer += Time.deltaTime;
		transform.GetChild(0).gameObject.SetActive(_moveTimer > _idleSecondsBeforeMoveHint);
		transform.GetChild(1).gameObject.SetActive(_tapTimer > _idleSecondsBeforeTapTime);
    }
}
