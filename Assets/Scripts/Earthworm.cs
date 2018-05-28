using UnityEngine;

public class Earthworm : MonoBehaviour
{
    public GameObject MiPrefab;
    public float force = 10;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = transform.GetChild(0).GetComponent<Rigidbody2D>();
    }

    void Update ()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        //HingeJoint2D joint = new HingeJoint2D();
        //joint.

        if (Input.GetKeyDown(KeyCode.Space))
            AddMi();

        if (Input.GetKeyDown(KeyCode.C))
            RemoveMi();
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
