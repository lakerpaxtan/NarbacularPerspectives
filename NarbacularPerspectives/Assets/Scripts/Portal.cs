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
    private GameObject reversePlane;
    private GameObject borderPlane;
    private Vector3 upDir;
    float width;
    float height;
    private GameObject playerObject; 

    private string name;
    private RenderTexture cameraTexture;
    public static Shader portalShader;
    


    public Portal(float width, float height, Vector3 middlePos, Vector3 normal, GameObject player, string str)
    {
        this.width = width;
        this.height = height;
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
        //upDir = new Vector3(topLeftCoord[0] - top)
        actualPlane.transform.localScale = new Vector3(width, height, 1);

        actualPlane.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -normalVec);
        //actualPlane.transform.rotation = Quaternion.LookRotation(-normalVec, Vector3.up);

        borderPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        borderPlane.transform.position = gameObjectPos - 0.01f * normalVec;
        borderPlane.name = name + "border";
        borderPlane.transform.localScale = actualPlane.transform.localScale + new Vector3(0.1f, 0.1f, 0f);
        borderPlane.transform.rotation = actualPlane.transform.rotation;

        reversePlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        reversePlane.transform.position = gameObjectPos;
        reversePlane.name = name + "reversePlane";
        reversePlane.transform.localScale = new Vector3(width, height, 1);
        reversePlane.transform.rotation = Quaternion.FromToRotation(Vector3.forward, normalVec);
        //reversePlane.transform.rotation = Quaternion.LookRotation(normalVec, Vector3.up);
        //reversePlane.transform.Rotate(reversePlane.transform.forward, 180);
        reversePlane.SetActive(false);
        upDir = this.actualPlane.transform.up;

    }

    private void createPortalCamera(){
        this.portalCam = new GameObject();
        portalCam.AddComponent<Camera>().enabled = false;
        portalCam.name = name + "Cam"; 
    }


    public void updateCameraRelativeToPlayer(){
        if (isPaired){

            //Debug.Log(this.playerObject.transform.eulerAngles);
           
         
            Vector3 relativePos = actualPlane.transform.InverseTransformPoint(playerObject.transform.position);
            this.portalCam.transform.position = this.otherPortal.reversePlane.transform.TransformPoint(relativePos);
            //this.portalCam.transform.RotateAround(this.otherPortal.actualPlane.transform.position, this.otherPortal.actualPlane.transform.right, 180);
            
           
            //this.portalCam.transform.up = this.otherPortal.reversePlane.transform.up;
            //this.portalCam.transform.Rotate(this.portalCam.transform.up, 180);
            Quaternion relativeRot = Quaternion.FromToRotation(-normalVec, this.otherPortal.normalVec);
            this.portalCam.transform.rotation = relativeRot * this.playerObject.transform.rotation;
          

            // if (normalVec == otherPortal.normalVec) {
            //     this.portalCam.transform.RotateAround(this.otherPortal.actualPlane.transform.position, this.otherPortal.actualPlane.transform.forward, 180);
            // } 

            if (portalCam.transform.eulerAngles[2] == 180 || portalCam.transform.eulerAngles[2] == -180 || normalVec == otherPortal.normalVec){
                this.portalCam.transform.RotateAround(this.otherPortal.actualPlane.transform.position, this.otherPortal.actualPlane.transform.forward, 180);
            }


        }
    }

    public static void pairPortals(Portal portal1, Portal portal2){
        portal1.isPaired = true;
        portal2.isPaired = true; 

        portal1.otherPortal = portal2;
        portal2.otherPortal = portal1;

    }

    public void texturePortal(){
        actualPlane.GetComponent<Renderer>().material.shader = portalShader;
        otherPortal.actualPlane.GetComponent<Renderer>().material.shader = portalShader;

        portalCam.GetComponent<Camera>().targetTexture = cameraTexture;
        otherPortal.portalCam.GetComponent<Camera>().targetTexture = otherPortal.cameraTexture;

        portalCam.GetComponent<Camera>().Render();
        otherPortal.portalCam.GetComponent<Camera>().Render();

        actualPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", cameraTexture);
        otherPortal.actualPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", otherPortal.cameraTexture);
    }

   

    public void updateNearClipPlane() {
        //Camera cam = otherPortal.portalCam.GetComponent<Camera>();
        //Vector3 cameraPosition = cam.transform.position;
        //Vector3 quadPosition = actualPlane.transform.position;
        //float distToQuad = (quadPosition - cameraPosition).magnitude;
        //cam.nearClipPlane = distToQuad;

        // Proper oblique clipping plane technique thanks to: https://danielilett.com/2019-12-18-tut4-3-matrix-matching/
        normalVec.Normalize();
        float dist = -(Vector3.Dot(normalVec, actualPlane.transform.position));
        Vector4 obliquePlane = new Vector4(reversePlane.transform.forward.x, reversePlane.transform.forward.y, reversePlane.transform.forward.z, dist);
        Vector4 cameraSpaceObliquePlane = Matrix4x4.Transpose(Matrix4x4.Inverse(otherPortal.portalCam.GetComponent<Camera>().worldToCameraMatrix)) * obliquePlane;
        otherPortal.portalCam.GetComponent<Camera>().projectionMatrix = playerObject.GetComponent<Camera>().CalculateObliqueMatrix(cameraSpaceObliquePlane);
    }

    private void updateCameraTexture() {
       // actualPlane.GetComponent<Renderer>().material.shader.
    }

    public void update() {
        updateCameraRelativeToPlayer();
        updateNearClipPlane();
        otherPortal.updateCameraRelativeToPlayer();
        otherPortal.updateNearClipPlane();
        texturePortal();
    }

}
