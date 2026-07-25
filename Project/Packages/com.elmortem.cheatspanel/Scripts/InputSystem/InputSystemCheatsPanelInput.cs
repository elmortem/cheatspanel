using UnityEngine;
using UnityEngine.InputSystem;

namespace CheatsPanels
{
	/// <summary>
	/// Провайдер ввода на основе нового Input System.
	/// Сборка компилируется только когда установлен пакет com.unity.inputsystem
	/// и Active Input Handling включает новую систему ввода.
	/// </summary>
	internal sealed class InputSystemCheatsPanelInput : ICheatsPanelInput
	{
		/// <inheritdoc />
		public bool IsToggleKeyDown
		{
			get
			{
				var keyboard = Keyboard.current;
				return keyboard != null && keyboard.backquoteKey.wasPressedThisFrame;
			}
		}

		/// <inheritdoc />
		public int TouchCount
		{
			get
			{
				var touchscreen = Touchscreen.current;
				if (touchscreen == null)
				{
					return 0;
				}

				var count = 0;
				var touches = touchscreen.touches;
				for (var i = 0; i < touches.Count; i++)
				{
					if (touches[i].press.isPressed)
					{
						count++;
					}
				}

				return count;
			}
		}

		/// <summary>
		/// Регистрирует провайдер до загрузки первой сцены, перекрывая старый Input Manager
		/// в режиме Active Input Handling = Both.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Install()
		{
			CheatsPanelInput.SetAutoProvider(new InputSystemCheatsPanelInput());
		}
	}
}
