using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
[RequireComponent(typeof(FirstPersonController))]
public class InteractionController : MonoBehaviour {

    private FirstPersonController m_Controller;
    public InteractableObject m_SelectedObject;
    private Camera cam;

    public Texture2D crosshairImage;
    public Material crosshairMat;
    public Transform objectGoToTransform;

    public Vector3 objectOriginalPos;
    public Quaternion objectOriginalRotation;
    private bool characterMovement = true;
    private bool interactionMode = false;

    public LayerMask l_Mask;
    // Use this for initialization
    void Start () {
        m_Controller = GetComponent<FirstPersonController>();
        cam = Camera.main;
    }

    InteractableObject obj;

    void ViewObject()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, l_Mask))
        {
            if (hit.transform.GetComponent<InteractableObject>() != null)
            {
                obj = hit.transform.GetComponent<InteractableObject>();
                obj.m_lookedAt = true;
                obj.ShowMessage();
            }else if(obj != null)
            {
                obj.m_lookedAt = false;
                obj.RemoveMessage();
            }
        }
        else if(obj != null)
        {
            obj.m_lookedAt = false;
            obj.RemoveMessage();
        }
    }

    bool SelectObject()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, l_Mask))
        {
            if (hit.transform.tag == "InteractableObject")
            {
                print("I'm looking at " + hit.transform.name);
                m_SelectedObject = hit.transform.GetComponent<InteractableObject>();
                if (m_SelectedObject == null)
                    return false;
                return true;
            }
            print("I'm looking at nothing!");
            return false;
        }
        else
        {
            print("I'm looking at nothing!");
            return false;
        }
    }

    Vector3 currentMousePosition;
    Vector3 lastMousePosition;
    void InteractionMode()
    {
        if (Input.GetMouseButton(0))
        {
            float yRot = Input.GetAxis("Mouse Y");
            float xRot = Input.GetAxis("Mouse X");
            currentMousePosition = new Vector3(0, yRot, xRot) * 2;
            Quaternion rotation = m_SelectedObject.transform.rotation;
            rotation *= Quaternion.Euler(currentMousePosition);
            m_SelectedObject.transform.rotation = rotation;
        }
    }

    public Vector2 crossHairScale;
    void OnGUI()
    {
        if (!crosshairImage)
        {
            print("No Crosshair!");
            return;
        }
        float xMin = (Screen.width / 2) - ((crosshairImage.width * crossHairScale.x) / 2);
        float yMin = (Screen.height / 2) - ((crosshairImage.height * crossHairScale.y) / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width * crossHairScale.x, crosshairImage.height * crossHairScale.y), crosshairMat.mainTexture);
    }

    void SetplayerMovement(bool active)
    {
        m_Controller.m_movable = active;
    }

    public float speed = 1f;
    IEnumerator MoveObjectClose()
    {
        SetplayerMovement(false);

        m_SelectedObject.m_selected = true;
        objectOriginalPos = m_SelectedObject.transform.position;
        objectOriginalRotation = m_SelectedObject.transform.rotation;

        while (Vector3.Distance(m_SelectedObject.transform.position, objectGoToTransform.position) > 0.01f)
        {
            Vector3 direction = objectGoToTransform.position - m_SelectedObject.transform.position;
            m_SelectedObject.transform.position += direction.normalized * speed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        interactionMode = true;
      
    }

    IEnumerator RotateLerp(Transform target, Quaternion endRot)
    {
        float duration = 1.5f;
        var startRot = target.rotation; // current rotation
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            target.rotation = Quaternion.Slerp(startRot, endRot, timer / duration);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ReturnObject()
    {
        interactionMode = false;
        m_SelectedObject.m_selected = false;

        while (Vector3.Distance(m_SelectedObject.transform.position, objectOriginalPos) > 0.01f)
        {
 
            Vector3 direction = objectOriginalPos - m_SelectedObject.transform.position;
            Vector3 position = m_SelectedObject.transform.position + direction.normalized * speed * Time.deltaTime;
            m_SelectedObject.transform.position = (position);
            yield return new WaitForEndOfFrame();
        }
        SetplayerMovement(true);
        m_SelectedObject = null;
    }

    // Update is called once per frame
    IEnumerator MoveObject()
    {
        if (m_SelectedObject.stateChanger)
        {
            yield return StartCoroutine(MoveObjectClose());
            m_SelectedObject.gameStateSwitcher.SwitchState();
            if(m_SelectedObject.dplayer!= null)
            m_SelectedObject.dplayer.PlayDialogue();
            yield return StartCoroutine(RotateLerp(m_SelectedObject.transform, objectOriginalRotation));
            yield return StartCoroutine(ReturnObject());
        }
        else
        {
            if (m_SelectedObject.dplayer != null)
                m_SelectedObject.dplayer.PlayDialogue();
            yield return StartCoroutine(MoveObjectClose());
        }
    }
    

    void Update () {
        ViewObject();
        if (Input.GetMouseButtonUp(0) && !interactionMode)
        {
            if (SelectObject())
            {
                StartCoroutine(MoveObject());
            }
        }

        if (Input.GetMouseButtonUp(1) && m_SelectedObject != null)
        {
            StartCoroutine(RotateLerp(m_SelectedObject.transform, objectOriginalRotation));
            StartCoroutine(ReturnObject());
        }

        if (interactionMode)
        {
            InteractionMode();
        }
    }
}
