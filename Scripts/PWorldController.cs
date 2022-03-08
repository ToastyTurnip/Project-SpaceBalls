using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PWorldController : MonoBehaviour
{
    public static PWorldController Instance { get; protected set; }
    public Dictionary<Tuple<string,string>, AttractRule> AttractRules = new Dictionary<Tuple<string,string>, AttractRule>();
    public GameObject bodyPrefab;
    List<GravityObject> gravityObjects;
    Action<GravityObject> gravy;
    Action<GravityObject> ungravy;
    Action tickTime;
    public bool isPaused { get; protected set; }

    public void ToggleStatic(GravityObject grav)
    {
        grav.ToggleBodyType();
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
    }
    public AttractRule GetAttractRule(string a, string b)
    {
        Tuple<string,string> IDs = new Tuple<string, string>(a, b);
        if (AttractRules.ContainsKey(IDs))
            return AttractRules[IDs];
        return null;
    }

    public void CreateAttractRule(string aid, string bid, bool posattr, float mag)
    {
        AttractRules[new Tuple<string, string>(aid,bid)] = new AttractRule(aid, bid, posattr, mag);
    }

    public void ClearRules()
    {
        AttractRules = new Dictionary<Tuple<string, string>, AttractRule>();
    }

    public void DestroyAllObjects()
    {
        int len = gravityObjects.Count;
        for (int i = 0; i < len; i++)
        {
            Despawn(gravityObjects[0].gameObject);
        }
    }

    public void Tick()
    {
        //Debug.Log("Ticked.");
        if (tickTime != null)
            tickTime();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Tried to initialize two world controllers.");
        isPaused = true;
        gravityObjects = new List<GravityObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Tick();

        if (Mathf.Abs(transform.position.x) > 1000 || Mathf.Abs(transform.position.y) > 1000)
            PWorldController.Instance.Despawn(gameObject);
    }

    public void Spawn(string name, string ID, float mass, Vector2 position)
    {
        GameObject go = Instantiate(bodyPrefab, position, Quaternion.identity);
        GravityObject grav = go.AddComponent<GravityObject>();
        gravityObjects.Add(grav);
        grav.ID = ID;
        go.GetComponentInChildren<Text>().text = name;
        go.name = name;
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        rb.mass = mass;
        gravy(grav);

        go.transform.SetParent(transform);
    }

    public void Despawn(GameObject go)
    {
        gravityObjects.Remove(go.GetComponent<GravityObject>());
        Destroy(go);
    }

    public void RegisterGravCreated(Action<GravityObject> callback)
    {
        gravy += callback;
    }

    public void UnregisterGravCreated(Action<GravityObject> callback)
    {
        gravy -= callback;
    }
    
    public void RegisterGravDestroy(Action<GravityObject> callback)
    {
        ungravy += callback;
    }

    public void UnRegisterGravDestroy(Action<GravityObject> callback)
    {
        ungravy -= callback;
    }

    public void RegisterOnTickTimer(Action callback)
    {
        tickTime += callback;
    }

    public void UnRegisterOnTickTimer(Action callback)
    {
        tickTime -= callback;
    }

    public void DestoryGravBroadcast(GravityObject grav)
    {
        ungravy?.Invoke(grav);
    }

}

public class AttractRule
{
    public string aID { get; protected set; }
    public string bID { get; protected set; }
    public bool isPositiveAttraction { get; protected set; }
    public float magnitude { get; protected set; }

    public AttractRule(string aid, string bid, bool posattr, float mag)
    {
        aID = aid;
        bID = bid;
        isPositiveAttraction = posattr;
        magnitude = mag;
    }
}
