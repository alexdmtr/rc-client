using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CardHand))]
public class CardHandEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CardHand CardHand = (CardHand)target;
        CardHand.Opacity = EditorGUILayout.FloatField("Opacity", CardHand.Opacity);

    }
}
