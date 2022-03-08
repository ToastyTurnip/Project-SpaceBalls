using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PInputController : MonoBehaviour
{
    public static PInputController Instance { get; protected set; }

    [SerializeField] private float panSpeed = 1f;
    public Text instances;

    Transform following;

    public Text ntext;
    public Text idtext;
    public Slider massslider;
    public Toggle staticSpawn;
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

        PWorldController.Instance.RegisterGravDestroy(OnGravDestroyed);
    }

    void CameraPan()
    {
        if (Input.GetKey("w"))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + Time.deltaTime * panSpeed, Camera.main.transform.position.z);
            UnFollow();
        }
        if (Input.GetKey("a"))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x - Time.deltaTime * panSpeed, Camera.main.transform.position.y, Camera.main.transform.position.z);
            UnFollow();
        }
        if (Input.GetKey("s"))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - Time.deltaTime * panSpeed, Camera.main.transform.position.z);
            UnFollow();
        }
        if (Input.GetKey("d"))
        {
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x + Time.deltaTime * panSpeed, Camera.main.transform.position.y, Camera.main.transform.position.z);
            UnFollow();
        }

        if (Input.GetKeyDown("p"))
            TogglePause();

        //scrollwheel for controlling zoom
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            Camera.main.orthographicSize += panSpeed/15;
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            Camera.main.orthographicSize -= panSpeed/15;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 0.5f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            CameraPan();
        }

        //if (Input.GetKeyDown("space") && !EventSystem.current.IsPointerOverGameObject())
        //    PWorldController.Instance.Tick();

        if (Input.GetMouseButtonUp(0) && inBuildMode.isOn && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (ntext.text == "")
            {
                if (idtext.text == "")
                {
                    Debug.LogError("Must have an id.");
                }
                else
                {
                    PWorldController.Instance.Spawn(idtext.text, idtext.text, massslider.value, spawnPos, staticSpawn.isOn);
                }
            }
            else
            {
                PWorldController.Instance.Spawn(ntext.text, idtext.text, massslider.value, spawnPos, staticSpawn.isOn);
            }
                
        }
        InteractObjects();
        Follow();
        instances.text = "Object Instances: " + GravityObject.instances.ToString();
    }

    void OnGravDestroyed(GravityObject grav)
    {
        if (grav.transform == following)
        {
            UnFollow();
        }
    }

    private void Follow()
    {
        if (following != null)
        {
            Camera.main.transform.position = new Vector3(following.position.x,following.position.y,Camera.main.transform.position.z);
        }
    }

    void UnFollow()
    {
        following = null;
    }

    private void InteractObjects()
    {
        if (Input.GetMouseButton(2))
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
            if (hit.collider != null)
            {
                following = hit.collider.transform;
            }
        }
        if (Input.GetMouseButtonDown(1) && !inBuildMode.isOn)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
            if (hit.collider != null)
            {
                PWorldController.Instance.Despawn(hit.collider.gameObject);
                //Debug.Log("Initiated Destroy.");
            }
        }
        if (Input.GetMouseButtonDown(0) && !inBuildMode.isOn)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y), Vector2.zero, 0f);
            if (hit.collider != null)
            {
                PWorldController.Instance.ToggleStatic(hit.collider.gameObject.GetComponent<GravityObject>());
                //Debug.Log("Initiated Toggle Static");
            }
        }
    }

    public void ClearRules()
    {
        PWorldController.Instance.ClearRules();
    }

    public void DestroyAllGrav()
    {
        PWorldController.Instance.DestroyAllObjects();
    }

    public void TogglePause()
    {
        PWorldController.Instance.TogglePause();
    }
}
