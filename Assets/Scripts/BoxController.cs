using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour, ISaveable
{
    public string color;
    public float moveDistance = 1f; // Distance moved when pushed
    public LayerMask collisionStop; // Layers that prevent box from moving past
    private Rigidbody2D rb;
    public string SaveID { get; set; }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SaveID = gameObject.name;
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

    public ObjectSaveData SaveData()
    {
        ObjectSaveData data = new ObjectSaveData();
        data.id = SaveID;
        data.type = "Box";
        data.data["posX"] = transform.position.x.ToString();
        data.data["posY"] = transform.position.y.ToString();
        data.data["rot"] = transform.eulerAngles.z.ToString();
        data.data["active"] = gameObject.activeSelf.ToString();
        data.data["color"] = color;
        return data;
    }

    public void LoadData(ObjectSaveData data)
    {
        Debug.Log("loading");
        float x = float.Parse(data.data["posX"]);
        float y = float.Parse(data.data["posY"]);
        float rotation = float.Parse(data.data["rot"]);

        transform.position = new Vector2(x,y);
        transform.eulerAngles = new Vector3(0,0,rotation);
        gameObject.SetActive(bool.Parse(data.data["active"]));
        color = data.data["color"];
    }
}
