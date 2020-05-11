using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalOverlap : MonoBehaviour
{
    public bool valid = true;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Portal"))
        {
            valid = false;
        } 
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Portal"))
        {
            valid = true;
        }
    }

    public void UpdatePlane(Vector3 mid, Vector3 norm, float width, float height)
    {
        transform.position = mid;
        transform.localScale = new Vector3(width, height, 1);
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, -norm);
    }
}
