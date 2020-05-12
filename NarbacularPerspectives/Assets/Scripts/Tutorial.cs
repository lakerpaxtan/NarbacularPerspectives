using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Tutorial : MonoBehaviour
{
    GameManager gm;
    public GameObject panel;
    public List<GameObject> texts;
    public GameObject lightObject;

    // Start is called before the first frame update
    void Start()
    {
        gm = GetComponent<GameManager>();
        Portal testPortal = new Portal(4, 4, new Vector3(0, 2f, 0), new Vector3(1, 0, 0), gm.playerObject, "testOne", gm.portalPref, gm.borderMat);
        Portal testPortal2 = new Portal(4, 4, new Vector3(0, 12f, 0), new Vector3(1, 0, 0), gm.playerObject, "testTwo", gm.portalPref, gm.borderMat);
        Portal.pairPortals(testPortal, testPortal2);
        gm.portalList.Add(testPortal);
        //gm.TestPortals();
        lightObject.SetActive(false);
        StartCoroutine(FadeOut());
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<GameManager>().playerObject.transform.position.y < -25)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(5);
        Color alpha = panel.GetComponent<Image>().color;
        Color b = texts[0].GetComponent<TMPro.TextMeshProUGUI>().color;

        while (alpha.a >= 0 || b.a >= 0)
        {
            foreach (GameObject text in texts)
            {
                text.GetComponent<TMPro.TextMeshProUGUI>().color = b;
            }
            panel.GetComponent<Image>().color = alpha;
            alpha.a -= .025f;
            b.a -= .025f;

            yield return null;
        }

        foreach (GameObject text in texts)
        {
            text.SetActive(false);
        }

        panel.SetActive(false);
        lightObject.SetActive(true);
    }
}
