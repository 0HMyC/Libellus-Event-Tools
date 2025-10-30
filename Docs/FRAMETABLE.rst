=========
FrameTable Documentation
=========

The FrameTable is a table containing a list of "Targets" (aka functions) to be executed during the frames of an event. This file documents and lists relevant info corresponding to the Targets and any info specific to the table itself.

*Please note that this file is currently very much a work in progress, and as such is very incomplete and will be missing information.*

.. contents::

----

FrameTable Targets List
=======================

Target Base
-------------
The exact values stored for targets tends to vary with each specific PMD version, however the following values have been determined to be core and unchanged across all known PMD versions.

+----------------+------------+------------------------------------------------------------------------------+
| Name           | Data Type  | Description                                                                  |
+================+============+==============================================================================+
| TargetType     | UShort     | ID of FrameTable Target function to call.                                    |
+----------------+------------+------------------------------------------------------------------------------+
| StartFrame     | UShort     | Frame to execute Target on.                                                  |
+----------------+------------+------------------------------------------------------------------------------+
| Length         | UShort     | Number of frames to execute for. Effect varies with certain Target IDs.      |
+----------------+------------+------------------------------------------------------------------------------+
| NameIndex      | UShort     | Index of custom Target name or relevant file to load data from in NameTable. |
|                |            |                                                                              |
|                |            | TODO: Some Targets may override this value with their own. Should explain.   |
+----------------+------------+------------------------------------------------------------------------------+

Differing PMD versions tend to store extra values in addition to the ones above. These extensions to the base are listed directly below per-version.

V9 (DDS1&2) Extensions
^^^^^^^^^^^^^^^^^^^^^^
+----------------+------------+------------------------------------------------------------------------------+
| Name           | Data Type  | Description                                                                  |
+================+============+==============================================================================+
| Field08        | UInt       | Unknown.                                                                     |
+----------------+------------+------------------------------------------------------------------------------+

TODO: Figure out what Field08 does and document it.

V10 & V11 (Unused only), V12 (P3/P4) Extensions
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
+----------------+--------------+------------------------------------------------------------------------------+
| Name           | Data Type    | Description                                                                  |
+================+==============+==============================================================================+
| ResourceID     | Byte         | Identifies which loaded resource target should apply to.                     |
+----------------+--------------+------------------------------------------------------------------------------+
| ResourceType   | Byte         | Identifies what type of resource target should apply to (IE: A loaded model.)|
|                |              |                                                                              |
|                |              | TODO: More examples of uses + data values                                    |
+----------------+--------------+------------------------------------------------------------------------------+
| Field0A        | Short        | Unknown.                                                                     |
+----------------+--------------+------------------------------------------------------------------------------+
| Flags          | PmdFlags     | Used when a target's execution is conditional.                               |
|                |              | (ie. based upon a dialogue option or a bitflag)                              |
|                |              |                                                                              |
| FlagNumber     | UShort       | Which flag to check                                                          |
|                |              |                                                                              |
| CompareValue   | UShort       | Value that flag or stored SEL option should be equal to.                     |
|                |              |                                                                              |
| FlagType       | Enum (UShort)| Determines what type of flags are used or if comparision should happen at all|
|                |              |                                                                              |
|                |              | Values:                                                                      |
|                |              |                                                                              |
|                |              | 0. DISABLE                                                                   |
|                |              | 1. LOCAL                                                                     |
|                |              | 2. GLOABAL                                                                   |
|                |              |                                                                              |
| GlobalFlagType | Enum (UShort)| Determines which offset to use when comparing against "GLOBAL" bitflags.     |
|                |              |                                                                              |
|                |              | Values:                                                                      |
|                |              |                                                                              |
|                |              | 0. EVT                                                                       |
|                |              | 1. COMMU                                                                     |
|                |              | 2. SYS                                                                       |
|                |              | 3. EVT2 (P4(G? TODO: Figure this out) Only)                                  |
+----------------+--------------+------------------------------------------------------------------------------+
TODO: Clean up and better document what does what (ie what does Field0A do, how can these be used etc.,) show "structs" like Flags better (ideally seperate lines but still clearly part of the same "object")

----

QUAKE (7)
---------
Shakes the screen for the amount of frames as specified in the base "Length" value. Used to visually simulate earthquakes as the name implies.

+----------------+------------+-----------------------------------------------------------------------+
| Name           | Data Type  | Description                                                           |
+================+============+=======================================================================+
| Range          | Short      | Intensity of effect.                                                  |
+----------------+------------+-----------------------------------------------------------------------+

----

PADACT (28)
-----------
Sets controller rumble properties, such as strength, duration and intervals.

V9 (DDS1&2)
^^^^^^^^^^^
TODO.

V10-12 (P3/P4)
^^^^^^^^^^^^^^
+----------------+------------+-----------------------------------------------------------------------+
| Name           | Data Type  | Description                                                           |
+================+============+=======================================================================+
| PadactMode     | Enum (Byte)| Determines whether to activate or halt controller rumble.             |
|                |            |                                                                       |
|                |            | Values:                                                               |
|                |            |                                                                       |
|                |            | 0. START                                                              |
|                |            | 1. STOP                                                               |
+----------------+------------+-----------------------------------------------------------------------+
| Field15        | Byte       | Unknown. May determine which PADACT entry ID to list under in editor. |
+----------------+------------+-----------------------------------------------------------------------+
| Field16        | UShort     | Unknown.                                                              |
+----------------+------------+-----------------------------------------------------------------------+
| RumbleDuration | Short      | Total number of frames to rumble for. (30 = rumble for 1 sec span)    |
+----------------+------------+-----------------------------------------------------------------------+
| RumbleStrength | Short      | Intensity of rumble. Range of 0-255 (eff. 0% to 100%) in editor.      |
+----------------+------------+-----------------------------------------------------------------------+
| RumbleOnFrames | Short      | Number of frames to actively rumble for. (15 = rumble for 0.5 seconds)|
|                |            |                                                                       |
|                |            | Used in conjunction with RumbleOffFrames to allow intermittent rumble.|
|                |            |                                                                       |
|                |            | (If set to 15, and RumbleOffFrames is set to 15, rumble will occur for|
|                |            | 15 frames, then will stop for 15 frames and repeat.)                  |
+----------------+------------+-----------------------------------------------------------------------+
| RumbleOffFrames| Short      | Number of frames to wait before rumble. (15 = wait for 0.5 seconds)   |
+----------------+------------+-----------------------------------------------------------------------+

Note that while PADACT does appear and is used in Persona 4 Golden's PMD files, the actual function appears to have been removed/stubbed so that it does nothing (likely a result of that version first being made for the PS Vita, which lacked rumble support.)

This means that PADACT only functions on the PS2 versions of Persona 3 and Persona 4 respectively.
