using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal
{
    private bool isPaired = false; 
    private Vector2 topLeftCoord;
    private Vector2 bottomRightCoord;
    private Vector3 gameObjectPos;
    public Vector3 normalVec;
    public Portal otherPortal;
    public GameObject attachedTo;

    public GameObject portalCam;

    public GameObject prefabPlane;

    public static int recursiveRenderLimit = 8;

    public GameObject actualPlane;
    public GameObject reversePlane;
    public GameObject borderPlane;
    float width;
    float height;

    public Material borderMat;
    private GameObject playerObject; 

    public float scaleFactor;
    private string name;
    private RenderTexture cameraTexture;
    public static Shader portalShader;

    public static Dictionary<GameObject, Portal> portalTable = new Dictionary<GameObject, Portal>();
    
    public Portal(float width, float height, Vector3 middlePos, Vector3 normal, GameObject player, string str, GameObject planePrefab, Material borderMat)
    {
        this.width = width;
        this.height = height;
        gameObjectPos = middlePos;
        normalVec = normal;
        normalVec.Normalize();
        playerObject = player;
        this.borderMat = borderMat;
        name = str;
        cameraTexture = new RenderTexture(Screen.width, Screen.height, 0);
        portalShader = Shader.Find("Unlit/PortalShader");
        this.prefabPlane = planePrefab;

        createPortalBoundaries();
        createPortalCamera();
        
    }

    
    //Gonna use a plane for now --> Planning on just drawing right in front of it
    private void createPortalBoundaries()
    {

        actualPlane = MonoBehaviour.Instantiate(prefabPlane);
        actualPlane.tag = "Portal";
        actualPlane.transform.position = gameObjectPos;
        actualPlane.name = name;
        actualPlane.transform.localScale = new Vector3(width, height, 1);
        actualPlane.transform.rotation = Quaternion.FromToRotation(Vector3.forward, -normalVec);

        borderPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        borderPlane.tag = "Portal";
        UnityEngine.Object.Destroy(borderPlane.GetComponent<MeshCollider>());
        borderPlane.transform.position = gameObjectPos - 0.01f * normalVec;
        borderPlane.name = name + "border";
        borderPlane.transform.localScale = 1.05f* actualPlane.transform.localScale;
        borderPlane.transform.rotation = actualPlane.transform.rotation;
        borderPlane.transform.GetComponent<Renderer>().material = borderMat;

        reversePlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        reversePlane.tag = "Portal";
        UnityEngine.Object.Destroy(reversePlane.GetComponent<MeshCollider>());
        reversePlane.transform.position = gameObjectPos;
        reversePlane.name = name + "reversePlane";
        reversePlane.transform.localScale = new Vector3(width, height, 1);
        reversePlane.AddComponent<MeshCollider>();
        reversePlane.transform.rotation = Quaternion.LookRotation(normalVec, -actualPlane.transform.up);

        actualPlane.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        actualPlane.GetComponent<MeshRenderer>().receiveShadows = false;

        borderPlane.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        borderPlane.GetComponent<MeshRenderer>().receiveShadows = false;

        reversePlane.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        reversePlane.GetComponent<MeshRenderer>().receiveShadows = false;
        reversePlane.layer = 1;
        
        //borderPlane.SetActive(false);
        //reversePlane.SetActive(false);
        

        this.setupTrigger();
        portalTable.Add(actualPlane, this);
    }

    private void setupTrigger() {
        UnityEngine.Object.Destroy(this.actualPlane.gameObject.GetComponent<MeshCollider>()); 
       
        BoxCollider tempColl = actualPlane.gameObject.AddComponent<BoxCollider>();
        tempColl.isTrigger = true;
        tempColl.size = new Vector3(1,1,0.70f);
        //tempColl.center += new Vector3(0,0,-.10f);
    }


    private void createPortalCamera() {
        this.portalCam = new GameObject();
        Camera tempCam = portalCam.AddComponent<Camera>();
        tempCam.enabled = false;
        tempCam.cullingMask = tempCam.cullingMask & ~(1<<1);
        portalCam.name = name + "Cam"; 
        
    }


    public void updateCameraRelativeToPlayer() {
        if (isPaired) {
            
            Vector3 oldReverse = this.otherPortal.reversePlane.transform.localScale;
            this.otherPortal.reversePlane.transform.localScale = this.actualPlane.transform.localScale;
            
            Vector3 relativePos = actualPlane.transform.InverseTransformPoint(playerObject.transform.position);
            this.portalCam.transform.position = this.otherPortal.reversePlane.transform.TransformPoint(relativePos);

            Quaternion relativeRot = Quaternion.FromToRotation(-normalVec, this.otherPortal.normalVec);
            this.portalCam.transform.rotation = relativeRot * this.playerObject.transform.rotation;

            if (portalCam.transform.eulerAngles[2] == 180 || portalCam.transform.eulerAngles[2] == -180 || normalVec == otherPortal.normalVec){
                //Debug.Log("rotate around me daddy");
                this.portalCam.transform.RotateAround(this.otherPortal.actualPlane.transform.position, this.otherPortal.actualPlane.transform.forward, 180);
            }

           
            this.otherPortal.reversePlane.transform.localScale = oldReverse;

        }
    }

    public static void pairPortals(Portal portal1, Portal portal2) {
        portal1.isPaired = true;
        portal2.isPaired = true; 

        portal1.otherPortal = portal2;
        portal2.otherPortal = portal1;

        portal2.scaleFactor = (portal1.width * portal1.height) / (portal2.width * portal2.height);
        portal1.scaleFactor = (portal2.width * portal2.height) /  (portal1.width * portal1.height);

        //portal2.portalCam.transform.localScale *= portal1.scaleFactor;
        //portal1.portalCam.transform.localScale *= portal2.scaleFactor;

    }

    public void texturePortal() {
        actualPlane.GetComponent<Renderer>().material.shader = portalShader;
        otherPortal.actualPlane.GetComponent<Renderer>().material.shader = portalShader;

        portalCam.GetComponent<Camera>().targetTexture = cameraTexture;
        otherPortal.portalCam.GetComponent<Camera>().targetTexture = otherPortal.cameraTexture;

        actualPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", Texture2D.blackTexture);
        otherPortal.actualPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", Texture2D.blackTexture);

        portalCam.GetComponent<Camera>().Render();
        otherPortal.portalCam.GetComponent<Camera>().Render();

        actualPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", cameraTexture);
        otherPortal.actualPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", otherPortal.cameraTexture);

        renderCamera();
        otherPortal.renderCamera(); 
    }

   

    public void updateNearClipPlane() {
        // Proper oblique clipping plane technique thanks to: https://danielilett.com/2019-12-18-tut4-3-matrix-matching/
        normalVec.Normalize();
        float dist = -(Vector3.Dot(normalVec, actualPlane.transform.position));
        Vector4 obliquePlane = new Vector4(reversePlane.transform.forward.x, reversePlane.transform.forward.y, reversePlane.transform.forward.z, dist);
        Vector4 cameraSpaceObliquePlane = Matrix4x4.Transpose(Matrix4x4.Inverse(otherPortal.portalCam.GetComponent<Camera>().worldToCameraMatrix)) * obliquePlane;
        otherPortal.portalCam.GetComponent<Camera>().projectionMatrix = playerObject.transform.GetComponent<Camera>().CalculateObliqueMatrix(cameraSpaceObliquePlane);
    }

    private void updateCameraTexture() {
       // actualPlane.GetComponent<Renderer>().material.shader.
    }

    public void update() {
        
        updateNearClipPlane();
        otherPortal.updateNearClipPlane();
        texturePortal();
    }


    public void renderCamera() {
        ArrayList positions = new ArrayList();
        ArrayList rotations = new ArrayList();
        
        Vector3 tempPos = new Vector3(this.playerObject.transform.position[0], this.playerObject.transform.position[1], this.playerObject.transform.position[2]);
        Quaternion tempRot = new Quaternion(this.playerObject.transform.rotation[0], this.playerObject.transform.rotation[1], this.playerObject.transform.rotation[2], this.playerObject.transform.rotation[3]);

        for (int i = 0; i < recursiveRenderLimit; i ++) {
            Vector3 oldReverse = this.otherPortal.reversePlane.transform.localScale;
            this.otherPortal.reversePlane.transform.localScale = this.actualPlane.transform.localScale;


            Vector3 relativePos = actualPlane.transform.InverseTransformPoint(tempPos);
            tempPos = this.otherPortal.reversePlane.transform.TransformPoint(relativePos);

            Quaternion relativeRot = Quaternion.FromToRotation(-normalVec, this.otherPortal.normalVec);
            tempRot = relativeRot * tempRot;

            this.otherPortal.reversePlane.transform.localScale = oldReverse;
            
            positions.Add(new Vector3(tempPos[0], tempPos[1], tempPos[2]));
            rotations.Add(new Quaternion(tempRot[0], tempRot[1], tempRot[2], tempRot[3]));
        }

        for (int i = recursiveRenderLimit - 1; i >= 0; i -= 1) {
            this.portalCam.transform.position = (Vector3) positions[i];
            this.portalCam.transform.rotation = (Quaternion) rotations[i];

            if (portalCam.transform.eulerAngles[2] == 180 || portalCam.transform.eulerAngles[2] == -180 || normalVec == otherPortal.normalVec){
                this.portalCam.transform.RotateAround(this.otherPortal.actualPlane.transform.position, this.otherPortal.actualPlane.transform.forward, 180);
            }

            //updateNearClipPlane();
            portalCam.GetComponent<Camera>().Render();
        }
    }



}
