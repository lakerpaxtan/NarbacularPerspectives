using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCreation : MonoBehaviour
{
    GameObject player;
    LineRenderer rayLine;
    Ray ray;
    public GameObject portalShape;
    LineRenderer portalShapeLine;
    Vector3 topLeftCoord;
    Vector3 bottomRightCoord;
    public Vector3 offset;

    Portal a;
    Portal b;
    int numPortals;

    // Start is called before the first frame update
    void Start()
    {
        //rayLine = gameObject.GetComponent<LineRenderer>();
        portalShapeLine = portalShape.GetComponent<LineRenderer>();
        numPortals = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //ray = new Ray(transform.position, transform.forward);
        //rayLine.SetPosition(0, ray.origin);
        //rayLine.SetPosition(1, ray.GetPoint(30));

        if (Input.GetButtonDown("CreatePortal"))
        {
            portalShape.SetActive(true);
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity))
            {
                portalShapeLine.transform.position = hit.point + new Vector3(0, 0, .1f);
                //portalShapeLine.transform.rotation = hit.collider.transform.rotation;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (numPortals % 2 == 0)
            {
                a = new Portal(new Vector2(4, 4), new Vector2(0, 0), portalShapeLine.transform.position, new Vector3(0, 0, -1), player, "a" + Mathf.Floor(numPortals / 2));
            } else
            {
                b = new Portal(new Vector2(4, 4), new Vector2(0, 0), portalShapeLine.transform.position, new Vector3(0, 0, -1), player, "b" + Mathf.Floor(numPortals / 2));
                Portal.pairPortals(a, b);
            }
            numPortals++;
        }



        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (Input.GetButtonDown("CreatePortal"))
        //{
        //    rayLine.SetPosition(0, gameObject.transform.position + offset);
        //    rayLine.SetPosition(1, ray.GetPoint(10));
        //    rayLine.enabled = true;

        //    portalShape.SetActive(true);
        //    topLeftCoord = ray.GetPoint(10);
        //    portalShapeLine.SetPosition(0, topLeftCoord);
        //}

        //if (Input.GetButton("CreatePortal"))
        //{
        //    rayLine.SetPosition(0, gameObject.transform.position + offset);
        //    rayLine.SetPosition(1, ray.GetPoint(10));

        //    portalShapeLine.SetPosition(1, new Vector3(ray.GetPoint(10).x, topLeftCoord.y));
        //    portalShapeLine.SetPosition(2, ray.GetPoint(10));
        //    portalShapeLine.SetPosition(3, new Vector3(topLeftCoord.x, ray.GetPoint(10).y));
        //}

        //if (Input.GetButtonUp("CreatePortal"))
        //{
        //    //bottomRightCoord = ray.GetPoint(10);
        //    //portalShape.SetActive(false);

        //    rayLine.enabled = false;


        //}
    }
}
