using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public delegate void AddScore();
    public static event AddScore IncreaseScore;

	private static int currentScore;

	public static int CurrentScore
	{
		get { return PlayerPrefs.GetInt("score"); }
		set
		{
			PlayerPrefs.SetInt("score", value);
		}
	}

	private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }  

    private void ApplyScore()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        IncreaseScore?.Invoke();
    }
}
