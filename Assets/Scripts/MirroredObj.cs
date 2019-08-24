using UnityEngine;

public class MirroredObj : MonoBehaviour
{
    GameManager mgr { get { return GameManager.instance; } }
    GameObject mirrored = null;
    Rigidbody2D rigid2d = null;

    [HideInInspector]
    public bool isMirroredObj = false;

    void Start()
    {
        if (!isMirroredObj)
        {
            Vector2 pos = transform.localPosition;
            mirrored = Instantiate(this.gameObject, transform.parent);
            mirrored.transform.localPosition = new Vector2(mgr.worldRadius * 2 - pos.x, 0f);
            mirrored.GetComponent<MirroredObj>().isMirroredObj = true;
        }

        rigid2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (rigid2d == null)
        {
            return;
        }

        if (mirrored)
        {
            // Mirrored rigidbody position
            Vector2 pos = transform.localPosition;
            mirrored.transform.localPosition = new Vector2(mgr.worldRadius * 2 - pos.x, 0f);
        }
    }
}