// @(h)XStringBuilder.cs ver 0.00 ( '22.05.15 Nov-Lab ) 作成開始
// @(h)XStringBuilder.cs ver 0.21 ( '22.05.15 Nov-Lab ) アルファ版完成
// @(h)XStringBuilder.cs ver 0.21a( '24.01.21 Nov-Lab ) 仕変対応：AutoTest クラスの仕様変更に対応した。機能変更なし。

// @(s)
// 　【StringBuilder 拡張メソッド】System.Text.StringBuilder クラスに拡張メソッドを追加します。

#if DEBUG
//#define CTRL_F5 // Ctrl+F5テスト：中断対象例外もテストします。中断対象例外は、例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能です。
#endif

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【StringBuilder 拡張メソッド】System.Text.StringBuilder クラスに拡張メソッドを追加します。
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・対象インスタンスの内容を変更するものなど、StringBuilder で処理したほうがパフォーマンスが良い処理を扱います。
    /// </remarks>
    //====================================================================================================
    public static partial class XStringBuilder
    {
        //====================================================================================================
        // 拡張メソッド(文字列切り出し系)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【左側文字列切り出し】可変型文字列の左側から取得文字数だけ切り出します。切り出した箇所は可変型文字列から削除されます。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <param name="length">[in ]：取得文字数(0以上)</param>
        /// <returns>
        /// 切り出した文字列
        /// </returns>
        /// <remarks>
        /// 備考<br></br>
        /// ・取得文字数が文字列長以上の場合は文字列全体を返します。<br></br>
        /// ・文字列が空文字列の場合は、空文字列を返します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XCutLeft(this StringBuilder target, int length)
        {
            //------------------------------------------------------------
            /// 可変型文字列の左側から指定された文字数を切り出す
            //------------------------------------------------------------
            var cutStr = target.ToString().XLeft(length);               //// 文字列の左側から指定された文字数を取得する
            if (cutStr.Length >= 1)
            {                                                           //// １文字以上取得できた場合
                target.Remove(0, cutStr.Length);                        /////  対象インスタンスの左側から、取得文字数分を削除する
            }
            return cutStr;                                              //// 戻り値 = 取得文字列 で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【右側文字列切り出し】可変型文字列の右側から取得文字数だけ切り出します。切り出した箇所は可変型文字列から削除されます。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <param name="length">[in ]：取得文字数(0以上)</param>
        /// <returns>
        /// 切り出した文字列
        /// </returns>
        /// <remarks>
        /// 備考<br></br>
        /// ・取得文字数が文字列長以上の場合は文字列全体を返します。<br></br>
        /// ・文字列が空文字列の場合は、空文字列を返します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XCutRight(this StringBuilder target, int length)
        {
            //------------------------------------------------------------
            /// 可変型文字列の右側から指定された文字数を切り出す
            //------------------------------------------------------------
            var cutStr = target.ToString().XRight(length);              //// 文字列の右側から指定された文字数を取得する
            if (cutStr.Length >= 1)
            {                                                           //// １文字以上取得できた場合
                target.Remove(target.Length - cutStr.Length,            /////  対象インスタンスの右側から、取得文字数分を削除する
                              cutStr.Length);
            }
            return cutStr;                                              //// 戻り値 = 取得文字列 で関数終了
        }


        //--------------------------------------------------------------------------------
        // テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod("XStringBuilder.XCutLeft/XCutRight")]
        public static void ZZZ_XCutLeft_XCutRight()
        {
            var testStr = new StringBuilder("AB文字CD");
            SubRoutineL(0, "");
            SubRoutineL(1, "A");
            SubRoutineL(2, "B文");
            SubRoutineL(4, "字CD", "取得文字数が残存文字列の長さを超える");
            SubRoutineL(1, "", "残存文字列なし");
            SubRoutineL(-1, typeof(ArgumentOutOfRangeException), "取得文字数が不正");

            testStr = new StringBuilder("AB文字CD");
            SubRoutineR(0, "");
            SubRoutineR(1, "D");
            SubRoutineR(2, "字C");
            SubRoutineR(4, "AB文", "取得文字数が残存文字列の長さを超える");
            SubRoutineR(1, "", "残存文字列なし");
            SubRoutineR(-1, typeof(ArgumentOutOfRangeException), "取得文字数が不正");

            testStr = new StringBuilder("");
            SubRoutineL(0, "", "空文字列に対する操作");
            SubRoutineL(3, "", "空文字列に対する操作");
            SubRoutineL(999, "", "空文字列に対する操作");
            SubRoutineR(0, "", "空文字列に対する操作");
            SubRoutineR(3, "", "空文字列に対する操作");
            SubRoutineR(999, "", "空文字列に対する操作");

#if CTRL_F5 // 中断対象例外のテスト。例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能
            testStr = null;
            SubRoutineL(3, typeof(NullReferenceException), "null参照に対する操作");
            SubRoutineR(3, typeof(NullReferenceException), "null参照に対する操作");
#endif

            void SubRoutineL(int length,                                // [in ]：文字数
                             AutoTestResultInfo<string> expectResult,   // [in ]：予想結果(文字列 または 例外の型情報)
                             string testPattern = null)                 // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.TestX(XCutLeft, testStr, length, expectResult, testPattern);
            }

            void SubRoutineR(int length,                                // [in ]：文字数
                             AutoTestResultInfo<string> expectResult,   // [in ]：予想結果(文字列 または 例外の型情報)
                             string testPattern = null)                 // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.TestX(XCutRight, testStr, length, expectResult, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【トークン切り出し】可変型文字列からトークンを切り出します。切り出した箇所は可変型文字列から削除されます。
        /// </summary>
        /// <param name="target">   [in ]：対象インスタンス</param>
        /// <param name="separator">[in ]：区切り文字列</param>
        /// <returns>
        /// トークン文字列[null = 取得失敗(対象インスタンスが空っぽ)]
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・区切り文字列が見つからない場合は残存文字列全体をトークンとして切り出し、可変型文字列はクリアします。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XCutToken(this StringBuilder target, string separator)
        {
            int findIndex;  // 検索文字列発見位置

            //------------------------------------------------------------
            /// エラーチェック
            //------------------------------------------------------------
            if (separator == null) throw new ArgumentNullException();   //// 区切り文字列 = null の場合、引数null例外をスローする
            if (separator == "") throw new ArgumentException();         //// 区切り文字列 = 空文字列 の場合、引数不正例外をスローする

            //------------------------------------------------------------
            /// 可変型文字列からトークンを切り出す
            //------------------------------------------------------------
            if (target.Length == 0)
            {                                                           //// 文字列が空っぽの場合
                return null;                                            /////  戻り値 = null(取得失敗) で関数終了
            }

            findIndex = target.ToString().IndexOf(separator);           //// 区切り文字列を検索する
            if (findIndex == -1)
            {                                                           //// 見つからなかった場合
                var remainString = target.ToString();                   /////  残存文字列を取得する
                target.Clear();                                         /////  対象インスタンスをクリアする
                return remainString;                                    /////  戻り値 = 残存文字列 で関数終了
            }

            var tokenString = target.XCutLeft(findIndex);               //// 区切り文字列の手前までを切り出してトークン文字列とする
            target.Remove(0, separator.Length);                         //// 対象インスタンスの左側から、区切り文字列を削除する
            return tokenString;                                         //// 戻り値 = トークン文字列 で関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod]
        public static void ZZZ_XCutToken()
        {
            var testStr = new StringBuilder("apple, スモモ,,もも肉, ,モカ");
            SubRoutine(",", "apple", "カンマ区切り - 1回目");
            SubRoutine(",", " スモモ", "カンマ区切り - 2回目");
            SubRoutine(",", "", "カンマ区切り - 3回目(空トークン)");
            SubRoutine(",", "もも肉", "カンマ区切り - 4回目");
            SubRoutine(",", " ", "カンマ区切り - 5回目");
            SubRoutine(",", "モカ", "カンマ区切り - 6回目");
            SubRoutine(",", (string)null, "カンマ区切り - 残存文字列なし");

            testStr = new StringBuilder("apple and スモモ andand もも肉 and and モカ");
            SubRoutine("and", "apple ", "and区切り - 1回目");
            SubRoutine("and", " スモモ ", "and区切り - 2回目");
            SubRoutine("and", "", "and区切り - 3回目(空トークン)");
            SubRoutine("and", " もも肉 ", "and区切り - 4回目");
            SubRoutine("and", " ", "and区切り - 5回目");
            SubRoutine("and", " モカ", "and区切り - 6回目");
            SubRoutine("and", (string)null, "and区切り - 残存文字列なし");

            testStr = new StringBuilder(""); SubRoutine(",", (string)null, "空文字列に対する操作");
            testStr = new StringBuilder(""); SubRoutine("and", (string)null, "空文字列に対する操作");

            testStr = new StringBuilder("apple and スモモ andand もも肉 and and モカ");
            SubRoutine(null, typeof(ArgumentNullException), "区切り文字列 = null");
            SubRoutine("", typeof(ArgumentException), "区切り文字列 = 空文字列");

#if CTRL_F5 // 中断対象例外のテスト。例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能
            testStr = null; SubRoutine(",", typeof(NullReferenceException), "null参照に対する操作");
#endif

            void SubRoutine(string separator,                           // [in ]：区切り文字列
                            AutoTestResultInfo<string> expectResult,    // [in ]：予想結果(文字列 または 例外の型情報)
                            string testPattern = null)                  // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.TestX(XCutToken, testStr, separator, expectResult, testPattern);
            }
        }
#endif

        //--------------------------------------------------------------------------------
        // 手動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [ManualTestMethod]
        public static void ZZZ_XCutTokenManualTest()
        {
            SubRoutine("『古池や 蛙飛びこむ 水の音』  松尾芭蕉 -貞享三年-", " ");
            SubRoutine("<key> = <value>", "=");
            SubRoutine("Information, Warning, Error,Exception", ",");

            void SubRoutine(string text, string separator)
            {
                Debug.Print("●分割前文字列：" + text);
                var testStr = new StringBuilder(text);
                string token;
                do
                {
                    token = testStr.XCutToken(separator);
                    if (token != null) Debug.Print(token.Trim());
                } while (token != null);
            }
        }
#endif

    } // class

} // namespace
