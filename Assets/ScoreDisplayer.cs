using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour
{

    private void OnEnable()
    {
        ScoreSystem.IncreaseScore += UpdateScore;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void UpdateScore()
    {
        GetComponent<Text>().text = ScoreSystem.currentScore.ToString();

    }
}
