// @(h)RegularString.cs ver 0.00 ( '24.01.18 Nov-Lab ) 作成開始
// @(h)RegularString.cs ver 0.51 ( '24.01.20 Nov-Lab ) ベータ版完成

// @(s)
// 　【定型文字列操作】定型文字列の作成機能を提供します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【定型文字列作成関数】オブジェクトから定型文字列を作成する関数メソッドを表します。
    /// </summary>
    /// <param name="target">[in ]：対象オブジェクト</param>
    /// <returns>定型文字列</returns>
    /// <remarks>
    /// 補足<br></br>
    /// ・よく使う定型文字列作成関数は <see cref="RegularString"/> に用意してあります。<br></br>
    /// </remarks>
    //====================================================================================================
    public delegate string RegularStringFunc(object target);


    //====================================================================================================
    /// <summary>
    /// 【定型文字列操作】定型文字列の作成機能を提供します。
    /// </summary>
    //====================================================================================================
    public class RegularString
    {
        //====================================================================================================
        // 型名併記文字列
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【定型文字列作成(型名併記文字列)】
        /// オブジェクトから「(float)1.2」のような型名併記文字列を作成します。
        /// </summary>
        /// <param name="target">[in ]：対象オブジェクト</param>
        /// <returns>
        /// 型名併記文字列
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・<see cref="RegularStringFunc"/> デリゲートに合致しています。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string WithTypeName(object target)
        {
            string strTarget;   // 対象オブジェクトの文字列表現

            //------------------------------------------------------------
            /// オブジェクトから型名併記文字列を作成する
            //------------------------------------------------------------
            if (target is char)
            {                                                           //// 対象オブジェクトが char型の場合
                strTarget = "'" + target.ToString() + "'";              /////  文字列表現 = 'C'形式
            }
            else if (target is string)
            {                                                           //// 対象オブジェクトが string型の場合
                strTarget = "\"" + target.ToString() + "\"";            /////  文字列表現 = "String"形式
            }
            else
            {                                                           //// 上記以外の場合
                strTarget = target.ToString();                          /////  文字列表現 = ToString() で取得
            }

            return "(" + target.GetType().Name + ")" + strTarget;       /////  戻り値 = 型名併記文字列 で関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(RegularString) + "." + nameof(WithTypeName))]
        public static void ZZZ_WithTypeName()
        {
            AutoTest.Test<object, string>(WithTypeName, (byte)123, "(Byte)123");
            AutoTest.Test<object, string>(WithTypeName, (int)123456, "(Int32)123456");
            AutoTest.Test<object, string>(WithTypeName, (float)1.2, "(Single)1.2");
            AutoTest.Test<object, string>(WithTypeName, 'C', "(Char)'C'");
            AutoTest.Test<object, string>(WithTypeName, "文字列", "(String)\"文字列\"");
        }
#endif


        //====================================================================================================
        // アドレス文字列
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【定型文字列作成(アドレス文字列)】
        /// オブジェクトからメモリ上のアドレスを表す16進数文字列を作成します。
        /// </summary>
        /// <param name="target">[in ]：対象オブジェクト</param>
        /// <returns>
        /// アドレス文字列(32ビット環境では「0x12345678」形式、64ビット環境では「0x123456789ABCDEF0」形式)
        /// </returns>
        /// <exception cref="FormatException">対象オブジェクトの型からはアドレス文字列を作成できません。</exception>
        /// <remarks>
        /// 補足<br></br>
        /// ・<see cref="RegularStringFunc"/> デリゲートに合致しています。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string ToAddress(object target)
        {
            //------------------------------------------------------------
            /// メモリ上のアドレスを表す16進数文字列を作成する
            //------------------------------------------------------------
            {
                if (target is IntPtr specified)
                {                                                       //// 対象オブジェクトが IntPtr型の場合
                    UIntPtr address = specified.XToAddressPtr();        /////  アドレス値(メモリ上の同じアドレス値を指す UIntPtr値)に変換する
                    return ToAddress(address.ToUInt64());               /////  定型文字列を作成して戻り値とし、関数終了(UIntPtr は IFormattable I/Fを実装しないので、UInt64経由で作成する)
                }
            }

            {
                if (target is UIntPtr specified)
                {                                                       //// 対象オブジェクトが UIntPtr型の場合
                    return ToAddress(specified.ToUInt64());             /////  定型文字列を作成して戻り値とし、関数終了(UIntPtr は IFormattable I/Fを実装しないので、UInt64経由で作成する)
                }
            }

            if (target is IFormattable ifFormattable)
            {                                                           //// 対象オブジェクトが IFormattable I/F を持つ場合
                return ToAddress(ifFormattable);                        /////  戻り値 = IFormattable を用いて作成した定型文字列 で関数終了
            }


            throw new FormatException(target.GetType() + " 型からはアドレス文字列を作成できません。");
        }


        // ＜メモ＞
        // ・formatProvider を null にすると、formatProvider を指定しない ToString(format) と同じように、
        //   現在のカルチャの NumberFormatInfo オブジェクトを用いる。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【定型文字列作成(アドレス文字列)】
        /// IFormattable I/F を持つオブジェクトからメモリ上のアドレスを表す16進数文字列を作成します。<br></br>
        /// 32ビット環境では「0x12345678」形式に、64ビット環境では「0x123456789ABCDEF0」形式になります。
        /// </summary>
        /// <param name="ifFormattable">[in ]：IFormattable I/F</param>
        /// <returns>
        /// アドレス文字列
        /// </returns>
        /// <exception cref="FormatException">対象オブジェクトの型からはアドレス文字列を作成できません。</exception>
        /// <remarks>
        /// 補足<br></br>
        /// ・<see cref="RegularStringFunc"/> デリゲートに合致しています。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string ToAddress(IFormattable ifFormattable)
        {
            //------------------------------------------------------------
            /// メモリ上のアドレスを表す16進数文字列を作成する
            //------------------------------------------------------------
            if (IntPtr.Size == 4)
            {                                                           //// 32ビットプラットフォームの場合
                return "0x" + ifFormattable.ToString("X8", null);       /////  戻り値 = 8桁の16進数文字列 で関数終了
            }
            else
            {                                                           //// 64ビットプラットフォームの場合
                return "0x" + ifFormattable.ToString("X16", null);      /////  戻り値 = 16桁の16進数文字列 で関数終了
            }
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(RegularString) + "." + nameof(ToAddress))]
        public static void ZZZ_ToAddress()
        {
            var testOptions = new AutoTestOptions()
            {                                           // テストオプション
                fncArg1RegularString = WithTypeName,    // ・引数１(対象オブジェクト)は型名併記
            };

            if (IntPtr.Size == 4)
            {
                AutoTest.Print("＜符号なし整数型＞");
                AutoTest.Test<object, string>(ToAddress, (byte)0xFF, "0x000000FF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (ushort)0xFFFF, "0x0000FFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (uint)0xFFFFFFFF, "0xFFFFFFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (ulong)0xFFFFFFFFFFFFFFFF, "0xFFFFFFFFFFFFFFFF", testOptions);
                AutoTest.Print("");

                AutoTest.Print("＜符号付き整数型＞");
                AutoTest.Test<object, string>(ToAddress, (sbyte)-1, "0x000000FF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (short)-1, "0x0000FFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (int)-1, "0xFFFFFFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (long)-1, "0xFFFFFFFFFFFFFFFF", testOptions);
                AutoTest.Print("");

                AutoTest.Print("＜ポインター型＞");
                AutoTest.Test<object, string>(ToAddress, (IntPtr)(-1), "0xFFFFFFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, XUIntPtr.XGetLastAddressValue(), "0xFFFFFFFF", testOptions);
                AutoTest.Print("");
            }
            else
            {
                AutoTest.Print("＜符号なし整数型＞");
                AutoTest.Test<object, string>(ToAddress, (byte)0xFF, "0x00000000000000FF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (ushort)0xFFFF, "0x000000000000FFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (uint)0xFFFFFFFF, "0x00000000FFFFFFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (ulong)0xFFFFFFFFFFFFFFFF, "0xFFFFFFFFFFFFFFFF", testOptions);
                AutoTest.Print("");

                AutoTest.Print("＜符号付き整数型＞");
                AutoTest.Test<object, string>(ToAddress, (sbyte)-1, "0x00000000000000FF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (short)-1, "0x000000000000FFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (int)-1, "0x00000000FFFFFFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, (long)-1, "0xFFFFFFFFFFFFFFFF", testOptions);
                AutoTest.Print("");

                AutoTest.Print("＜ポインター型＞");
                AutoTest.Test<object, string>(ToAddress, (IntPtr)(-1), "0xFFFFFFFFFFFFFFFF", testOptions);
                AutoTest.Test<object, string>(ToAddress, XUIntPtr.XGetLastAddressValue(), "0xFFFFFFFFFFFFFFFF", testOptions);
                AutoTest.Print("");
            }

            AutoTest.Print("＜変換不能な型＞");
            AutoTest.Test<object, string>(ToAddress, (float)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToAddress, (double)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToAddress, (decimal)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToAddress, 'C', typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToAddress, "String", typeof(FormatException), testOptions);
        }
#endif


        // ＜メモ＞
        // ・System.Numerics.BigInteger (System.Numerics.dll 内) は別途参照が必要なので未テスト
        //====================================================================================================
        // 各種サイズ用16進数文字列
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【定型文字列作成(1バイト用16進数文字列)】
        /// オブジェクトから1バイト用16進数文字列(「0x12」形式)を作成します。
        /// </summary>
        /// <param name="target">[in ]：対象オブジェクト</param>
        /// <returns>
        /// 1バイト用16進数文字列
        /// </returns>
        /// <exception cref="FormatException">対象オブジェクトの型からは1バイト用16進数文字列を作成できません。</exception>
        /// <remarks>
        /// 補足<br></br>
        /// ・<see cref="RegularStringFunc"/> デリゲートに合致しています。<br></br>
        /// ・対象オブジェクトが1バイトを超えるサイズの場合は、そのサイズに合わせた16進数文字列になります。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string ToHex1byte(object target)
        {
            //------------------------------------------------------------
            /// オブジェクトから1バイト用16進数文字列を作成する
            //------------------------------------------------------------
            if (target is IFormattable ifFormattable)
            {                                                           //// 対象オブジェクトが IFormattable I/F を持つ場合
                return "0x" + ifFormattable.ToString("X2", null);       /////  戻り値 = 1バイト用16進数文字列 で関数終了
            }

            throw new FormatException(target.GetType() + " 型からは1バイト用16進数文字列を作成できません。");
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(RegularString) + "." + nameof(ToHex1byte))]
        public static void ZZZ_ToHex1byte()
        {
            var testOptions = new AutoTestOptions()
            {                                           // テストオプション
                fncArg1RegularString = WithTypeName,    // ・引数１(対象オブジェクト)は型名併記
            };

            AutoTest.Print("＜符号なし整数型＞");
            AutoTest.Test<object, string>(ToHex1byte, (byte)0xFF, "0xFF", testOptions);
            AutoTest.Test<object, string>(ToHex1byte, (ushort)0xFFFF, "0xFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex1byte, (uint)0xFFFFFFFF, "0xFFFFFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex1byte, (ulong)0xFFFFFFFFFFFFFFFF, "0xFFFFFFFFFFFFFFFF", testOptions);
            AutoTest.Print("");

            AutoTest.Print("＜符号付き整数型＞");
            AutoTest.Test<object, string>(ToHex1byte, (sbyte)-1, "0xFF", testOptions);
            AutoTest.Test<object, string>(ToHex1byte, (short)-1, "0xFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex1byte, (int)-1, "0xFFFFFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex1byte, (long)-1, "0xFFFFFFFFFFFFFFFF", testOptions);
            AutoTest.Print("");

            AutoTest.Print("＜変換不能な型＞");
            AutoTest.Test<object, string>(ToHex1byte, (float)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex1byte, (double)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex1byte, (decimal)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex1byte, 'C', typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex1byte, "String", typeof(FormatException), testOptions);
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【定型文字列作成(2バイト用16進数文字列)】
        /// オブジェクトから2バイト用16進数文字列(「0x1234」形式)を作成します。
        /// </summary>
        /// <param name="target">[in ]：対象オブジェクト</param>
        /// <returns>
        /// 2バイト用16進数文字列
        /// </returns>
        /// <exception cref="FormatException">対象オブジェクトの型からは2バイト用16進数文字列を作成できません。</exception>
        /// <remarks>
        /// 補足<br></br>
        /// ・<see cref="RegularStringFunc"/> デリゲートに合致しています。<br></br>
        /// ・対象オブジェクトが2バイトを超えるサイズの場合は、そのサイズに合わせた16進数文字列になります。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string ToHex2byte(object target)
        {
            //------------------------------------------------------------
            /// オブジェクトから2バイト用16進数文字列を作成する
            //------------------------------------------------------------
            if (target is IFormattable ifFormattable)
            {                                                           //// 対象オブジェクトが IFormattable I/F を持つ場合
                return "0x" + ifFormattable.ToString("X4", null);       /////  戻り値 = 2バイト用16進数文字列 で関数終了
            }

            throw new FormatException(target.GetType() + " 型からは2バイト用16進数文字列を作成できません。");
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(RegularString) + "." + nameof(ToHex2byte))]
        public static void ZZZ_ToHex2byte()
        {
            var testOptions = new AutoTestOptions()
            {                                           // テストオプション
                fncArg1RegularString = WithTypeName,    // ・引数１(対象オブジェクト)は型名併記
            };

            AutoTest.Print("＜符号なし整数型＞");
            AutoTest.Test<object, string>(ToHex2byte, (byte)0xFF, "0x00FF",  testOptions);
            AutoTest.Test<object, string>(ToHex2byte, (ushort)0xFFFF, "0xFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex2byte, (uint)0xFFFFFFFF, "0xFFFFFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex2byte, (ulong)0xFFFFFFFFFFFFFFFF, "0xFFFFFFFFFFFFFFFF", testOptions);
            AutoTest.Print("");

            AutoTest.Print("＜符号付き整数型＞");
            AutoTest.Test<object, string>(ToHex2byte, (sbyte)-1, "0x00FF", testOptions);
            AutoTest.Test<object, string>(ToHex2byte, (short)-1, "0xFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex2byte, (int)-1, "0xFFFFFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex2byte, (long)-1, "0xFFFFFFFFFFFFFFFF", testOptions);
            AutoTest.Print("");

            AutoTest.Print("＜変換不能な型＞");
            AutoTest.Test<object, string>(ToHex2byte, (float)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex2byte, (double)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex2byte, (decimal)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex2byte, 'C', typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex2byte, "String", typeof(FormatException), testOptions);
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【定型文字列作成(4バイト用16進数文字列)】
        /// オブジェクトから4バイト用16進数文字列(「0x12345678」形式)を作成します。
        /// </summary>
        /// <param name="target">[in ]：対象オブジェクト</param>
        /// <returns>
        /// 4バイト用16進数文字列
        /// </returns>
        /// <exception cref="FormatException">対象オブジェクトの型からは4バイト用16進数文字列を作成できません。</exception>
        /// <remarks>
        /// 補足<br></br>
        /// ・<see cref="RegularStringFunc"/> デリゲートに合致しています。<br></br>
        /// ・対象オブジェクトが4バイトを超えるサイズの場合は、そのサイズに合わせた16進数文字列になります。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string ToHex4byte(object target)
        {
            //------------------------------------------------------------
            /// オブジェクトから4バイト用16進数文字列を作成する
            //------------------------------------------------------------
            if (target is IFormattable ifFormattable)
            {                                                           //// 対象オブジェクトが IFormattable I/F を持つ場合
                return "0x" + ifFormattable.ToString("X8", null);       /////  戻り値 = 4バイト用16進数文字列 で関数終了
            }

            throw new FormatException(target.GetType() + " 型からは4バイト用16進数文字列を作成できません。");
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(RegularString) + "." + nameof(ToHex4byte))]
        public static void ZZZ_ToHex4byte()
        {
            var testOptions = new AutoTestOptions()
            {                                           // テストオプション
                fncArg1RegularString = WithTypeName,    // ・引数１(対象オブジェクト)は型名併記
            };

            AutoTest.Print("＜符号なし整数型＞");
            AutoTest.Test<object, string>(ToHex4byte, (byte)0xFF, "0x000000FF", testOptions);
            AutoTest.Test<object, string>(ToHex4byte, (ushort)0xFFFF, "0x0000FFFF", testOptions);
            AutoTest.Test<object, string>(ToHex4byte, (uint)0xFFFFFFFF, "0xFFFFFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex4byte, (ulong)0xFFFFFFFFFFFFFFFF, "0xFFFFFFFFFFFFFFFF", testOptions);
            AutoTest.Print("");

            AutoTest.Print("＜符号付き整数型＞");
            AutoTest.Test<object, string>(ToHex4byte, (sbyte)-1, "0x000000FF", testOptions);
            AutoTest.Test<object, string>(ToHex4byte, (short)-1, "0x0000FFFF", testOptions);
            AutoTest.Test<object, string>(ToHex4byte, (int)-1, "0xFFFFFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex4byte, (long)-1, "0xFFFFFFFFFFFFFFFF", testOptions);
            AutoTest.Print("");

            AutoTest.Print("＜変換不能な型＞");
            AutoTest.Test<object, string>(ToHex4byte, (float)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex4byte, (double)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex4byte, (decimal)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex4byte, 'C', typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex4byte, "String", typeof(FormatException), testOptions);
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【定型文字列作成(8バイト用16進数文字列)】
        /// オブジェクトから8バイト用16進数文字列(「0x123456789ABCDEF0」形式)を作成します。
        /// </summary>
        /// <param name="target">[in ]：対象オブジェクト</param>
        /// <returns>
        /// 4バイト用16進数文字列
        /// </returns>
        /// <exception cref="FormatException">対象オブジェクトの型からは8バイト用16進数文字列を作成できません。</exception>
        /// <remarks>
        /// 補足<br></br>
        /// ・<see cref="RegularStringFunc"/> デリゲートに合致しています。<br></br>
        /// ・対象オブジェクトが8バイトを超えるサイズの場合は、そのサイズに合わせた16進数文字列になります。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string ToHex8byte(object target)
        {
            //------------------------------------------------------------
            /// オブジェクトから8バイト用16進数文字列を作成する
            //------------------------------------------------------------
            if (target is IFormattable ifFormattable)
            {                                                           //// 対象オブジェクトが IFormattable I/F を持つ場合
                return "0x" + ifFormattable.ToString("X16", null);      /////  戻り値 = 8バイト用16進数文字列 で関数終了
            }

            throw new FormatException(target.GetType() + " 型からは8バイト用16進数文字列を作成できません。");
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(RegularString) + "." + nameof(ToHex8byte))]
        public static void ZZZ_ToHex8byte()
        {
            var testOptions = new AutoTestOptions()
            {                                           // テストオプション
                fncArg1RegularString = WithTypeName,    // ・引数１(対象オブジェクト)は型名併記
            };

            AutoTest.Print("＜符号なし整数型＞");
            AutoTest.Test<object, string>(ToHex8byte, (byte)0xFF, "0x00000000000000FF", testOptions);
            AutoTest.Test<object, string>(ToHex8byte, (ushort)0xFFFF, "0x000000000000FFFF", testOptions);
            AutoTest.Test<object, string>(ToHex8byte, (uint)0xFFFFFFFF, "0x00000000FFFFFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex8byte, (ulong)0xFFFFFFFFFFFFFFFF, "0xFFFFFFFFFFFFFFFF", testOptions);
            AutoTest.Print("");

            AutoTest.Print("＜符号付き整数型＞");
            AutoTest.Test<object, string>(ToHex8byte, (sbyte)-1, "0x00000000000000FF", testOptions);
            AutoTest.Test<object, string>(ToHex8byte, (short)-1, "0x000000000000FFFF", testOptions);
            AutoTest.Test<object, string>(ToHex8byte, (int)-1, "0x00000000FFFFFFFF", testOptions);
            AutoTest.Test<object, string>(ToHex8byte, (long)-1, "0xFFFFFFFFFFFFFFFF", testOptions);
            AutoTest.Print("");

            AutoTest.Print("＜変換不能な型＞");
            AutoTest.Test<object, string>(ToHex8byte, (float)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex8byte, (double)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex8byte, (decimal)1.2, typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex8byte, 'C', typeof(FormatException), testOptions);
            AutoTest.Test<object, string>(ToHex8byte, "String", typeof(FormatException), testOptions);
        }
#endif

        //[-] 保留：必要になったら作る。ToYen 「\123,456,789」形式。円金額用。"C"＋ja-JPカルチャ

    } // class

} // namespace
