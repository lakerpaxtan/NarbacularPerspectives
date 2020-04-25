using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public GameObject flareGun;
  

    


    // Start is called before the first frame update
    void Start()
    {
        flareGun.SetActive(true);
        Portal testPortal = new Portal(new Vector2(4,4), new Vector2(0,0), new Vector3(2,2,2), new Vector3(0,0,1));
        Portal testPortal2 = new Portal(new Vector2(4,4), new Vector2(0,0), new Vector3(-5,2,2), new Vector3(0,0,1));
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

}
