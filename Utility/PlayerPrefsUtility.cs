using UnityEngine;
using System.Collections;
using UnityEditor;

namespace A2Unity.Utility
{
	public class Sample {
		private int GetNewVersion() {
			int version = PlayerPrefs.GetInt ("version", -1);
			PlayerPrefs.SetInt ("version", ++version);
			return version;
		}
	}
}
