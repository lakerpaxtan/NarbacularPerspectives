using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    public GameObject mainText;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WriteText("Narbacular \nPerspectives", mainText));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WriteText(string str, GameObject text)
    {
        
        while (str != "")
        {
            mainText.GetComponent<TMPro.TextMeshProUGUI>().text += str[0];
            str = str.Substring(1);
            yield return new WaitForSeconds(.25f);
        }
    }
}
