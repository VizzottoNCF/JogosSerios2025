using Unity.Cinemachine;
using UnityEngine;

public class CameraControlTrigger : MonoBehaviour
{
    public rc_CustomInspectorObjects customInspectorObjects;

    private Collider2D _coll;

    private void Start()
    {
        _coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                // pan the camera into the collider
                CameraManager.instance.rf_PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;

            if (customInspectorObjects.swapCameras && customInspectorObjects.CameraOnLeft != null && customInspectorObjects.CameraOnRight != null)
            {
                // swap cameras
                CameraManager.instance.rf_SwapCamera(customInspectorObjects.CameraOnLeft, customInspectorObjects.CameraOnRight, exitDirection);
            }

            if (customInspectorObjects.panCameraOnContact)
            {
                // pan the camera out of the collider
                CameraManager.instance.rf_PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }
}

[System.Serializable]
public class rc_CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineCamera CameraOnLeft;
    [HideInInspector] public CinemachineCamera CameraOnRight;

    [HideInInspector] public re_PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = 0.35f;
}

public enum re_PanDirection
{
    Up,
    Down,
    Left,
    Right
}

//#if UNITY_EDITOR
//using UnityEditor;
//[CustomEditor(typeof(CameraControlTrigger))]
//public class MyScriptEditor : Editor
//{
//    CameraControlTrigger cameraControlTrigger;

//    private void OnEnable()
//    {
//        cameraControlTrigger = (CameraControlTrigger)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        if (cameraControlTrigger.customInspectorObjects.swapCameras)
//        {
//            cameraControlTrigger.customInspectorObjects.CameraOnLeft = EditorGUILayout.ObjectField("Camera on Left", cameraControlTrigger.customInspectorObjects.CameraOnLeft, typeof(CinemachineCamera), true) as CinemachineCamera;

//            cameraControlTrigger.customInspectorObjects.CameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraControlTrigger.customInspectorObjects.CameraOnRight, typeof(CinemachineCamera), true) as CinemachineCamera;
//        }

//        if (cameraControlTrigger.customInspectorObjects.panCameraOnContact)
//        {
//            cameraControlTrigger.customInspectorObjects.panDirection = (re_PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction", cameraControlTrigger.customInspectorObjects.panDirection);

//            cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspectorObjects.panDistance);
//            cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspectorObjects.panTime);
//        }

//        if (GUI.changed)
//        {
//            EditorUtility.SetDirty(cameraControlTrigger);
//        }
//    }
//}
//#endif