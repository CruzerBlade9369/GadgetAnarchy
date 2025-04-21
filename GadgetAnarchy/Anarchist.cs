using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

using DV.Customization.Gadgets;
using DV.Items;

using UnityEngine;

namespace GadgetAnarchy
{
	class Anarchist
	{
		[HarmonyPatch(typeof(GadgetItem), nameof(GadgetItem.CanPlace))]
		static class GadgetItemCanPlacePatch
		{
			static bool Prefix(ref bool __result)
			{
				if (!Main.settings.spaceAnarchy)
				{
					return true;
				}

				__result = true;
				return false;
			}
		}

		[HarmonyPatch(typeof(FlatSurfaceAttachmentOption), nameof(FlatSurfaceAttachmentOption.GetPossibleAttachmentPositions))]
		static class FlatSurfaceAttachmentOptionSizesPatch
		{
			static void Prefix(FlatSurfaceAttachmentOption __instance)
			{
				if (!Main.settings.mountAnarchy)
				{
					return;
				}

				var attachmentAreaMaxField = typeof(FlatSurfaceAttachmentOption).GetField("attachmentAreaMax",
					BindingFlags.NonPublic | BindingFlags.Instance);
				var attachmentAreaMinField = typeof(FlatSurfaceAttachmentOption).GetField("attachmentAreaMin",
					BindingFlags.NonPublic | BindingFlags.Instance);

				if (attachmentAreaMaxField == null || attachmentAreaMinField == null)
				{
					return;
				}

				attachmentAreaMaxField.SetValue(__instance, new Vector2(10.0f, 10.0f));
				attachmentAreaMinField.SetValue(__instance, new Vector2(0.01f, 0.01f));
			}
		}

		[HarmonyPatch(typeof(ItemWorkingAnimation), nameof(ItemWorkingAnimation.StartAnimating))]
		static class ItemWorkingAnimationMinWorkTimePatch
		{
			static void Postfix(ItemWorkingAnimation __instance)
			{
				if (__instance.gameObject.name != "HandDrill")
				{
					return;
				}

				if (!Main.settings.instantDrill)
				{
					if (__instance.minWorkTime != 0.1f)
					{
						return;
					}

					__instance.minWorkTime = 4f;
					return;
				}

				if (__instance.minWorkTime != 0.1f)
				{
					__instance.minWorkTime = 0.1f;
				}
			}
		}

		[HarmonyPatch(typeof(Drillable), nameof(Drillable.CheckIfCanChangeToState))]
		static class GadgetMountCheckIfCanChangeToStatePatch
		{
			static bool Prefix(int index, MountPoint.States desiredState, ref bool __result, Drillable __instance)
			{
				if (!Main.settings.drillAnarchy)
				{
					return true;
				}

				var mountPointsField = typeof(Drillable).GetField("mountPoints", BindingFlags.NonPublic | BindingFlags.Instance);
				if (mountPointsField == null)
				{
					return true;
				}

				var mountPoints = mountPointsField.GetValue(__instance) as List<MountPoint>;
				if (mountPoints == null)
				{
					return true;
				}

				if (index < 0 || index >= mountPoints.Count)
				{
					return true;
				}

				var stateProperty = typeof(MountPoint).GetProperty("State",
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				if (stateProperty == null)
				{
					return true;
				}

				var mountPointState = (MountPoint.States)stateProperty.GetValue(mountPoints[index]);
				if (mountPointState == MountPoint.States.Mounted)
				{
					return true;
				}

				__result = true;
				return false;
			}
		}
	}
}
