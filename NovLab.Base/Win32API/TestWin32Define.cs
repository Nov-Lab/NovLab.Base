// @(h)TestWin32Define.cs ver 0.00 ( '22.04.28 Nov-Lab ) 作成開始
// @(h)TestWin32Define.cs ver 0.51 ( '22.04.28 Nov-Lab ) ベータ版完成
// @(h)TestWin32Define.cs ver 0.52 ( '22.05.01 Nov-Lab ) 機能修正：既定関数以外のテスト結果出力コールバックを指定できるようにした。DLLが配置されていない場合に対応した。
// @(h)TestWin32Define.cs ver 0.52a( '22.05.24 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【Win32定義テスト】TestWin32Define.dll を使用してWin32API定義が正しいかどうかをテストする機能を提供します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace NovLab.Win32
{
    //====================================================================================================
    /// <summary>
    /// 【Win32定義テスト】TestWin32Define.dll を使用してWin32API定義が正しいかどうかをテストする機能を提供します。
    /// </summary>
    /// <code>
    /// ＜使い方＞
    /// ・bin\Debugフォルダに TestWin32Define.dll と TestWin32Define.pdb を配置しておきます。
    ///   (TestWin32Define リポジトリから入手できます。)
    ///
    /// ・Win32定義テストを行うときは、アプリケーション初期化処理などに以下を追加します。
    ///    #if DEBUG
    ///        NovLab.Win32.TestWin32Define.PrepareTest();
    ///    #endif
    ///
    /// ・DllImportを用いたAPI関数定義部分を、TestWin32Define.dll 内のテスト用エクスポート関数を呼び出すように修正します。
    ///   (テストが終わったら元に戻します。)
    ///   使用例：<see cref="Win32API.CloseHandle(IntPtr)"/>
    /// </code>
    //====================================================================================================
#if DEBUG
    public static class TestWin32Define
    {
        //====================================================================================================
        // 定数定義
        //====================================================================================================
        /// <summary>
        /// 【Win32定義テスト用DLL名】DEBUGビルドでない場合にエラーを発生させるため、DLL名を直接記述せずにこの定義を使います。
        /// </summary>
        public const string DLLNAME = "TestWin32Define";


        //====================================================================================================
        // TestWin32Define.dll のエクスポート関数定義
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果文字列取得】TestWin32Define.dll からテスト結果文字列を取得します。
        /// </summary>
        /// <returns>
        /// テスト結果文字列へのポインタ
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・戻り値は、呼び出し元で解放してはならない文字列へのポインタ であるため、string ではなく IntPtr で受け取り、
        ///   Marshal.PtrToStringAuto で文字列に変換します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [DllImport(DLLNAME, EntryPoint = "GetTestResult", SetLastError = false, CharSet = CharSet.Auto)]
        private static extern IntPtr M_GetTestResult();


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コールバック登録】TestWin32Define.dll にコールバック関数を登録します。
        /// </summary>
        /// <param name="lpfncTestResultOutput">[in ]：テスト結果出力コールバック関数へのポインタ</param>
        //--------------------------------------------------------------------------------
        [DllImport(DLLNAME, EntryPoint = "RegisterCallback", SetLastError = false, CharSet = CharSet.Auto)]
        private static extern void M_RegisterCallback(FNC_TestResultOutput lpfncTestResultOutput);


        //====================================================================================================
        // TestWin32Define.dll から呼び出されるコールバック関数
        // ・TestWin32Define.dll から出力ウィンドウへ直接デバッグ出力することはできないため、コールバック関数を呼び出させて出力します。
        //====================================================================================================

        // ＜メモ＞
        // ・TestWin32Define.dll 側の定義と一致させておくこと。
        /// <summary>
        /// 【テスト結果出力コールバック関数の型定義】
        /// </summary>
        public delegate void FNC_TestResultOutput();


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果出力コールバックの既定関数】
        /// TestWin32Define.dll からコールバックされ、テスト結果文字列を取得してデバッグ出力します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public static void TestResultOutput() => Debug.Print(GetTestResult());


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果文字列取得】TestWin32Define.dll からテスト結果文字列を取得します。
        /// </summary>
        /// <returns>
        /// テスト結果文字列
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string GetTestResult() => Marshal.PtrToStringAuto(M_GetTestResult());


        //====================================================================================================
        // static 公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【Win32定義テスト準備】Win32定義テストの準備をします。
        /// </summary>
        /// <param name="fncTestResultOutput">[in ]：テスト結果出力コールバック関数[null = 既定関数を使う]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・既定関数では、テスト結果は Debug.Print で出力します。
        ///   それ以外の方法で出力したい場合は、引数で独自のコールバック関数を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static void PrepareTest(FNC_TestResultOutput fncTestResultOutput = null)
        {
            //------------------------------------------------------------
            /// Win32定義テストの準備をする
            //------------------------------------------------------------
            if (fncTestResultOutput == null)
            {                                                           //// テスト結果出力コールバック関数 = null(既定関数を使う) の場合
                fncTestResultOutput = TestResultOutput;                 /////  テスト結果出力コールバック関数 = 既定関数
            }

            try
            {                                                           //// try開始
                M_RegisterCallback(fncTestResultOutput);                /////  コールバック登録処理を行う
            }
            catch (System.DllNotFoundException ex)
            {                                                           //// catch：DllNotFoundException
                throw new DllNotFoundException(                         /////  開発者向けのメッセージで例外を再スローする
                    DLLNAME + ".dll が見つかりません。" +
                    "Win32定義テストを行う場合はDLLを配置してください。" +
                    "Win32定義テストを行わない場合は " + nameof(TestWin32Define) + ".PrepareTest の呼び出しをコメントアウトしてください。",
                    ex);
            }
        }
    }
#endif
}
