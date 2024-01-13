// @(h)XIntPtr.cs ver 0.00 ( '24.01.14 Nov-Lab ) 作成開始
// @(h)XIntPtr.cs ver 0.51 ( '24.01.15 Nov-Lab ) ベータ版完成
// @(h)XIntPtr.cs ver 0.52 ( '24.01.20 Nov-Lab ) XUIntPtr.XGetLastAddressValue() を追加した

// @(s)
// 　【IntPtr拡張メソッド】IntPtr型やUIntPtr型に拡張メソッドを追加します。

using System;
using System.Diagnostics;
using System.Collections.Generic;

using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【IntPtr拡張メソッド】IntPtr型に拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XIntPtr
    {
        // ＜メモ＞
        // ・ポインター操作はCLSに準拠しているIntPtrを使うのが標準的手法
        //   しかし：IntPtr には大小比較機能がない
        //   なので：範囲チェック機能を自作する
        //   しかし：符号付きであるIntPtr型でそのまま比較すると正しく比較できない
        //   なので：UIntPtr に変換してから比較する
        //   しかし：IntPtr から UIntPtr へ変換する機能がないので、それも自作する
        //   さらに：UIntPtr にも大小比較機能がないので、それも自作する
        //
        // ・uncheckedブロックを使い、ビット表現が同じになるように IntPtr から UIntPtr へ変換する方法
        //   も考えたが、インスタンスデータのサイズ・配置・構造が合致しているという完全な保証はないので、
        //   32ビット版の変換処理と64ビット版の変換処理を用意し、実行中の環境に合わせて呼び出すことにした。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【アドレス範囲内チェック】
        /// メモリ上のアドレスを指すIntPtr値が、最小アドレスから最大アドレスの範囲内にあるかどうかをチェックします。
        /// </summary>
        /// <param name="address">   [in ]：対象IntPtr値</param>
        /// <param name="minAddress">[in ]：最小アドレスを指すIntPtr値</param>
        /// <param name="maxAddress">[in ]：最大アドレスを指すIntPtr値</param>
        /// <returns>
        /// チェック結果[true = 範囲内 / false = 範囲外]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static bool XIsInAddressRange(this IntPtr address, IntPtr minAddress, IntPtr maxAddress)
        {
            //------------------------------------------------------------
            /// アドレスが最小から最大の範囲内にあるかどうかをチェックする
            //------------------------------------------------------------
            UIntPtr value = address.XToAddressPtr();                    //// 各IntPtr値からそれぞれにUIntPtr値を取得する
            UIntPtr minValue = minAddress.XToAddressPtr();
            UIntPtr maxValue = maxAddress.XToAddressPtr();
            return value.XIsInAddressRange(minValue, maxValue);         //// 戻り値 = UIntPtr値としてアドレス範囲内チェックした結果 で関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(XIntPtr) + "." + nameof(XIsInAddressRange))]
        public static void ZZZ_XIsInAddressRange()
        {
            IntPtr minAddress; // 範囲の最小アドレス
            IntPtr maxAddress; // 範囲の最大アドレス
            IntPtr lastAddress;// メモリ空間の最終アドレス

            // 32ビット環境用のテスト
            if (IntPtr.Size == 4)
            {
                minAddress = (IntPtr)0x7FFFFFFF;
                maxAddress = (IntPtr)(-3);  // 0xFFFFFFFD;
                lastAddress = (IntPtr)(-1); // 0xFFFFFFFF;
            }
            // 64ビット環境用のテスト
            else
            {
                minAddress = (IntPtr)0x7FFFFFFFFFFFFFFF;
                maxAddress = (IntPtr)0xFFFFFFFFFFFFFFFD;
                lastAddress = (IntPtr)0xFFFFFFFFFFFFFFFF;
            }

            SubRoutine((IntPtr)0x00000000, false, "メモリ空間の先頭アドレス");
            SubRoutine(minAddress - 1, false, "範囲の直前");
            SubRoutine(minAddress + 0, true, "範囲の最小アドレス");
            SubRoutine(minAddress + 1, true, "範囲の最小アドレス＋１");
            SubRoutine(maxAddress - 1, true, "範囲の最大アドレス－１");
            SubRoutine(maxAddress + 0, true, "範囲の最大アドレス");
            SubRoutine(maxAddress + 1, false, "範囲の直後");
            SubRoutine(lastAddress - 1, false, "メモリ空間の最終アドレス－１");
            SubRoutine(lastAddress + 0, false, "メモリ空間の最終アドレス");

            void SubRoutine(IntPtr value,                           // [in ]：IntPtr値
                            AutoTestResultInfo<bool> expectResult,  // [in ]：予想結果(チェック結果)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                       // テストオプション
                    fncInstanceRegularString = RegularString.ToAddress, // ・対象インスタンスはアドレス文字列表記
                    fncArg1RegularString = RegularString.ToAddress,     // ・引数１(最小アドレス)はアドレス文字列表記
                    fncArg2RegularString = RegularString.ToAddress,     // ・引数２(最大アドレス)はアドレス文字列表記
                };

                AutoTest.TestX(XIsInAddressRange, value, minAddress, maxAddress, expectResult, testOptions);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【アドレス文字列作成】メモリ上のアドレスを表す16進数文字列を作成します。
        /// </summary>
        /// <param name="target">[in ]：対象IntPtr値</param>
        /// <returns>
        /// アドレス文字列
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・32ビット環境では "0x12345678" 形式に、64ビット環境では "0x123456789ABCDEF0" 形式になります。
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XToAddressString(this IntPtr target)
        {
            // ＜メモ＞
            // ・RegularString.ToAddress(IFormattable) を呼び出しても良いが、仕様や書式を別物にする可能性もあるので個別に作っておく。
            //------------------------------------------------------------
            /// メモリ上のアドレスを表す16進数文字列を作成する
            //------------------------------------------------------------
            if (IntPtr.Size == 4)
            {                                                           //// 32ビットプラットフォームの場合
                return "0x" + ((int)target).ToString("X8");             /////  戻り値 = 8桁の16進数文字列 で関数終了(IntPtr は IFormattable I/Fを実装しないので、int経由で作成する)
            }
            else
            {                                                           //// 64ビットプラットフォームの場合
                return "0x" + ((long)target).ToString("X16");           /////  戻り値 = 16桁の16進数文字列 で関数終了(IntPtr は IFormattable I/Fを実装しないので、long経由で作成する)
            }
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(XIntPtr) + "." + nameof(XToAddressString))]
        public static void ZZZ_XToAddressString()
        {
            // 32ビット環境用のテスト
            if (IntPtr.Size == 4)
            {
                SubRoutine((IntPtr)0x00000000, "0x00000000", "32ビットの先頭アドレス");
                SubRoutine((IntPtr)0x00000001, "0x00000001", "32ビットの先頭アドレス＋１");
                SubRoutine((IntPtr)0x12345678, "0x12345678", "32ビットの適当な中間アドレス");
                SubRoutine((IntPtr)0x7fffffff, "0x7FFFFFFF", "32ビットの前半の最終アドレス");
                SubRoutine((IntPtr)int.MinValue, "0x80000000", "32ビットの後半の先頭アドレス");  // 32ビット環境では (IntPtr)0x80000000 だと実行時にオーバーフロー例外が発生する
                SubRoutine((IntPtr)(-2), "0xFFFFFFFE", "32ビットの最終アドレス－１");            // 32ビット環境では (IntPtr)0xfffffffe だと実行時にオーバーフロー例外が発生する
                SubRoutine((IntPtr)(-1), "0xFFFFFFFF", "32ビットの最終アドレス");                // 32ビット環境では (IntPtr)0xffffffff だと実行時にオーバーフロー例外が発生する
            }
            // 64ビット環境用のテスト
            else
            {
                SubRoutine((IntPtr)0x0000000000000000, "0x0000000000000000", "64ビットの先頭アドレス");
                SubRoutine((IntPtr)0x0000000000000001, "0x0000000000000001", "64ビットの先頭アドレス＋１");
                SubRoutine((IntPtr)0x123456789ABCDEF0, "0x123456789ABCDEF0", "64ビットの適当な中間アドレス");
                SubRoutine((IntPtr)0x7FFFFFFFFFFFFFFF, "0x7FFFFFFFFFFFFFFF", "64ビットの前半の最終アドレス");
                SubRoutine((IntPtr)0x8000000000000000, "0x8000000000000000", "64ビットの後半の先頭アドレス");
                SubRoutine((IntPtr)0xFFFFFFFFFFFFFFFE, "0xFFFFFFFFFFFFFFFE", "64ビットの最終アドレス－１");
                SubRoutine((IntPtr)0xFFFFFFFFFFFFFFFF, "0xFFFFFFFFFFFFFFFF", "64ビットの最終アドレス");
            }


            void SubRoutine(IntPtr value,                               // [in ]：IntPtr値
                            AutoTestResultInfo<string> expectResult,    // [in ]：予想結果(アドレス文字列)
                            string testPattern = null)                  // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.TestX(XToAddressString, value, expectResult, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【32ビットアドレス値取得】IntPtr値を32ビットアドレス値(メモリ上の同じアドレスを指すUInt32値)に変換します。
        /// </summary>
        /// <param name="target">[in ]：対象IntPtr値</param>
        /// <returns>
        /// 32ビットアドレス値
        /// </returns>
        /// <exception cref="OverflowException">
        /// 64 ビット プラットフォームでは、このインスタンスの値は大きすぎる、または小さすぎて、
        /// 32 ビット符号付き整数として表すことができません。
        /// </exception>
        /// <remarks>
        /// 補足<br></br>
        /// ・このメソッドはCLSに準拠していません。符号なし整数に対応していない言語からは利用できないものと思われます。<br></br>
        /// ・数値として等価な値ではなく、メモリ上の同じアドレスを指す値を取得します。
        /// <code>
        /// アドレス   / UInt32        / IntPtr
        /// 0x00000000 /             0 /              0
        /// 0x00000001 /             1 /              1
        /// 0x12345678 /   305,419,896 /    305,419,896
        /// 0x7FFFFFFF / 2,147,483,647 /  2,147,483,647
        /// 0x80000000 / 2,147,483,648 / -2,147,483,648 (数値として等価な値は UIntPtr では表現不能)
        /// 0xFFFFFFFE / 4,294,967,294 /             -2 (数値として等価な値は UIntPtr では表現不能)
        /// 0xFFFFFFFF / 4,294,967,295 /             -1 (数値として等価な値は UIntPtr では表現不能)
        /// </code>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static uint XToAddress32(this IntPtr target)
        {
            //------------------------------------------------------------
            /// IntPtr値を32ビットアドレス値に変換する
            //------------------------------------------------------------
            uint uint32Value;   // 32ビットアドレス値

            int int32Value = target.ToInt32();                          //// IntPtr値をInt32値に変換する
            unchecked { uint32Value = (uint)int32Value; }               //// ビット表現が同じ UInt32値に変換する
            return uint32Value;                                         //// 戻り値 = 32ビットアドレス値(UInt32値) で関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(XIntPtr) + "." + nameof(XToAddress32))]
        public static void ZZZ_XToAddress32()
        {
            SubRoutine((IntPtr)0x00000000, 0, "0x00000000：32ビットの先頭アドレス");
            SubRoutine((IntPtr)0x00000001, 1, "0x00000001：32ビットの先頭アドレス＋１");
            SubRoutine((IntPtr)0x12345678, 0x12345678, "0x12345678：32ビットの適当な中間アドレス");
            SubRoutine((IntPtr)0x7FFFFFFF, 0x7FFFFFFF, "0x7FFFFFFF：32ビットの前半の最終アドレス");
            SubRoutine((IntPtr)int.MinValue, 0x80000000, "0x80000000：32ビットの後半の先頭アドレス");  // 32ビット環境では (IntPtr)0x80000000 だと実行時にオーバーフロー例外が発生する
            SubRoutine((IntPtr)(-2), 0xFFFFFFFE, "0xFFFFFFFE：32ビットの最終アドレス－１");            // 32ビット環境では (IntPtr)0xFFFFFFFE だと実行時にオーバーフロー例外が発生する
            SubRoutine((IntPtr)(-1), 0xFFFFFFFF, "0xFFFFFFFF：32ビットの最終アドレス");                // 32ビット環境では (IntPtr)0xFFFFFFFF だと実行時にオーバーフロー例外が発生する

            void SubRoutine(IntPtr value,                           // [in ]：IntPtr値
                            AutoTestResultInfo<uint> expectResult,  // [in ]：予想結果(32ビットアドレス値)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                       // テストオプション
                    fncResultRegularString = RegularString.ToAddress,   // ・戻り値はアドレス文字列表記
                };

                AutoTest.TestX(XToAddress32, value, expectResult, testOptions);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【64ビットアドレス値取得】IntPtr値を64ビットアドレス値(メモリ上の同じアドレスを指すUInt64値)に変換します。
        /// </summary>
        /// <param name="target">[in ]：対象IntPtr値</param>
        /// <returns>
        /// 64ビットアドレス値
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・このメソッドはCLSに準拠していません。符号なし整数に対応していない言語からは利用できないものと思われます。<br></br>
        /// ・数値として等価な値ではなく、メモリ上の同じアドレスを指す値を取得します。
        /// <code>
        /// メモリ上のアドレス / UInt64(符号なし)           / IntPtr(符号付きポインター)
        /// 0x0000000000000000 /                          0 /                          0
        /// 0x0000000000000001 /                          1 /                          1
        /// 0x123456789ABCDEF0 /  1,311,768,467,463,790,320 /  1,311,768,467,463,790,320
        /// 0x7FFFFFFFFFFFFFFF /  9,223,372,036,854,775,807 /  9,223,372,036,854,775,807
        /// 0x8000000000000000 /  9,223,372,036,854,775,808 / -9,223,372,036,854,775,808 (数値として等価な値は UIntPtr では表現不能)
        /// 0xFFFFFFFFFFFFFFFE / 18,446,744,073,709,551,614 /                         -2 (数値として等価な値は UIntPtr では表現不能)
        /// 0xFFFFFFFFFFFFFFFF / 18,446,744,073,709,551,615 /                         -1 (数値として等価な値は UIntPtr では表現不能)
        /// </code>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static ulong XToAddress64(this IntPtr target)
        {
            //------------------------------------------------------------
            /// IntPtr値を64ビットアドレス値に変換する
            //------------------------------------------------------------
            ulong uint64Value;  // 64ビットアドレス値

            var int64Value = target.ToInt64();                          //// IntPtr値をInt64値に変換する
            unchecked { uint64Value = (ulong)int64Value; }              //// ビット表現が同じUInt64値に変換する
            return uint64Value;                                         //// 戻り値 = 64ビットアドレス値(UInt64値) で関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(XIntPtr) + "." + nameof(XToAddress64))]
        public static void ZZZ_XToAddress64()
        {
            SubRoutine((IntPtr)0x00000000, 0, "0x00000000：32ビットの先頭アドレス");
            SubRoutine((IntPtr)0x00000001, 1, "0x00000001：32ビットの先頭アドレス＋１");
            SubRoutine((IntPtr)0x12345678, 0x12345678, "0x12345678：32ビットの適当な中間アドレス");
            SubRoutine((IntPtr)0x7FFFFFFF, 0x7FFFFFFF, "0x7FFFFFFF：32ビットの前半の最終アドレス");

            // 32ビット環境で正しく扱える64ビットアドレス値は 0x00000000～0x7fffffff の範囲のみ
            if (IntPtr.Size != 4)
            {
                SubRoutine((IntPtr)0x80000000, 0x80000000, "0x0000000080000000：32ビットの後半の先頭アドレス");
                SubRoutine((IntPtr)0xFFFFFFFE, 0xFFFFFFFE, "0x00000000FFFFFFFE：32ビットの最終アドレス－１");
                SubRoutine((IntPtr)0xFFFFFFFF, 0xFFFFFFFF, "0x00000000FFFFFFFF：32ビットの最終アドレス");

                SubRoutine((IntPtr)0x123456789ABCDEF0, 0x123456789ABCDEF0, "0x123456789ABCDEF0：64ビットの適当な中間アドレス");
                SubRoutine((IntPtr)0x7FFFFFFFFFFFFFFF, 0x7FFFFFFFFFFFFFFF, "0x7FFFFFFFFFFFFFFF：64ビットの前半の最終アドレス");
                SubRoutine((IntPtr)0x8000000000000000, 0x8000000000000000, "0x8000000000000000：64ビットの後半の先頭アドレス");
                SubRoutine((IntPtr)0xFFFFFFFFFFFFFFFE, 0xFFFFFFFFFFFFFFFE, "0xFFFFFFFFFFFFFFFE：64ビットの最終アドレス－１");
                SubRoutine((IntPtr)0xFFFFFFFFFFFFFFFF, 0xFFFFFFFFFFFFFFFF, "0xFFFFFFFFFFFFFFFF：64ビットの最終アドレス");
            }

            void SubRoutine(IntPtr value,                           // [in ]：IntPtr値
                            AutoTestResultInfo<ulong> expectResult, // [in ]：予想結果(64ビットアドレス値)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                       // テストオプション
                    fncResultRegularString = RegularString.ToAddress,   // ・戻り値はアドレス文字列表記
                };

                AutoTest.TestX(XToAddress64, value, expectResult, testOptions);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【プラットフォーム固有のアドレス値取得】
        /// IntPtr値をプラットフォーム固有のアドレス値(メモリ上の同じアドレスを指すUIntPtr値)に変換します。
        /// </summary>
        /// <param name="target">[in ]：対象IntPtr値</param>
        /// <returns>
        /// プラットフォーム固有のアドレス値<br></br>
        /// 32ビット環境では <see cref="XToAddress32(IntPtr)"/> の結果に、<br></br>
        /// 64ビット環境では <see cref="XToAddress64(IntPtr)"/> の結果になります。
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・このメソッドはCLSに準拠していません。符号なし整数に対応していない言語からは利用できないものと思われます。<br></br>
        /// ・数値として等価な値ではなく、メモリ上の同じアドレスを指す値を取得します。
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static UIntPtr XToAddressPtr(this IntPtr target)
        {
            //------------------------------------------------------------
            /// IntPtr値をプラットフォーム固有のアドレス値に変換する
            //------------------------------------------------------------
            if (IntPtr.Size == 4)
            {                                                           //// 32ビットプラットフォームの場合
                return (UIntPtr)target.XToAddress32();
            }
            else
            {                                                           //// 64ビットプラットフォームの場合
                return (UIntPtr)target.XToAddress64();
            }
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(XIntPtr) + "." + nameof(XToAddressPtr))]
        public static void ZZZ_XToAddressPtr()
        {
            // 32ビット環境用のテスト
            if (IntPtr.Size == 4)
            {
                SubRoutine((IntPtr)0x00000000, (UIntPtr)0, "0x00000000：32ビットの先頭アドレス");
                SubRoutine((IntPtr)0x00000001, (UIntPtr)1, "0x00000001：32ビットの先頭アドレス＋１");
                SubRoutine((IntPtr)0x12345678, (UIntPtr)0x12345678, "0x12345678：32ビットの適当な中間アドレス");
                SubRoutine((IntPtr)0x7FFFFFFF, (UIntPtr)0x7FFFFFFF, "0x7FFFFFFF：32ビットの前半の最終アドレス");
                SubRoutine((IntPtr)int.MinValue, (UIntPtr)0x80000000, "0x80000000：32ビットの後半の先頭アドレス");  // 32ビット環境では (IntPtr)0x80000000 だと実行時にオーバーフロー例外が発生する
                SubRoutine((IntPtr)(-2), (UIntPtr)0xFFFFFFFE, "0xFFFFFFFE：32ビットの最終アドレス－１");            // 32ビット環境では (IntPtr)0xFFFFFFFE だと実行時にオーバーフロー例外が発生する
                SubRoutine((IntPtr)(-1), (UIntPtr)0xFFFFFFFF, "0xFFFFFFFF：32ビットの最終アドレス");                // 32ビット環境では (IntPtr)0xFFFFFFFF だと実行時にオーバーフロー例外が発生する
            }
            // 64ビット環境用のテスト
            else
            {
                SubRoutine((IntPtr)0x0000000000000000, (UIntPtr)0, "0x0000000000000000：64ビットの先頭アドレス");
                SubRoutine((IntPtr)0x0000000000000001, (UIntPtr)1, "0x0000000000000001：64ビットの先頭アドレス＋１");
                SubRoutine((IntPtr)0x123456789ABCDEF0, (UIntPtr)0x123456789ABCDEF0, "0x123456789ABCDEF0：64ビットの適当な中間アドレス");
                SubRoutine((IntPtr)0x7FFFFFFFFFFFFFFF, (UIntPtr)0x7FFFFFFFFFFFFFFF, "0x7FFFFFFFFFFFFFFF：64ビットの前半の最終アドレス");
                SubRoutine((IntPtr)0x8000000000000000, (UIntPtr)0x8000000000000000, "0x8000000000000000：64ビットの後半の先頭アドレス");
                SubRoutine((IntPtr)0xFFFFFFFFFFFFFFFE, (UIntPtr)0xFFFFFFFFFFFFFFFE, "0xFFFFFFFFFFFFFFFE：64ビットの最終アドレス－１");
                SubRoutine((IntPtr)0xFFFFFFFFFFFFFFFF, (UIntPtr)0xFFFFFFFFFFFFFFFF, "0xFFFFFFFFFFFFFFFF：64ビットの最終アドレス");
            }


            void SubRoutine(IntPtr value,                               // [in ]：IntPtr値
                            AutoTestResultInfo<UIntPtr> expectResult,   // [in ]：予想結果(32ビットアドレス値)
                            string testPattern = null)                  // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                       // テストオプション
                    fncResultRegularString = RegularString.ToAddress,   // ・戻り値はアドレス文字列表記
                };

                AutoTest.TestX(XToAddressPtr, value, expectResult, testOptions);
            }
        }
#endif

    } // class


    //====================================================================================================
    /// <summary>
    /// 【UIntPtr拡張メソッド】UIntPtr型に拡張メソッドを追加します。
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・このクラスはCLSに準拠していません。符号なし整数に対応していない言語からは利用できないものと思われます。<br></br>
    /// </remarks>
    //====================================================================================================
    public static partial class XUIntPtr
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【アドレス範囲内チェック】
        /// メモリ上のアドレスを指すUIntPtr値が、最小アドレスから最大アドレスの範囲内にあるかどうかをチェックします。
        /// </summary>
        /// <param name="address">   [in ]：対象UIntPtr値</param>
        /// <param name="minAddress">[in ]：最小アドレスを指すUIntPtr値</param>
        /// <param name="maxAddress">[in ]：最大アドレスを指すUIntPtr値</param>
        /// <returns>
        /// チェック結果[true = 範囲内 / false = 範囲外]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static bool XIsInAddressRange(this UIntPtr address, UIntPtr minAddress, UIntPtr maxAddress)
        {
            //------------------------------------------------------------
            /// アドレスが最小から最大の範囲内にあるかどうかをチェックする
            //------------------------------------------------------------
            if (IntPtr.Size == 4)
            {                                                           //// 32ビットプラットフォームの場合
                uint value = address.ToUInt32();                        /////  各UIntPtr値からそれぞれにUInt32値を取得する
                uint minValue = minAddress.ToUInt32();
                uint maxValue = maxAddress.ToUInt32();
                return                                                  /////  戻り値 = UInt32値として範囲内チェックした結果 で関数終了
                    NLMath<uint>.IsInRange(value, minValue, maxValue);
            }
            else
            {                                                           //// 64ビットプラットフォームの場合
                ulong value = address.ToUInt64();                       /////  各UIntPtr値からそれぞれにUInt64値を取得する
                ulong minValue = minAddress.ToUInt64();
                ulong maxValue = maxAddress.ToUInt64();
                return                                                  /////  戻り値 = UInt64値として範囲内チェックした結果 で関数終了
                    NLMath<ulong>.IsInRange(value, minValue, maxValue);
            }
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(XUIntPtr) + "." + nameof(XIsInAddressRange))]
        public static void ZZZ_XIsInAddressRange()
        {
            UIntPtr minAddress; // 範囲の最小アドレス
            UIntPtr maxAddress; // 範囲の最大アドレス
            UIntPtr lastAddress;// メモリ空間の最終アドレス

            // 32ビット環境用のテスト
            if (IntPtr.Size == 4)
            {
                minAddress = (UIntPtr)0x7FFFFFFF;
                maxAddress = (UIntPtr)0xFFFFFFFD;
                lastAddress = (UIntPtr)0xFFFFFFFF;
            }
            // 64ビット環境用のテスト
            else
            {
                minAddress = (UIntPtr)0x7FFFFFFFFFFFFFFF;
                maxAddress = (UIntPtr)0xFFFFFFFFFFFFFFFD;
                lastAddress = (UIntPtr)0xFFFFFFFFFFFFFFFF;
            }

            SubRoutine((UIntPtr)0x00000000, false, "メモリ空間の先頭アドレス");
            SubRoutine(minAddress - 1, false, "範囲の直前");
            SubRoutine(minAddress + 0, true, "範囲の最小アドレス");
            SubRoutine(minAddress + 1, true, "範囲の最小アドレス＋１");
            SubRoutine(maxAddress - 1, true, "範囲の最大アドレス－１");
            SubRoutine(maxAddress + 0, true, "範囲の最大アドレス");
            SubRoutine(maxAddress + 1, false, "範囲の直後");
            SubRoutine(lastAddress - 1, false, "メモリ空間の最終アドレス－１");
            SubRoutine(lastAddress + 0, false, "メモリ空間の最終アドレス");

            void SubRoutine(UIntPtr value,                          // [in ]：UIntPtr値
                            AutoTestResultInfo<bool> expectResult,  // [in ]：予想結果(チェック結果)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                       // テストオプション
                    fncInstanceRegularString = RegularString.ToAddress, // ・対象インスタンスはアドレス文字列表記
                    fncArg1RegularString = RegularString.ToAddress,     // ・引数１(最小アドレス)はアドレス文字列表記
                    fncArg2RegularString = RegularString.ToAddress,     // ・引数２(最大アドレス)はアドレス文字列表記
                };

                AutoTest.TestX(XIsInAddressRange, value, minAddress, maxAddress, expectResult, testOptions);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【最終アドレス値取得】最終アドレス値(メモリ空間の最大アドレスを示す値。UIntPtr型の最大値)を取得します。<br></br>
        /// 32ビット環境では 0xFFFFFFFF で、64ビット環境では 0xFFFFFFFFFFFFFFFF です。
        /// </summary>
        /// <returns>
        /// 最大アドレス値
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・IntPtr型の場合は (IntPtr)(-1) で取得できます。
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static UIntPtr XGetLastAddressValue()
        {
            //------------------------------------------------------------
            /// 最終アドレス値を取得する
            //------------------------------------------------------------
            if (IntPtr.Size == 4)
            {                                                           //// 32ビットプラットフォームの場合
                return (UIntPtr)0xFFFFFFFF;                             /////  戻り値 = 0xFFFFFFFF で関数終了
            }
            else
            {                                                           //// 64ビットプラットフォームの場合
                return (UIntPtr)0xFFFFFFFFFFFFFFFF;                     /////  戻り値 = 0xFFFFFFFFFFFFFFFF で関数終了
            }
        }


        // ＜メモ＞
        // ・IntPtr.ToString には書式指定版があるのに UIntPtr.ToString にはなかった。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【書式指定付き文字列形式作成】このインスタンスの内容を表す文字列を取得します。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <param name="format">[in ]：書式指定</param>
        /// <returns>文字列形式</returns>
        //--------------------------------------------------------------------------------
        public static string XToString(this UIntPtr target, string format)
        {
            //------------------------------------------------------------
            /// このインスタンスの内容を表す文字列を取得する
            //------------------------------------------------------------
            if (IntPtr.Size == 4)
            {                                                           //// 32ビットプラットフォームの場合
                return ((uint)target).ToString(format);                 /////  UInt32に変換してから文字列形式を作成して戻り値とし、関数終了
            }
            else
            {                                                           //// 64ビットプラットフォームの場合
                return ((ulong)target).ToString(format);                /////  UInt64に変換してから文字列形式を作成して戻り値とし、関数終了
            }
        }

    } // class



    // 手動テスト用のクラス
#if DEBUG
    //====================================================================================================
    // XIntPtr の手動テスト用クラス
    //====================================================================================================
    public class ZZZTest_XIntPtr
    {
        [ManualTestMethod("XIntPtr / XUIntPtr の総合的テスト")]
        public static void ZZZ_OverallTest()
        {
            IntPtr adr;

            // 32ビット環境用のテスト
            if (IntPtr.Size == 4)
            {
                Debug.Print("アドレス   / アドレス値(UIntPtr) / IntPtr値");

                adr = (IntPtr)0x00000000;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));

                adr = (IntPtr)0x12345678;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));

                adr = (IntPtr)0x7fffffff;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));

                adr = (IntPtr)(-2); // 0xfffffffe;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));
            }
            // 64ビット環境用のテスト
            else
            {
                Debug.Print("アドレス           / アドレス値(UIntPtr) / IntPtr値");

                adr = (IntPtr)0x00000000;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));

                adr = (IntPtr)0x123456789abcdef0;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));

                adr = (IntPtr)0x7fffffffffffffff;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));

                adr = (IntPtr)0xfffffffffffffffe;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));
                adr = adr + 1;
                Debug.Print(AddressString(adr));
            }


            // アドレスを 16進数表記 / 符号なし / 符号あり で文字列を作成する
            string AddressString(IntPtr address)
            {
                return address.XToAddressString() + " / " + address.XToAddressPtr().XToString("N0") + " / " + address.ToString("N0");
            }
        }
    } // class
#endif

} // namespace
