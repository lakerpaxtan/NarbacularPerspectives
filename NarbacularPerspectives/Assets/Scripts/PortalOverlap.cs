using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalOverlap : MonoBehaviour
{
    public bool valid = true;
    static float offset = .1f;
    public List<GameObject> overlap;

    void Start()
    {
        overlap = new List<GameObject>();
    }

        private void Update()
    {
        if (overlap.Count == 0)
        {
            valid = true;
        } else
        {
            valid = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Portal"))
        {
            overlap.Add(collider.gameObject);
        } 
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Portal"))
        {
            overlap.Remove(collider.gameObject);
        }
    }

    public void UpdatePlane(Vector3 mid, Vector3 norm, float width, float height)
    {
        transform.position = mid + norm * offset;
        transform.localScale = new Vector3(width, height, .2f);
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, -norm);
    }
}
