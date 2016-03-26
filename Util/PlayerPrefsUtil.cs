/// <summary>
/// ただのサンプルプロジェクト特に何をするわけでもなく
/// </summary>
using UnityEngine;

namespace org.a2dev.UnityScripts.Util
{
    public class PlayerPresUtil {
		private int GetNewVersion() {
			int version = PlayerPrefs.GetInt ("version", -1);
			PlayerPrefs.SetInt ("version", ++version);
			return version;
		}
	}
}
