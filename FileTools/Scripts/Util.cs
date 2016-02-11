using UnityEngine;
using System.Collections;

namespace FileTools {
	public static class Util {
		//Application.dataPath - asset directory
		//Application.persistentDataPath - 端末依存アプリ内保存ディレクトリ
		//Application.temporaryCachePath - 端末依存アプリ内保存ディレクトリ(テンポラリー扱い)

		// ヘッダタイプ
		public enum HeadType {
			HTTP,
			HTTPS,
			FILE,
		};

		// ヘッダ名を取得
		public static string GetHeadName(HeadType type) {
			string name = "";

			switch (type) {
			case HeadType.FILE:
				{
					name = "file://";
					break;
				}
			case HeadType.HTTP:
				{
					name = "http://";
					break;
				}
			case HeadType.HTTPS:
				{
					name = "https://";
					break;
				}
			}

			return name;
		}
	}
}
