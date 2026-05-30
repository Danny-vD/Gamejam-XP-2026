using System;
using FMODUnity;
using UnityEditor;
using UnityEngine;
using VDPackages.FMODUtilityPackage.Core;
using VDPackages.FMODUtilityPackage.Enums;
using static VDPackagesEditor.UtilityPackage.EditorUtils;

namespace VDPackagesEditor.FMODUtilityPackage.CustomEditors
{
	[CustomEditor(typeof(AudioManager))]
	public class AudioManagerEditor : Editor
	{
		// AudioManager
		private AudioManager audioManager;
		private bool showBusVolume;
		private bool[] busVolumeFoldout;

		// EventPaths
		private bool showBuses;
		private bool[] busesFoldout;

		private bool showEmitterEvents;
		private bool[] emitterEventsFoldout;

		// Fmod
		private static Type eventBrowser;

		// Icons
		private static Texture fmodIcon;

		private static Texture folderIconClosed;
		private static Texture folderIconOpen;

		private static Texture[] eventIcon;
		private static Texture[] busIcon;

		//////////////////////////////////////////////////

		// AudioManager
		private SerializedProperty initialVolumes;

		// EventPaths
		private SerializedProperty buses;
		private SerializedProperty emitterEvents;

		private void OnEnable()
		{
			// AudioManager
			audioManager = (AudioManager)target;
			
			initialVolumes   = serializedObject.FindProperty("initialVolumes");
			busVolumeFoldout = new bool[initialVolumes.arraySize];

			// FMODPathResolver
			buses         = serializedObject.FindProperty("FMODPathResolver.buses");
			emitterEvents = serializedObject.FindProperty("FMODPathResolver.emitterEvents");
			
			busesFoldout         = new bool[buses.arraySize];
			emitterEventsFoldout = new bool[emitterEvents.arraySize];

			// Icons
			fmodIcon = EditorUtils.LoadImage("StudioIcon.png");

			folderIconClosed = EditorUtils.LoadImage("FolderIconClosed.png");
			folderIconOpen   = EditorUtils.LoadImage("FolderIconOpen.png");

			eventIcon = new[]
			{
				EditorUtils.LoadImage("EventIcon.png"),
			};

			busIcon = new[]
			{
				GetTexture("FMODUtilityPackage/BusIcon.png"),
			};
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			DrawBusPaths();

			DrawSeperatorLine(); //-------------------

			DrawEmitterEvents();

			DrawSeperatorLine(); //-------------------

			DrawInitialVolumes();

			DrawSeperatorLine(); //-------------------

			DrawEventBrowserButton();

			serializedObject.ApplyModifiedProperties();
		}

		private void DrawBusPaths()
		{
			if (IsFoldOut(ref showBuses, showBuses ? folderIconOpen : folderIconClosed, "Bus Paths"))
			{
				DrawFoldoutKeyValueArray<AudioBus>(buses, "key", "value", busesFoldout, busIcon, DrawElement);
			}

			void DrawElement(int index, SerializedProperty key, SerializedProperty value)
			{
				string path = value.stringValue;

				if (index == 0)
				{
					EditorGUILayout.LabelField("Path", path);
					return;
				}

				if (!path.StartsWith("Bus:/") && !path.StartsWith("bus:/"))
				{
					if (value.stringValue == string.Empty)
					{
						value.stringValue = $"bus:/{key.enumNames[key.enumValueIndex]}";
					}
					else
					{
						value.stringValue = path.Insert(0, "bus:/");
					}
				}

				EditorGUILayout.PropertyField(value, new GUIContent("Path"));
			}
		}

		private void DrawEmitterEvents()
		{
			if (IsFoldOut(ref showEmitterEvents, showEmitterEvents ? folderIconOpen : folderIconClosed, "Emitters"))
			{
				DrawFoldoutKeyValueArray<GlobalEmitter>(emitterEvents, "key", "value", emitterEventsFoldout,
					new GUIContent("Event to play", eventIcon[0]));
			}
		}

		private void DrawInitialVolumes()
		{
			if (IsFoldOut(ref showBusVolume, showBusVolume ? folderIconOpen : folderIconClosed, "Volume"))
			{
				DrawFoldoutKeyValueArray<AudioBus>(initialVolumes, "key", busVolumeFoldout, busIcon, DrawStruct);
			}

			void DrawStruct(int i, SerializedProperty @struct)
			{
				SerializedProperty value = @struct.FindPropertyRelative("value");
				SerializedProperty isMuted = @struct.FindPropertyRelative("isMuted");

				float volume = value.floatValue;

				EditorGUILayout.PropertyField(isMuted, new GUIContent("Mute"));
				value.floatValue = EditorGUILayout.Slider("Volume", volume, 0.0f, 1.0f);
			}
		}

		private static void DrawEventBrowserButton()
		{
			if (GUILayout.Button(new GUIContent("Event Browser", fmodIcon)))
			{
				EventBrowser.ShowWindow();
			}
		}
	}
}