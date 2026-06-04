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
			string fileName = Path.GetFileNameWithoutExtension(assembly.Location);
			Console.WriteLine($"Welcome to LEET!\nLibellus Event Editing Tool: v{version}\nNow with better syntax!\n");

			_recurse = args.Contains("-r", StringComparer.OrdinalIgnoreCase);
			int numberPaths = _recurse ? args.Length - 1 : args.Length;
			if (numberPaths < 1)
			{
				Warn("Not enough args!\n");
				Console.WriteLine($"usage: \"{fileName}\" [-r] <PATH>...");
				Console.WriteLine("options:");
				Console.WriteLine("  -r,    Controls whether to convert all PMD or JSON\n" + 
				                  "         files contained within a passed folder and it's subfolders.\n");
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
			foreach (string path in paths)
			{
				if (!File.Exists(path) && !Directory.Exists(path))
				{
					Warn($"'{path}' is not a valid file or directory!");
					continue;
				}
				
				string ext = Path.GetExtension(path).ToLower();
				if (ext == ".pm1" || ext == ".pm2" || ext == ".pm3")
				{
					Console.WriteLine($"Converting to Json: {path}");
					PmdReader reader = new();
					PolyMovieData pmd = await reader.ReadPmd(path);
					// the "!" in Path.GetDirectoryName(file)! indicates null forgiveness; should be safe & addresses CS8604
					string folder = Path.Combine(Path.GetDirectoryName(path)!, Path.GetFileNameWithoutExtension(path));
					try
					{
						await pmd.ExtractPmd(folder, Path.GetFileName(path));
					}
					catch (Exception ex)
					{
						Warn($"Cannot extract '{path}': {ex.Message}");
					}
				}
				else if (ext == ".json")
				{
					Console.WriteLine($"Converting to PMD: {path}");
					PolyMovieData pmd = new PolyMovieData();
					try
					{
						pmd = await PolyMovieData.LoadPmd(path);
					}
					catch (Exception ex)
					{
						Warn($"Cannot convert '{path}' to PMD: {ex.Message}");
						continue;
					}
					
					pmd.SavePmd($"{path}.PM{pmd.MagicCode[3]}");
				}
				else if (Directory.Exists(path))
				{
					await ConvertPaths(Directory.GetFiles(path, "*", _recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
				}
			}
			Console.Write("\n");
		}
	}
}