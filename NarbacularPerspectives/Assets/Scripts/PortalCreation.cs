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
    float speed = .1f;
    public LineRenderer lineOfSight;

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

    public Material borderMat;
    public GameObject prefPortal;

    float scale;
    public Text scaleText;

    class Outline
    {
        public LineRenderer outline;
        public Vector3 norm;
        public Vector3 mid;
        public float w;
        public float h;
        public GameObject attachedTo;
        public GameObject c;

        public Outline(LineRenderer x, GameObject collider)
        {
            this.outline = x;
            this.c = collider;
            this.c.SetActive(false);
        }

        public void Set(float height, float width, Vector3 m)
        {
            this.h = height;
            this.w = width;
            this.mid = m;
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
                    Validate(false);
                    return false;
                }
            }
            bool v = NotOverlapping();
            Validate(v);
            return v;
        }

        bool NotOverlapping()
        {
            this.c.SetActive(true);
            this.c.GetComponent<PortalOverlap>().UpdatePlane(mid, norm, w, h);
            return c.GetComponent<PortalOverlap>().valid;
        }

        public void Validate(bool valid)
        {
            if (valid)
            {
                this.outline.startColor = this.outline.endColor = Color.white;
            }
            else
            {
                this.outline.startColor = this.outline.endColor = Color.red;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        numPortals = 0;
        a = new Outline(outlineA, planeA);
        b = new Outline(outlineB, planeB);
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        // Raycasting line of sight
        Ray ray = new Ray(transform.position, Camera.main.ScreenPointToRay(Input.mousePosition).direction);

        Physics.Raycast(ray, out RaycastHit hit, range);
        lineOfSight.SetPosition(0, ray.origin);

        if (hit.collider)
        {
            lineOfSight.SetPosition(1, hit.point);
        }
        else
        {
            lineOfSight.SetPosition(1, ray.GetPoint(range));
        }

        // Start creating portals
        if (Input.GetButtonDown("Portal"))
        {
            if (one_click) // double click -- cancel portal
            {
                one_click = false;
                Reset();
            }
            else // first click -- create portals
            {
                one_click = true;
                timer = Time.time;

                if (stage == 0) // Start off A
                {
                    if (hit.collider)
                    {
                        if (!hit.transform.gameObject.CompareTag("Portal"))
                        {
                            stage = 1;
                            a.norm = hit.normal;
                            a.attachedTo = hit.transform.gameObject;
                            StartA(hit.point);
                        }
                    }
                    //else
                    //{
                    //    stage = 1;
                    //    a.norm = hit.normal;
                    //    a.attachedTo = null;
                    //    StartA(ray.GetPoint(range));
                    //}
                } else if (stage == 1) // Set Outline A, start off B
                {
                    if (hit.collider && a.IsValid())
                    {
                        scaleText.enabled = true;
                        stage = 2;
                        b.outline.enabled = true;
                    }
                }
                else if (stage == 2) // Set Outline B, create portals
                {
                    if (b.IsValid())
                    {
                        StartCoroutine(SpecialPortal());
                    }
                }

            }
        }

        if (stage == 1)
        {
            if (hit.collider)
            {
                if (!hit.collider.CompareTag("Portal") && a.attachedTo &&
                hit.collider == a.attachedTo.GetComponent<Collider>() && hit.normal == a.norm)
                {
                    UpdateA(hit.point);
                }
            }
            //else if (!a.attachedTo)
            //{
            //    //Vector3 point = ProjectPointOnPlane(-transform.TransformDirection(Vector3.forward), a.outline.GetPosition(0), ray.GetPoint(range));
            //    //Debug.DrawLine(a.outline.GetPosition(0), point, color: Color.black);
            //    UpdateA(ray.GetPoint(range));
            //}
        } else if (stage == 2)
        {
            if (Input.GetAxis("Scale") != 0)
            {
                scale = Mathf.Max(scale + Input.GetAxis("Scale"), .1f);
                scaleText.text = "Scale: " + scale.ToString("x#.##");
            }

            if (Input.GetButton("Range"))
            {
                range = Mathf.Max((range + speed) % 20f, 3);
            }

            if (hit.collider)
            {
                if (!hit.collider.CompareTag("Portal"))
                {

                    b.attachedTo = hit.transform.gameObject;
                    UpdateB(hit.point, hit.normal);
                }
            } else
            {
                UpdateB(ray.GetPoint(range), -transform.TransformDirection(Vector3.forward));
            }
        } 

        // reset double click timer
        if (one_click && Time.time - timer > doubleClickDelay)
        {
            one_click = false;
        }
    }

    /// <summary>
    /// This resets creating any portals.
    /// </summary>
    void Reset()
    {
        stage = 0;
        scale = 1;
        range = 20;
        a.outline.enabled = false;
        b.outline.enabled = false;
        a.c.SetActive(false);
        b.c.SetActive(false);
        scaleText.enabled = false;
    }

    /// <summary>
    /// This method kicks off A's outline.
    /// </summary>
    void StartA(Vector3 hit)
    {
        a.outline.enabled = true;
        a.outline.SetPosition(0, hit + a.norm * offset);
        stage = 1;
    }

    /// <summary>
    /// Update A's outline according to hitpoint, a corner point.
    /// </summary>
    void UpdateA(Vector3 hitPoint)
    {
        Vector3 u = a.outline.GetPosition(0);
        Vector3 v = hitPoint + a.norm * offset;
        a.outline.SetPosition(2, v);

        Vector3 mid = (u + v) / 2;
        Vector3 orth1 = a.mid; // width
        Vector3 orth2 = a.mid; // height
        // me trying dumb shit ugh
        Vector3 ground = new Vector3(0, 0, 0);
        //Ray g = new Ray(mid, ground);
        //Ray no = new Ray(mid, a.norm);
        //Debug.DrawLine(mid, no.GetPoint(10));

        Vector3.OrthoNormalize(ref a.norm, ref ground, ref orth1);
        Vector3.OrthoNormalize(ref a.norm, ref ground, ref orth2);
        Vector3.OrthoNormalize(ref a.norm, ref orth1, ref orth2);

        //Debug.DrawLine(mid, g.GetPoint(10));
        //// get the j point
        LineIntersection(out Vector3 j, u, orth1, v, orth2);
        LineIntersection(out Vector3 i, u, orth2, v, orth1);

        //Ray m = new Ray(v, orth1);
        //Ray n = new Ray(v, orth2);
        //Debug.DrawLine(v, m.GetPoint(10), color: Color.blue);
        //Debug.DrawLine(v, n.GetPoint(10), color: Color.red);

        //Ray q = new Ray(u, orth1);
        //Ray r = new Ray(u, orth2);
        //Debug.DrawLine(u, q.GetPoint(10), color: Color.blue);
        //Debug.DrawLine(u, r.GetPoint(10), color: Color.red);

        a.outline.SetPosition(1, i);
        a.outline.SetPosition(3, j);

        float height = Vector3.Distance(u, i);
        float width = Vector3.Distance(u, j);

        a.Set(height, width, mid);
        a.IsValid();
    }


    /// <summary>
    /// Given a point p0 and its line line0, return if there exists 
    /// an intersection with point p2 and line2.
    /// </summary>
    bool LineIntersection(out Vector3 intersection, Vector3 p0, Vector3 line0, Vector3 p2, Vector3 line2)
    {
        Vector3 line3 = p2 - p0;
        Vector3 cross1and2 = Vector3.Cross(line0, line2);
        Vector3 cross3and2 = Vector3.Cross(line3, line2);

        float planarFactor = Vector3.Dot(line3, cross1and2);

        if (Mathf.Abs(planarFactor) < 0.0001f && cross1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(cross3and2, cross1and2) / cross1and2.sqrMagnitude;
            intersection = p0 + line0 * s;
            return true;
        }
        Debug.Log("WHYY");
        intersection = Vector3.zero;
        return false;
    }

    Vector3 ProjectPointOnPlane(Vector3 planeNormal, Vector3 planePoint, Vector3 point)
    {
        float distance;
        Vector3 translationVector;

        //First calculate the distance from the point to the plane:
        distance = Vector3.Dot(planeNormal, (point - planePoint));

        //Reverse the sign of the distance
        distance *= -1;

        //Get a translation vector
        translationVector = Vector3.Normalize(planeNormal) * distance;

        //Translate the point to form a projection
        return point + translationVector;
    }

    /// <summary>
    /// Update B's outline according to hitpoint, a mid point.
    /// </summary>
    void UpdateB(Vector3 hitPoint, Vector3 n)
    {
        b.norm = n;
        b.Set(a.h * scale, a.w * scale, hitPoint + b.norm * offset);
        Vector3 orth1 = b.mid;
        Vector3 orth2 = b.mid;
        Vector3 ground = new Vector3(0, 0, 0);
        Vector3.OrthoNormalize(ref b.norm, ref ground, ref orth1);
        Vector3.OrthoNormalize(ref b.norm, ref ground, ref orth2);
        Vector3.OrthoNormalize(ref b.norm, ref orth1, ref orth2);

        float width = b.w / 2;
        float height = b.h / 2;

        b.outline.SetPosition(0, b.mid - orth1 * width + orth2 * height);
        b.outline.SetPosition(1, b.mid + orth1 * width + orth2 * height);
        b.outline.SetPosition(2, b.mid + orth1 * width - orth2 * height);
        b.outline.SetPosition(3, b.mid - orth1 * width - orth2 * height);

        b.IsValid();
    }

    /// <summary>
    /// Kick off portal creation. Add special effects here.
    /// </summary>
    IEnumerator SpecialPortal()
    {
        if (stage == 2)
        {
            stage = 3;
            yield return new WaitForSeconds(doubleClickDelay);
            if (a.outline.enabled)
            {
                CreatePortals();
                Reset();
            }
        }
    }
    void CreatePortals()
    {
        Portal portalA = new Portal(a.w, a.h, a.mid, a.norm, player, "a" + numPortals, prefPortal, borderMat);
        portalA.attachedTo = a.attachedTo;


        Portal portalB = new Portal(b.w, b.h, b.mid, b.norm, player, "b" + numPortals, prefPortal, borderMat);
        portalB.attachedTo = b.attachedTo;

        Portal.pairPortals(portalA, portalB);
        gameManager.GetComponent<GameManager>().portalList.Add(portalA);

        numPortals++;
    }
}
