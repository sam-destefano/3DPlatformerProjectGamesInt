using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(CameraFocuser))]
public class CameraFocuserEditor : Editor
{
    private RenderTexture _rt;
    private Camera _cam;
    private GameObject _camGO;
 
    void OnEnable()
    {
        _camGO = EditorUtility.CreateGameObjectWithHideFlags("~PreviewCam", HideFlags.HideAndDontSave, typeof(Camera));
        _cam = _camGO.GetComponent<Camera>();
        _cam.enabled = false;
    }
 
    void OnDisable()
    {
        if (_camGO) DestroyImmediate(_camGO);
        if (_rt) { _rt.Release(); DestroyImmediate(_rt); }
    }
 
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var t = (CameraFocuser)target;
 
        // Match preview aspect to inspector width
        int w = Mathf.RoundToInt(EditorGUIUtility.currentViewWidth - 24f);
        int h = Mathf.RoundToInt(w * 9f / 16f);
 
        // (Re)create RT if needed
        if (_rt == null || _rt.width != w || _rt.height != h)
        {
            if (_rt != null) { _rt.Release(); DestroyImmediate(_rt); }
            _rt = new RenderTexture(w, h, 24) { hideFlags = HideFlags.HideAndDontSave };
        }
 
        // Copy main camera settings, place at this transform
        Camera main = Camera.main;
        if (main != null)
        {
            _cam.CopyFrom(main);
            _cam.enabled = false; // CopyFrom re-enables it
        }
        _cam.transform.SetPositionAndRotation(t.cameraAngle.position, t.cameraAngle.rotation);
        _cam.targetTexture = _rt;
        _cam.Render();
        _cam.targetTexture = null;
 
        Rect rect = GUILayoutUtility.GetRect(w, h);
        GUI.DrawTexture(rect, _rt, ScaleMode.StretchToFill, false);
 
        Repaint();
    }
}