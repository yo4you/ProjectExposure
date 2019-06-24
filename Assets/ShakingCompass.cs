using UnityEngine;

public class ShakingCompass : MonoBehaviour
{
	private Animator _anim;
	private NodeTransverser _player;
	private Exit _exit;
	private float _lastDist;


	private void Start()
	{
		_anim = GetComponent<Animator>();
		FindObjectOfType<NodeTransverser>().OnPathResolve += Step;

	}

	private void Step()
	{
		_exit = _exit ?? FindObjectOfType<Exit>();
		_player = _player ?? FindObjectOfType<NodeTransverser>();
		if (_exit == null)
		{
			return;
		}

		var dist = Vector2.Distance(_player.transform.position, _exit.transform.position);
		if (_lastDist == dist)
		{
			return;
		}
		if (_lastDist < dist)
		{
			_anim.SetBool("Shaking", true);

		}
		else
		{
			_anim.SetBool("Shaking", false);
		}
		_lastDist = dist;

	}

}
