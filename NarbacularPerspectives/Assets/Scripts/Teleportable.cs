using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class Teleportable : MonoBehaviour
{
    public bool insidePortal = false;
    bool pastHalfway = false;
    Portal telePortal;
    GameObject copiedObject; 
    //Gives more leeway on camera obstruction when teleporting
    float deadzone2 = 0.20f;
    //Gives more leeway on strafing 
    float deadzone = 0.40f;
    public bool teleported = false;
    private GameObject capsuleObject;
    private Material originalMat;
    public Material shaderMat;

    

    public GameObject playerCross;
    public GameObject objectCross;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(insidePortal){
            updateCopy();
            pastHalfway = Vector3.Dot(telePortal.normalVec, this.gameObject.transform.position - (deadzone2*telePortal.normalVec + telePortal.actualPlane.transform.position )) > 0 ? false: true;
            if (pastHalfway && gameObject.tag == "Player") {
                teleported = true;
                telePortal.otherPortal.reversePlane.SetActive(false);
                Debug.Log(copiedObject.transform.position + "before tp and math");
                Debug.Log(copiedObject.transform.position +  deadzone *(copiedObject.transform.forward));
                teleportObject(copiedObject.transform.position +  deadzone *(copiedObject.transform.forward));
                insidePortal = false;
                Destroy(copiedObject);
                removeCrossSections(); 
            }  
        }
    }
    
    private void updateCopy(){
        Vector3 relativePos = telePortal.actualPlane.transform.InverseTransformPoint(this.gameObject.transform.position);
        
        //Needed for Local Scale Teleporting
        //Vector3 tempVec = this.telePortal.otherPortal.reversePlane.transform.localScale;
        //this.telePortal.otherPortal.reversePlane.transform.localScale = this.telePortal.actualPlane.transform.localScale;
        this.copiedObject.transform.position  = this.telePortal.otherPortal.reversePlane.transform.TransformPoint(relativePos);
        //this.telePortal.otherPortal.reversePlane.transform.localScale = tempVec;
        

        // Rotate Clone
        Vector3 relativeRot = telePortal.actualPlane.transform.InverseTransformDirection(this.gameObject.transform.rotation * Vector3.right);
        this.copiedObject.transform.rotation = Quaternion.LookRotation(this.telePortal.otherPortal.reversePlane.transform.TransformDirection(relativeRot), Vector3.up);
        this.copiedObject.transform.RotateAround(this.copiedObject.transform.position, this.copiedObject.transform.up, -90);

        if (this.copiedObject.transform.eulerAngles[2] == 180 || this.copiedObject.transform.eulerAngles[2] == -180 || telePortal.normalVec == telePortal.otherPortal.normalVec) {
            this.copiedObject.transform.RotateAround(this.telePortal.otherPortal.actualPlane.transform.position, this.telePortal.otherPortal.actualPlane.transform.forward, 180);
            this.copiedObject.transform.RotateAround(this.copiedObject.transform.position, this.copiedObject.transform.right, 180);
        }

        //Debug.Log(copiedObject.transform.position + "update");
    }



    private void OnTriggerEnter(Collider other) {
        Debug.Log("On Trigger Enter" + other.gameObject.name);
        //Not inside portal is to make sure you arent already in a portal bc they are too close together
        if (Portal.portalTable.ContainsKey(other.gameObject) && !insidePortal) {
            telePortal = Portal.portalTable[other.gameObject];
            

            //Wall Portal specifics
            if (telePortal.attachedTo){
                telePortal.attachedTo.GetComponent<Collider>().enabled = false;
                telePortal.otherPortal.reversePlane.SetActive(false);
            }


            insidePortal = true;
            copiedObject = Instantiate(this.gameObject, this.gameObject.transform.position, this.gameObject.transform.rotation);
            copiedObject.transform.localScale = this.gameObject.transform.localScale * Mathf.Sqrt(telePortal.scaleFactor);
            if (copiedObject.transform.localScale[0] < 0.010f){
                copiedObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }

            

            if(copiedObject.tag == "Player"){
                capsuleObject = this.gameObject.transform.GetChild(0).GetChild(0).gameObject;
                telePortal.borderPlane.SetActive(false);
                telePortal.reversePlane.SetActive(false);
                Destroy(copiedObject.GetComponent<FirstPersonController>());
                Destroy(copiedObject.GetComponent<AudioSource>());
                copiedObject.GetComponent<Rigidbody>().isKinematic = true;
                Destroy(copiedObject.GetComponent<CharacterController>());
                Destroy(copiedObject.GetComponentInChildren<FlareLayer>());
                Destroy(copiedObject.GetComponentInChildren<Camera>());
                Destroy(copiedObject.GetComponentInChildren<FlareGunFiring>());
                Destroy(copiedObject.transform.GetChild(0).GetComponentInChildren<AudioListener>());
                Destroy(copiedObject.GetComponent<FirstPersonAIO>());
                Destroy(copiedObject.GetComponent<Teleportable>());
                Destroy(copiedObject.GetComponent<CapsuleCollider>());
                Destroy(copiedObject.transform.GetChild(0).GetChild(0).GetComponentInChildren<PortalCreation>());
                setupCrossSections();            
            } 
            else if (copiedObject.tag == "Bullet") {
                Destroy(copiedObject.GetComponent<CapsuleCollider>());
                Destroy(copiedObject.GetComponent<Teleportable>());
            }   
        }
    }

    private void OnTriggerExit(Collider other){
        //Case where you enter another portal's trigger before exiting the original because they are back to back (too close together)
        Debug.Log("On Trigger Exit" + other.gameObject.name);
        if(telePortal.actualPlane != other.gameObject){
            //Debug.Log("WOAH DUDE");
            return;
        }
        //Resetting the reversePlane
       
        telePortal.reversePlane.SetActive(true);
        
        //Resetting the borderPlane
        telePortal.borderPlane.SetActive(true);

        //Skipping if already pre-empted
        if (teleported) {
            //Debug.Log("blublu");
            teleported = false;
            telePortal.otherPortal.reversePlane.SetActive(false);
            //Reset collider if teleported
            if (telePortal.attachedTo) {
                //Debug.Log("blah");
                telePortal.attachedTo.GetComponent<Collider>().enabled = true;
            }
            return;
        }

        //Checking if past halfway on exit
        pastHalfway = Vector3.Dot(telePortal.normalVec, this.gameObject.transform.position - telePortal.actualPlane.transform.position) > 0 ? false: true;
        if (pastHalfway) {
            //Debug.Log("did a weird exit");
            teleportObject(copiedObject.transform.position);
        }

        insidePortal = false;
        Destroy(copiedObject);
        //Debug.Log(this.gameObject.tag);
        if(this.gameObject.tag == "Player"){
            removeCrossSections(); 
        }
        //Reset Collider if leaving
        if (telePortal.attachedTo) {
            //Debug.Log("blah");
            telePortal.attachedTo.GetComponent<Collider>().enabled = true;
        }
    }

    void enableFPC() {
        this.gameObject.GetComponent<FirstPersonController>().enabled = true; 
    }


    void teleportObject(Vector3 pos) {
        if (this.tag == "Player") {
            this.gameObject.GetComponent<FirstPersonAIO>().enableCameraMovement = false;       
            Vector3 cameraRot = new Vector3(-this.gameObject.transform.GetChild(0).GetChild(1).transform.rotation.eulerAngles[0], this.copiedObject.transform.eulerAngles[1], this.copiedObject.transform.eulerAngles[2]);
            Vector3 bodyRot = new Vector3(this.copiedObject.transform.eulerAngles[0], this.copiedObject.transform.eulerAngles[1], this.copiedObject.transform.eulerAngles[2]);
            this.gameObject.transform.eulerAngles = bodyRot;
            this.gameObject.GetComponent<FirstPersonAIO>().targetAngles = cameraRot;
            this.gameObject.GetComponent<FirstPersonAIO>().followAngles = cameraRot;
            this.gameObject.transform.position = pos;
            this.gameObject.GetComponent<FirstPersonAIO>().enableCameraMovement = true;

            //Debug.Log("I AM DIFF SIZE MY DUDE " + telePortal.scaleFactor);
            //Debug.Log(this.gameObject.transform.localScale + this.gameObject.name);
            this.gameObject.transform.localScale = this.gameObject.transform.localScale * Mathf.Sqrt(telePortal.scaleFactor);
            if (gameObject.transform.localScale[0] < 0.01f){
                gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }
            Debug.Log("finished scaling");
            //Debug.Log(this.gameObject.transform.localScale + this.gameObject.name);
            this.gameObject.transform.GetChild(0).GetChild(1).GetComponent<Camera>().nearClipPlane *= Mathf.Sqrt(telePortal.scaleFactor);
            this.gameObject.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<PortalCreation>().lineOfSight.widthMultiplier *= Mathf.Sqrt(telePortal.scaleFactor);
            //this.telePortal.portalCam.transform.localScale *= telePortal.scaleFactor;
            //this.telePortal.otherPortal.portalCam.transform.localScale *= telePortal.scaleFactor;


        } else if (this.tag == "Bullet") {
            Rigidbody tempRB = this.GetComponent<Rigidbody>();
            Vector3 tempVelocityMag = this.gameObject.transform.InverseTransformDirection(tempRB.velocity);
            this.gameObject.transform.rotation = this.copiedObject.transform.rotation;
            this.gameObject.transform.localRotation = this.copiedObject.transform.localRotation;
            this.gameObject.transform.position = this.copiedObject.transform.position;
            tempRB.velocity = this.gameObject.transform.TransformDirection(tempVelocityMag);
            this.gameObject.transform.localScale = this.gameObject.transform.localScale * Mathf.Sqrt(telePortal.scaleFactor);
        }
        
    }


    private void setupCrossSections(){
        
        OnePlaneCuttingController playerController = capsuleObject.AddComponent<OnePlaneCuttingController>();
        OnePlaneCuttingController objectController = copiedObject.transform.GetChild(0).GetChild(0).gameObject.AddComponent<OnePlaneCuttingController>();

        MeshRenderer capsuleMesh = capsuleObject.GetComponent<MeshRenderer>();
        originalMat = capsuleMesh.material;
        capsuleMesh.material = shaderMat;

        GameObject tempGun = capsuleObject.transform.GetChild(0).gameObject;
        foreach (Transform child in tempGun.transform){
            child.GetComponent<MeshRenderer>().material = capsuleMesh.material;
        }

        playerCross.transform.position = telePortal.reversePlane.transform.position;
        playerCross.transform.rotation = telePortal.reversePlane.transform.rotation;

        playerController.plane = playerCross;

        objectCross.transform.position = telePortal.otherPortal.reversePlane.transform.position;
        objectCross.transform.rotation = telePortal.otherPortal.reversePlane.transform.rotation;

        objectController.plane = objectCross;

        GameObject copiedCapsule = copiedObject.transform.GetChild(0).GetChild(0).gameObject;
        copiedCapsule.GetComponent<MeshRenderer>().material = shaderMat;
        tempGun = copiedCapsule.transform.GetChild(0).gameObject;
        foreach (Transform child in tempGun.transform){
            child.GetComponent<MeshRenderer>().material = copiedCapsule.GetComponent<MeshRenderer>().material;
        }
    }

    private void removeCrossSections(){

        MeshRenderer capsuleMesh = capsuleObject.GetComponent<MeshRenderer>();
        capsuleMesh.material = originalMat;

        
        foreach (Transform child in capsuleObject.transform.GetChild(0).transform){
            child.GetComponent<MeshRenderer>().material = capsuleMesh.material;
        }


        Destroy(capsuleObject.GetComponent<OnePlaneCuttingController>());

    }
}
