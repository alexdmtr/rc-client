using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CardFrontHand))]
public class CardFrontHandEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CardFrontHand CardFrontHand = (CardFrontHand)target;
        CardFrontHand.Opacity = EditorGUILayout.FloatField("Opacity", CardFrontHand.Opacity);
        
    }
}
