#if ENABLE_LEGACY_INPUT_MANAGER

using UnityEngine;

namespace CheatsPanels
{
	/// <summary>
	/// Провайдер ввода на основе старого Input Manager (UnityEngine.Input).
	/// Компилируется только когда Active Input Handling включает старую систему ввода.
	/// </summary>
	internal sealed class LegacyCheatsPanelInput : ICheatsPanelInput
	{
		/// <inheritdoc />
		public bool IsToggleKeyDown => Input.GetKeyDown(KeyCode.BackQuote);

		/// <inheritdoc />
		public int TouchCount => Input.touchCount;
	}
}

#endif
