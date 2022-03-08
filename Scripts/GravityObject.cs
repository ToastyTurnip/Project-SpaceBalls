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
    SpriteRenderer sr;
    string _id;

    [SerializeField] Color staticColor = Color.yellow;
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
        //rb.bodyType = RigidbodyType2D.Static;
        rb.gravityScale = 0;
        PWorldController.Instance.RegisterGravCreated(NewGravObjectSpawned);
        PWorldController.Instance.RegisterOnTickTimer(Tick);
        PWorldController.Instance.RegisterGravDestroy(OnGravDestroy);

        sr = GetComponent<SpriteRenderer>();
        //sr.color = Color.yellow;
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
        if (PWorldController.Instance.isPaused)
        {
            return;
        }
        if(Mathf.Abs(transform.position.x) > 1000 || Mathf.Abs(transform.position.y) > 1000)
            PWorldController.Instance.Despawn(gameObject);
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
        instances--;
        PWorldController.Instance.UnregisterGravCreated(NewGravObjectSpawned);
        PWorldController.Instance.UnRegisterOnTickTimer(Tick);
        PWorldController.Instance.UnRegisterGravDestroy(OnGravDestroy);
        PWorldController.Instance.DestoryGravBroadcast(this);
    }

    public void ToggleBodyType()
    {
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            sr.color = staticColor;
            rb.bodyType = RigidbodyType2D.Static;
            return;
        }
        if (rb.bodyType == RigidbodyType2D.Static)
        {
            sr.color = Color.white;
            rb.bodyType = RigidbodyType2D.Dynamic;
            return;
        }
    }

    void OnDestroy()
    {
        SelfDestruct();   
    }
}
