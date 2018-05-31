using System.Collections.Generic;
using UnityEngine;

public class Earthworm : MonoBehaviour
{
    public GameObject LinePrefab;
    public GameObject MiPrefab;
    public float force = 10;
    public float ropeSpeed = 2;
    public LayerMask mask;

    private Vector3 position;
    private Dictionary<KeyCode, ActionItem> actions = new Dictionary<KeyCode, ActionItem>();


    void Update ()
    {
        position = transform.GetChild(transform.childCount / 2).position;

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, position.y, Camera.main.transform.position.z);

        #region Manette
        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");
        //Vector2 direction = new Vector2(horizontal, vertical).normalized;
        #endregion
        Vector3 camera = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = camera - position;

        if (Input.GetKeyDown(KeyCode.Q))
            Grab(KeyCode.Q, direction);
        if (Input.GetKeyDown(KeyCode.D))
            Grab(KeyCode.D, direction);

        if (Input.GetKey(KeyCode.Z))
            ChangeRopeSize(-1);
        if (Input.GetKey(KeyCode.S))
            ChangeRopeSize(1);

        if (Input.GetKeyDown(KeyCode.Space))
            AddMi();

        if (Input.GetKeyDown(KeyCode.C))
            RemoveMi();

        if (Input.GetKeyUp(KeyCode.Q) && actions.ContainsKey(KeyCode.Q))
        {
            Destroy(actions[KeyCode.Q].joint);
            Destroy(actions[KeyCode.Q].line);
            actions.Remove(KeyCode.Q);
        }
        if (Input.GetKeyUp(KeyCode.D) && actions.ContainsKey(KeyCode.D))
        {
            Destroy(actions[KeyCode.D].joint);
            Destroy(actions[KeyCode.D].line);
            actions.Remove(KeyCode.D);
        }

        if(actions.ContainsKey(KeyCode.Q))
            actions[KeyCode.Q].line.GetComponent<LineRenderer>().SetPosition(0, transform.GetChild(0).position);
        if (actions.ContainsKey(KeyCode.D))
            actions[KeyCode.D].line.GetComponent<LineRenderer>().SetPosition(0, transform.GetChild(transform.childCount - 1).position);
    }

    void Grab(KeyCode key, Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, direction, key == KeyCode.Q ? 20 : 150, mask);
        if (hit.collider != null)
        {
            DistanceJoint2D joint = null;
            switch (key)
            {
                case KeyCode.Q:
                    joint = (DistanceJoint2D)transform.GetChild(0).gameObject.AddComponent(typeof(DistanceJoint2D));
                    if (actions.ContainsKey(KeyCode.Q))
                    {
                        Destroy(actions[KeyCode.Q].joint);
                        Destroy(actions[KeyCode.Q].line);
                        actions.Remove(KeyCode.Q);
                    }
                    break;
                case KeyCode.D:
                    joint = (DistanceJoint2D)transform.GetChild(transform.childCount - 1).gameObject.AddComponent(typeof(DistanceJoint2D));
                    if (actions.ContainsKey(KeyCode.D))
                    {
                        Destroy(actions[KeyCode.D].joint);
                        Destroy(actions[KeyCode.D].line);
                        actions.Remove(KeyCode.D);
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

    void ChangeRopeSize(int direction)
    {
        if (actions.ContainsKey(KeyCode.Q))
        {
            DistanceJoint2D joint = actions[KeyCode.Q].joint;
            if (direction > 0)
                joint.distance += Time.deltaTime * ropeSpeed;
            else
                joint.distance -= Time.deltaTime * ropeSpeed;
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
