# Libellus Event Tools
A set of tools meant to help with the conversion and editing of the PM1/PM2/PM3 file formats found in various games created by Atlus, ranging from SMT Nocturne to Persona 4 Golden.

These files are referred to as PMD (which is short for Polygon Movie Data or PolyMovieData) and are all structured practically identical to one another, usually mainly differing in the identifier saved in their header section. However, different versions of the format (of which is also stored within the header section) do structure some of the tables and contents within somewhat differently to other versions.

## Current Features
- Conversion of PMD to and from JSON
- Extraction and packing of embedded files (such as .BMD/.MSG, .RMD/.AMD, .EPL, etc.)

## Supported PMD Versions
- 3
  - Primarily used by PM1 files
  - Used in Digital Devil Saga 1&2, Persona 3/FES, Persona 4/Golden
- 4 (Partial)
  - Used in Shin Megami Tensei 3: Nocturne
- 9 (Partial)
  - Used in Digital Devil Saga 1&2
- 10 (Partial, P3 Unused only)
  - Used in Devil Summoner: Raidou Kuzunoha vs. the Soulless Army
  - Unused PMD's identifying as this version can be found in Persona 3's files, but are actually v12 format
- 11 (Partial, P3 Unused only)
  - Used in Devil Summoner: Raidou Kuzunoha vs. the Soulless Army
  - Unused PMD's identifying as this version can be found in Persona 3's files, but are actually v12 format
- 12
  - Used in Persona 3/FES, Persona 4/Golden
