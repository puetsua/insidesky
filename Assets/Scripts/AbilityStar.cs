using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityStar : MonoBehaviour
{
    public GameController gameCtrl { get { return GameController.instance; } }

    public enum Type
    {
        Jump,
        Dig
    }
    public Type type;

    void OnTriggerEnter2D(Collider2D other)
    {
        PhysicsObject obj = other.gameObject.GetComponent<PhysicsObject>();
        if (gameCtrl.player == obj)
        {
            gameCtrl.PlayerGotAbility(type);
            Destroy(this.gameObject);
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
