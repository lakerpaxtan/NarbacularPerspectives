using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public GameObject flareGun;
    public GameObject playerObject;
    
    Portal testPortal;
    Portal testPortal2;

    


    // Start is called before the first frame update
    void Start()
    {
        flareGun.SetActive(true);
        testPortal = new Portal(new Vector2(4,4), new Vector2(0,0), new Vector3(-8f,2, 14), new Vector3(0,0,-1), playerObject, "testOne");
        testPortal2 = new Portal(new Vector2(4,4), new Vector2(0,0), new Vector3(8f,2,-16), new Vector3(0,0,1), playerObject, "testTwo");
       
    }

    // Update is called once per frame
    void Update()
    {
        Portal.pairPortals(testPortal, testPortal2);
        //testPortal.updateCameraRelativeToPlayer();
        //testPortal2.updateCameraRelativeToPlayer();

        testPortal.update();
        testPortal2.update();

        //testPortal.setTexture();
        //testPortal2.setTexture();
    }

    

}
