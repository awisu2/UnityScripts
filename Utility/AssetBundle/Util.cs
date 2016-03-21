using UnityEngine;
using System.Collections;

namespace A2Unity.Utility.AssetBundle {
	public class Util {
		// platformから名前を取得
		public static string GetPlatformName(RuntimePlatform platform) 
		{
			string name = "";

			switch (platform) {
			case RuntimePlatform.IPhonePlayer:
				{
					name = "iOS";
					break;
				}
			case RuntimePlatform.Android:
				{
					name = "Android";
					break;
				}
			case RuntimePlatform.OSXPlayer:
			default:
				{
					name = "OSX";
					break;
				}
			}
			return name;
		}
	}
}
