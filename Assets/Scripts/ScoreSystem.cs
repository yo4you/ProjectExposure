using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    public delegate void AddScore();
    public static event AddScore IncreaseScore;

    public static int currentScore;

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
