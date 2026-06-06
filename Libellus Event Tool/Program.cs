using LibellusLibrary.Event;

namespace LibellusEventTool
{
	class Program
	{
		/// <summary>
		/// Controls whether to convert all PMD or JSON files contained within a passed folder and it's subfolders.
		/// </summary>
		private static bool _recurse = false;

		private static void Warn(string message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write($"[WARN] ");
			Console.ResetColor();
			Console.WriteLine(message);
		}
		static async Task Main(string[] args)
		{
			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
			string version = fvi.FileVersion ?? "Null Version"; // Address CS8600 warning
			Console.WriteLine($"Welcome to LEET!\nLibellus Event Editing Tool: v{version}\nNow with better syntax!\n");

			_recurse = args.Contains("-r", StringComparer.OrdinalIgnoreCase);
			int numberPaths = _recurse ? args.Length - 1 : args.Length;
			if (numberPaths < 1)
			{
				Warn("Not enough args!\n");
				Console.WriteLine($"usage: \"{Path.GetFileNameWithoutExtension(assembly.Location)}\" [-r] <PATH>...");
				Console.WriteLine("options:");
				Console.WriteLine("  -r, \tControls whether to convert all PMD or JSON"); 
				Console.WriteLine("      \tfiles contained within a passed folder and it's subfolders.\n");
				Console.WriteLine("Press any button to exit.");
				Console.ReadKey();
				return;
			}
			await ConvertPaths(args);
			Console.WriteLine("Press Any Button To Exit.");
			Console.ReadKey();
		}

		private static async Task ConvertPaths(string[] paths)
		{
			foreach (string current_path in paths)
			{
				if (!File.Exists(current_path) && !Directory.Exists(current_path))
				{
					Warn($"'{current_path}' is not a valid file or directory!");
					continue;
				}
				
				string ext = Path.GetExtension(current_path).ToLower();
				if (ext == ".pm1" || ext == ".pm2" || ext == ".pm3")
				{
					Console.WriteLine($"Converting to Json: {current_path}");
					PmdReader reader = new();
					PolyMovieData pmd = await reader.ReadPmd(current_path);
					// the "!" in Path.GetDirectoryName(file)! indicates null forgiveness; should be safe & addresses CS8604
					string folder = Path.Combine(Path.GetDirectoryName(current_path)!, Path.GetFileNameWithoutExtension(current_path));
					try
					{
						await pmd.ExtractPmd(folder, Path.GetFileName(current_path));
					}
					catch (Exception ex)
					{
						Warn($"Cannot extract '{current_path}': {ex.Message}");
					}
				}
				else if (ext == ".json")
				{
					Console.WriteLine($"Converting to PMD: {current_path}");
					PolyMovieData pmd = new PolyMovieData();
					try
					{
						pmd = await PolyMovieData.LoadPmd(current_path);
					}
					catch (Exception ex)
					{
						Warn($"Cannot convert '{current_path}' to PMD: {ex.Message}");
						continue;
					}
					
					pmd.SavePmd($"{current_path}.PM{pmd.MagicCode[3]}");
				}
				else if (Directory.Exists(current_path))
				{
					await ConvertPaths(Directory.GetFiles(current_path, "*", _recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
				}
			}
			Console.Write("\n");
		}
	}
}