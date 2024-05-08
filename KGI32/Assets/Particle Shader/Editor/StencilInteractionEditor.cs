using System;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(StencilInteraction),true)]
public class StencilInteractionEditor : Editor
{
    private StencilInteraction editObject;
    private SerializedProperty interaction;
    private SerializedProperty independentMaterial;

    private void OnEnable()
    {
        editObject = this.serializedObject.targetObject as StencilInteraction;
        interaction = this.serializedObject.FindProperty("interaction");
        independentMaterial = this.serializedObject.FindProperty("UseIndependentMaterial");
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        EditorGUILayout.PropertyField(interaction);
        editObject.Interaction = (SpriteMaskInteraction)interaction.enumValueIndex;
        EditorGUILayout.PropertyField(independentMaterial);

        this.serializedObject.ApplyModifiedProperties();
    }
}
