﻿using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace ControllerSettingsHelper.HarmonyPatches {
	[HarmonyPatch]
	static class CustomOffset {
		static IEnumerable<MethodBase> TargetMethods() {
			yield return AccessTools.Method(typeof(OculusVRHelper), nameof(OculusVRHelper.AdjustControllerTransform));
			yield return AccessTools.Method(typeof(OpenVRHelper), nameof(OpenVRHelper.AdjustControllerTransform));
		}

		[HarmonyPriority(int.MinValue)]
		static bool Prefix(XRNode node, Transform transform, Vector3 position, ref Vector3 rotation) {
			if(Config.Instance.MirrorZForLeft && node == XRNode.LeftHand)
				rotation.z = -rotation.z;

			if(!Config.Instance.OverrideControllerOffset)
				return true;

			if(node != XRNode.LeftHand && node != XRNode.RightHand)
				return false;

			transform.Rotate(new Vector3(0, 0, rotation.z));
			transform.Translate(position);
			transform.Rotate(new Vector3(rotation.x, rotation.y, 0));
			return false;
		}
	}
}
