﻿using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(LoopScrollRectBase), true)]
public class LoopScrollRectInspector : Editor
{
    SerializedProperty m_Content;
    SerializedProperty m_Horizontal;
    SerializedProperty m_Vertical;
    SerializedProperty m_MovementType;
    SerializedProperty m_Elasticity;
    SerializedProperty m_Inertia;
    SerializedProperty m_DecelerationRate;
    SerializedProperty m_ScrollSensitivity;
    SerializedProperty m_Viewport;
    SerializedProperty m_HorizontalScrollbar;
    SerializedProperty m_VerticalScrollbar;
    SerializedProperty m_HorizontalScrollbarVisibility;
    SerializedProperty m_VerticalScrollbarVisibility;
    SerializedProperty m_HorizontalScrollbarSpacing;
    SerializedProperty m_VerticalScrollbarSpacing;
    SerializedProperty m_OnValueChanged;
    AnimBool m_ShowElasticity;
    AnimBool m_ShowDecelerationRate;
    bool m_ViewportIsNotChild, m_HScrollbarIsNotChild, m_VScrollbarIsNotChild;
    static string s_HError = "For this visibility mode, the Viewport property and the Horizontal Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";
    static string s_VError = "For this visibility mode, the Viewport property and the Vertical Scrollbar property both needs to be set to a Rect Transform that is a child to the Scroll Rect.";

    //==========LoopScrollRect==========
    SerializedProperty totalCount;
    SerializedProperty reverseDirection;
    SerializedProperty m_InertiaSnapToCell;
    SerializedProperty m_SnappingScrollMode;
    SerializedProperty m_InertiaSnapThreshold;
    SerializedProperty m_SnapAnimationTime;
    int index = 0;
    float offset = 0;
    LoopScrollRectBase.ScrollMode scrollMode = LoopScrollRectBase.ScrollMode.ToStart;
    float speed = 1000, time = 1;

    protected virtual void OnEnable()
    {
        m_Content = serializedObject.FindProperty("m_Content");
        m_Horizontal = serializedObject.FindProperty("m_Horizontal");
        m_Vertical = serializedObject.FindProperty("m_Vertical");
        m_MovementType = serializedObject.FindProperty("m_MovementType");
        m_Elasticity = serializedObject.FindProperty("m_Elasticity");
        m_Inertia = serializedObject.FindProperty("m_Inertia");
        m_DecelerationRate = serializedObject.FindProperty("m_DecelerationRate");
        m_ScrollSensitivity = serializedObject.FindProperty("m_ScrollSensitivity");
        m_Viewport = serializedObject.FindProperty("m_Viewport");
        m_HorizontalScrollbar = serializedObject.FindProperty("m_HorizontalScrollbar");
        m_VerticalScrollbar = serializedObject.FindProperty("m_VerticalScrollbar");
        m_HorizontalScrollbarVisibility = serializedObject.FindProperty("m_HorizontalScrollbarVisibility");
        m_VerticalScrollbarVisibility = serializedObject.FindProperty("m_VerticalScrollbarVisibility");
        m_HorizontalScrollbarSpacing = serializedObject.FindProperty("m_HorizontalScrollbarSpacing");
        m_VerticalScrollbarSpacing = serializedObject.FindProperty("m_VerticalScrollbarSpacing");
        m_OnValueChanged = serializedObject.FindProperty("m_OnValueChanged");

        m_ShowElasticity = new AnimBool(Repaint);
        m_ShowDecelerationRate = new AnimBool(Repaint);
        SetAnimBools(true);

        //==========LoopScrollRect==========
        totalCount = serializedObject.FindProperty("totalCount");
        reverseDirection = serializedObject.FindProperty("reverseDirection");
        m_InertiaSnapToCell = serializedObject.FindProperty("m_InertiaSnapToCell");
        m_SnappingScrollMode = serializedObject.FindProperty("m_SnappingScrollMode");
        m_InertiaSnapThreshold = serializedObject.FindProperty("m_InertiaSnapThreshold");
        m_SnapAnimationTime = serializedObject.FindProperty("m_SnapAnimationTime");
    }

    protected virtual void OnDisable()
    {
        m_ShowElasticity.valueChanged.RemoveListener(Repaint);
        m_ShowDecelerationRate.valueChanged.RemoveListener(Repaint);
    }

    void SetAnimBools(bool instant)
    {
        SetAnimBool(m_ShowElasticity, !m_MovementType.hasMultipleDifferentValues && m_MovementType.enumValueIndex == (int)ScrollRect.MovementType.Elastic, instant);
        SetAnimBool(m_ShowDecelerationRate, !m_Inertia.hasMultipleDifferentValues && m_Inertia.boolValue == true, instant);
    }

    void SetAnimBool(AnimBool a, bool value, bool instant)
    {
        if (instant)
            a.value = value;
        else
            a.target = value;
    }

    void CalculateCachedValues()
    {
        m_ViewportIsNotChild = false;
        m_HScrollbarIsNotChild = false;
        m_VScrollbarIsNotChild = false;
        if (targets.Length == 1)
        {
            Transform transform = ((LoopScrollRectBase)target).transform;
            if (m_Viewport.objectReferenceValue == null || ((RectTransform)m_Viewport.objectReferenceValue).transform.parent != transform)
                m_ViewportIsNotChild = true;
            if (m_HorizontalScrollbar.objectReferenceValue == null || ((Scrollbar)m_HorizontalScrollbar.objectReferenceValue).transform.parent != transform)
                m_HScrollbarIsNotChild = true;
            if (m_VerticalScrollbar.objectReferenceValue == null || ((Scrollbar)m_VerticalScrollbar.objectReferenceValue).transform.parent != transform)
                m_VScrollbarIsNotChild = true;
        }
    }

    public override void OnInspectorGUI()
    {
        SetAnimBools(false);

        serializedObject.Update();
        // Once we have a reliable way to know if the object changed, only re-cache in that case.
        CalculateCachedValues();

        EditorGUILayout.LabelField("Origin UGUI Scroll Rect", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_Content);

        EditorGUILayout.PropertyField(m_Horizontal);
        EditorGUILayout.PropertyField(m_Vertical);

        EditorGUILayout.PropertyField(m_MovementType);
        if (EditorGUILayout.BeginFadeGroup(m_ShowElasticity.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_Elasticity);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUILayout.PropertyField(m_Inertia);
        if (EditorGUILayout.BeginFadeGroup(m_ShowDecelerationRate.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_DecelerationRate);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        EditorGUILayout.PropertyField(m_ScrollSensitivity);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_Viewport);

        EditorGUILayout.PropertyField(m_HorizontalScrollbar);
        if (m_HorizontalScrollbar.objectReferenceValue && !m_HorizontalScrollbar.hasMultipleDifferentValues)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_HorizontalScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

            if ((ScrollRect.ScrollbarVisibility)m_HorizontalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport
                && !m_HorizontalScrollbarVisibility.hasMultipleDifferentValues)
            {
                if (m_ViewportIsNotChild || m_HScrollbarIsNotChild)
                    EditorGUILayout.HelpBox(s_HError, MessageType.Error);
                EditorGUILayout.PropertyField(m_HorizontalScrollbarSpacing, EditorGUIUtility.TrTextContent("Spacing"));
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(m_VerticalScrollbar);
        if (m_VerticalScrollbar.objectReferenceValue && !m_VerticalScrollbar.hasMultipleDifferentValues)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_VerticalScrollbarVisibility, EditorGUIUtility.TrTextContent("Visibility"));

            if ((ScrollRect.ScrollbarVisibility)m_VerticalScrollbarVisibility.enumValueIndex == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport
                && !m_VerticalScrollbarVisibility.hasMultipleDifferentValues)
            {
                if (m_ViewportIsNotChild || m_VScrollbarIsNotChild)
                    EditorGUILayout.HelpBox(s_VError, MessageType.Error);
                EditorGUILayout.PropertyField(m_VerticalScrollbarSpacing, EditorGUIUtility.TrTextContent("Spacing"));
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_OnValueChanged);

        //==========LoopScrollRect==========
        EditorGUILayout.LabelField("Loop Scroll Rect", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(totalCount);
        EditorGUILayout.PropertyField(reverseDirection);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Snapping Options", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_InertiaSnapToCell, new GUIContent("Snap To Cell On Inertia End", "Whether the scroll view should snap to a cell when inertia scrolling ends"));
        if (m_InertiaSnapToCell.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_SnappingScrollMode, new GUIContent("Snapping Mode", "The mode used when snapping to a cell after inertia scrolling"));
            EditorGUILayout.PropertyField(m_InertiaSnapThreshold, new GUIContent("Snap Threshold", "When inertia velocity magnitude is below this value, snapping to cell will start"));
            EditorGUILayout.PropertyField(m_SnapAnimationTime, new GUIContent("Snap Animation Time", "Duration in seconds for the snap animation"));
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();

        LoopScrollRectBase scroll = (LoopScrollRectBase)target;
        GUI.enabled = Application.isPlaying;

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
        {
            scroll.ClearCells();
        }
        if (GUILayout.Button("Refresh"))
        {
            scroll.RefreshCells();
        }
        if (GUILayout.Button("Refill"))
        {
            scroll.RefillCells();
        }
        if (GUILayout.Button("RefillFromEnd"))
        {
            scroll.RefillCellsFromEnd();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        float w = (EditorGUIUtility.currentViewWidth - 50) / 3;
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 45;
        index = EditorGUILayout.IntField("Index", index, GUILayout.Width(w));
        offset = EditorGUILayout.FloatField("Offset", offset, GUILayout.Width(w));
        scrollMode = (LoopScrollRectBase.ScrollMode)EditorGUILayout.EnumPopup("Mode", scrollMode, GUILayout.Width(w));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 60;
        EditorGUI.indentLevel++;
        speed = EditorGUILayout.FloatField("Speed", speed, GUILayout.Width(w + 15));
        EditorGUI.indentLevel--;
        if (GUILayout.Button("Scroll With Speed", GUILayout.Width(130)))
        {
            scroll.ScrollToCell(index, speed, offset, scrollMode);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 60;
        EditorGUI.indentLevel++;
        time = EditorGUILayout.FloatField("Time", time, GUILayout.Width(w + 15));
        EditorGUI.indentLevel--;
        if (GUILayout.Button("Scroll Within Time", GUILayout.Width(130)))
        {
            scroll.ScrollToCellWithinTime(index, time, offset, scrollMode);
        }
        EditorGUILayout.EndHorizontal();
    }
}