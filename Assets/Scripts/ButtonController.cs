using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour, ISaveable
{
    public ConveyorManager connectedConveyor; // Reference to conveyor
    public GameObject interactPrompt; // The prompt for interaction
    private SpriteRenderer sr; // Sprite renderer
    private int savedConveyorIndex = -1; // The index of conveyor connection
    public bool playerNearby = false; // Is the player near?
    public bool pressed = false; // Is pressed?
    public string SaveID { get; set; } // Save ID
    public string objectType = "Button"; // 'Tis a button


    private void Awake()
    {
        if (string.IsNullOrEmpty(SaveID)) // The Save ID
            SaveID = System.Guid.NewGuid().ToString();

        sr = GetComponent<SpriteRenderer>();

        SaveHolder.Register(this); // Register it

        if (interactPrompt == null) // find the interact prompt in scene
        {
            Transform found = transform.Find("InteractPrompt");
            if (found != null)
                interactPrompt = found.gameObject;
        }

        if (interactPrompt != null)
            interactPrompt.SetActive(false); // No prompt shown
    }

    private void OnDestroy()
    {
        SaveHolder.Unregister(this); // Remove it from list
    }

    private void Start()
    {
        if (connectedConveyor == null && savedConveyorIndex >= 0)
            StartCoroutine(ReconnectWhenObjectsStopDoing()); // Connect after everything has been done
    }

    private IEnumerator ReconnectWhenObjectsStopDoing()
    {
        int lastCount = -1; // Wait until the number of objects is done

        while (true) // Checks the objects to ensure waiting
        {
            int current = SaveHolder.objects.Count;

            if (current == lastCount)
                break;

            lastCount = current;
            yield return null;
        }

        int index = 0; // Conveyor reconnection index

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

    private void Update()
    {
        if (!playerNearby || connectedConveyor == null)
            return;

        if (Input.GetKeyDown(KeyCode.E)) // Change the direction of conveyor on E
            ToggleConveyorDirection();
    }

    private void ToggleConveyorDirection()
    {
        pressed = !pressed; // switches if pressed

        if (connectedConveyor != null)
            connectedConveyor.direction = -connectedConveyor.direction; // Opposite direction

        ApplyPressedState(); // Applies state
    }

    private void ApplyPressedState()
    {
        sr.color = pressed ? Color.red : Color.white; // Sets the color to match on/off
    }

    private void OnTriggerEnter2D(Collider2D other) // Checks if player is on
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) // Checks if player is off
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }

    public ObjectSaveData SaveData() // Saves the object data
    {
        ObjectSaveData data = new ObjectSaveData();
        data.id = SaveID;
        data.type = "Button";

        data.Set("posX", transform.position.x.ToString());
        data.Set("posY", transform.position.y.ToString());
        data.Set("rot", transform.eulerAngles.z.ToString());
        data.Set("active", gameObject.activeSelf.ToString());
        data.Set("pressed", pressed.ToString());

        int index = 0;
        int foundIndex = -1;
        foreach (var obj in SaveHolder.objects.Values)
        {
            if (obj is ConveyorManager conv) // Search for conveyor
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

    public void LoadData(ObjectSaveData data) // load all the data
    {
        float x = float.Parse(data.Get("posX"));
        float y = float.Parse(data.Get("posY"));
        float rot = float.Parse(data.Get("rot"));

        transform.position = new Vector2(x, y);
        transform.eulerAngles = new Vector3(0, 0, rot);
        gameObject.SetActive(bool.Parse(data.Get("active")));

        pressed = bool.Parse(data.Get("pressed"));
        savedConveyorIndex = int.Parse(data.Get("conveyorIndex"));
    }
}
