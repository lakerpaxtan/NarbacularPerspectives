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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   
        //Debug.Log("update");  
        if(insidePortal){
            if(this.gameObject.tag == "MainCamera"){
                Vector3 relativePos = telePortal.actualPlane.transform.InverseTransformPoint(this.gameObject.transform.position);
                lastKnownSwapPos = this.telePortal.otherPortal.reversePlane.transform.TransformPoint(relativePos);
                this.copiedObject.transform.position = lastKnownSwapPos;
            }
        }
    }

    private void OnTriggerEnter(Collider other){
        //Debug.Log("ON TRIGGER ENTER START");
        ignoreUpdate = false;
        if (Portal.portalTable.ContainsKey(other.gameObject)) {
            telePortal = Portal.portalTable[other.gameObject];
            insidePortal = true;
            copiedObject = Instantiate(this.gameObject);
            if(copiedObject.tag == "MainCamera"){
                Destroy(copiedObject.GetComponent<FirstPersonController>());
                Destroy(copiedObject.GetComponent<AudioSource>());
                copiedObject.GetComponent<Rigidbody>().isKinematic = true;
                Destroy(copiedObject.GetComponent<CharacterController>());
                Destroy(copiedObject.GetComponentInChildren<FlareLayer>());
                Destroy(copiedObject.GetComponentInChildren<Camera>());
                Destroy(copiedObject.GetComponentInChildren<FlareGunFiring>());
                Destroy(copiedObject.GetComponentInChildren<AudioListener>());

                Destroy(copiedObject.GetComponent<Teleportable>());
            }    
        }
        //Debug.Log("ON TRIGGER ENTER STOP");
    }

    private void OnTriggerExit(Collider other){
        //Debug.Log("ON TRIGGER EXIT START");
        // if (Portal.portalTable.ContainsKey(other.gameObject) && other.gameObject == telePortal.actualPlane) {
        //     telePortal = null;
        //     insidePortal = false;
        //     Destroy(copiedObject);
        //     copiedObject = null;
        // }
        pastHalfway = Vector3.Dot(telePortal.normalVec, this.gameObject.transform.GetChild(0).transform.position - telePortal.actualPlane.transform.position) > 0 ? false: true;
        if (pastHalfway) {
            teleportObject(copiedObject.transform.position);
        }
        Destroy(copiedObject);
        //Debug.Log("ON TRIGGER EXIT STOP");
    }

    void enableFPC() {
        this.gameObject.GetComponent<FirstPersonController>().enabled = true; 
    }


    void teleportObject(Vector3 pos){
        Debug.Log("TELEPORTING");
        this.gameObject.GetComponent<FirstPersonController>().enabled = false; 
        this.gameObject.transform.position = pos;
        //this.gameObject.GetComponent<FirstPersonController>().enabled = true;
        Invoke("enableFPC", 0.05f);
    }
}
