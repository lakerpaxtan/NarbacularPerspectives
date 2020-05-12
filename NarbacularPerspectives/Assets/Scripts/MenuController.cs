using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    AudioSource audioSource;
    public AudioClip typing;
    public GameObject mainText;
    public GameObject background;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(WriteText("Narbacular \nPerspectives", mainText));
        StartCoroutine(MoveBackground());
    }

    // Update is called once per frame
    void Update()
    {
        
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

        yield return new WaitForSeconds(1f);
        Color alpha = background.GetComponent<Image>().color;
        alpha.a = 1;
        while (alpha.a >= 0)
        {
            mainText.GetComponent<TMPro.TextMeshProUGUI>().color = alpha;
            alpha.a -= .025f;
            yield return null;
        }

        mainText.SetActive(false);
    }

    IEnumerator MoveBackground()
    {
        Color alpha = background.GetComponent<Image>().color;
        alpha.a = 0;
        float x = 0.005f;
        while (background.transform.position.y < 800)
        {
            background.transform.position += Vector3.up * Time.deltaTime * 75;
            alpha.a += x;

            if (alpha.a > .6f && x > 0)
            {
                x -= .001f;
            } 

            background.GetComponent<Image>().color = alpha;
            yield return null;
        }
    }
}
