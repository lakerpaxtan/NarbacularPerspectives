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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {     
    
        if(insidePortal){
            if(this.gameObject.tag == "MainCamera"){
                pastHalfway = Vector3.Dot(telePortal.normalVec, this.gameObject.transform.GetChild(0).transform.position - telePortal.actualPlane.transform.position) > 0 ? false: true;
                Debug.Log(pastHalfway + " " + telePortal.actualPlane.gameObject.name);
                
                if(pastHalfway){
                    
                    ignoreUpdate = true;
                    teleportObject(copiedObject.transform.position);

                    //copiedObject = null;
                    //telePortal = null; 
                    //telePortal = telePortal.otherPortal;
                }else{
                    Vector3 relativePos = telePortal.actualPlane.transform.InverseTransformPoint(this.gameObject.transform.position);
                    this.copiedObject.transform.position = this.telePortal.otherPortal.reversePlane.transform.TransformPoint(relativePos);
                }
                
                




            }


        }
    }

    private void OnTriggerEnter(Collider other){
        Debug.Log("ON TRIGGER ENTER");
        ignoreUpdate = false;
        if (Portal.portalTable.ContainsKey(other.gameObject)){
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
            }
            
        }
    }

    private void OnTriggerExit(Collider other){
        Debug.Log("ON TRIGGER EXIT");
        if (Portal.portalTable.ContainsKey(other.gameObject) && other.gameObject == telePortal.actualPlane){
            telePortal = null;
            insidePortal = false;
            Destroy(copiedObject);
            copiedObject = null;
        }
    }

    void enableFPC() {
        this.gameObject.GetComponent<FirstPersonController>().enabled = true; 
    }


    void teleportObject(Vector3 pos){
        this.gameObject.GetComponent<FirstPersonController>().enabled = false; 
        this.gameObject.transform.position = pos;
        Invoke("enableFPC", 0.017f);
    }





}
