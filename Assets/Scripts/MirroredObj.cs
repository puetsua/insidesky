using UnityEngine;

public class MirroredObj : MonoBehaviour
{
    GameManager mgr { get { return GameManager.instance; } }
    PhysicsSystem psys { get { return PhysicsSystem.instance; } }

    GameObject mirrored = null;
    Rigidbody2D rigid2d = null;

    [HideInInspector]
    public bool isMirroredObj = false;

    void Start()
    {
        if (!isMirroredObj)
        {
            Vector2 pos = transform.localPosition;
            Vector2 inwardDir = psys.TowardCenter(transform);

            mirrored = Instantiate(this.gameObject, transform.parent);
            var mirrorComp = mirrored.GetComponent<MirroredObj>();
            var mirrorPhys = mirrored.GetComponent<PhysicsObject>();
            if (mirrorPhys)
            {
                // inverted state
                mirrorPhys.isUnderground = !mirrorPhys.isUnderground;
            }
            mirrored.transform.localPosition = inwardDir * (psys.radius * 2 - psys.Dist2Center(transform));
            mirrorComp.enabled = false;
            Destroy(mirrorComp);
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
            Vector2 inwardDir = psys.TowardCenter(transform);
            mirrored.transform.localPosition = inwardDir * (psys.radius * 2 - psys.Dist2Center(transform));
        }
    }
}