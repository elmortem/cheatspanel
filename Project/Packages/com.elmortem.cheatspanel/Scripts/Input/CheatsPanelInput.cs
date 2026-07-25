namespace CheatsPanels
{
	/// <summary>
	/// Точка доступа к провайдеру ввода чит-панели.
	/// Провайдер выбирается автоматически по настройке Active Input Handling проекта:
	/// новый Input System (если пакет установлен и включён) имеет приоритет над старым Input Manager.
	/// Проект может подменить его своей реализацией через <see cref="SetProvider"/>.
	/// </summary>
	public static class CheatsPanelInput
	{
		private static ICheatsPanelInput _customProvider;
		private static ICheatsPanelInput _autoProvider;
		private static bool _autoResolved;

		/// <summary>
		/// Текущий провайдер ввода.
		/// Возвращает null, если ни одна система ввода недоступна — тогда панель открывается только из кода
		/// либо резервным способом через события IMGUI.
		/// </summary>
		public static ICheatsPanelInput Current => _customProvider ?? ResolveAutoProvider();

		/// <summary>
		/// Задаёт собственный провайдер ввода. Передайте null, чтобы вернуться к автоматическому выбору.
		/// </summary>
		/// <param name="provider">Реализация ввода проекта.</param>
		public static void SetProvider(ICheatsPanelInput provider)
		{
			_customProvider = provider;
		}

		/// <summary>
		/// Регистрирует автоматически выбранный провайдер.
		/// Вызывается сборкой поддержки нового Input System при инициализации рантайма.
		/// </summary>
		internal static void SetAutoProvider(ICheatsPanelInput provider)
		{
			_autoProvider = provider;
			_autoResolved = true;
		}

		private static ICheatsPanelInput ResolveAutoProvider()
		{
			if (_autoResolved)
			{
				return _autoProvider;
			}

			_autoResolved = true;

#if ENABLE_LEGACY_INPUT_MANAGER
			_autoProvider = new LegacyCheatsPanelInput();
#endif

			return _autoProvider;
		}
	}
}
