#if CHEATS_ENABLE

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CheatsPanels
{
	/// <summary>
	/// Компонент, отвечающий за отображение окна и обработку ввода.
	/// </summary>
	internal sealed class CheatsPanelBehaviour : MonoBehaviour
	{
		private const int WindowId = 9999;
		private const float WindowWidth = 420f;
		private const float MinWindowHeight = 320f;
		private const float Margin = 24f;
		private const float TripleTapInterval = 0.4f;

		private readonly List<string> _groupNames = new ();
		private readonly List<CheatsPanel.CheatEntry> _cheatEntries = new ();

		private Rect _windowRect = new Rect(32f, 32f, WindowWidth, 520f);
		private Vector2 _scrollPosition;
		private GUIStyle _groupLabelStyle;
		private GUIStyle _buttonStyle;
		private GUIStyle _emptyLabelStyle;

		private bool _isVisible;
		private bool _toggleRequested;

		private void Update()
		{
			if (_toggleRequested)
			{
				_toggleRequested = false;
				CheatsPanel.Toggle();
			}

			var input = CheatsPanelInput.Current;
			if (input == null)
			{
				return;
			}

			if (input.IsToggleKeyDown)
			{
				CheatsPanel.Toggle();
			}

			DetectTripleTap(input);
		}

		private void DetectTripleTap(ICheatsPanelInput input)
		{
			if (!_isVisible && input.TouchCount == 3)
			{
				CheatsPanel.Toggle();
			}
		}

		private void OnGUI()
		{
			HandleFallbackToggle();

			if (!_isVisible)
			{
				return;
			}

			EnsureStyles();

			var availableWidth = Mathf.Max(160f, Screen.width - Margin * 2f);
			var availableHeight = Mathf.Max(MinWindowHeight, Screen.height - Margin * 2f);

			_windowRect.width = Mathf.Min(WindowWidth, availableWidth);
			_windowRect.height = Mathf.Clamp(_windowRect.height, MinWindowHeight, availableHeight);

			_windowRect.x = Mathf.Clamp(_windowRect.x, Margin, Screen.width - _windowRect.width - Margin);
			_windowRect.y = Mathf.Clamp(_windowRect.y, Margin, Screen.height - _windowRect.height - Margin);

			_windowRect = GUI.ModalWindow(WindowId, _windowRect, DrawWindowContents, "Cheats");
		}

		/// <summary>
		/// Резервное переключение панели по событию IMGUI, когда ни один провайдер ввода недоступен
		/// (например, выбран новый Input System, но пакет com.unity.inputsystem не установлен).
		/// Переключение откладывается до Update, чтобы не менять разметку окна между событиями Layout и Repaint.
		/// </summary>
		private void HandleFallbackToggle()
		{
			if (CheatsPanelInput.Current != null)
			{
				return;
			}

			var current = Event.current;
			if (current == null || current.type != EventType.KeyDown || current.keyCode != KeyCode.BackQuote)
			{
				return;
			}

			current.Use();
			_toggleRequested = true;
		}

		private void DrawWindowContents(int windowId)
		{
			CheatsPanel.CollectGroupNames(_groupNames);

			var scrollHeight = Mathf.Max(0f, _windowRect.height - 90f);
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(scrollHeight));

			if (_groupNames.Count == 0)
			{
				GUILayout.Label("Нет доступных читов", _emptyLabelStyle);
			}
			else
			{
				for (var i = 0; i < _groupNames.Count; i++)
				{
					DrawGroup(_groupNames[i]);
					GUILayout.Space(10f);
				}
			}

			GUILayout.EndScrollView();

			if (GUILayout.Button("Закрыть", _buttonStyle))
			{
				CheatsPanel.Hide();
			}

			GUI.DragWindow();
		}

		private void DrawGroup(string group)
		{
			GUILayout.Label(group, _groupLabelStyle);
			CheatsPanel.CollectCheats(group, _cheatEntries);

			for (var i = 0; i < _cheatEntries.Count; i++)
			{
				var entry = _cheatEntries[i];
				if (!GUILayout.Button(entry.Name, _buttonStyle))
				{
					continue;
				}

				try
				{
					entry.Action?.Invoke();
				}
				catch (Exception exception)
				{
					Debug.LogError($"Cheat '{entry.Name}' threw an exception: {exception}");
				}
			}
		}

		internal void SyncVisibility(bool visible)
		{
			_isVisible = visible;
			enabled = true;
			if (!_isVisible)
			{
				_scrollPosition = Vector2.zero;
			}
		}

		private void EnsureStyles()
		{
			if (_groupLabelStyle != null)
			{
				return;
			}

			_groupLabelStyle = new GUIStyle(GUI.skin.label)
			{
				fontStyle = FontStyle.Bold,
				fontSize = 16,
				alignment = TextAnchor.MiddleLeft
			};

			_buttonStyle = new GUIStyle(GUI.skin.button)
			{
				fontSize = 14,
				alignment = TextAnchor.MiddleCenter,
				padding = new RectOffset(10, 10, 6, 6)
			};

			_emptyLabelStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 14,
				alignment = TextAnchor.MiddleCenter,
				wordWrap = true
			};
		}
	}
}

#endif
