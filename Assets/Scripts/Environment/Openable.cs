using UnityEngine;
using System.Collections; // Needed for coroutines

public class Openable : MonoBehaviour, IOpenable
{
    public enum ObjectType { Door, Drawer }

    public ObjectType objectType; // Specify if it's a door or a drawer
    public float openDistance = 0.5f; // How far the drawer slides out
    public float openAngle = 90f;   // How far the door rotates
    public float speed = 2f;        // Speed of opening/closing

    private Vector3 closedPosition; // Position when the drawer is closed
    private Vector3 openPosition;   // Position when the drawer is open
    private Quaternion closedRotation; // Rotation when the door is closed
    private Quaternion openRotation;   // Rotation when the door is open
    private bool isOpen = false;

    public AK.Wwise.Event doorOpenSound;
    public AK.Wwise.Event doorCloseSound;
    public AK.Wwise.Event drawerOpenSound;
    public AK.Wwise.Event drawerCloseSound;

    void Start()
    {
        // Initialize positions and rotations
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(openDistance, 0f, 0f); // Drawer opens along the X-axis

        closedRotation = transform.rotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0); // Door opens with rotation
    }

    // Implement the ToggleOpen method from IOpenable
    public void ToggleOpen()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    // Open the object (drawer or door)
    public void Open()
    {
        if (objectType == ObjectType.Drawer)
        {
            drawerOpenSound.Post(this.gameObject);
            StartCoroutine(MoveDrawer(openPosition)); // Move drawer to open position
        }
        else if (objectType == ObjectType.Door)
        {
            doorOpenSound.Post(this.gameObject);
            StartCoroutine(RotateDoor(openRotation)); // Rotate door to open position
        }
    }

    // Close the object (drawer or door)
    public void Close()
    {
        if (objectType == ObjectType.Drawer)
        {
            drawerCloseSound.Post(this.gameObject);
            StartCoroutine(MoveDrawer(closedPosition)); // Move drawer to closed position
        }
        else if (objectType == ObjectType.Door)
        {
            doorCloseSound.Post(this.gameObject);
            StartCoroutine(RotateDoor(closedRotation)); // Rotate door to closed position
        }
    }

    // Coroutine to smoothly move the drawer
    private IEnumerator MoveDrawer(Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < 1f)
        {
            time += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, time);
            // Ensure rotation is not altered for the drawer
            transform.rotation = closedRotation; // Keep the initial rotation fixed
            yield return null;
        }
    }

    // Coroutine to smoothly rotate the door
    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        float time = 0;
        Quaternion startRotation = transform.rotation;

        while (time < 1f)
        {
            time += Time.deltaTime * speed;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time);
            yield return null;
        }
    }
}
