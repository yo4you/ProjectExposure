using System.Collections;
using UnityEngine;

public class ShrimpController : MonoBehaviour
{
	public enum MovementState
	{
		APPEARING,
		IDLE,
		HIDING,
		HIDDEN
	}

	public delegate void OnHandleBubblePop(Vector3 pos);
	public event OnHandleBubblePop OnBubblePop;

	private MovementState _state = MovementState.IDLE;
	private Vector3 _target;
	private GameObject _player;
	[SerializeField]
	private float _moveSpeed;
	private float _moveTime;
	private Vector3 _start;
	private float _dist;
	[SerializeField]
	private float _hideDist;
	[SerializeField]
	private float _digDepth;

	public MovementState State
	{
		get => _state;
		private set
		{
			SwitchState(value);
			_state = value;
		}
	}

	private void SwitchState(MovementState value)
	{
		Debug.Log(value);
		if(value==MovementState.IDLE)
			OnBubblePop?.Invoke(transform.position);
	}

	private void Start()
	{
		FindObjectOfType<LevelMaterialFixer>().OnGenerationComplete += Spawn;
	}

	private void Spawn()
	{
		_player = FindObjectOfType<SetPlayerSpawnPos>().gameObject;
		_target = _player.transform.position;
		Appear();
		_dist = 1f;
	}

	private void Update()
	{
		switch (State)
		{
			case MovementState.IDLE:
			case MovementState.HIDDEN:
				break;
			case MovementState.APPEARING:
			case MovementState.HIDING:
				Approach(_target);
				break;
			default:
				break;
		}
	}

	private void Approach(Vector3 target)
	{
		_moveTime +=  Time.deltaTime * _moveSpeed / _dist;
		transform.position = Vector3.Lerp(_start,target, _moveTime);
		//todo correct Z
		if (_moveTime > 1f)
		{
			State++;
			transform.position = target;
			_moveTime = 0;
		}
	}

	private void Appear()
	{
		_start = transform.position;
		_dist = Vector3.Distance(transform.position, _target);
		State = MovementState.APPEARING;
	}

	private void Hide()
	{
		_start = transform.position;
		_dist = Vector3.Distance(transform.position, _target);
		State = MovementState.HIDING;
	}

	public void MoveTo(Vector3 pos)
	{
		StartCoroutine(StartMoveTo(pos));
	}

	private IEnumerator StartMoveTo(Vector3 pos)
	{
		yield return new WaitForEndOfFrame();

		switch (State)
		{
			case MovementState.APPEARING:
			case MovementState.HIDING:
			case MovementState.HIDDEN:
				yield break;

			case MovementState.IDLE:
				_target = FindHidingSpot(transform.position);
				Hide();
				break;
		}
		while (State != MovementState.HIDDEN)
		{
			yield return new WaitForEndOfFrame();
		}
		transform.position = FindHidingSpot(pos);
		_target = pos;
		Appear();

	}

	private Vector3 FindHidingSpot(Vector3 start)
	{
		Vector3 rand = UnityEngine.Random.insideUnitCircle * _hideDist;
		rand.z = _digDepth;
		return start + rand;
	}
}
