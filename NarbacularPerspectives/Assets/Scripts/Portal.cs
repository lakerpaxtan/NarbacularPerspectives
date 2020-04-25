using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal 
{

    private Vector2 topLeftCoord;
    private Vector2 bottomRightCoord;
    private Vector3 gameObjectPos;
    private Vector3 normalVec;

    private GameObject actualPlane; 
    


    public Portal(Vector2 first, Vector2 second, Vector3 middlePos, Vector3 normal)
    {
        topLeftCoord = first;
        bottomRightCoord = second;
        gameObjectPos = middlePos;
        normalVec = normal;
        createPortalBoundaries();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Gonna use a plane for now --> Planning on just drawing right in front of it
    private void createPortalBoundaries()
    {
        actualPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        actualPlane.transform.position = gameObjectPos;

        Vector3 scaleLengths = topLeftCoord - bottomRightCoord;
        actualPlane.transform.localScale = new Vector3(scaleLengths[0], scaleLengths[1], 1);

        actualPlane.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -normalVec);

    }
}
