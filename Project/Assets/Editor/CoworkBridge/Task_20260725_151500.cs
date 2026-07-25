using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

public static class Task_20260725_151500
{
	public static Task<string> Run()
	{
		var sb = new StringBuilder();

		var group = EditorUserBuildSettings.selectedBuildTargetGroup;
		sb.AppendLine("BuildTargetGroup: " + group);
		sb.AppendLine("Defines: '" + PlayerSettings.GetScriptingDefineSymbolsForGroup(group) + "'");

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
		sb.AppendLine("Player assemblies total: " + player.Length);
		foreach (var assembly in player.Where(a => a.name.IndexOf("Cheat", System.StringComparison.OrdinalIgnoreCase) >= 0))
		{
			sb.AppendLine("  " + assembly.name + " sourceFiles=" + assembly.sourceFiles.Length);
			foreach (var file in assembly.sourceFiles)
			{
				sb.AppendLine("    " + file);
			}
			var inputDefines = assembly.defines
				.Where(d => d.Contains("INPUT") || d.Contains("CHEATS"))
				.OrderBy(d => d);
			sb.AppendLine("    defines: " + string.Join(", ", inputDefines));
		}

		var editor = CompilationPipeline.GetAssemblies(AssembliesType.Editor);
		sb.AppendLine("Editor assemblies with 'Cheat': " +
			string.Join(", ", editor.Where(a => a.name.IndexOf("Cheat", System.StringComparison.OrdinalIgnoreCase) >= 0).Select(a => a.name)));

		var inputSystemPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName("CheatPanel.InputSystem");
		sb.AppendLine("CheatPanel.InputSystem asmdef path: " + (inputSystemPath ?? "<null>"));

		Debug.Log(sb.ToString());
		return Task.FromResult(sb.ToString());
	}
}
