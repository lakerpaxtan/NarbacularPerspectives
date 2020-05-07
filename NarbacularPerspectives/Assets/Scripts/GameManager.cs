using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public GameObject flareGun;
    public GameObject playerObject;


    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    public List<Portal> portalList;


    // Start is called before the first frame update
    void Start()
    {
        portalList = new List<Portal>();
        //TestPortals();
    }

    void TestPortals()
    {
        flareGun.SetActive(true);
        Portal testPortal = new Portal(4, 4, new Vector3(8f, 2, 10), new Vector3(1, 0, -1), playerObject, "testOne");
        Portal testPortal2 = new Portal(4, 4, new Vector3(8f, 2, -14), new Vector3(0, 0, 1), playerObject, "testTwo");
        Portal.pairPortals(testPortal, testPortal2);
        portalList.Add(testPortal);


        Portal testPortal3 = new Portal(4, 4, new Vector3(-8f, 2, 10), new Vector3(0, 0, 1), playerObject, "testThree");
        Portal testPortal4 = new Portal(4, 4, new Vector3(-8f, 2, -10), new Vector3(0, 0, 1), playerObject, "testFour");
        Portal.pairPortals(testPortal3, testPortal4);
        portalList.Add(testPortal3);

        Portal testPortal5 = new Portal(4, 4, new Vector3(0f, 2, 8), new Vector3(0, 0, -1), playerObject, "testFive");
        Portal testPortal6 = new Portal(4, 4, new Vector3(0f, 2, -8), new Vector3(0, 0, 1), playerObject, "testSix");
        Portal.pairPortals(testPortal5, testPortal6);
        portalList.Add(testPortal5);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Portal tempPort in portalList){
            tempPort.update();
        }
    }

}
