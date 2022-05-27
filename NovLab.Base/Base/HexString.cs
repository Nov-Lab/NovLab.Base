// @(h)HexString.cs ver 0.00 ( '22.03.24 Nov-Lab ) 作成開始
// @(h)HexString.cs ver 0.21 ( '22.03.24 Nov-Lab ) アルファ版完成
// @(h)HexString.cs ver 0.21a( '22.05.24 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【16進数文字列】16進数文字列の操作機能を提供します。

using System;
using System.Collections.Generic;
using System.Text;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【16進数文字列】16進数文字列の操作機能を提供します。
    /// </summary>
    //====================================================================================================
    public class HexString
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【16進数文字列をバイト配列に変換】
        /// </summary>
        /// <param name="hexString">[in ]：16進数文字列(区切り文字なし)</param>
        /// <returns>
        /// バイト配列
        /// </returns>
        //--------------------------------------------------------------------------------
        public static byte[] ToBytes(string hexString)
        {
            //------------------------------------------------------------
            /// 16進数文字列をバイト配列に変換する
            //------------------------------------------------------------
            if (hexString.Length % 2 != 0)
            {                                                           //// 16進数文字列の文字数が２の倍数でない場合
                throw new FormatException(                              /////  書式不正例外をスローする
                    "入力文字列の形式が正しくありません。");
            }

            var bytes = new byte[hexString.Length / 2];                 //// バイト配列を生成する
            for (var index = 0; index < bytes.Length; index++)
            {                                                           //// バイト配列を繰り返す
                bytes[index] = byte.Parse(hexString.Substring(index * 2, 2),
                                System.Globalization.NumberStyles.HexNumber);
            }

            return bytes;                                               //// 戻り値 = バイト配列 で関数終了


            // 例外メッセージは byte.Parse("ZZ", System.Globalization.NumberStyles.HexNumber); を実行したときの内容を参考にした。
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【バイト配列を16進数文字列に変換】
        /// </summary>
        /// <param name="bytes">[in ]：バイト配列</param>
        /// <returns>
        /// 16進数文字列(区切り文字なし)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string FromBytes(byte[] bytes)
        {
            //------------------------------------------------------------
            /// バイト配列を16進数文字列に変換する
            //------------------------------------------------------------
            var hexString = new StringBuilder(bytes.Length * 2);        //// バイト配列サイズ×２の容量で16進数文字列を生成する
            foreach (var byt in bytes)
            {                                                           /////  バイト配列を繰り返す
                hexString.Append(byt.ToString("X2"));                   //////   バイト値を16進数文字列に変換して追加する
            }

            return hexString.ToString();                                //// 戻り値 = 16進数文字列 で関数終了
        }

    }
}
