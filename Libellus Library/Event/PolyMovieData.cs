﻿using System.Text.Json;
using System.Text.Json.Serialization;
using LibellusLibrary.Event.Types;

namespace LibellusLibrary.Event
{
	[JsonConverter(typeof(PmdJsonReader))]
	public class PolyMovieData
	{
		public string MagicCode { get; set; } = string.Empty;
		public uint Version { get; set; }

		public List<PmdDataType> PmdDataTypes { get; set; }

		//Returns TypeTable Count
		public uint ReadHeader(BinaryReader reader)
		{
			reader.BaseStream.Position = 8;
			MagicCode = new(reader.ReadChars(4));

			reader.BaseStream.Position = 0x10;
			uint TypeTableCount = reader.ReadUInt32();

			reader.BaseStream.Position = 0x14;
			Version = reader.ReadUInt32();

			return TypeTableCount;
		}

		/// <summary>
		/// Serialize PMD
		/// </summary>
		/// <param name="directoryToExtract"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public async Task ExtractPmd(string directoryToExtract, string filename)
		{
			JsonSerializerOptions options = new();
			options.WriteIndented = true;
			string json = JsonSerializer.Serialize<PolyMovieData>(this, options);
			json = JSON.Utilities.BeautifyJson(json);
			Directory.CreateDirectory(directoryToExtract);
			List<Task> tasks = new();
			tasks.Add(Task.Run(() => File.WriteAllTextAsync(Path.Combine(directoryToExtract, filename + ".json"), json)));
			foreach (PmdDataType dataType in PmdDataTypes)
			{
				if (dataType is IExternalFile fileType)
				{
					tasks.Add(fileType.SaveExternalFile(directoryToExtract));
				}
			}

			await Task.WhenAll(tasks);
		}

		public static async Task<PolyMovieData> LoadPmd(string path)
		{
			if (!File.Exists(path))
			{
				throw new ArgumentException($"Error while opening file.\nFile does not exist!\nFile: {path}");
			}
			using (MemoryStream stream = new(await File.ReadAllBytesAsync(path)))
			{
				PolyMovieData pmd = JsonSerializer.Deserialize<PolyMovieData>(stream)!;
				List<Task> tasks = new();
				// This the final step, the last battle!
				foreach (PmdDataType data in pmd.PmdDataTypes)
				{
					if (data is IExternalFile externalData)
					{
						tasks.Add(externalData.LoadExternalFile(Directory.GetParent(path)!.FullName));
					}
				}
				await Task.WhenAll(tasks);
				return pmd;
			}
		}

		public async void SavePmd(string path)
		{
			PmdBuilder builder = new(this);
			MemoryStream stream = await builder.CreatePmd();
			File.WriteAllBytes(path, stream.ToArray());
			stream.Close();
		}
	}
}
