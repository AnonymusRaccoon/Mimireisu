using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Earthworm : MonoBehaviour
{
    public GameObject LinePrefab;
    public GameObject MiPrefab;
    public float force = 10;
    public float ropeSpeed = 2;
    public LayerMask mask;

    private Vector3 position;
    private List<Vector3> rope1Position = new List<Vector3>();
    private List<Vector3> rope2Position = new List<Vector3>();
    private Dictionary<KeyCode, ActionItem> actions = new Dictionary<KeyCode, ActionItem>();


    void Update ()
    {
        position = transform.GetChild(transform.childCount / 2).position;

        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, position.y, Camera.main.transform.position.z);

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
            rope1Position.Clear();
        }
        if (Input.GetKeyUp(KeyCode.D) && actions.ContainsKey(KeyCode.D))
        {
            Destroy(actions[KeyCode.D].joint);
            Destroy(actions[KeyCode.D].line);
            actions.Remove(KeyCode.D);
            rope2Position.Clear();
        }

        if (actions.ContainsKey(KeyCode.Q))
        {
            LineRenderer line = actions[KeyCode.Q].line.GetComponent<LineRenderer>();
            rope1Position[0] = transform.GetChild(0).position;
            Vector3 hookDirection = rope1Position[1] - transform.GetChild(0).position;

            RaycastHit2D ray = Physics2D.Raycast(transform.GetChild(0).position, hookDirection.normalized, hookDirection.magnitude, mask);
            if (ray.collider != null && !RopeContain(1, ray.point))
            {
                print(ray.collider.transform.name);
                PolygonCollider2D collider = ray.collider as PolygonCollider2D;
                print(collider.transform.name);
                rope1Position.Insert(rope1Position.Count - 2, GetClosestColliderPointFromRaycastHit(ray, collider));
            }

            line.positionCount = rope1Position.Count;
            line.SetPositions(rope1Position.ToArray());
        }
        if (actions.ContainsKey(KeyCode.D))
            actions[KeyCode.D].line.GetComponent<LineRenderer>().SetPosition(0, transform.GetChild(transform.childCount - 1).position);
    }

    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        var distanceDictionary = polyCollider.points.ToDictionary<Vector2, float, Vector2>(
            position => Vector2.Distance(hit.point, polyCollider.transform.TransformPoint(position)),
            position => polyCollider.transform.TransformPoint(position));

        var orderedDictionary = distanceDictionary.OrderBy(e => e.Key);
        return orderedDictionary.Any() ? orderedDictionary.First().Value : Vector2.zero;
    }

    bool RopeContain(int ropeNumber, Vector3 point)
    {
        if(ropeNumber == 1)
        {
            foreach(Vector3 pos in rope1Position)
            {
                if (pos == point)
                    return true;
                //if (Vector3.Distance(pos, point) < 0.5f)
                //    return true;
            }
        }
        return false;
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
                        rope1Position.Clear();
                    }
                    rope1Position.Add(transform.GetChild(0).position);
                    rope1Position.Add(hit.point);
                    break;
                case KeyCode.D:
                    joint = (DistanceJoint2D)transform.GetChild(transform.childCount - 1).gameObject.AddComponent(typeof(DistanceJoint2D));
                    if (actions.ContainsKey(KeyCode.D))
                    {
                        Destroy(actions[KeyCode.D].joint);
                        Destroy(actions[KeyCode.D].line);
                        actions.Remove(KeyCode.D);
                        rope2Position.Clear();
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
