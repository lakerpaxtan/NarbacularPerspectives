using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PortalCreation : MonoBehaviour
{
    // check for double click
    bool one_click = false;
    float timer;
    public float doubleClickDelay = .2f;

    // ui kinda
    int stage = 0; // 0: no portals created / 1: portal a being created / 2: portal a created, portal b ready to place
    public float range = 20f;
    LineRenderer lineOfSight;

    public float offset = 0.04f;

    // check for overlap
    public GameObject planeA;
    public GameObject planeB;

    // portal meta
    Outline a;
    Outline b;

    public LineRenderer outlineA;
    public LineRenderer outlineB;
    public GameObject player;
    public GameObject gameManager;
    float numPortals;

    float scale;
    public Text scaleText;
    public Text text;
    public float readTime;

    class Outline
    {
        public LineRenderer outline;
        //public bool set;
        public Vector3 norm;
        public Vector3 mid;
        public float w;
        public float h;
        public GameObject attachedTo;

        GameObject c;

        public Outline(LineRenderer x, GameObject collider)
        {
            outline = x;
            c = collider;
        }

        public void Set(float height, float width, Vector3 m)
        {
            h = height;
            w = width;
            mid = m;
        }

        // Check that portal is on the same surface
        public bool IsValid()
        {
            Ray ray = new Ray(mid + norm, -norm);
            Physics.Raycast(ray, out RaycastHit x);
            for (int i = 0; i < 4; i++)
            {
                ray = new Ray(outline.GetPosition(i) + norm, -norm);
                Physics.Raycast(ray, out RaycastHit hit);
                if (hit.collider != x.collider)
                {
                    return false;
                }
            }

            return NotOverlapping();
        }

        bool NotOverlapping()
        {
            return true;
        }

        public void Validate(bool valid)
        {
            outline.enabled = valid;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        stage = 0;
        lineOfSight = gameObject.transform.GetComponentInChildren<LineRenderer>();
        outlineA.enabled = false;
        outlineB.enabled = false;
        numPortals = 0;

        a = new Outline(outlineA, planeA);
        b = new Outline(outlineB, planeB);

        text.text = "right click to create portal";
        StartCoroutine(Instruct());
    }

    IEnumerator Instruct()
    {
        text.enabled = true;
        text.text = text.text.ToUpper();
        yield return new WaitForSeconds(readTime);
        text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Raycasting
        Ray ray = new Ray(transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction);
        Physics.Raycast(ray, out RaycastHit hit, range);
        lineOfSight.SetPosition(0, ray.origin);
        if (hit.collider)
        {
            lineOfSight.SetPosition(1, hit.point);
        } else
        {
            lineOfSight.SetPosition(1, ray.GetPoint(range));
        }

        if (Input.GetButtonDown("Portal"))
        {
            if (one_click) // double click
            {
                one_click = false;
                Reset();
            }
            else // first click
            {
                one_click = true;
                timer = Time.time;

                if (hit.collider)
                {
                    if (stage == 0)
                    {
                        stage = 1;
                        a.norm = hit.normal;
                        a.attachedTo = hit.transform.gameObject;
                        StartA(a, hit.point);
                    }
                    else if (stage == 1)
                    {
                        scaleText.enabled = true;
                        scale = 1;
                        stage = 2;

                        float heightA = (a.outline.GetPosition(0) - a.outline.GetPosition(1)).magnitude;
                        float widthA = (a.outline.GetPosition(1) - a.outline.GetPosition(2)).magnitude;
                        Vector3 midA = (a.outline.GetPosition(0) + a.outline.GetPosition(2)) / 2;

                        a.Set(heightA, widthA, midA);
                    }
                    else if (stage == 2 && outlineB.enabled)
                    {
                        if (scale != 0)
                        {
                            StartCoroutine(SpecialPortal());
                        }
                    }
                }
                else
                {
                    Reset();
                }

            }
        }

        //if (stage == 1)
        //{
        //    UpdateShape(ray.GetPoint(range));
        //}

        if (stage == 1 && hit.collider && hit.collider == a.attachedTo.GetComponent<Collider>() 
            && hit.normal == a.norm)
        {
            UpdateA(a, hit.point);
        } else if (stage == 2 && hit.collider)
        {
            if (Input.GetAxis("Scale") != 0 )
            {
                if (scale > 0 || Input.GetAxis("Scale") > 0)
                {
                    scale += Input.GetAxis("Scale");
                } else
                {
                    scale = 0;
                }
                scaleText.text = "Scale: " + scale;
            }

            SetB(hit.point, hit.normal, hit.collider);
            b.attachedTo = hit.transform.gameObject;
            //DrawB(hit.point, hit.normal);
        }

        // reset double click
        if (one_click && Time.time - timer > doubleClickDelay)
        {
            one_click = false;
        }
    }

    void Reset()
    {
        stage = 0;
        a.outline.enabled = false;
        outlineB.enabled = false;
        scaleText.enabled = false;
    }
    void StartA(Outline a, Vector3 hit)
    {
        a.outline.enabled = true;
        a.outline.SetPosition(0, new Vector3(hit.x, hit.y, hit.z) + a.norm * offset);
        stage = 1;
    }

    void UpdateA(Outline p, Vector3 hit)
    {
        Vector3 b = hit + p.norm * offset;
        p.outline.SetPosition(2, b);

        Vector3 a = p.outline.GetPosition(0);

        if (Mathf.Abs(p.norm.z) > .5)
        {
            p.outline.SetPosition(1, new Vector3(a.x, hit.y, a.z));
            p.outline.SetPosition(3, new Vector3(hit.x, a.y, a.z));
        }
        else if (Mathf.Abs(p.norm.x) > .5)
        {
            p.outline.SetPosition(1, new Vector3(a.x, a.y, b.z));
            p.outline.SetPosition(3, new Vector3(a.x, b.y, a.z));
        }
        else if (Mathf.Abs(p.norm.y) > .5)
        {
            p.outline.SetPosition(1, new Vector3(a.x, a.y, b.z));
            p.outline.SetPosition(3, new Vector3(b.x, a.y, a.z));
        }
    }

    void SetB(Vector3 hit, Vector3 n, Collider collider = null)
    {
        b.norm = n;
        b.Set(a.h * scale, a.w * scale, hit + b.norm * offset);
        b.h = a.h * scale;
        b.w = a.w * scale;

        float height = b.h / 2;
        float width = b.w / 2;

        if (Mathf.Abs(b.norm.z) > .5)
        {
            b.outline.SetPosition(0, new Vector3(b.mid.x - width, b.mid.y + height, b.mid.z));
            b.outline.SetPosition(1, new Vector3(b.mid.x + width, b.mid.y + height, b.mid.z));
            b.outline.SetPosition(2, new Vector3(b.mid.x + width, b.mid.y - height, b.mid.z));
            b.outline.SetPosition(3, new Vector3(b.mid.x - width, b.mid.y - height, b.mid.z));
        }
        else if (Mathf.Abs(b.norm.x) > .5)
        {
            b.outline.SetPosition(0, new Vector3(b.mid.x, b.mid.y - width, b.mid.z + height));
            b.outline.SetPosition(1, new Vector3(b.mid.x, b.mid.y + width, b.mid.z + height));
            b.outline.SetPosition(2, new Vector3(b.mid.x, b.mid.y + width, b.mid.z - height));
            b.outline.SetPosition(3, new Vector3(b.mid.x, b.mid.y - width, b.mid.z - height));
        }
        else if (Mathf.Abs(b.norm.y) > .5)
        {
            b.outline.SetPosition(0, new Vector3(b.mid.x - width, b.mid.y, b.mid.z + height));
            b.outline.SetPosition(1, new Vector3(b.mid.x + width, b.mid.y, b.mid.z + height));
            b.outline.SetPosition(2, new Vector3(b.mid.x + width, b.mid.y, b.mid.z - height));
            b.outline.SetPosition(3, new Vector3(b.mid.x - width, b.mid.y, b.mid.z - height));
        }

        if (collider)
        {
            b.Validate(b.IsValid());
        }
    }

    // Sets B at any rotation/norm BUT won't be consistent parallel with ground...pros/cons?
    void DrawB(Vector3 hitPoint, Vector3 norm)
    {
        Vector3 midB = hitPoint + norm * offset;
        b.norm = norm;
        outlineB.enabled = true;

        float height = (outlineA.GetPosition(0) - outlineA.GetPosition(1)).magnitude / 2;
        float width = (outlineA.GetPosition(1) - outlineA.GetPosition(2)).magnitude / 2;

        Vector3 orth1 = midB;
        Vector3 orth2 = midB;

        Vector3.OrthoNormalize(ref norm, ref orth1, ref orth2);

        //Ray a = new Ray(midB, orth1);
        //Ray b = new Ray(midB, orth2);

        outlineB.SetPosition(0, midB - orth1 * width + orth2 * height);
        outlineB.SetPosition(1, midB + orth1 * width + orth2 * height);
        outlineB.SetPosition(2, midB + orth1 * width - orth2 * height);
        outlineB.SetPosition(3, midB - orth1 * width - orth2 * height);
    }

    // IEnumerator in case player resets portal instead of instantiates one
    IEnumerator SpecialPortal()
    {
        yield return new WaitForSeconds(doubleClickDelay);

        if (stage == 2)
        {
            CreatePortals(a, b);
            Reset();
        }
    }
    void CreatePortals(Outline a, Outline b)
    {
        Portal portalA = new Portal(a.w, a.h, a.mid, a.norm, player, "a" + numPortals);
        portalA.attachedTo = a.attachedTo;

        Portal portalB = new Portal(b.w, b.h, b.mid, b.norm, player, "b" + numPortals);
        portalB.attachedTo = b.attachedTo;

        Portal.pairPortals(portalA, portalB);
        gameManager.GetComponent<GameManager>().portalList.Add(portalA);

        numPortals++;
    }

    bool CheckOverlap(Outline b)
    {
        return true;
    }
}
