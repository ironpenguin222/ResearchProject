using UnityEngine;
using System.Collections;

public class Button2Controller : MonoBehaviour, ISaveable
{
    public ConveyorManager connectedConveyor;
    private SpriteRenderer sr;

    public string SaveID { get; set; } // Save system fields
    public string objectType = "Button";
    private int savedConveyorIndex = -1;

    public bool pressed = false; // Pressed state
    private int objectsOnPlate = 0; // Counts objects inside trigger

    private void Awake()
    {
        if (string.IsNullOrEmpty(SaveID))
            SaveID = System.Guid.NewGuid().ToString();

        sr = GetComponent<SpriteRenderer>();

        SaveHolder.Register(this);
    }

    private void OnDestroy()
    {
        SaveHolder.Unregister(this);
    }

    private void Start()
    {
        if (connectedConveyor == null && savedConveyorIndex >= 0)
            StartCoroutine(ReconnectWhenObjectsStopDoing());
    }

    private IEnumerator ReconnectWhenObjectsStopDoing() // Reconnect after loads
    {
        int lastCount = -1;

        while (true)
        {
            int current = SaveHolder.objects.Count;
            if (current == lastCount)
                break;

            lastCount = current;
            yield return null;
        }

        int index = 0;
        foreach (var obj in SaveHolder.objects.Values)
        {
            if (obj is ConveyorManager conveyor)
            {
                if (index == savedConveyorIndex)
                {
                    connectedConveyor = conveyor;
                    ApplyPressedState();
                    yield break;
                }
                index++;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // When touching something
    {
        objectsOnPlate++;

        if (!pressed && objectsOnPlate > 0)
            SetPressed(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        objectsOnPlate = Mathf.Max(0, objectsOnPlate - 1);

        if (pressed && objectsOnPlate == 0)
            SetPressed(false);
    }

    private void SetPressed(bool state)
    {
        pressed = state;

        if (connectedConveyor != null)
            connectedConveyor.direction = -connectedConveyor.direction;

        ApplyPressedState();
    }

    private void ApplyPressedState()
    {
        sr.color = pressed ? Color.red : Color.white;
    }


    public ObjectSaveData SaveData() // Saving
    {
        ObjectSaveData data = new ObjectSaveData();
        data.id = SaveID;
        data.type = "Button";

        data.Set("posX", transform.position.x.ToString());
        data.Set("posY", transform.position.y.ToString());
        data.Set("rot", transform.eulerAngles.z.ToString());
        data.Set("active", gameObject.activeSelf.ToString());
        data.Set("pressed", pressed.ToString());

        int index = 0;  // Save conveyor index
        int foundIndex = -1;
        foreach (var obj in SaveHolder.objects.Values)
        {
            if (obj is ConveyorManager conv)
            {
                if (conv == connectedConveyor)
                {
                    foundIndex = index;
                    break;
                }
                index++;
            }
        }
        data.Set("conveyorIndex", foundIndex.ToString());

        return data;
    }

    public void LoadData(ObjectSaveData data) // Loading
    {
        float x = float.Parse(data.Get("posX"));
        float y = float.Parse(data.Get("posY"));
        float rot = float.Parse(data.Get("rot"));

        transform.position = new Vector2(x, y);
        transform.eulerAngles = new Vector3(0, 0, rot);
        gameObject.SetActive(bool.Parse(data.Get("active")));

        pressed = bool.Parse(data.Get("pressed"));
        savedConveyorIndex = int.Parse(data.Get("conveyorIndex"));

        ApplyPressedState();
    }
}