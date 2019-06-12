using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int scoreToGive;

    public int Score { get { return scoreToGive; } }

    public int GetScore()
    {
        return scoreToGive;
    }

}
