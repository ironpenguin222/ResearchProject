using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour, ISaveable
{
    public string color;
    public float moveDistance = 1f; // Distance moved when pushed
    public LayerMask collisionStop; // Layers that prevent box from moving past
    private Rigidbody2D rb;
    public string SaveID { get; set; } // The ID of the object
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SaveID = gameObject.name; // Sets the object id to the gameObject's name
    }

    public void TryPush(Vector2 direction)
    {
            direction = direction.normalized;
            Vector2 posToGo = rb.position + direction * moveDistance; // Finds the location it should move to
        RaycastHit2D moveHit = Physics2D.Raycast(rb.position, direction, moveDistance, collisionStop); // Checks if its going to be moving into something it shouldn't
        if(moveHit.collider == null)
        {
            rb.MovePosition(posToGo); // Moves it
        }
    }

    public ObjectSaveData SaveData() // Sets up the keys with their values
    {
        ObjectSaveData data = new ObjectSaveData();
        data.id = SaveID;
        data.type = "Box";
        data.Set("posX", transform.position.x.ToString());
        data.Set("posY", transform.position.y.ToString());
        data.Set("rot", transform.eulerAngles.z.ToString());
        data.Set("active", gameObject.activeSelf.ToString());
        data.Set("color", color);
        return data;
    }

    public void LoadData(ObjectSaveData data) // Grabs the necessary values from the keys and gives those values to boxes
    {
        Debug.Log("loading");
        float x = float.Parse(data.Get("posX"));
        float y = float.Parse(data.Get("posY"));
        float rotation = float.Parse(data.Get("rot"));

        transform.position = new Vector2(x,y);
        transform.eulerAngles = new Vector3(0,0,rotation);
        gameObject.SetActive(bool.Parse(data.Get("active")));
        color = data.Get("color");
    }
}
