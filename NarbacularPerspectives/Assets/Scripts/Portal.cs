using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal 
{
    private bool isPaired = false; 
    private Vector2 topLeftCoord;
    private Vector2 bottomRightCoord;
    private Vector3 gameObjectPos;
    private Vector3 normalVec;
    private Portal otherPortal;

    private GameObject portalCam;

    private GameObject actualPlane;

    private GameObject playerObject; 

    private string name;
    


    public Portal(Vector2 first, Vector2 second, Vector3 middlePos, Vector3 normal, GameObject player, string str)
    {
        topLeftCoord = first;
        bottomRightCoord = second;
        gameObjectPos = middlePos;
        normalVec = normal;
        playerObject = player;
        name = str;
        createPortalBoundaries();
        createPortalCamera();

    }

    
    //Gonna use a plane for now --> Planning on just drawing right in front of it
    private void createPortalBoundaries()
    {
        actualPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        actualPlane.transform.position = gameObjectPos;
        actualPlane.name = name;

        Vector3 scaleLengths = topLeftCoord - bottomRightCoord;
        actualPlane.transform.localScale = new Vector3(scaleLengths[0], scaleLengths[1], 1);

        actualPlane.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -normalVec);

    }

    private void createPortalCamera(){
        this.portalCam = new GameObject();
        portalCam.AddComponent<Camera>().enabled = false;
        portalCam.name = name + "Cam"; 
    }


    public void updateCameraRelativeToPlayer(){
        if (isPaired){
            Vector3 relativePos = actualPlane.transform.InverseTransformPoint(playerObject.transform.position);
            this.portalCam.transform.position = this.otherPortal.actualPlane.transform.TransformPoint(relativePos);
            this.portalCam.transform.RotateAround(this.otherPortal.actualPlane.transform.position, this.otherPortal.actualPlane.transform.right, 180);

            //Debug.Log("player" + playerObject.transform.position);
            //Debug.Log("relative" + relativePos);
            //Debug.Log(this.name + this.portalCam.transform.position);
        }
        
    }





    public static void pairPortals(Portal portal1, Portal portal2){
        portal1.isPaired = true;
        portal2.isPaired = true; 

        portal1.otherPortal = portal2;
        portal2.otherPortal = portal1; 
    }

}
