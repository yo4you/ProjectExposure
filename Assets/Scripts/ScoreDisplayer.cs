using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (isBonus) GetComponent<Text>().text = ScoreSystem.
        GetComponent<Text>().text = ScoreSystem.currentScore.ToString();
    }

}
