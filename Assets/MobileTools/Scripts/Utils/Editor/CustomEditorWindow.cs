using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BBG.MobileTools
{
	public abstract class CustomEditorWindow : EditorWindow
	{
		#region Member Variables

		private Texture2D	lineTexture;
		private Vector2		windowScrollPosition;

		#endregion

		#region Properties

		protected int IndentLevel { get { return EditorGUI.indentLevel; } set { EditorGUI.indentLevel = value; } }

		private Texture2D LineTexture
		{
			get
			{
				if (lineTexture == null)
				{
					lineTexture = new Texture2D(1, 1);
					lineTexture.SetPixel(0, 0, new Color(37f/255f, 37f/255f, 37f/255f));
					lineTexture.Apply();
				}

				return lineTexture;
			}
		}

		#endregion

		#region Abstract Methods

		public abstract void DoGUI();

		#endregion

		#region Unity Methods

		private void OnDisable()
		{
			DestroyImmediate(LineTexture);
		}

		#endregion

		#region Draw Methods

		private void OnGUI()
		{
			windowScrollPosition = EditorGUILayout.BeginScrollView(windowScrollPosition);

			EditorGUILayout.Space();

			DoGUI();

			EditorGUILayout.Space();

			EditorGUILayout.EndScrollView();
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Begins a new box, must call EndBox
		/// </summary>
		protected void BeginBox(string boxTitle = "")
		{
			GUIStyle style		= new GUIStyle("HelpBox");
			style.padding.left	= 0;
			style.padding.right	= 0;

			GUILayout.BeginVertical(style);

			if (!string.IsNullOrEmpty(boxTitle))
			{
				DrawBoldLabel(boxTitle);

				DrawLine();
			}
		}

		/// <summary>
		/// Begins a new foldout box, must call EndBox
		/// </summary>
		protected bool BeginFoldoutBox(string boxTitle)
		{
			GUIStyle style		= new GUIStyle("HelpBox");
			style.padding.left	= 0;
			style.padding.right	= 0;

			GUILayout.BeginVertical(style);

			if (!string.IsNullOrEmpty(boxTitle))
			{
				bool wasExpanded = IsBoxExpanded(boxTitle);

				bool isExpanded = DrawBoldFoldout(wasExpanded, boxTitle);

				if (isExpanded)
				{
					DrawLine();
				}

				if (wasExpanded != isExpanded)
				{
					if (isExpanded)
					{
						SetBoxExpanded(boxTitle);
					}
					else
					{
						SetBoxCollapsed(boxTitle);
					}
				}

				return isExpanded;
			}

			return true;
		}

		/// <summary>
		/// Ends the box.
		/// </summary>
		protected void EndBox()
		{
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Draws a bold label
		/// </summary>
		protected void DrawLabel(string text)
		{
			EditorGUILayout.LabelField(text);
		}

		/// <summary>
		/// Draws a bold label
		/// </summary>
		protected void DrawBoldLabel(string text)
		{
			EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
		}

		/// <summary>
		/// Draws a bold label
		/// </summary>
		protected bool DrawFoldout(bool isExpanded, string text)
		{
			return EditorGUILayout.Foldout(isExpanded, text);
		}

		/// <summary>
		/// Draws a bold label
		/// </summary>
		protected bool DrawBoldFoldout(bool isExpanded, string text)
		{
			GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);

			foldoutStyle.fontStyle = FontStyle.Bold;

			return EditorGUILayout.Foldout(isExpanded, text, foldoutStyle);
		}

		/// <summary>
		/// Draws a simple 1 pixel height line
		/// </summary>
		protected void DrawLine()
		{
			GUIStyle lineStyle			= new GUIStyle();
			lineStyle.normal.background	= LineTexture;

			GUILayout.BeginVertical(lineStyle);
			GUILayout.Space(1);
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Checks if a box is expanded
		/// </summary>
		protected bool IsBoxExpanded(string key)
		{
			string[] editorExpandedBoxes = EditorPrefs.GetString("bbb-box-expanded").Split(';');

			for (int i = 0; i < editorExpandedBoxes.Length; i++)
			{
				if (editorExpandedBoxes[i] == key)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Sets the box as expanded
		/// </summary>
		protected void SetBoxExpanded(string key)
		{
			string boxExpandedStr = EditorPrefs.GetString("bbb-box-expanded");

			if (!string.IsNullOrEmpty(boxExpandedStr))
			{
				boxExpandedStr += ";";
			}

			boxExpandedStr += key;

			EditorPrefs.SetString("bbb-box-expanded", boxExpandedStr);
		}

		/// <summary>
		/// Sets the box as collapses
		/// </summary>
		protected void SetBoxCollapsed(string key)
		{
			string[] editorExpandedBoxes = EditorPrefs.GetString("bbb-box-expanded").Split(';');

			string boxExpandedStr = "";

			for (int i = 0; i < editorExpandedBoxes.Length; i++)
			{
				if (editorExpandedBoxes[i] == key)
				{
					continue;
				}

				if (!string.IsNullOrEmpty(boxExpandedStr))
				{
					boxExpandedStr += ";";
				}

				boxExpandedStr += editorExpandedBoxes[i];
			}

			EditorPrefs.SetString("bbb-box-expanded", boxExpandedStr);
		}

		protected void Space()
		{
			EditorGUILayout.Space();
		}

		protected void Space(float pixels)
		{
			GUILayout.Space(pixels);
		}

		protected void DrawPropertyField(SerializedProperty serializedProperty, string field, bool includeChildren = false)
		{
			EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative(field), includeChildren);
		}

		protected bool DrawButton(string btnText)
		{
			return GUILayout.Button(btnText);
		}

		#endregion
	}
}
