using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PWorldController : MonoBehaviour
{
    public static PWorldController Instance { get; protected set; }
    public readonly Dictionary<string, List<AttractRule>> AttractRules = new Dictionary<string, List<AttractRule>>();
    public GameObject bodyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Tried to initialize two world controllers.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(string name, string ID, float mass, Vector2 position)
    {
        GameObject go = Instantiate(bodyPrefab, position, Quaternion.identity);
        GravityObject grav = go.AddComponent<GravityObject>();
        grav.setID(ID);
        go.name = name;
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        rb.mass = mass;
    }
}

public class AttractRule
{
    string aID;
    string bID;
    bool isPositiveAttraction;
    float magnitude;

    public AttractRule(string aid, string bid, bool posattr, float mag)
    {
        aID = aid;
        bID = bid;
        isPositiveAttraction = posattr;
        magnitude = mag;
    }
}
