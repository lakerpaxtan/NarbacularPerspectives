using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Tutorial : MonoBehaviour
{
    GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GetComponent<GameManager>();
        Portal testPortal = new Portal(4, 4, new Vector3(7f, 2, -7), new Vector3(-1, 0, 0), gm.playerObject, "testOne", gm.portalPref, gm.borderMat);
        Portal testPortal2 = new Portal(4, 4, new Vector3(0, 12, 0), new Vector3(1, 0, 0), gm.playerObject, "testTwo", gm.portalPref, gm.borderMat);
        Portal.pairPortals(testPortal, testPortal2);
        gm.portalList.Add(testPortal);
        //gm.TestPortals();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<GameManager>().playerObject.transform.position.y < -25)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
