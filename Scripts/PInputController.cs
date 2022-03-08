using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PInputController : MonoBehaviour
{
    public static PInputController Instance { get; protected set; }

    [SerializeField] private float panSpeed = 1f;
    public Text ntext;
    public Text idtext;
    public Slider massslider;
    public Toggle inBuildMode;

    public Text aID;
    public Text bID;
    public Toggle posAttr;
    public Slider fmag;

    public void CreateRule()
    {
        PWorldController.Instance.CreateAttractRule(aID.text,bID.text,posAttr.isOn,fmag.value);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Tried to create two input controller instances.");
    }

    void CameraPan()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            CameraPan();
        }

        if (Input.GetKeyDown("space") && !EventSystem.current.IsPointerOverGameObject())
            PWorldController.Instance.Tick();

        if (Input.GetMouseButtonUp(0) && inBuildMode.isOn && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            PWorldController.Instance.Spawn(ntext.text, idtext.text, massslider.value , spawnPos);
            ntext.text = "";
        }

    }
}
