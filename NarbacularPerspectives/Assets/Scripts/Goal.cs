using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    public GameObject text;
    int score;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        text.GetComponent<Text>().text = "Score: " + score;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "flareBullet(Clone)") {
            score++;
            Destroy(other.gameObject);
        }
    }
}
