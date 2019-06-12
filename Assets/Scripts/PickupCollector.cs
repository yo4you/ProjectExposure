using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCollector : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Pickup>())
        {
            ScoreSystem.currentScore += other.GetComponent<Pickup>().Score;
            Destroy(this);
        }
    }

}
