//#define CHEATS_ENABLE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace CheatsPanels
{
	/// <summary>
	/// Глобальная точка доступа к чит-панели.
	/// Позволяет добавлять и удалять команды, отображаемые в отладочном окне.
	/// </summary>
	public static class CheatsPanel
	{
		/// <summary>
		/// Имя дефайна, управляющего наличием чит-панели в рантайме.
		/// </summary>
		public const string DefineSymbol = "CHEATS_ENABLE";

#if CHEATS_ENABLE
		private const string BehaviourName = "CheatPanel";
		private const string DefaultGroupName = "Common";

		private static readonly Dictionary<string, List<CheatEntry>> _groups =
			new (StringComparer.Ordinal);

		private static readonly List<string> _groupOrder = new List<string>();

		private static CheatsPanelBehaviour _behaviour;
		private static bool _isVisible;

		/// <summary>
		/// Создаёт или находит существующий инстанс панели во время загрузки.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			EnsureBehaviourExists();
		}
#endif

		/// <summary>
		/// Регистрирует новый чит.
		/// </summary>
		/// <param name="name">Отображаемое имя чита.</param>
		/// <param name="action">Коллбек, вызываемый при нажатии на кнопку чита.</param>
		/// <param name="group">Название группы, в которой будет отображаться чит. Если не указано, используется группа "Common".</param>
		[Conditional(DefineSymbol)]
		public static void Bind(string name, Action action, string group = null)
		{
#if CHEATS_ENABLE
			if (group == null)
			{
				group = DefaultGroupName;
			}

			if (string.IsNullOrWhiteSpace(group))
			{
				throw new ArgumentException("Group name must be a non-empty string.", nameof(group));
			}

			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Cheat name must be a non-empty string.", nameof(name));
			}

			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			EnsureBehaviourExists();

			if (!_groups.TryGetValue(group, out var cheats))
			{
				cheats = new List<CheatEntry>();
				_groups.Add(group, cheats);
				_groupOrder.Add(group);
			}

			for (var i = 0; i < cheats.Count; i++)
			{
				if (string.Equals(cheats[i].Name, name, StringComparison.Ordinal))
				{
					cheats[i] = new CheatEntry(name, action);
					return;
				}
			}

			cheats.Add(new CheatEntry(name, action));
#endif
		}

		/// <summary>
		/// Удаляет чит по имени и группе.
		/// </summary>
		/// <param name="name">Имя чита.</param>
		/// <param name="group">Название группы. Если не указано, используется группа "Common".</param>
		[Conditional(DefineSymbol)]
		public static void Unbind(string name, string group = null)
		{
#if CHEATS_ENABLE
			if (group == null)
			{
				group = DefaultGroupName;
			}

			if (string.IsNullOrWhiteSpace(group) || string.IsNullOrWhiteSpace(name))
			{
				return;
			}

			if (!_groups.TryGetValue(group, out var cheats))
			{
				return;
			}

			var removed = false;
			for (var i = cheats.Count - 1; i >= 0; i--)
			{
				if (!string.Equals(cheats[i].Name, name, StringComparison.Ordinal))
				{
					continue;
				}

				cheats.RemoveAt(i);
				removed = true;
			}

			if (!removed)
			{
				return;
			}

			if (cheats.Count == 0)
			{
				_groups.Remove(group);
				_groupOrder.Remove(group);
			}
#endif
		}

		/// <summary>
		/// Удаляет чит по имени, группе и точному действию.
		/// </summary>
		/// <param name="name">Имя чита.</param>
		/// <param name="action">Действие, соответствующее читу.</param>
		/// <param name="group">Название группы. Если не указано, используется группа "Common".</param>
		[Conditional(DefineSymbol)]
		public static void Unbind(string name, Action action, string group = null)
		{
#if CHEATS_ENABLE
			if (action == null)
			{
				Unbind(name, group);
				return;
			}

			if (group == null)
			{
				group = DefaultGroupName;
			}

			if (string.IsNullOrWhiteSpace(group) || string.IsNullOrWhiteSpace(name))
			{
				return;
			}

			if (!_groups.TryGetValue(group, out var cheats))
			{
				return;
			}

			for (var i = cheats.Count - 1; i >= 0; i--)
			{
				if (!string.Equals(cheats[i].Name, name, StringComparison.Ordinal) || cheats[i].Action != action)
				{
					continue;
				}

				cheats.RemoveAt(i);
			}

			if (cheats.Count == 0)
			{
				_groups.Remove(group);
				_groupOrder.Remove(group);
			}
#endif
		}

		/// <summary>
		/// Отображает чит-панель.
		/// </summary>
		[Conditional(DefineSymbol)]
		public static void Show()
		{
#if CHEATS_ENABLE
			SetVisible(true);
#endif
		}

		/// <summary>
		/// Скрывает чит-панель.
		/// </summary>
		[Conditional(DefineSymbol)]
		public static void Hide()
		{
#if CHEATS_ENABLE
			SetVisible(false);
#endif
		}

		/// <summary>
		/// Переключает отображение чит-панели.
		/// </summary>
		[Conditional(DefineSymbol)]
		public static void Toggle()
		{
#if CHEATS_ENABLE
			SetVisible(!_isVisible);
#endif
		}

#if CHEATS_ENABLE
		internal static bool HasCheats => _groups.Count > 0;

		internal static bool IsVisible => _isVisible;

		internal static void CollectGroupNames(List<string> target)
		{
			target.Clear();
			for (var i = 0; i < _groupOrder.Count; i++)
			{
				var group = _groupOrder[i];
				if (_groups.TryGetValue(group, out var cheats) && cheats.Count > 0)
				{
					target.Add(group);
				}
			}
		}

		internal static void CollectCheats(string group, List<CheatEntry> target)
		{
			target.Clear();
			if (!_groups.TryGetValue(group, out var cheats) || cheats.Count == 0)
			{
				return;
			}

			for (var i = 0; i < cheats.Count; i++)
			{
				target.Add(cheats[i]);
			}
		}

		private static void EnsureBehaviourExists()
		{
			if (_behaviour != null)
			{
				return;
			}

			_behaviour = FindExistingBehaviour();

			if (_behaviour == null)
			{
				var go = new GameObject("CheatsPanel")
				{
					//hideFlags = HideFlags.HideInHierarchy
				};

				_behaviour = go.AddComponent<CheatsPanelBehaviour>();
			}

			_behaviour.SyncVisibility(_isVisible);
		}

		private static CheatsPanelBehaviour FindExistingBehaviour()
		{
#if UNITY_2023_1_OR_NEWER
			return UnityEngine.Object.FindFirstObjectByType<CheatsPanelBehaviour>();
#elif UNITY_2022_2_OR_NEWER
			return UnityEngine.Object.FindFirstObjectByType<CheatsPanelBehaviour>();
#else
			return UnityEngine.Object.FindObjectOfType<CheatsPanelBehaviour>();
#endif
		}

		private static void SetVisible(bool visible)
		{
			EnsureBehaviourExists();

			if (_isVisible == visible)
			{
				return;
			}

			_isVisible = visible;
			if (_behaviour != null)
			{
				_behaviour.SyncVisibility(visible);
			}
		}

		internal readonly struct CheatEntry
		{
			public readonly string Name;
			public readonly Action Action;

			public CheatEntry(string name, Action action)
			{
				Name = name;
				Action = action;
			}
		}
#endif
	}
}
