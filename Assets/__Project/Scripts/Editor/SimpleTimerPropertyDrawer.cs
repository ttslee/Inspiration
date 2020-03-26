using UnityEditor;
using UnityEngine;

namespace Engarde.CustomEditors {

	[CustomPropertyDrawer(typeof(SimpleTimer))]
	public class SimpleTimerPropertyDrawer : PropertyDrawer {

		static readonly float Pad = 3;

		public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {

			EditorGUI.BeginProperty(pos, label, prop);

			SerializedProperty duration = prop.FindPropertyRelative("duration");
			SerializedProperty current = prop.FindPropertyRelative("current");

			var progress = Mathf.Clamp01(current.floatValue / duration.floatValue);

			var r = EditorGUI.PrefixLabel(pos, label);

			var prevIndent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			var width = (r.width - Pad) / 3;

			r.width = 2 * width;
			EditorGUI.ProgressBar(r, progress, progress.ToString("0.00"));

			r.x += 2 * width + Pad;
			r.width = width;
			EditorGUI.PropertyField(r, duration, GUIContent.none);

			EditorGUI.indentLevel = prevIndent;

			EditorGUI.EndProperty();

		}

	}

}