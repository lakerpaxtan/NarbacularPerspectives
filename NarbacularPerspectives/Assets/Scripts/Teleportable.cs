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

    Vector3 lastKnownSwapPos;

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
                Debug.Log("special boi");
                teleported = true;
                telePortal.otherPortal.reversePlane.SetActive(false);
                //Debug.Log("disable" + telePortal.otherPortal.borderPlane.name);
                //telePortal.borderPlane.SetActive(false);
                teleportObject(copiedObject.transform.position +  deadzone *(telePortal.otherPortal.normalVec));
                //telePortal.borderPlane.SetActive(true);
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
                }

            }    
        }
    }

    private void OnTriggerEnter(Collider other) {
        
        Debug.Log("ON TRIGGER ENTER START");
        ignoreUpdate = false;
        if (Portal.portalTable.ContainsKey(other.gameObject)) {
            telePortal = Portal.portalTable[other.gameObject];
            //telePortal.otherPortal.reversePlane.SetActive(false);
            //Debug.Log("disable" + telePortal.borderPlane.name);
            
            insidePortal = true;
            copiedObject = Instantiate(this.gameObject);
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
            } 
            else if (copiedObject.tag == "Bullet") {
                Destroy(copiedObject.GetComponent<CapsuleCollider>());
                Destroy(copiedObject.GetComponent<Teleportable>());
            }   
        }
        Debug.Log("ON TRIGGER ENTER STOP");
    }

    private void OnTriggerExit(Collider other){
        Debug.Log("ON TRIGGER EXIT START");
        if(teleported){
            Debug.Log("ultiamte boi");
            teleported = false;
            telePortal.otherPortal.reversePlane.SetActive(false);
            //Debug.Log("enable" + telePortal.borderPlane.name);
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
        Debug.Log("ON TRIGGER EXIT STOP");
    }

    void enableFPC() {
        this.gameObject.GetComponent<FirstPersonController>().enabled = true; 
    }


    void teleportObject(Vector3 pos){
        Debug.Log("TELEPORTING");
        this.gameObject.transform.rotation = this.copiedObject.transform.rotation;
        this.gameObject.transform.position = pos;
    }
}
