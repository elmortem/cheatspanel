using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public static class Task_20260725_153000
{
	public static Task<string> Run()
	{
		var sb = new StringBuilder();

		sb.AppendLine("scriptCompilationFailed: " + EditorUtility.scriptCompilationFailed);

#if ENABLE_LEGACY_INPUT_MANAGER
		sb.AppendLine("ENABLE_LEGACY_INPUT_MANAGER: yes");
#else
		sb.AppendLine("ENABLE_LEGACY_INPUT_MANAGER: no");
#endif
#if ENABLE_INPUT_SYSTEM
		sb.AppendLine("ENABLE_INPUT_SYSTEM: yes");
#else
		sb.AppendLine("ENABLE_INPUT_SYSTEM: no");
#endif

		var player = CompilationPipeline.GetAssemblies(AssembliesType.PlayerWithoutTestAssemblies);
		foreach (var assembly in player.Where(a => a.name.IndexOf("Cheat", System.StringComparison.OrdinalIgnoreCase) >= 0))
		{
			sb.AppendLine("Assembly " + assembly.name + ": sourceFiles=" + assembly.sourceFiles.Length +
				" outputPath=" + assembly.outputPath);
		}

		var panelType = System.AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(a => a.GetTypes())
			.Where(t => t.Namespace == "CheatsPanels")
			.OrderBy(t => t.FullName)
			.Select(t => t.FullName + " [" + t.Assembly.GetName().Name + "]");
		sb.AppendLine("Loaded types in CheatsPanels:");
		foreach (var type in panelType)
		{
			sb.AppendLine("  " + type);
		}

		Debug.Log(sb.ToString());
		return Task.FromResult(sb.ToString());
	}
}
