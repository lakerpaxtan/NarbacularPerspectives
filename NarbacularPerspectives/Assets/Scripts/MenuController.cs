using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    AudioSource audioSource;
    public AudioClip typing;
    public GameObject mainText;
    public GameObject button1;
    public GameObject button2;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(WriteText("Narbacular \nPerspectives", mainText));

        button1.SetActive(false);
        button2.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    public void StartExperiment()
    {
        SceneManager.LoadScene(1);
    }


    IEnumerator WriteText(string str, GameObject text)
    {
        audioSource.clip = typing;
        audioSource.pitch = 0.75f;
        audioSource.Play();
        mainText.GetComponent<TMPro.TextMeshProUGUI>().text = "";

        while (str != "")
        {
            mainText.GetComponent<TMPro.TextMeshProUGUI>().text += str[0];
            str = str.Substring(1);
            yield return new WaitForSeconds(.25f);
        }

        audioSource.Stop();
        audioSource.pitch = 1;
        button1.SetActive(true);
        button2.SetActive(true);
    }

}
