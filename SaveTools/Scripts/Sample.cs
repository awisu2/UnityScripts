using UnityEngine;
using System.Collections;

namespace SaveTools {
	class Sample {
		private int GetNewVersion() {
			int version = PlayerPrefs.GetInt ("version", -1);
			PlayerPrefs.SetInt ("version", ++version);
			return version;
		}
	}
}
