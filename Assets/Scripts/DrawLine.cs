using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour {

    protected List<Vector3> m_Points = new List<Vector3>();

    [SerializeField] Camera camera;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
            
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            if (!m_Points.Contains(mousePosition))
            {
                m_Points.Add(mousePosition);
                this.GetComponent<LineRenderer>().positionCount = m_Points.Count;
                this.GetComponent<LineRenderer>().SetPosition(this.GetComponent<LineRenderer>().positionCount - 1, mousePosition);
            }
        }
    }
}
