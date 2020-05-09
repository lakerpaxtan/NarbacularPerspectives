using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class Teleportable : MonoBehaviour
{
    bool insidePortal = false;
    bool pastHalfway = false;
    Portal telePortal;
    bool ignoreUpdate = false;
    GameObject copiedObject; 
    float deadzone = 0.30f;
    public bool teleported = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {   
        if(insidePortal){
            pastHalfway = Vector3.Dot(telePortal.normalVec, this.gameObject.transform.position - telePortal.actualPlane.transform.position ) > 0 ? false: true;
            if (pastHalfway && gameObject.tag == "Player") {
                teleported = true;
                telePortal.otherPortal.reversePlane.SetActive(false);
                teleportObject(copiedObject.transform.position +  deadzone *(telePortal.otherPortal.normalVec));
                insidePortal = false;
                Destroy(copiedObject);
            } else {
                Vector3 relativePos = telePortal.actualPlane.transform.InverseTransformPoint(this.gameObject.transform.position);
                this.copiedObject.transform.position  = this.telePortal.otherPortal.reversePlane.transform.TransformPoint(relativePos);

                // Rotate Clone
                Vector3 relativeRot = telePortal.actualPlane.transform.InverseTransformDirection(this.gameObject.transform.rotation * Vector3.right);
                this.copiedObject.transform.rotation = Quaternion.LookRotation(this.telePortal.otherPortal.reversePlane.transform.TransformDirection(relativeRot), Vector3.up);
                this.copiedObject.transform.RotateAround(this.copiedObject.transform.position, this.copiedObject.transform.up, -90);

                if (this.copiedObject.transform.eulerAngles[2] == 180 || this.copiedObject.transform.eulerAngles[2] == -180 || telePortal.normalVec == telePortal.otherPortal.normalVec) {
                    this.copiedObject.transform.RotateAround(this.telePortal.otherPortal.actualPlane.transform.position, this.telePortal.otherPortal.actualPlane.transform.forward, 180);
                    this.copiedObject.transform.RotateAround(this.copiedObject.transform.position, this.copiedObject.transform.right, 180);
                }
            }    
        }
    }

    private void OnTriggerEnter(Collider other) {
        ignoreUpdate = false;
        if (Portal.portalTable.ContainsKey(other.gameObject)) {
            telePortal = Portal.portalTable[other.gameObject];
            if (telePortal.attachedTo){
                telePortal.attachedTo.GetComponent<Collider>().enabled = false;
            }
            insidePortal = true;
            copiedObject = Instantiate(this.gameObject, this.gameObject.transform.position, this.gameObject.transform.rotation);
            if(copiedObject.tag == "Player"){
                telePortal.borderPlane.SetActive(false);
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
            } 
            else if (copiedObject.tag == "Bullet") {
                Destroy(copiedObject.GetComponent<CapsuleCollider>());
                Destroy(copiedObject.GetComponent<Teleportable>());
            }   
        }
    }

    private void OnTriggerExit(Collider other){
        if (teleported) {
            teleported = false;
            telePortal.otherPortal.reversePlane.SetActive(false);
            telePortal.borderPlane.SetActive(true);
            return;
        }
        pastHalfway = Vector3.Dot(telePortal.normalVec, this.gameObject.transform.position - telePortal.actualPlane.transform.position) > 0 ? false: true;
        if (pastHalfway) {
            teleportObject(copiedObject.transform.position);
        }
        insidePortal = false;
        Destroy(copiedObject);
        telePortal.reversePlane.SetActive(true);
        telePortal.borderPlane.SetActive(true);
        if (telePortal.attachedTo) {
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
        } else if (this.tag == "Bullet") {
            Rigidbody tempRB = this.GetComponent<Rigidbody>();
            Vector3 tempVelocityMag = this.gameObject.transform.InverseTransformDirection(tempRB.velocity);
            this.gameObject.transform.rotation = this.copiedObject.transform.rotation;
            this.gameObject.transform.localRotation = this.copiedObject.transform.localRotation;
            this.gameObject.transform.position = this.copiedObject.transform.position;
            tempRB.velocity = this.gameObject.transform.TransformDirection(tempVelocityMag);
        }
    }
}
