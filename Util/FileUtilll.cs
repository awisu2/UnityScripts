using UnityEngine;
using System.IO;

namespace org.a2dev.UnityScripts.Util
{
    public static class FileUtilll
    {
        // クラス名
        public static readonly string className = typeof(FileUtilll).Name;
        
		// ファイル削除
        public static void Delete(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

		// ディレクトリ削除
        public static void DeleteDirectory(string path, bool recursive = false)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive);
            }
        }

        // ディレクトリ作成
        public static void CreateDirectory(string path)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
        
        // 拡張子取得
        public static string GetExtension(string path)
        {
            FileInfo info = new FileInfo(path);
            if(info == null)
            {
                Debug.logger.LogWarning(className, "no　fileInfo. path : " + path);
                return "";
            }
            return info.Extension;
        }
    }
}
