using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareGunFiring : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource firingSound;
    public int forceConst;
    public GameObject flareBullet; 

    public GameObject player;
    void Start()
    {
        firingSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Camera.main.transform.rotation;
        //Checks every frame to see if you pressed fire (left click) while flare gun is enabled
        if (Input.GetButtonDown("FireGun")){
            fireFlareGun();
            //Debug.Log("firing flare gun");
        }
    }

    // Fire the Gun using forces instead of raycasting. 
    // The main idea is as follows: 
    // Spawn Bullet in front of gun 
    // Make sure Bullet's orientation is correct using Transform Rotations
    // Add force in the direction in which you want the gun to 'fire' 
    // Make sure bullet is set up with colliders correctly to detect impact! 
    // (This requires some work in the bullet prefab script!)
    // After you've done all your work, call fireFlareGunSounds() for fancy sounds!

    //In your on collisionenter script, call the onHit from the target if a hit is detected.
    void fireFlareGun(){
        //fireFlareGunSounds();
        //Cursor.lockState = CursorLockMode.Locked;
        
        GameObject newBullet = GameObject.Instantiate(flareBullet, this.transform.position + 0.25f * this.transform.forward + 0.1f * this.transform.up, Quaternion.Euler(new Vector3(0,0,0)));
        newBullet.GetComponent<Rigidbody>().AddForce(forceConst * this.transform.forward);
        Destroy(newBullet, 3f);
    }

    void fireFlareGunSounds(){
        firingSound.Play();
    }

    
}
