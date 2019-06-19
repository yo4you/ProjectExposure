using UnityEngine;

public class PointTowardExit : MonoBehaviour
{
	[SerializeField]
	private float _speed;
	private Exit _exit;
	private NodeGraphActor _player;
	Vector2 _randDir;

	private void Start()
	{
	}

	private void Update()
	{
		_exit = _exit ?? FindObjectOfType<Exit>();
		_player = _player ?? FindObjectOfType<NodeGraphActor>();
		if (_exit == null)
		{
			return;

		}
		
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(_exit.transform.position.y - _player.transform.position.y, _exit.transform.position.x - _player.transform.position.x));
	}
}
