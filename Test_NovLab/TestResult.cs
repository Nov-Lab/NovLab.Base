// @(h)TestResult.cs ver 0.00 ( '22.05.14 Nov-Lab ) 作成開始
// @(h)TestResult.cs ver 0.21 ( '22.05.14 Nov-Lab ) アルファ版完成
// @(h)TestResult.cs ver 0.22 ( '22.05.15 Nov-Lab ) 機能修正：テストパターン名を追加した。
// @(h)TestResult.cs ver 0.23 ( '22.05.24 Nov-Lab ) 機能修正：例外メッセージを追加した。

// @(s)
// 　【テスト結果情報】自動テストのテスト結果を管理します。リストボックス項目として使用します。

using System;
using System.Text;
using System.Collections.Generic;

using NovLab;
using NovLab.DebugSupport;


namespace Test_NovLab
{

#if DEBUG   // DEBUGビルドのみ有効
    //====================================================================================================
    /// <summary>
    /// 【テスト結果情報】自動テストのテスト結果を管理します。リストボックス項目として使用します。
    /// </summary>
    //====================================================================================================
    public class TestResult
    {
        //====================================================================================================
        // 公開フィールド
        //====================================================================================================
        /// <summary>
        /// 【自動テスト結果種別】
        /// </summary>
        public AutoTestResultKind autoTestResult;

        /// <summary>
        /// 【テスト内容文字列】
        /// </summary>
        public string testDescription;

        /// <summary>
        /// 【テストパターン名】null = 省略
        /// </summary>
        public string testPattern;

        /// <summary>
        /// 【実行結果】戻り値 または 例外の型情報。メソッドの実行結果
        /// </summary>
        public object execResult;

        /// <summary>
        /// 【予想結果】戻り値 または 例外の型情報。成功時に返されるべき結果。
        /// </summary>
        public object expectResult;

        /// <summary>
        /// 【例外メッセージ】
        /// </summary>
        public string exceptionMessage;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列化】このインスタンスの内容を文字列形式に変換します。リストボックスでの一行表示に使用します。
        /// </summary>
        /// <returns>文字列形式</returns>
        //--------------------------------------------------------------------------------
        public override string ToString()
        {
            //------------------------------------------------------------
            /// このインスタンスの内容を文字列形式に変換する
            //------------------------------------------------------------
            var workStr = new StringBuilder(                            //// 文字列形式の必須部分を作成する
                autoTestResult + "：" + testDescription + " = " + AutoTest.ToDisplayString(execResult));

            if (exceptionMessage.XIsValid())
            {                                                           //// 例外メッセージが設定されている場合
                workStr.Append(" (" + exceptionMessage + ")");          /////  文字列形式に例外メッセージを追加する
            }

            if (autoTestResult != AutoTestResultKind.Succeeded)
            {                                                           //// テスト結果が成功でない場合
                workStr.Append(" ≠ " +                                 /////  文字列形式に予想結果を追加する
                               AutoTest.ToDisplayString(expectResult));
            }

            return workStr.ToString();                                  //// 戻り値 = 文字列形式 で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【詳細文字列化】テスト結果テキストボックスに表示するための詳細文字列を作成します。
        /// </summary>
        /// <returns>詳細文字列</returns>
        //--------------------------------------------------------------------------------
        public string ToDetailString()
        {
            //------------------------------------------------------------
            /// テスト結果テキストボックスに表示するための詳細文字列を作成する
            //------------------------------------------------------------
            string exceptionStr = "";                                   //// 例外文字列 = 空文字列 に初期化する

            if (exceptionMessage.XIsValid())
            {                                                           //// 例外メッセージが設定されている場合
                exceptionStr =                                          /////  例外文字列を作成する
                    " (" + exceptionMessage.XRemoveNewLineChars() + ")";
            }

            var workStr = new StringBuilder(                            //// 詳細文字列の必須部分を作成する
                "テスト結果：" + autoTestResult.ToString() + "\r\n" +
                "テスト内容：" + testDescription + "\r\n" +
                "実行結果  ：" + AutoTest.ToDisplayString(execResult) + exceptionStr + "\r\n" +
                "予想結果  ：" + AutoTest.ToDisplayString(expectResult));


            if (testPattern.XIsValid())
            {                                                           //// テストパターン名に有効文字列が設定されている場合
                workStr.Append("\r\nパターン名：" + testPattern);       /////  詳細文字列にテストパターン名を追加する
            }

            return workStr.ToString();                                  //// 戻り値 = 詳細文字列 で関数終了
        }


        //====================================================================================================
        // コンストラクター
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【完全コンストラクター】すべての内容を指定してテスト結果情報を生成します。
        /// </summary>
        /// <param name="autoTestResult">  [in ]：自動テスト結果種別</param>
        /// <param name="testDescription"> [in ]：テスト内容文字列</param>
        /// <param name="testPattern">     [in ]：テストパターン名[null = 省略]</param>
        /// <param name="execResult">      [in ]：実行結果(戻り値 または 例外の型情報)</param>
        /// <param name="expectResult">    [in ]：予想結果(戻り値 または 例外の型情報)</param>
        /// <param name="exceptionMessage">[in ]：例外メッセージ</param>
        //--------------------------------------------------------------------------------
        public TestResult(AutoTestResultKind autoTestResult,
                          string testDescription, string testPattern,
                          object execResult, object expectResult,
                          string exceptionMessage)
        {
            this.autoTestResult = autoTestResult;
            this.testDescription = testDescription;
            this.testPattern = testPattern;
            this.execResult = execResult;
            this.expectResult = expectResult;
            this.exceptionMessage = exceptionMessage;
        }

    }
#endif

}
