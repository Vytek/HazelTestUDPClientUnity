using UnityEngine;
using System.Collections;

/// <summary>
///     Simple script to handle the functionality of the Translate Gizmo (i.e. move the gizmo
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
public class GizmoTranslateScript : MonoBehaviour {

    /// <summary>
    ///     X axis of gizmo
    /// </summary>
    public GameObject xAxisObject;

    /// <summary>
    ///     Y axis of gizmo
    /// </summary>
    public GameObject yAxisObject;

    /// <summary>
    ///     Z axis of gizmo
    /// </summary>
    public GameObject zAxisObject;

    /// <summary>
    ///     Target for translation
    /// </summary>
    public GameObject translateTarget;

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
        detectors[0] = xAxisObject.GetComponent<GizmoClickDetection>();
        detectors[1] = yAxisObject.GetComponent<GizmoClickDetection>();
        detectors[2] = zAxisObject.GetComponent<GizmoClickDetection>();

        // Set the same position for the target and the gizmo
        transform.position = translateTarget.transform.position;
    }

    /// <summary>
    ///     Once per frame
    /// </summary>
    public void Update() {
        for (int i = 0; i < 3; i++) {
            if (Input.GetMouseButton(0) && detectors[i].pressing) {

                // Get the distance from the camera to the target (used as a scaling factor in translate)
                float distance = Vector3.Distance(Camera.main.transform.position, translateTarget.transform.position);
                distance = distance * 2.0f;

                // Will store translate values
                Vector3 offset = Vector3.zero;

                switch (i) {
                    // X Axis
                    case 0:
                        {
                            // If the user is pressing the plane, move along Y and Z, else move along X

                            if (detectors[i].pressingPlane) {
                                float deltaY = Input.GetAxis("Mouse Y") * (Time.deltaTime * distance);
                                offset = Vector3.up * deltaY;
                                offset = new Vector3(0.0f, offset.y, 0.0f);
                                translateTarget.transform.Translate(offset);

                                float deltaZ = Input.GetAxis("Mouse X") * (Time.deltaTime * distance);
                                offset = Vector3.forward * deltaZ;
                                offset = new Vector3(0.0f, 0.0f, offset.z);
                                translateTarget.transform.Translate(offset);

                            } else {
                                float delta = Input.GetAxis("Mouse X") * (Time.deltaTime * distance);
                                offset = Vector3.left * delta;
                                offset = new Vector3(offset.x, 0.0f, 0.0f);
                                translateTarget.transform.Translate(offset);
                            }
                        }
                        break;

                    // Y Axis
                    case 1:
                        {
                            // If the user is pressing the plane, move along X and Z, else just move along X

                            if (detectors[i].pressingPlane) {
                                float deltaX = Input.GetAxis("Mouse X") * (Time.deltaTime * distance);
                                offset = Vector3.left * deltaX;
                                offset = new Vector3(offset.x, 0.0f, 0.0f);
                                translateTarget.transform.Translate(offset);

                                float deltaZ = Input.GetAxis("Mouse Y") * (Time.deltaTime * distance);
                                offset = Vector3.forward * deltaZ;
                                offset = new Vector3(0.0f, 0.0f, -offset.z);
                                translateTarget.transform.Translate(offset);

                            } else {
                                float delta = Input.GetAxis("Mouse Y") * (Time.deltaTime * distance);
                                offset = Vector3.up * delta;
                                offset = new Vector3(0.0f, offset.y, 0.0f);
                                translateTarget.transform.Translate(offset);
                            }
                        }
                        break;

                    // Z Axis
                    case 2:
                        {
                            // If the user is pressing the plane, move along X and Y, else just move along Z

                            if (detectors[i].pressingPlane) {
                                float deltaX = Input.GetAxis("Mouse X") * (Time.deltaTime * distance);
                                offset = Vector3.left * deltaX;
                                offset = new Vector3(offset.x, 0.0f, 0.0f);
                                translateTarget.transform.Translate(offset);

                                float deltaY = Input.GetAxis("Mouse Y") * (Time.deltaTime * distance);
                                offset = Vector3.up * deltaY;
                                offset = new Vector3(0.0f, offset.y, 0.0f);
                                translateTarget.transform.Translate(offset);

                            } else {
                                float delta = Input.GetAxis("Mouse X") * (Time.deltaTime * distance);
                                offset = Vector3.forward * delta;
                                offset = new Vector3(0.0f, 0.0f, offset.z);
                                translateTarget.transform.Translate(offset);
                            }
                        }
                        break;
                }

                // Move the gizmo to match the target position
                transform.position = translateTarget.transform.position;

                break;
            }
        }
    }

}
// End of script.
