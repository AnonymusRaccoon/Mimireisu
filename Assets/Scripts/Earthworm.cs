using System.Collections.Generic;
using UnityEngine;

public class Earthworm : MonoBehaviour
{
    public GameObject LinePrefab;
    public GameObject MiPrefab;
    public float force = 10;
    public float distance = 20;
    public LayerMask mask;

    private Vector3 position;
    private Dictionary<KeyCode, ActionItem> actions = new Dictionary<KeyCode, ActionItem>();


    void Update ()
    {
        position = transform.GetChild(transform.childCount / 2).position;

        #region Manette
        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");
        //Vector2 direction = new Vector2(horizontal, vertical).normalized;
        #endregion
        Vector3 camera = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = camera - position;

        if (Input.GetKeyDown(KeyCode.A))
            Grab(KeyCode.A, direction);
        if (Input.GetKeyDown(KeyCode.Z))
            Grab(KeyCode.Z, direction);
        if (Input.GetKeyDown(KeyCode.E))
            Grab(KeyCode.E, direction);
        if (Input.GetKeyDown(KeyCode.R) && transform.childCount > 3)
            Grab(KeyCode.R, direction);

        if (Input.GetKeyDown(KeyCode.Space))
            AddMi();

        if (Input.GetKeyDown(KeyCode.C))
            RemoveMi();

        if (Input.GetKeyUp(KeyCode.A) && actions.ContainsKey(KeyCode.A))
        {
            Destroy(actions[KeyCode.A].joint);
            Destroy(actions[KeyCode.A].line);
            actions.Remove(KeyCode.A);
        }
        if (Input.GetKeyUp(KeyCode.Z) && actions.ContainsKey(KeyCode.Z))
        {
            Destroy(actions[KeyCode.Z].joint);
            Destroy(actions[KeyCode.Z].line);
            actions.Remove(KeyCode.Z);
        }
        if (Input.GetKeyUp(KeyCode.E) && actions.ContainsKey(KeyCode.E))
        {
            Destroy(actions[KeyCode.E].joint);
            Destroy(actions[KeyCode.E].line);
            actions.Remove(KeyCode.E);
        }
        if (Input.GetKeyUp(KeyCode.R) && actions.ContainsKey(KeyCode.R))
        {
            Destroy(actions[KeyCode.R].joint);
            Destroy(actions[KeyCode.R].line);
            actions.Remove(KeyCode.R);
        }

        foreach(var item in actions)
        {
            item.Value.line.GetComponent<LineRenderer>().SetPosition(0, position);
        }
    }

    void Grab(KeyCode key, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, direction, 20, mask);
        if (hit.collider != null)
        {
            DistanceJoint2D joint = null;
            switch (key)
            {
                case KeyCode.A:
                    joint = (DistanceJoint2D)transform.GetChild(0).gameObject.AddComponent(typeof(DistanceJoint2D));
                    if (actions.ContainsKey(KeyCode.A))
                    {
                        Destroy(actions[KeyCode.A].joint);
                        Destroy(actions[KeyCode.A].line);
                        actions.Remove(KeyCode.A);
                    }
                    break;
                case KeyCode.Z:
                    joint = (DistanceJoint2D)transform.GetChild(transform.childCount == 3 ? 1 : transform.childCount / 4).gameObject.AddComponent(typeof(DistanceJoint2D));
                    if (actions.ContainsKey(KeyCode.Z))
                    {
                        Destroy(actions[KeyCode.Z].joint);
                        Destroy(actions[KeyCode.Z].line);
                        actions.Remove(KeyCode.Z);
                    }
                    break;
                case KeyCode.E:
                    joint = (DistanceJoint2D)transform.GetChild(transform.childCount * 3 / 4).gameObject.AddComponent(typeof(DistanceJoint2D));
                    if (actions.ContainsKey(KeyCode.E))
                    {
                        Destroy(actions[KeyCode.E].joint);
                        Destroy(actions[KeyCode.E].line);
                        actions.Remove(KeyCode.E);
                    }
                    break;
                case KeyCode.R:
                    joint = (DistanceJoint2D)transform.GetChild(transform.childCount - 1).gameObject.AddComponent(typeof(DistanceJoint2D));
                    if (actions.ContainsKey(KeyCode.R))
                    {
                        Destroy(actions[KeyCode.R].joint);
                        Destroy(actions[KeyCode.R].line);
                        actions.Remove(KeyCode.R);
                    }
                    break;
                default:
                    return;
            }

            joint.connectedAnchor = hit.point;
            joint.maxDistanceOnly = true;

            GameObject line = Instantiate(LinePrefab);
            LineRenderer renderer = line.GetComponent<LineRenderer>();
            renderer.SetPositions(new Vector3[] { position, hit.point });

            actions.Add(key, new ActionItem(joint, line));
        }
    }

    void AddMi()
    {
        Transform tsu = transform.GetChild(transform.childCount - 1);
        Vector3 distance = tsu.localPosition - transform.GetChild(transform.childCount - 2).localPosition;
        GameObject mi = Instantiate(MiPrefab, transform);

        mi.transform.localPosition = tsu.localPosition;
        tsu.SetAsLastSibling();
        tsu.localPosition = mi.transform.localPosition + distance;
        tsu.GetComponent<HingeJoint2D>().connectedBody = mi.GetComponent<Rigidbody2D>();
        mi.GetComponent<HingeJoint2D>().connectedBody = transform.GetChild(transform.childCount - 3).GetComponent<Rigidbody2D>();
    }

    void RemoveMi()
    {
        if(transform.childCount > 3)
        {
            Transform tsu = transform.GetChild(transform.childCount - 1);
            Transform mi = transform.GetChild(transform.childCount - 2);
            Transform lastMi = transform.GetChild(transform.childCount - 3);
            Vector2 pos = mi.localPosition;
            Destroy(mi.gameObject);
            tsu.localPosition = pos;
            tsu.GetComponent<HingeJoint2D>().connectedBody = lastMi.GetComponent<Rigidbody2D>();
        }
    }
}
