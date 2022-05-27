// @(h)XChar.cs ver 0.00 ( '22.05.08 Nov-Lab ) 作成開始
// @(h)XChar.cs ver 0.51 ( '22.05.10 Nov-Lab ) ベータ版完成
// @(h)XChar.cs ver 0.51a( '22.05.18 Nov-Lab ) その他  ：クラス名変更に対応した(ManualTestMethodAttribute)。機能変更なし。

// @(s)
// 　【char 拡張メソッド】System.Char クラスに拡張メソッドを追加します。

using System;
using System.Diagnostics;
using System.Collections.Generic;

using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【char 拡張メソッド】System.Char クラスに拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XChar
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字エスケープ】エスケープ対象文字をエスケープシーケンスに変換して返します。エスケープ対象外文字はそのまま返します。
        /// </summary>
        /// <param name="sourceChar">     [in ]：変換元文字</param>
        /// <param name="escapeConverter">[in ]：エスケープコンバーター[null = C#形式]</param>
        /// <returns>
        /// 変換結果
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・Char オブジェクト主体で EscapeConverter.Escape を呼び出すユーティリティーメソッドです。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XEscape(this char sourceChar, EscapeConverter escapeConverter = null)
        {
            //------------------------------------------------------------
            /// エスケープ対象文字をエスケープシーケンスに変換する
            //------------------------------------------------------------
            EscapeConverter.Normalize(ref escapeConverter);             //// エスケープコンバーターを正規化する(null の場合は CSharp に差し替える)
            return escapeConverter.Escape(sourceChar);                  //// エスケープコンバーターでエスケープした結果を戻り値とし、関数終了
        }

        //------------------------------------------------------------
        // テスト用メソッド(エスケープコンバーター = 指定なし)
        //------------------------------------------------------------
#if DEBUG
        [ManualTestMethod]
        public static void ZZZ_XEscape_CSharp()
        {
            for (int charCode = 0; charCode <= 255; charCode++)
            {
                var chr = (char)charCode;
                Debug.Print("0x" + charCode.ToString("X4") + ":" + chr.XEscape());
            }
        }
#endif

        //------------------------------------------------------------
        // テスト用メソッド(エスケープコンバーター = CcVisualization)
        //------------------------------------------------------------
#if DEBUG
        [ManualTestMethod]
        public static void ZZZ_XEscape_CcVisualization()
        {
            for (int charCode = 0; charCode <= 255; charCode++)
            {
                var chr = (char)charCode;
                Debug.Print("0x" + charCode.ToString("X4") + ":" + chr.XEscape(EscapeConverter.CcVisualization));
            }
        }
#endif

    }
}
