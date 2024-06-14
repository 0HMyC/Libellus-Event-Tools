﻿using LibellusLibrary.Event.Types;
using System.Collections.Concurrent;
using LibellusLibrary.Utils.IO;

namespace LibellusLibrary.Event
{
	internal class PmdBuilder
	{
		internal PolyMovieData Pmd;

		public List<PmdTypeID> ExistingTypes = new();

		internal Dictionary<PmdTypeID, List<byte[]>> ReferenceTables = new();

		internal Dictionary<IReferenceType, int> nameTableReferenceIndices = new();

		internal ConcurrentDictionary<PmdTypeID, List<PmdDataType>> typeTable = new();

		internal PmdBuilder(PolyMovieData Pmd)
		{
			this.Pmd = Pmd;
		}

		// I sincerely apologize for the absolute hell that is the writing code
		// I dont know what the fuck I was smoking but I know if I touch it, the whole thing will explode
		// TODO: Fix CS1998?
		internal async Task<MemoryStream> CreatePmd(string path)
		{
			MemoryStream pmdFile = new();
			using var writer = new BinaryWriter(pmdFile);
			Dictionary<PmdDataType, long> dataTypes = new();
			// Type, offset
			foreach (PmdDataType pmdData in Pmd.PmdDataTypes)
			{
				if(pmdData is IReferenceType reference)
				{
					reference.SetReferences(this);
				}
			}
			writer.FSeek(0x20 + 0x10 * (Pmd.PmdDataTypes.Count + ReferenceTables.Count));
			foreach (var referenceType in ReferenceTables)
			{
				PmdData_RawData dataType = new();
				dataType.Type = referenceType.Key;
				dataType.Data = referenceType.Value;
				long start = writer.FTell();
				dataType.SaveData(this, writer);
				dataTypes.Add(dataType, start);
			}

			foreach (PmdDataType pmdData in Pmd.PmdDataTypes)
			{
				long start = writer.FTell();
				pmdData.SaveData(this, writer);
				dataTypes.Add(pmdData, start);
			}

			// Write Header
			writer.Seek(0, SeekOrigin.Begin);
			writer.Write(0); // Filetype/format/userid
			writer.Write((int)pmdFile.Length);
			writer.Write(Pmd.MagicCode.ToCharArray());
			writer.Write(0); // Expand Size
			writer.Write(dataTypes.Count);
			writer.Write(Pmd.Version);
			writer.Write(0); //Reserve
			writer.Write(0);

			// Create Type table
			writer.FSeek(0x20);
			// Write the type table in the correct order
			foreach (KeyValuePair<PmdDataType, long> dataType in dataTypes)
			{
				writer.Write((int)dataType.Key.Type);
				writer.Write(dataType.Key.GetSize()); // Size
				writer.Write(dataType.Key.GetCount());
				writer.Write((int)dataType.Value); // Offset
			}

			return pmdFile;
		}


		/// <summary>
		/// Creates another datatype and returns it's index
		/// </summary>
		/// <param name="id"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		internal int AddReference(PmdTypeID id, byte[] data)
		{
			if (ReferenceTables.ContainsKey(id))
			{
				ReferenceTables[id].Add(data);
				return ReferenceTables.Count - 1;
			}
			ReferenceTables.Add(id, new List<byte[]>());
			ReferenceTables[id].Add(data);
			return ReferenceTables.Count - 1;
		}

	}
}
