using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MouseLook: MonoBehaviour
{
    public float sensitivity = 2.0f;
    public float smoothing = 5.0f;
    public float lookYLimit = 60.0f;

    public GameObject character;
    private Vector2 mouseLook;
    private Vector2 smoothV;
    void Start()
    {
        character = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

            smoothV.x = Mathf.Lerp(smoothV.x, mouseDelta.x, 1f / smoothing);
            smoothV.y = Mathf.Lerp(smoothV.y, mouseDelta.y, 1f / smoothing);

            mouseLook += smoothV;
            mouseLook.y = Mathf.Clamp(mouseLook.y, -lookYLimit, lookYLimit);

            transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
            character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
