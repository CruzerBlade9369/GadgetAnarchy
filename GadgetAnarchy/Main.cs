using System;
using System.Reflection;
using DV.Customization.Gadgets;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;

namespace GadgetAnarchy
{
	public static class Main
	{
		public static bool enabled;
		public static UnityModManager.ModEntry? mod;

		public static Settings settings { get; private set; }

		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Harmony? harmony = null;

			try
			{
				try
				{
					settings = Settings.Load<Settings>(modEntry);
				}
				catch
				{
					Debug.LogWarning("Unabled to load mod settings. Using defaults instead.");
					settings = new Settings();
				}

				mod = modEntry;

				harmony = new Harmony(modEntry.Info.Id);
				harmony.PatchAll(Assembly.GetExecutingAssembly());
				DebugLog("Attempting patch.");

				modEntry.OnGUI = OnGui;
				modEntry.OnSaveGUI = OnSaveGui;
			}
			catch (Exception ex)
			{
				modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
				harmony?.UnpatchAll(modEntry.Info.Id);
				return false;
			}

			return true;
		}

		static void OnGui(UnityModManager.ModEntry modEntry)
		{
			settings.Draw(modEntry);
		}

		static void OnSaveGui(UnityModManager.ModEntry modEntry)
		{
			settings.Save(modEntry);
		}

		public static void DebugLog(string message)
		{
			if (settings.isLoggingEnabled)
				mod?.Logger.Log(message);
		}
	}

	[HarmonyPatch(typeof(GadgetItem), nameof(GadgetItem.CanPlace))]
	static class GadgetItemCanPlacePatch
	{
		static bool Prefix(ref bool __result)
		{
			__result = true;
			return false;
		}
	}

	[HarmonyPatch(typeof(GadgetMount), nameof(GadgetMount.CheckIfCanChangeToState))]
	static class GadgetMountCheckIfCanChangeToStatePatch
	{
		static bool Prefix(ref bool __result)
		{
			__result = true;
			return false;
		}
	}

	[HarmonyPatch(typeof(MountPoint), nameof(MountPoint.IsOnGlass), MethodType.Getter)]
	static class MountPointIsOnGlassPatch
	{
		static bool Prefix(ref bool __result)
		{
			__result = false;
			return false;
		}
	}
}
