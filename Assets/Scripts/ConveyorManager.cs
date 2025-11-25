using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ConveyorManager : MonoBehaviour, ISaveable
{
    public string color;
    public Vector2 direction = Vector2.right; // What direction is it going in?
    public float speed = 5f; // How fast is it moving?
    private Rigidbody2D rb;
    public bool isOn; // Is it on? Can the player interact?
    public List<Rigidbody2D> touchingObjects = new List<Rigidbody2D>(); // All objexts touching it
    public string SaveID { get; set; } // The ID of the object 
    public string objectType = "Conveyor"; // The object's type
    private SpriteRenderer sr;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (string.IsNullOrEmpty(SaveID))
            SaveID = System.Guid.NewGuid().ToString();
        SaveHolder.Register(this);
    }

    void OnDestroy()
    {
        SaveHolder.Unregister(this);
    }

    private void FixedUpdate()
    {
        if (!isOn) return;

        foreach (Rigidbody2D rb in touchingObjects)
        {
            if (rb != null)
            {
                // If it’s the player, we add force to them using their script
                PlayerController player = rb.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.AddExternalForce(direction.normalized * speed);
                }
                else
                {
                    // For boxes or other objects
                    rb.MovePosition(rb.position + direction.normalized * speed * Time.fixedDeltaTime);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.attachedRigidbody; // Finds objects touching the conveyor
        if (rb != null && !touchingObjects.Contains(rb))
        {
            touchingObjects.Add(rb);
        }
    }
    private void OnTriggerExit2D(Collider2D collision) // Removes touching objects when no longer touching
    {
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null && touchingObjects.Contains(rb))
        {
            touchingObjects.Remove(rb);
        }
    }

    private void ApplyColor()
    {
        if (string.IsNullOrEmpty(color)) // Makes white if there is no color
        {
            sr.color = Color.white;
            return;
        }

        if (color == "Blue")
            sr.color = Color.blue; // Makes blue if its blue
        else if (color == "Green")
            sr.color = Color.green; // makes green if its green
        else
            sr.color = Color.white; // Fallback thing
    }

    public ObjectSaveData SaveData() // Sets up the keys with their values
    {
        ObjectSaveData data = new ObjectSaveData();
        data.id = SaveID;
        data.type = "Conveyor";
        data.Set("posX", transform.position.x.ToString());
        data.Set("posY", transform.position.y.ToString());
        data.Set("rot", transform.eulerAngles.z.ToString());
        data.Set("active", gameObject.activeSelf.ToString());
        data.Set("color", color);
        data.Set("speed", speed.ToString());
        data.Set("dirX", direction.x.ToString());
        data.Set("dirY", direction.y.ToString());
        data.Set("isOn", isOn.ToString());
        return data;
    }

    public void LoadData(ObjectSaveData data) // Grabs the necessary values from the keys and gives those values to boxes
    {
        float x = float.Parse(data.Get("posX"));
        float y = float.Parse(data.Get("posY"));
        float rotation = float.Parse(data.Get("rot"));

        transform.position = new Vector2(x, y);
        transform.eulerAngles = new Vector3(0, 0, rotation);
        gameObject.SetActive(bool.Parse(data.Get("active")));
        color = data.Get("color");
        speed = float.Parse(data.Get("speed"));
        direction = new Vector2(float.Parse(data.Get("dirX")), float.Parse(data.Get("dirY")));
        isOn = bool.Parse(data.Get("isOn"));
        ApplyColor();
    }
}