using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject flareGun;
    public GameObject playerObject;
    
    Portal testPortal;
    Portal testPortal2;

    List<Portal> portals;

    // Start is called before the first frame update
    void Start()
    {
        flareGun.SetActive(true);
        portals = new List<Portal>();


        //testPortal = new Portal(new Vector2(4, 4), new Vector2(0, 0), new Vector3(-8f, 2, 14), new Vector3(0, 0, -1), playerObject, "testOne");
        //testPortal2 = new Portal(new Vector2(4, 4), new Vector2(0, 0), new Vector3(8f, 2, -16), new Vector3(0, 0, 1), playerObject, "testTwo");
    }

    // Update is called once per frame
    void Update()
    {
        //Portal.pairPortals(testPortal, testPortal2);
        //portals.Add(testPortal);
        //portals.Add(testPortal2);
        //testPortal.updateCameraRelativeToPlayer();
        //testPortal2.updateCameraRelativeToPlayer();

        foreach (Portal p in portals)
        {
            p.update();
        }

        //testPortal.setTexture();
        //testPortal2.setTexture();
    }

    public void AddPortals(Portal a, Portal b)
    {
        portals.Add(a);
        portals.Add(b);
    }
    

}
