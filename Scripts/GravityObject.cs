using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    private Rigidbody2D rb;
    // Start is called before the first frame update
    Action<GravityObject> newGravObject;
    public string ID { get; protected set; }

    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    public void RegisterNewGravityObject(Action<GravityObject> callback)
    {
        newGravObject += callback;
    }

    public void UnRegisterGravityObject(Action<GravityObject> callback)
    {
        newGravObject -= callback;
    }

    public float getMass()
    {
        return rb.mass;
    }

    public Vector2 getPosition()
    {
        return transform.position;
    }

    public void setID(string id)
    {
        ID = id;
    }

    public void Tick()
    {
        newGravObject(this);
    }
}
