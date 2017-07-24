using UnityEngine;
using System.Collections;

/// <summary>
///     Simple script to handle the functionality of the Rotate Gizmo (i.e. rotate the gizmo
///     and the target object along the axis the user is dragging towards)
/// </summary>
/// 
/// <author>
///     Michael Hillman - thisishillman.co.uk
/// </author>
/// 
/// <version>
///     1.0.0 - 01st January 2016
/// </version>
public class GizmoRotateScript : MonoBehaviour {

    /// <summary>
    ///     Rotation speed scalar
    /// </summary>
    public float rotationSpeed = 75.0f;

    /// <summary>
    ///     X torus of gizmo
    /// </summary>
    public GameObject xTorus;

    /// <summary>
    ///     Y torus of gizmo
    /// </summary>
    public GameObject yTorus;

    /// <summary>
    ///     Z torus of gizmo
    /// </summary>
    public GameObject zTorus;

    /// <summary>
    ///     Target for rotation
    /// </summary>
    public GameObject rotateTarget;

    /// <summary>
    ///     Array of detector scripts stored as [x, y, z]
    /// </summary>
    private GizmoClickDetection[] detectors;

    /// <summary>
    ///     On wake up
    /// </summary>
    public void Awake() {

        // Get the click detection scripts
        detectors = new GizmoClickDetection[3];
        detectors[0] = xTorus.GetComponent<GizmoClickDetection>();
        detectors[1] = yTorus.GetComponent<GizmoClickDetection>();
        detectors[2] = zTorus.GetComponent<GizmoClickDetection>();

        // Set the same position for the target and the gizmo
        transform.position = rotateTarget.transform.position;
    }

    /// <summary>
    ///     Once per frame
    /// </summary>
    public void Update() {
        for (int i = 0; i < 3; i++) {

            if (Input.GetMouseButton(0) && detectors[i].pressing) {

                // Rotation angle
                float delta = (Input.GetAxis("Mouse X") - Input.GetAxis("Mouse Y")) * (Time.deltaTime);
                delta *= rotationSpeed;

                switch (i) {
                    // X Axis
                    case 0:
                        rotateTarget.transform.Rotate(Vector3.right, delta);
                        gameObject.transform.Rotate(Vector3.right, delta);
                        break;

                    // Y Axis
                    case 1:
                        rotateTarget.transform.Rotate(Vector3.up, delta);
                        gameObject.transform.Rotate(Vector3.up, delta);
                        break;

                    // Z Axis
                    case 2:
                        rotateTarget.transform.Rotate(Vector3.forward, delta);
                        gameObject.transform.Rotate(Vector3.forward, delta);
                         break;
                }

                break;
            }
        }
    }

}
// End of script.
