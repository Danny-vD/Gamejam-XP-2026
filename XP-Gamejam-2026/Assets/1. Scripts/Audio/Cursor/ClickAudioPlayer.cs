using AYellowpaper;
using UnityEngine;
using UnityEngine.InputSystem;
using VDFramework;
using XPGJ2026.Audio.Interfaces;

namespace XPGJ2026.Audio.Cursor
{
	public class ClickAudioPlayer : SafeEnableBehaviour
	{
		[SerializeField]
		private InputActionReference onMouseDown;
		
		[Space]
		[SerializeField]
		private InterfaceReference<IAudioEventInstancePlayer> mouseDownEventPlayer;
		
		[SerializeField]
		private InterfaceReference<IAudioEventInstancePlayer> mouseUpEventPlayer;

		protected override void OnEnabled()
		{
			if (mouseDownEventPlayer.Value is { IsNull: false })
			{
				onMouseDown.action.performed += OnMouseButtonDown;
			}

			if (mouseUpEventPlayer.Value is { IsNull: false })
			{
				onMouseDown.action.canceled += OnMouseButtonUp;
			}
		}

		private void OnMouseButtonDown(InputAction.CallbackContext callbackContext)
		{
			mouseDownEventPlayer.Value.Play();
		}
		
		private void OnMouseButtonUp(InputAction.CallbackContext callbackContext)
		{
			mouseUpEventPlayer.Value.Play();
		}

		private void OnDisable()
		{
			if (mouseDownEventPlayer.Value is { IsNull: false })
			{
				onMouseDown.action.performed -= OnMouseButtonDown;
			}

			if (mouseUpEventPlayer.Value is { IsNull: false })
			{
				onMouseDown.action.canceled -= OnMouseButtonUp;
			}
		}
	}
}