// @(h)FourCC.cs ver 0.00 ( '22.03.24 Nov-Lab ) 作成開始
// @(h)FourCC.cs ver 0.21 ( '22.03.24 Nov-Lab ) アルファ版完成。isprint は仮
// @(h)FourCC.cs ver 0.21a( '22.05.25 Nov-Lab ) その他  ：自動テスト用メソッドを追加した。機能変更なし。

// @(s)
// 　【FourCC値操作】FourCC値(Four-Character Codes：4文字コード)の操作機能を提供します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using NovLab;
using NovLab.DebugSupport;


// 【参考：MMSystem.hから】
//====================================================================================================
// typedef DWORD           FOURCC;         /* a four character code */
//
// #define MAKEFOURCC(ch0, ch1, ch2, ch3)                              \
//                 ((DWORD)(BYTE)(ch0) | ((DWORD)(BYTE)(ch1) << 8) |   \
//                 ((DWORD)(BYTE)(ch2) << 16) | ((DWORD)(BYTE)(ch3) << 24 ))
//
// #define mmioFOURCC(ch0, ch1, ch2, ch3)  MAKEFOURCC(ch0, ch1, ch2, ch3)
//
// #define FOURCC_RIFF     mmioFOURCC('R', 'I', 'F', 'F')
//====================================================================================================

// 【参考：旧MSDNライブラリから】
//====================================================================================================
// mmioStringToFOURCC
//   NULL で終わる文字列を 4 文字コードに変換します。
// 
//   FOURCC mmioStringToFOURCC(
//     LPCSTR sz, 
//     UINT wFlags
//   );
// 
// パラメータ
//   sz 
//     4 文字コードに変換する NULL で終わる文字列のアドレスを指定します。 
//   wFlags 
//     変換のためのフラグを指定します。次の値が定義されています。 
//     MMIO_TOUPPER 
//     すべての文字を大文字に変換します。 
// 
// 戻り値
//   関数が成功すると、指定された文字列から作成された 4 文字コードが返ります。
// 
// 解説
//   この関数は文字列を 4 文字コードにコピーし、必要に応じてスペースの埋め込みや切り捨てを行います。返されたコードが有効かどうかはチェックしません。
// 
// 対応情報
//   Windows NT/2000：Windows NT 3.1 以降
//   Windows 95/98：Windows 95 以降
//   ヘッダー：mmsystem.h 内で宣言
//   インポートライブラリ：winmm.lib を使用
//   Unicode：Windows NT/2000 は Unicode 版と ANSI 版を実装
//====================================================================================================
// Video formats are identified using a four character code (FOURCC).
// A FOURCC value is a DWORD value containing the ASCII hexadecimal representation of four characters.
// The character codes are stored in the DWORD in ascending byte order.
// 
// For example, the FOURCC for content encoded with the Windows Media Video 9 codec is "WMV3".
// The ASCII representation of those four characters (in hexadecimal) is: 'W' = 0x57, 'M' = 0x4D, 'V' = 0x56, '3' = 0x33.
// Stored in a DWORD, this FOURCC value is 0x33564D57.
//====================================================================================================


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【FourCC値操作】FourCC値(Four-Character Codes：4文字コード)の操作機能を提供します。
    /// </summary>
    /// <remarks>
    /// ・FourCC値(4文字コード)は、識別子などをASCII文字列4文字で表現し、それを32ビット符号なし整数に変換
    ///   したものです。RIFFファイルやマルチメディアファイルなどで使用します。<br></br>
    /// ・4文字に満たない場合は半角スペースで補填しておきます。<br></br>
    /// </remarks>
    //====================================================================================================
    public static class FourCC
    {
        //[-] 仮のisprint
        /// <summary>
        /// 【印刷可能文字チェック】仮のisprint
        /// </summary>
        /// <param name="byt"></param>
        /// <returns></returns>
        private static bool M_KariIsPrint(byte byt)
        {
            if (byt.XIsInRange(32, 126))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        //====================================================================================================
        // 内部定数
        //====================================================================================================
        /// <summary>
        /// 【例外メッセージ：FourCC文字列書式不正】
        /// </summary>
        private const string BAD_FOURCC_FORMAT = "FourCC文字列は4文字のASCII文字列でなければなりません";

        /// <summary>
        /// 【"?"のバイト値】ASCII文字"?"に対応するバイト値です。
        /// </summary>
        private const byte M_QUESTION_BYTE = 0x3f;


        //====================================================================================================
        // staticメソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列化】FourCC値を"0x33564D57:WMV3"のような文字列形式に変換します。
        /// </summary>
        /// <param name="fourCC">[in ]：FourCC値</param>
        /// <returns>
        /// 文字列形式
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string ToString(uint fourCC)
        {
            return "0x" + fourCC.ToString("X8") + ":" + FourCCToString(fourCC);
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【FourCC値から文字列へ変換】FourCC値をASCII文字列4文字に変換します。
        /// </summary>
        /// <param name="fourCC">[in ]：FourCC値</param>
        /// <returns>
        /// FourCC文字列(ASCII文字列4文字)
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・印刷可能文字でない部分は'?'になります。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string FourCCToString(uint fourCC)
        {
            //------------------------------------------------------------
            /// FourCC値をASCII文字列4文字に変換する
            //------------------------------------------------------------
            var bytes = BitConverter.GetBytes(fourCC);                  //// FourCC値をバイト配列に変換する

            for (int index = 0; index < bytes.Length; index++)
            {                                                           //// バイト配列を繰り返す
                if (M_KariIsPrint(bytes[index]) == false)
                {                                                       /////  印刷可能文字の文字コードでない場合
                    bytes[index] = M_QUESTION_BYTE;                     //////   "?"のバイト値に差し替える
                }
            }

            return Encoding.ASCII.GetString(bytes);                     //// バイト配列をASCII文字列に変換して戻り値とし、関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod]
        public static void ZZZ_FourCCToString(IAutoTestExecuter ifExecuter)
        {
            SubRoutine(0x44434241, "ABCD", "正常系");
            SubRoutine(0x33564D57, "WMV3", "正常系");
            SubRoutine(0x33560057, "W?V3", "不正系(制御コード文字が含まれる)");

            void SubRoutine(uint fourCC,                            // [in ]：FourCC値
                            AutoTestResultInfo<string> expectResult,// [in ]：予想結果(FourCC文字列(ASCII文字列4文字) または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.Test(FourCCToString, fourCC, expectResult, ifExecuter, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列からFourCC値へ変換】ASCII文字列4文字をFourCC値に変換します。
        /// </summary>
        /// <param name="fourCCString">[in ]：FourCC文字列(ASCII文字列4文字)</param>
        /// <returns>
        /// FourCC値
        /// </returns>
        //--------------------------------------------------------------------------------
        public static uint StringToFourCC(string fourCCString)
        {
            //------------------------------------------------------------
            /// ASCII文字列4文字をFourCC値に変換する
            //------------------------------------------------------------
            if (fourCCString == null)
            {                                                           //// FourCC文字列 = null の場合
                throw new ArgumentNullException(nameof(fourCCString));  /////  引数null例外をスローする
            }

            if (fourCCString.Length != 4)
            {                                                           //// FourCC文字列が4文字でない場合
                throw new FormatException(BAD_FOURCC_FORMAT);           /////  書式不正例外(FourCC文字列書式不正)をスローする
            }

            var fourCCBytes = Encoding.UTF8.GetBytes(fourCCString);     //// FourCC文字列をバイト配列に変換する

            if (fourCCBytes.Length != 4)
            {                                                           //// FourCCバイト配列が4バイトでない場合(ASCII文字でないものが含まれていた場合)
                throw new FormatException(BAD_FOURCC_FORMAT);           /////  書式不正例外(FourCC文字列書式不正)をスローする
            }

            for (int index = 0; index < fourCCBytes.Length; index ++)
            {                                                           //// FourCCバイト配列を繰り返す
                if (M_KariIsPrint(fourCCBytes[index]) == false)
                {                                                       /////  印刷可能文字の文字コードでない場合
                    throw new FormatException(BAD_FOURCC_FORMAT);       //////   書式不正例外(FourCC文字列書式不正)をスローする
                }
            }

            return BitConverter.ToUInt32(fourCCBytes, 0);               //// FourCCバイト配列をUInt32型に変換して戻り値とし、関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod]
        public static void ZZZ_StringToFourCC(IAutoTestExecuter ifExecuter)
        {
            SubRoutine("ABCD", 0x44434241, "正常系");
            SubRoutine("WMV3", 0x33564D57, "正常系");
            SubRoutine("AVI ", 0x20495641, "正常系(不足分を半角スペースで補填)");
            SubRoutine("W\0V3", typeof(FormatException), "書式不正(制御コード文字が含まれる)");
            SubRoutine("WあV3", typeof(FormatException), "書式不正(全角文字が含まれる)");
            SubRoutine("WMV-3", typeof(FormatException), "書式不正(文字数が多い)");
            SubRoutine("WV3", typeof(FormatException), "書式不正(文字数が少ない)");
            SubRoutine(null, typeof(ArgumentNullException), "FourCC文字列が null");

            void SubRoutine(string fourCCString,                    // [in ]：FourCC文字列(ASCII文字列4文字)
                            AutoTestResultInfo<uint> expectResult,  // [in ]：予想結果(FourCC値 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.Test(StringToFourCC, fourCCString, expectResult, ifExecuter, testPattern);
            }
        }
#endif

    }
}
