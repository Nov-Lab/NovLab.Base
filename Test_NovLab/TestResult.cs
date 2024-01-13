// @(h)TestResult.cs ver 0.00 ( '22.05.14 Nov-Lab ) 作成開始
// @(h)TestResult.cs ver 0.21 ( '22.05.14 Nov-Lab ) アルファ版完成
// @(h)TestResult.cs ver 0.22 ( '22.05.15 Nov-Lab ) 機能修正：テストパターン名を追加した。
// @(h)TestResult.cs ver 0.23 ( '22.05.24 Nov-Lab ) 機能修正：例外メッセージを追加した。
// @(h)TestResult.cs ver 0.24 ( '24.01.16 Nov-Lab ) 機能修正：実行前のインスタンス内容と実行後のインスタンス内容を追加した。
// @(h)TestResult.cs ver 0.25 ( '24.01.17 Nov-Lab ) 機能修正：実行結果と予想結果は文字列で扱うようにした。

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
    public partial class TestResult
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
        /// 【実行結果文字列】戻り値 または 例外の型情報。メソッドの実行結果
        /// </summary>
        public string execResult;

        /// <summary>
        /// 【予想結果文字列】戻り値 または 例外の型情報。成功時に返されるべき結果。
        /// </summary>
        public string expectResult;

        /// <summary>
        /// 【例外メッセージ】
        /// </summary>
        public string exceptionMessage;

        /// <summary>
        /// 【実行前のインスタンス内容文字列】
        /// </summary>
        public string befContent;

        /// <summary>
        /// 【実行後のインスタンス内容文字列】
        /// </summary>
        public string aftContent;


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
                autoTestResult + "：" + testDescription + " = " + execResult);

            if (exceptionMessage.XIsValid())
            {                                                           //// 例外メッセージが設定されている場合
                workStr.Append(" (" + exceptionMessage + ")");          /////  文字列形式に例外メッセージを追加する
            }

            if (autoTestResult != AutoTestResultKind.Succeeded)
            {                                                           //// テスト結果が成功でない場合
                workStr.Append(" ≠ " +                                 /////  文字列形式に予想結果を追加する
                               expectResult);
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
                "実行結果  ：" + execResult + exceptionStr + "\r\n" +
                "予想結果  ：" + expectResult);

            if( befContent.XIsValid())
            {                                                           //// 実行前のインスタンス内容文字列に有効文字列が設定されている場合
                workStr.Append("\r\n実行前内容：" + befContent);        /////  詳細文字列に実行前のインスタンス内容文字列を追加する
            }
            if (aftContent.XIsValid())
            {                                                           //// 実行後のインスタンス内容文字列に有効文字列が設定されている場合
                workStr.Append("\r\n実行後内容：" + aftContent);        /////  詳細文字列に実行後のインスタンス内容文字列を追加する
            }

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
        /// <param name="execResult">      [in ]：実行結果文字列(戻り値 または 例外の型情報)</param>
        /// <param name="expectResult">    [in ]：予想結果文字列(戻り値 または 例外の型情報)</param>
        /// <param name="exceptionMessage">[in ]：例外メッセージ</param>
        /// <param name="befContent">      [in ]：実行前のインスタンス内容文字列(null = 静的メソッド)</param>
        /// <param name="aftContent">      [in ]：実行後のインスタンス内容文字列(null = 静的メソッド)</param>
        //--------------------------------------------------------------------------------
        public TestResult(AutoTestResultKind autoTestResult,
                          string testDescription, string testPattern,
                          string execResult, string expectResult,
                          string exceptionMessage,
                          string befContent, string aftContent)
        {
            this.autoTestResult = autoTestResult;
            this.testDescription = testDescription;
            this.testPattern = testPattern;
            this.execResult = execResult;
            this.expectResult = expectResult;
            this.exceptionMessage = exceptionMessage;
            this.befContent = befContent;
            this.aftContent = aftContent;
        }

    } // class
#endif

} // namespace
