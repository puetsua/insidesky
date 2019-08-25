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
            var mirrorRigid = mirrored.GetComponent<Rigidbody2D>();
            if (mirrorPhys)
            {
                // inverted state
                if(mirrorPhys.isUnderground)
                {
                    mirrorPhys.SetDimension(PhysicsObject.Dimension.Ground);
                }
                else if(mirrorPhys.isInGround)
                {
                    mirrorPhys.SetDimension(PhysicsObject.Dimension.Underground);
                }
                mirrorRigid.bodyType = RigidbodyType2D.Kinematic;
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