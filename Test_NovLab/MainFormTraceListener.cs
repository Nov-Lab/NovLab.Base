// @(h)MainFormTraceListener.cs ver 0.00 ( '22.05.14 Nov-Lab ) 作成開始
// @(h)MainFormTraceListener.cs ver 0.51 ( '22.05.14 Nov-Lab ) ベータ版完成
// @(h)MainFormTraceListener.cs ver 0.51a( '22.05.25 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【メイン画面出力トレースリスナー】デバッグ出力やトレース出力をメイン画面へ出力する機能を提供します。

using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace Test_NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【メイン画面出力トレースリスナー】デバッグ出力やトレース出力をメイン画面へ出力する機能を提供します。
    /// </summary>
    /// <remarks>
    /// ・自動化に対応していないテスト用メソッドのデバッグ出力などをメイン画面へ出力するために使用します。<br></br>
    /// </remarks>
    //====================================================================================================
    public class MainFormTraceListener : TraceListener
    {
        /// <summary>
        /// 【出力先メイン画面】
        /// </summary>
        protected readonly FrmAppTestNovLab m_frmApp;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メイン画面出力トレースリスナー】デバッグ出力やトレース出力をメイン画面へ出力する機能を提供します。
        /// </summary>
        /// <param name="frmApp">[in ]：Test for NovLab メイン画面</param>
        /// <remarks>
        /// ・自動化に対応していないテスト用メソッドのデバッグ出力などをメイン画面へ出力するために使用します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public MainFormTraceListener(FrmAppTestNovLab frmApp)
        {
            m_frmApp = frmApp;
        }

        public override void Write(string message)
        {
            m_frmApp.AppendTestResult(M_IndentString() + message);  /// テスト結果追加処理を行う
        }

        public override void WriteLine(string message)
        {
            m_frmApp.AppendTestResult(M_IndentString() + message);  /// テスト結果追加処理を行う
        }

        protected string M_IndentString() => new string(' ', IndentSize * IndentLevel);
    }

}
