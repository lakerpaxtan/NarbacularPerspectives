using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCreation : MonoBehaviour
{
    public GameObject player;
    public GameObject gm;
    LineRenderer rayLine;
    Ray ray;
    public GameObject portalShape;
    LineRenderer portalShapeLine;
    Vector3 topLeftCoord;
    Vector3 bottomRightCoord;
    Vector3 norm;
    public float offset;

    Portal a;
    Portal b;
    public float size = 0.01f;
    public float sizeSpeedIncrease;
    int numPortals;

    // Start is called before the first frame update
    void Start()
    {
        //rayLine = gameObject.GetComponent<LineRenderer>();
        portalShapeLine = portalShape.GetComponent<LineRenderer>();
        numPortals = 0;
        size = 1;
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
            if (Physics.Raycast(transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction, out RaycastHit hit, Mathf.Infinity))
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                norm = hit.normal.normalized;
                portalShapeLine.transform.position = hit.point + norm * offset; 
                //portalShapeLine.transform.rotation = hit.collider.transform.rotation;
            }
        }

        if (Input.GetButton("CreatePortal") && numPortals % 2 == 0)
        {
            size += sizeSpeedIncrease;
            size %= 2;

            portalShapeLine.SetPosition(0, new Vector3(-size, -size));
            portalShapeLine.SetPosition(1, new Vector3(-size, size));
            portalShapeLine.SetPosition(2, new Vector3(size, size));
            portalShapeLine.SetPosition(3, new Vector3(size, -size));
            portalShapeLine.SetPosition(4, new Vector3(-size, -size));
        }

        // Press B to create the portal at the indicated space 
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            if (numPortals % 2 == 0)
            {
                a = new Portal(new Vector2(size, size), new Vector2(0, 0), portalShapeLine.transform.position, norm, player, "a" + Mathf.Floor(numPortals / 2));
            } else
            {
                b = new Portal(new Vector2(size, size), new Vector2(0, 0), portalShapeLine.transform.position, norm, player, "b" + Mathf.Floor(numPortals / 2));
                Portal.pairPortals(a, b);
                gm.GetComponent<GameManager>().AddPortals(a, b);
            }
            numPortals++;
            portalShape.SetActive(false);
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
