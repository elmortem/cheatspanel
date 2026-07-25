namespace CheatsPanels
{
	/// <summary>
	/// Абстракция ввода чит-панели.
	/// Позволяет панели работать и со старым Input Manager, и с новым Input System,
	/// а также подключить собственную реализацию ввода проекта.
	/// </summary>
	public interface ICheatsPanelInput
	{
		/// <summary>
		/// Нажата ли в текущем кадре клавиша переключения панели.
		/// </summary>
		bool IsToggleKeyDown { get; }

		/// <summary>
		/// Количество активных касаний экрана.
		/// </summary>
		int TouchCount { get; }
	}
}
