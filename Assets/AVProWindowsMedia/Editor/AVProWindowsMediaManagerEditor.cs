// Support for Editor.RequiresConstantRepaint()
#if UNITY_4_5 || UNITY_4_6 || UNITY_5_0
	#define AVPROWINDOWSMEDIA_UNITYFEATURE_EDITORAUTOREFRESH
#endif
// Supports Unity 4.x features
#if UNITY_5_0 || UNITY_4_6 || UNITY_4_5 || UNITY_4_4 || UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0
	#define AVPRO_UNITY_4_X
#endif
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
// Copyright 2012-2015 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

[CustomEditor(typeof(AVProWindowsMediaManager))]
public class AVProWindowsMediaManagerEditor : Editor
{
	private AVProWindowsMediaManager _manager;
	private AVProWindowsMediaMovie[] _movies;

	private void UpdateMovies()
	{
		_movies = (AVProWindowsMediaMovie[])FindObjectsOfType(typeof(AVProWindowsMediaMovie));
	}

#if AVPROWINDOWSMEDIA_UNITYFEATURE_EDITORAUTOREFRESH
	public override bool RequiresConstantRepaint ()
	{
		return (_movies != null);
	}
#endif

	public override void OnInspectorGUI()
	{
		_manager = (this.target) as AVProWindowsMediaManager;

		if (!Application.isPlaying)
		{
			this.DrawDefaultInspector();
		}

		/*if (!Application.isPlaying) 
		{
			_manager._useExternalTextures = GUILayout.Toggle(_manager._useExternalTextures, "Use External Textures (beta)");
		}*/

		if (GUILayout.Button ("Update"))
		{
			UpdateMovies();
		}

		if (_movies != null && _movies.Length > 0)
		{
			for (int i = 0; i < _movies.Length; i++)
			{
				GUILayout.BeginHorizontal();

				GUI.color = Color.white;
				if (!_movies[i].enabled ||
#if !AVPRO_UNITY_4_X
				    !_movies[i].gameObject.activeSelf
#else
				    !_movies[i].gameObject.activeInHierarchy
#endif
				    )
					GUI.color = Color.grey;

				AVProWindowsMedia media = _movies[i].MovieInstance;
				if (media != null)
				{
					GUI.color = Color.yellow;
					if (media.IsPlaying)
						GUI.color = Color.green;
				}

				if (GUILayout.Button("S"))
				{
					Selection.activeObject = _movies[i];
				}
				GUILayout.Label(i.ToString("D2") + " " + _movies[i].name, GUILayout.MinWidth(128f));
				//GUILayout.FlexibleSpace();
				if (media != null)
				{
					GUILayout.Label(media.Width + "x" + media.Height);
					GUILayout.FlexibleSpace();
					GUILayout.Label(string.Format("{0:00.0}", media.DisplayFPS) + " FPS");
					//GUILayout.FlexibleSpace();
				}
				else
				{
					GUILayout.FlexibleSpace();
				}



				GUILayout.EndHorizontal();

				if (media != null)
				{
					GUILayout.HorizontalSlider(media.PositionSeconds, 0f, media.DurationSeconds, GUILayout.MinWidth(128f), GUILayout.ExpandWidth(true));
				}
			}
		}
		else
		{
			if (Event.current.type.Equals(EventType.Repaint))
			{
				UpdateMovies();
			}
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(_manager);
		}		
	}
}