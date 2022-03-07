using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PInputController : MonoBehaviour
{
    public static PInputController Instance { get; protected set; }
    public bool inBuildMode { get; protected set; }
    [SerializeField] private float panSpeed = 1f;
    public Text tm;
    public Text ntext;
    public Text idtext;
    public Slider massslider;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Tried to create two input controller instances.");
        inBuildMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            inBuildMode = true;
        }
        if (Input.GetKeyDown("e"))
        {
            inBuildMode = false;
        }


        if (Input.GetKey("w"))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + Time.deltaTime * panSpeed, Camera.main.transform.position.z);
        }
        if (Input.GetKey("a"))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - Time.deltaTime * panSpeed, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }
        if (Input.GetKey("s"))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - Time.deltaTime * panSpeed, Camera.main.transform.position.z);
        }
        if (Input.GetKey("d"))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + Time.deltaTime * panSpeed, Camera.main.transform.position.y, Camera.main.transform.position.z);
        }

        if (Input.GetMouseButtonUp(0) && inBuildMode && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PWorldController.Instance.Spawn("Jezebel", "a", 1.0f , spawnPos);
        }

        tm.text = "Build mode = " + inBuildMode.ToString();
    }
}
