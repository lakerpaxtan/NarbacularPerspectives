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
    private GameObject borderPlane;

    private GameObject playerObject; 

    private string name;
    private RenderTexture cameraTexture;
    public static Shader portalShader;
    


    public Portal(Vector2 first, Vector2 second, Vector3 middlePos, Vector3 normal, GameObject player, string str)
    {
        topLeftCoord = first;
        bottomRightCoord = second;
        gameObjectPos = middlePos;
        normalVec = normal;
        normalVec.Normalize();
        playerObject = player;
        name = str;

        cameraTexture = new RenderTexture(Screen.width, Screen.height, 0);
        portalShader = Shader.Find("Unlit/PortalShader");

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

        borderPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        borderPlane.transform.position = gameObjectPos - 0.01f * normalVec;
        borderPlane.name = name + "border";
        borderPlane.transform.localScale = actualPlane.transform.localScale + new Vector3(0.1f, 0.1f, 0f);
        borderPlane.transform.rotation = actualPlane.transform.rotation;

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

            Quaternion relativeRot = Quaternion.FromToRotation(-normalVec, this.otherPortal.normalVec);
            this.portalCam.transform.rotation = relativeRot * this.playerObject.transform.rotation;

            if (normalVec == this.otherPortal.normalVec) {
                this.portalCam.transform.RotateAround(this.otherPortal.actualPlane.transform.position, this.otherPortal.actualPlane.transform.forward, 180);
            } 
        }
    }

    public static void pairPortals(Portal portal1, Portal portal2){
        portal1.isPaired = true;
        portal2.isPaired = true; 

        portal1.otherPortal = portal2;
        portal2.otherPortal = portal1;

        setTargetTexture(portal1, portal2);
    }

    private static void setTargetTexture(Portal portal1, Portal portal2) {
        portal1.actualPlane.GetComponent<Renderer>().material.shader = portalShader;
        portal2.actualPlane.GetComponent<Renderer>().material.shader = portalShader;

        portal1.portalCam.GetComponent<Camera>().targetTexture = portal1.cameraTexture;
        portal2.portalCam.GetComponent<Camera>().targetTexture = portal2.cameraTexture;

        portal1.portalCam.GetComponent<Camera>().Render();
        portal2.portalCam.GetComponent<Camera>().Render();

        portal1.actualPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", portal1.cameraTexture);
        portal2.actualPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", portal2.cameraTexture);
    }

    public void updateNearClipPlane() {
        Camera cam = otherPortal.portalCam.GetComponent<Camera>();
        Vector3 cameraPosition = cam.transform.position;
        Vector3 quadPosition = actualPlane.transform.position;
        float distToQuad = (quadPosition - cameraPosition).magnitude;
        cam.nearClipPlane = distToQuad;
    }

    private void updateCameraTexture() {
       // actualPlane.GetComponent<Renderer>().material.shader.
    }

    public void update() {
        updateCameraRelativeToPlayer();
        updateNearClipPlane();
    }

}
