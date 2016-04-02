using UnityEditor;

public static class OrientationUtil {
        // オリエンテーションの文字列リスト
        public static readonly string[] OrientationStrings = new string[] {
            "AutoRotation",
            "LandscapeLeft",
            "LandscapeRight",
            "Portrait",
            "PortraitUpsideDown",
        };
        
        public static readonly UIOrientation[] UIOrientations = new UIOrientation[] {
            UIOrientation.AutoRotation,
            UIOrientation.LandscapeLeft,
            UIOrientation.LandscapeRight,
            UIOrientation.Portrait,
            UIOrientation.PortraitUpsideDown,
        };

        /// <summary>
        /// indexに一致するオリエンテーションを取得
        /// </summary>
        /// <returns>一致したOrientation</returns>
        /// <param name="index">orientationのindex</param>
        public static UIOrientation GetUIOrientation(int index)
        {
            return UIOrientations[index];
        }
        
        /// <summary>
        /// オリエンテーションに対応するIndexを取得
        /// </summary>
        /// <returns>index</returns>
        /// <param name="orientation">UIOrientation</param>
        public static int GetUIOrientationIndex(UIOrientation orientation)
        {
            int index = -1;
            for(int i = 0; i < UIOrientations.Length; i++)
            {
                if(UIOrientations[i] == orientation)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
}
