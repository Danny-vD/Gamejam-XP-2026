namespace XPGJ2026.Bootstrapping.Enums
{
	/// <summary>
	/// Determine when a bootstrapping script should execute
	/// </summary>
	public enum ExecutionEnvironment
	{
		/// <summary>
		/// The effect will only happen in the editor
		/// </summary>
		EditorOnly,
		
		/// <summary>
		/// The effect will only happen in a build
		/// </summary>
		BuildOnly,
		
		/// <summary>
		/// The effect will always happen
		/// </summary>
		EditorAndBuild,
	}
}