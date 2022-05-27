// @(h)XObject.cs ver 0.00 ( '22.03.30 Nov-Lab ) 作成開始
// @(h)XObject.cs ver 0.21 ( '22.03.30 Nov-Lab ) ベータ版完成
// @(h)XObject.cs ver 0.21a( '22.05.24 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【Object 拡張メソッド】System.Object クラスに拡張メソッドを追加します。

using System;
using System.Collections.Generic;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【Object 拡張メソッド】System.Object クラスに拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XObject
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ToString(Null対応)】オブジェクトが null でない場合は ToString の結果を返し、null の場合は "(null)" を返します。
        /// </summary>
        /// <param name="obj">[in ]：オブジェクト</param>
        /// <returns>
        /// オブジェクトの文字列形式(オブジェクトが null の場合は "(null)")
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XNullSafeToString(object obj)
        {
            if (obj is null)
            {
                return "(null)";
            }
            else
            {
                return obj.ToString();
            }
        }

    }
}
