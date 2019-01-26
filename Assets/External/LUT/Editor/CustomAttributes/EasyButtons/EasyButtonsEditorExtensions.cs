﻿using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LUT
{
	public static class EasyButtonsEditorExtensions
	{
		public static void DrawEasyButtons(this Editor editor)
		{
			// Loop through all methods with no parameters
			var methods = editor.target.GetType()
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(m => m.GetParameters().Length == 0);
			foreach (var method in methods)
			{
				// Get the ButtonAttribute on the method (if any)
				var ba = (ButtonAttribute)Attribute.GetCustomAttribute(method, typeof(ButtonAttribute));

				if (ba != null)
				{
					// Determine whether the button should be enabled based on its mode
					GUI.enabled = ba.Mode == ButtonMode.AlwaysEnabled
						|| (EditorApplication.isPlaying ? ba.Mode == ButtonMode.EnabledInPlayMode : ba.Mode == ButtonMode.DisabledInPlayMode);

					// Draw a button which invokes the method
					var buttonName = String.IsNullOrEmpty(ba.Name) ? ObjectNames.NicifyVariableName(method.Name) : ba.Name;
					if (GUILayout.Button(buttonName))
					{
						foreach (var t in editor.targets)
						{
							method.Invoke(t, null);
						}
					}

					GUI.enabled = true;
				}
			}
		}
	}
}
