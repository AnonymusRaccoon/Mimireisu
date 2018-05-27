using UnityEngine;

public class Earthworm : MonoBehaviour
{
    public GameObject MiPrefab;
    public float force = 10;

    void Update ()
    {
        float movement = Input.GetAxis("Horizontal");
        //rb.AddForce(new Vector2(movement * force, 0));

        if (Input.GetKeyDown(KeyCode.Space))
            AddMi();

        if (Input.GetKeyDown(KeyCode.C))
            RemoveMi();
	}

    void AddMi()
    {
        Transform tsu = transform.GetChild(transform.childCount - 1);
        GameObject mi = Instantiate(MiPrefab, transform);
        mi.transform.localPosition = tsu.localPosition;
        tsu.SetAsLastSibling();
        tsu.localPosition = new Vector2(0.4f * (transform.childCount - 1), tsu.position.y);
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
            Vector2 pos = mi.position;
            Destroy(mi.gameObject);
            tsu.localPosition = pos;
            tsu.GetComponent<HingeJoint2D>().connectedBody = lastMi.GetComponent<Rigidbody2D>();
        }
    }
}
