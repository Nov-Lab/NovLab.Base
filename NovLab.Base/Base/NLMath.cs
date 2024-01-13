// @(h)NLMath.cs ver 0.00 ( '22.03.24 Nov-Lab ) 作成開始
// @(h)NLMath.cs ver 0.21 ( '22.03.24 Nov-Lab ) アルファ版完成
// @(h)NLMath.cs ver 0.22 ( '22.05.09 Nov-Lab ) 機能追加：XNLMath.XIsInRange の int版を追加した。
// @(h)NLMath.cs ver 0.22a( '22.05.17 Nov-Lab ) その他  ：テスト用メソッドを追加した。機能変更なし。
// @(h)NLMath.cs ver 0.22b( '22.05.18 Nov-Lab ) その他  ：AutoTestResultInfo のクラス名変更に対応した。機能変更なし。
// @(h)NLMath.cs ver 0.23 ( '24.01.14 Nov-Lab ) 機能追加：XNLMath.XIsInRange の long版と ulong版を追加した。
// @(h)NLMath.cs ver 0.23a( '24.01.21 Nov-Lab ) 仕変対応：AutoTest クラスの仕様変更に対応した。機能変更なし。

// @(s)
// 　【数学関数】数学関数を提供します。

#if DEBUG
//#define CTRL_F5 // Ctrl+F5テスト：中断対象例外もテストします。中断対象例外は、例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能です。
#endif

using System;
using System.Collections.Generic;

using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【ジェネリック数学関数】各種数値データ型に対応するジェネリックな数学関数を提供します。
    /// </summary>
    /// <typeparam name="TNumeric">数値データ型</typeparam>
    //====================================================================================================
    public static class NLMath<TNumeric> where TNumeric : struct, IComparable
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【数値範囲内チェック】数値が最小値から最大値の範囲内にあるかどうかをチェックします。
        /// </summary>
        /// <param name="value">   [in ]：数値</param>
        /// <param name="minValue">[in ]：最小値</param>
        /// <param name="maxValue">[in ]：最大値</param>
        /// <returns>
        /// チェック結果[true = 範囲内 / false = 範囲外]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static bool IsInRange(TNumeric value, TNumeric minValue, TNumeric maxValue)
        {
            //------------------------------------------------------------
            /// 数値が最小値から最大値の範囲内にあるかどうかをチェックする
            //------------------------------------------------------------
            if (value.CompareTo(minValue) < 0)
            {                                                           //// 数値が最小値よりも小さい場合
                return false;                                           /////  戻り値 = false(範囲外) で関数終了
            }
            else if (value.CompareTo(maxValue) > 0)
            {                                                           //// 数値が最大値よりも大きい場合
                return false;                                           /////  戻り値 = false(範囲外) で関数終了
            }
            else
            {                                                           //// 上記チェックをパスした場合(最小値から最大値の範囲内にある場合)
                return true;                                            /////  戻り値 = true(範囲内) で関数終了
            }
        }
    }


    //====================================================================================================
    /// <summary>
    /// 【数学関数拡張メソッド】各種数値データ型に数学関数の拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XNLMath
    {
        ///[-] 随時：必要になったら他の数値データ型にも展開する

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【byte値範囲内チェック】byte値が最小値から最大値の範囲内にあるかどうかをチェックします。
        /// </summary>
        /// <param name="value">   [in ]：値</param>
        /// <param name="minValue">[in ]：最小値</param>
        /// <param name="maxValue">[in ]：最大値</param>
        /// <returns>
        /// チェック結果[true = 範囲内 / false = 範囲外]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static bool XIsInRange(this byte value, byte minValue, byte maxValue)
            => NLMath<byte>.IsInRange(value, minValue, maxValue);

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod("byte.XIsInRange")]
        public static void ZZZ_XIsInRange_byte()
        {
            SubRoutine(0, 32, 127, false, "0 は範囲外");
            SubRoutine(32, 32, 127, true, "最小値は範囲内");
            SubRoutine(127, 32, 127, true, "最大値は範囲内");
            SubRoutine(31, 32, 127, false, "最小値-1は範囲外");
            SubRoutine(128, 32, 127, false, "最小値+1は範囲外");
            SubRoutine(byte.MinValue, 32, 127, false, "MinValueは範囲外");
            SubRoutine(byte.MaxValue, 32, 127, false, "MaxValueは範囲外");

            void SubRoutine(byte value,                             // [in ]：数値
                            byte minValue,                          // [in ]：最小値
                            byte maxValue,                          // [in ]：最大値
                            AutoTestResultInfo<bool> expectResult,  // [in ]：予想結果(bool値 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.TestX(XIsInRange, value, minValue, maxValue, expectResult, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【int値範囲内チェック】int値が最小値から最大値の範囲内にあるかどうかをチェックします。
        /// </summary>
        /// <param name="value">   [in ]：値</param>
        /// <param name="minValue">[in ]：最小値</param>
        /// <param name="maxValue">[in ]：最大値</param>
        /// <returns>
        /// チェック結果[true = 範囲内 / false = 範囲外]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static bool XIsInRange(this int value, int minValue, int maxValue)
            => NLMath<int>.IsInRange(value, minValue, maxValue);

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod("int.XIsInRange")]
        public static void ZZZ_XIsInRange_int()
        {
            SubRoutine(0, -100, 200, true, "0 は範囲内");
            SubRoutine(-100, -100, 200, true, "最小値は範囲内");
            SubRoutine(200, -100, 200, true, "最大値は範囲内");
            SubRoutine(-101, -100, 200, false, "最小値-1は範囲外");
            SubRoutine(201, -100, 200, false, "最小値+1は範囲外");
            SubRoutine(int.MinValue, -100, 200, false, "MinValueは範囲外");
            SubRoutine(int.MaxValue, -100, 200, false, "MaxValueは範囲外");

            void SubRoutine(int value,                              // [in ]：数値
                            int minValue,                           // [in ]：最小値
                            int maxValue,                           // [in ]：最大値
                            AutoTestResultInfo<bool> expectResult,  // [in ]：予想結果(bool値 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.TestX(XIsInRange, value, minValue, maxValue, expectResult, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【long値範囲内チェック】long値が最小値から最大値の範囲内にあるかどうかをチェックします。
        /// </summary>
        /// <param name="value">   [in ]：値</param>
        /// <param name="minValue">[in ]：最小値</param>
        /// <param name="maxValue">[in ]：最大値</param>
        /// <returns>
        /// チェック結果[true = 範囲内 / false = 範囲外]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static bool XIsInRange(this long value, long minValue, long maxValue)
            => NLMath<long>.IsInRange(value, minValue, maxValue);

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod("long.XIsInRange")]
        public static void ZZZ_XIsInRange_long()
        {
            SubRoutine(0, -100, 200, true, "0 は範囲内");
            SubRoutine(-100, -100, 200, true, "最小値は範囲内");
            SubRoutine(200, -100, 200, true, "最大値は範囲内");
            SubRoutine(-101, -100, 200, false, "最小値-1は範囲外");
            SubRoutine(201, -100, 200, false, "最小値+1は範囲外");
            SubRoutine(long.MinValue, -100, 200, false, "MinValueは範囲外");
            SubRoutine(long.MaxValue, -100, 200, false, "MaxValueは範囲外");

            void SubRoutine(long value,                             // [in ]：数値
                            long minValue,                          // [in ]：最小値
                            long maxValue,                          // [in ]：最大値
                            AutoTestResultInfo<bool> expectResult,  // [in ]：予想結果(bool値 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.TestX(XIsInRange, value, minValue, maxValue, expectResult, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ulong値範囲内チェック】ulong値が最小値から最大値の範囲内にあるかどうかをチェックします。
        /// </summary>
        /// <param name="value">   [in ]：値</param>
        /// <param name="minValue">[in ]：最小値</param>
        /// <param name="maxValue">[in ]：最大値</param>
        /// <returns>
        /// チェック結果[true = 範囲内 / false = 範囲外]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static bool XIsInRange(this ulong value, ulong minValue, ulong maxValue)
            => NLMath<ulong>.IsInRange(value, minValue, maxValue);

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod("ulong.XIsInRange")]
        public static void ZZZ_XIsInRange_ulong()
        {
            SubRoutine(0, 32, 123456789012345, false, "0 は範囲外");
            SubRoutine(32, 32, 123456789012345, true, "最小値は範囲内");
            SubRoutine(123456789012345, 32, 123456789012345, true, "最大値は範囲内");
            SubRoutine(31, 32, 123456789012345, false, "最小値-1は範囲外");
            SubRoutine(123456789012346, 32, 123456789012345, false, "最小値+1は範囲外");
            SubRoutine(ulong.MinValue, 32, 123456789012345, false, "MinValueは範囲外");
            SubRoutine(ulong.MaxValue, 32, 123456789012345, false, "MaxValueは範囲外");

            void SubRoutine(ulong value,                            // [in ]：数値
                            ulong minValue,                         // [in ]：最小値
                            ulong maxValue,                         // [in ]：最大値
                            AutoTestResultInfo<bool> expectResult,  // [in ]：予想結果(bool値 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.TestX(XIsInRange, value, minValue, maxValue, expectResult, testPattern);
            }
        }
#endif

    } // class

} // namespace
