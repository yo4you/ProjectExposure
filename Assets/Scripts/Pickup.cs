using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int scoreToGive;
    [SerializeField] GameObject player;

    private float _distanceToPlayer;

    public float Distance {
        get {
            return _distanceToPlayer;
        }
        set {
            _distanceToPlayer = value;
        }
    }

	private void Start()
	{
		player = FindObjectOfType<NodeTransverser>().gameObject;
	}
	public int Score { get { return scoreToGive; } }

    private void Update()
    {
        
    }
}
