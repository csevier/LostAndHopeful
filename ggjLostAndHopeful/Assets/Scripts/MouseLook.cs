using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 0.1f;
    public float smoothing = 10.0f;
    public float lookYLimit = 60.0f;

    public Character character;
    private Vector2 mouseLook;
    private Vector2 smoothV;
    void Start()
    {
        //character = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDelta = new Vector2(character.lookInput.x * sensitivity, character.lookInput.y * sensitivity);

        smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing); 
        smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);

        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, -lookYLimit, lookYLimit);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }

}
