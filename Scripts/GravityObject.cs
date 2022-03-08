using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public static int instances = 0;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    Action<GravityObject> GravObjects;
    string _id;
    public string ID
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }
       

    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        PWorldController.Instance.RegisterGravCreated(NewGravObjectSpawned);

        PWorldController.Instance.RegisterOnTickTimer(Tick);

        PWorldController.Instance.RegisterGravDestroy(OnGravDestroy);
        instances++;
    }

    private void RegisterNewGravityObject(Action<GravityObject> callback)
    {
        GravObjects += callback;
    }

    private void UnRegisterGravityObject(Action<GravityObject> callback)
    {
        GravObjects -= callback;
    }

    public Action<GravityObject> getGravFunction()
    {
        return ApplyGravRule;
    }

    private void ApplyGravRule(GravityObject grav)
    {
        AttractRule rule = PWorldController.Instance.GetAttractRule(grav.ID, _id);
        if (rule == null)
        {
            return;
        }
        Vector2 direction = rb.position - grav.getPosition();
        if (!rule.isPositiveAttraction)
            direction = -direction;

        float distance = direction.magnitude;

        float forceMagnitude = ((rb.mass * grav.getMass()) / Mathf.Pow(distance, 2))*rule.magnitude;
        Vector2 force = direction.normalized * forceMagnitude;

        rb.AddForce(force);
    }

    void NewGravObjectSpawned(GravityObject grav)
    {
        if (grav == this)
            return;
        GravObjects += grav.getGravFunction();
        grav.RegisterNewGravityObject(ApplyGravRule);
    }

    public float getMass()
    {
        return rb.mass;
    }

    public Vector2 getPosition()
    {
        return transform.position;
    }

    public void Tick()
    {
        GravObjects?.Invoke(this);
    }

    void OnGravDestroy(GravityObject grav)
    {
        if (grav == this)
            return;
        UnRegisterGravityObject(grav.getGravFunction());
    }

    void SelfDestruct()
    {
        PWorldController.Instance.UnregisterGravCreated(NewGravObjectSpawned);
        PWorldController.Instance.UnRegisterOnTickTimer(Tick);
        PWorldController.Instance.UnRegisterGravDestroy(OnGravDestroy);
        PWorldController.Instance.DestoryGravBroadcast(this);
    }

    void OnDestroy()
    {
        SelfDestruct();   
    }
}
