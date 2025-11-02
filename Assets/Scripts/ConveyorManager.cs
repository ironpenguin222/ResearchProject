using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ConveyorManager : MonoBehaviour
{
    public string color;
    public Vector2 direction = Vector2.right; // What direction is it going in?
    public float speed = 5f; // How fast is it moving?
    private Rigidbody2D rb;
    public bool isOn; // Is it on? Can the player interact?
    public List<Rigidbody2D> touchingObjects = new List<Rigidbody2D>(); // All objexts touching it

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        ObjectSaveData saveData = new ObjectSaveData();
        saveData.type = "Conveyor";
        saveData.position = transform.position;
        saveData.rotation = transform.eulerAngles.z;
        saveData.isActive = gameObject.activeSelf;
        saveData.color = color;
        saveData.speed = speed;
        saveData.direction = direction;
        saveData.isOn = isOn;
        return saveData;
    }

    public void LoadData(ObjectSaveData saveData)
    {
        transform.position = saveData.position;
        transform.eulerAngles = new Vector3(0, 0, saveData.rotation);
        gameObject.SetActive(saveData.isActive);

        speed = saveData.speed;
        direction = saveData.direction;
        isOn = saveData.isOn;
    }
}
