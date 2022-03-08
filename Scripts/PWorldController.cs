using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PWorldController : MonoBehaviour
{
    public static PWorldController Instance { get; protected set; }
    public readonly Dictionary<string, List<AttractRule>> AttractRules = new Dictionary<string, List<AttractRule>>();
    public GameObject bodyPrefab;
    List<GravityObject> gravityObjects;
    Action<GravityObject> gravy;
    Action tickTime;
    public AttractRule GetAttractRule(string a, string b)
    {
        if (AttractRules.ContainsKey(a)){
            foreach (AttractRule rule in AttractRules[a])
            {
                if (rule.bID == b)
                {
                    return rule;
                }
            }
            return null;
        }
        return null;
    }

    public void CreateAttractRule(string aid, string bid, bool posattr, float mag)
    {
        if (!AttractRules.ContainsKey(aid))
        {
            AttractRules[aid] = new List<AttractRule>();
            
        }
        AttractRules[aid].Add(new AttractRule(aid, bid, posattr, mag));
    }

    public void Tick()
    {
        Debug.Log("Ticked.");
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

        gravityObjects = new List<GravityObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

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
    }

    public void RegisterGravCreated(Action<GravityObject> callback)
    {
        gravy += callback;
    }
    
    public void RegisterOnTickTimer(Action callback)
    {
        tickTime += callback;
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
