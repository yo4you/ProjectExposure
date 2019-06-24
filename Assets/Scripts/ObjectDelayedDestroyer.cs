using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDelayedDestroyer : MonoBehaviour
{
    [SerializeField] float timeBeforeDestruction;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DieDieDieMyDarling");
    }

    IEnumerator DieDieDieMyDarling()
    {
        yield return new WaitForSeconds(timeBeforeDestruction);
        Destroy(this.gameObject);

    }
    
}
