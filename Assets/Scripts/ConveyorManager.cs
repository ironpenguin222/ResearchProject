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
    public string SaveID { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SaveID = gameObject.name;
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

    public ObjectSaveData SaveData()
    {
        ObjectSaveData data = new ObjectSaveData();
        data.id = SaveID;
        data.type = "Conveyor";
        data.data["posX"] = transform.position.x.ToString();
        data.data["posY"] = transform.position.y.ToString();
        data.data["rot"] = transform.eulerAngles.z.ToString();
        data.data["active"] = gameObject.activeSelf.ToString();
        data.data["color"] = color;
        data.data["speed"] = speed.ToString();
        data.data["dirX"] = direction.x.ToString();
        data.data["dirY"] = direction.y.ToString();
        data.data["isOn"] = isOn.ToString();
        return data;
    }

    public void LoadData(ObjectSaveData data)
    {
        float x = float.Parse(data.data["posX"]);
        float y = float.Parse(data.data["posY"]);
        float rotation = float.Parse(data.data["rot"]);

        transform.position = new Vector2(x, y);
        transform.eulerAngles = new Vector3(0, 0, rotation);
        gameObject.SetActive(bool.Parse(data.data["active"]));
        color = data.data["color"];
        speed = float.Parse(data.data["speed"]);
        direction = new Vector2(float.Parse(data.data["dirX"]), float.Parse(data.data["dirY"]));
        isOn = bool.Parse(data.data["isOn"]);

    }
}