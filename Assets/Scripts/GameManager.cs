using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using grouflon;

public enum GameState
{
    None,
    Inspecting,
    Travelling
}

public enum RoomSlot
{
    Room1,
    Room2
}

public class GameManager : MonoBehaviour
{
    public GameContent content;

    public Camera maskCamera;
    public Camera room1Camera;
    public Camera room2Camera;
    public Camera finalCamera;

    public Transform world;
    public Transform room1;
    public Transform room2;

    public MaskGenerator mask;

    public RawImage renderImage;

    public float rotationSpeed = 90.0f;
    public float transitionTime = 1.0f;

    public void SetState(GameState _state)
    {
        if (_state == m_currentState)
            return;

        // EXIT
        switch(m_currentState)
        {
            case GameState.None:
            {

            }
            break;

            case GameState.Inspecting:
            {
                if (m_hoveredObject)
                {
                    m_hoveredObject.hovered = false;
                    m_hoveredObject = null;
                }
            }
            break;

            case GameState.Travelling:
            {

            }
            break;
        }

        m_currentState = _state;

        // ENTER
        switch(m_currentState)
        {
            case GameState.None:
            {

            }
            break;

            case GameState.Inspecting:
            {
                Quaternion worldRotation = Quaternion.Euler(0.0f, m_isInRoom1 ? 45.0f : 225.0f, 0.0f);
                world.transform.rotation = worldRotation;
            }
            break;

            case GameState.Travelling:
            {
                m_transitionTimer = 0.0f;
            }
            break;
        }
    }

    void Start()
    {
        mask.generateMesh();

        _setupRoom(RoomSlot.Room1, "Bedroom");
        _setupRoom(RoomSlot.Room2, "");

        SetState(GameState.Inspecting);
    }

    void Update()
    {
        switch(m_currentState)
        {
            case GameState.None:
            {

            }
            break;

            case GameState.Inspecting:
            {
                 // MOVE
                /*{
                    float input = 0.0f;
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        input += -1.0f;
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        input += 1.0f;
                    }

                    Vector3 rotation = world.transform.rotation.eulerAngles;
                    rotation.y += input * rotationSpeed * Time.deltaTime;
                    world.transform.rotation = Quaternion.Euler(rotation);
                }*/

                // RAYCAST
                InteractiveItem hoveredObject = null;

                Vector3[] corners = new Vector3[4];
                renderImage.rectTransform.GetWorldCorners(corners);
                Vector3 renderImageSize = corners[2] - corners[0];
                //Debug.Log("" + corners[0] + " " + corners[1] + " " + corners[2] + " " + corners[3]);
                Vector3 mousePosition = finalCamera.ScreenToWorldPoint(Input.mousePosition);
                //Debug.Log(mousePosition.ToString());
                Vector3 relativePosition = mousePosition - corners[0];
                Vector3 normalizedMousePosition = new Vector3(relativePosition.x / renderImageSize.x, relativePosition.y / renderImageSize.y, 0.0f);
                //Debug.Log(normalizedMousePosition.ToString());
                Vector3 screenPoint = maskCamera.ViewportToScreenPoint(normalizedMousePosition);
                //Debug.Log(screenPoint.ToString());
                Ray ray = maskCamera.ScreenPointToRay(screenPoint);
                //Debug.Log(ray.ToString());
                RaycastHit hit = new RaycastHit();
                int pointedRoom = 0;
                if (Physics.Raycast(ray, out hit, 1000.0f, LayerMask.GetMask("Mask")))
                {
                    if (hit.transform.gameObject.tag == "Room1")
                    {
                        pointedRoom = 1;
                    }
                    else if (hit.transform.gameObject.tag == "Room2")
                    {
                        pointedRoom = 2;
                    }
                }

                if (pointedRoom != 0)
                {
                    LayerMask mask = LayerMask.GetMask(pointedRoom == 1 ? "Room1" : "Room2");
                    if (Physics.Raycast(ray, out hit, 1000.0f, mask))
                    {
                        hoveredObject = hit.transform.GetComponent<InteractiveItem>();
                    }
                }
                
                if (m_hoveredObject != null)
                {
                    m_hoveredObject.hovered = false;
                }
                m_hoveredObject = hoveredObject;
                if (m_hoveredObject != null)
                {
                    m_hoveredObject.hovered = true;
                }

                if (m_hoveredObject != null && Input.GetMouseButtonDown(0))
                {
                    if (m_isInRoom1)
                    {
                        _setupRoom(RoomSlot.Room2, m_hoveredObject.targetRoomId);
                    }
                    else
                    {
                        _setupRoom(RoomSlot.Room1, m_hoveredObject.targetRoomId);
                    }

                    SetState(GameState.Travelling);
                }
            }
            break;

            case GameState.Travelling:
            {
                m_transitionTimer += Time.deltaTime;

                Quaternion baseRotation = Quaternion.Euler(0.0f, (m_isInRoom1 ? 45.0f : 225.0f) + 0.1f, 0.0f);
                Quaternion targetRotation = Quaternion.Euler(0.0f, m_isInRoom1 ? 225.0f : 45.0f, 0.0f);
                
                float t1 = Mathf.Clamp01(m_transitionTimer / transitionTime);
                float t2 = Ease.QuadOut(t1);
                world.transform.rotation = Quaternion.LerpUnclamped(baseRotation, targetRotation, t2);
                if (t1 >= 1.0f)
                {
                    m_isInRoom1 = !m_isInRoom1;
                    if (m_isInRoom1)
                    {
                        _setupRoom(RoomSlot.Room2, "");
                    }
                    else
                    {
                        _setupRoom(RoomSlot.Room1, "");
                    }

                    SetState(GameState.Inspecting);
                }
            }
            break;
        }
    }

    private void _assignOutlinesInstance(Transform _parent, cakeslice.OutlineEffect _instance)
    {
        cakeslice.Outline[] outlines = _parent.GetComponentsInChildren<cakeslice.Outline>(true);
        foreach (cakeslice.Outline outline in outlines)
        {
            outline.Instance = _instance;
        }
    }

    private void _setupRoom(RoomSlot _slot, string _id)
    {
        string layerName = "";
        Transform parent = null;
        Camera camera = null;
        switch (_slot)
        {
            case RoomSlot.Room1:
            {
                layerName = "Room1";
                parent = room1;
                camera = room1Camera;
            }
            break;

            case RoomSlot.Room2:
            {
                layerName = "Room2";
                parent = room2;
                camera = room2Camera;
            }
            break;
        }

        UnityTools.DestroyAllChildren(parent);
        if (_id == "")
            return;

        RoomData data = content.FindRoomData(_id);
        if (data.roomPrefab == null)
        {
            Debug.LogError(string.Format("invalid room id \"{0}\"", _id));
            return;
        }

        Room room = Instantiate<Room>(data.roomPrefab, Vector3.zero, Quaternion.identity);
        room.transform.SetParent(parent, false);

        // Set layer to every child
        UnityTools.SetLayerRecursively(parent, LayerMask.NameToLayer(layerName));

        // Assign outline instance
        _assignOutlinesInstance(parent, camera.GetComponent<cakeslice.OutlineEffect>());

        // Set lights culling mask
        Light[] lights = parent.GetComponentsInChildren<Light>(true);
        foreach (Light light in lights)
        {
            light.cullingMask = LayerMask.GetMask(layerName);
        }
    }

    private bool m_isInRoom1 = true;
    private float m_transitionTimer = 0.0f;

    private InteractiveItem m_hoveredObject = null;
    private GameState m_currentState = GameState.None;
}
