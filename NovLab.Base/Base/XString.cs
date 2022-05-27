// @(h)XString.cs ver 0.00 ( '22.03.27 Nov-Lab ) 既存のソースを元に作成開始
// @(h)XString.cs ver 0.51 ( '22.04.22 Nov-Lab ) ベータ版完成
// @(h)XString.cs ver 0.52 ( '22.05.04 Nov-Lab ) 機能追加：XRemoveEnd を追加した
// @(h)XString.cs ver 0.53 ( '22.05.10 Nov-Lab ) 機能追加：XReplaceNewLineChars と XEscape を追加した
// @(h)XString.cs ver 0.53a( '22.05.14 Nov-Lab ) 微修正  ：一部メソッドを StringBuilder で効率化した。XReplaceNewLineChars を、置換後文字列に CR+LF や CR や LF を指定できるようにして改行文字の統一化に使えるようにした。
// @(h)XString.cs ver 0.53b( '22.05.15 Nov-Lab ) その他  ：テスト用メソッドを追加した
// @(h)XString.cs ver 0.54 ( '22.05.18 Nov-Lab ) 機能追加：XRemoveStart を追加した
// @(h)XString.cs ver 0.54a( '22.05.24 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【string 拡張メソッド】System.String クラスに拡張メソッドを追加します。

#if DEBUG
//#define CTRL_F5 // Ctrl+F5テスト：中断対象例外もテストします。中断対象例外は、例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能です。
#endif

using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Globalization;

using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【string 拡張メソッド】System.String クラスに拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XString
    {
        //====================================================================================================
        // 公開定数
        //====================================================================================================
        /// <summary>
        /// 【改行文字群】改行文字として扱う文字の配列です。CR(キャリッジ リターン文字)とLF(改行。ライン フィード文字)です。
        /// </summary>
        public static char[] NEWLINE_CHARS = new char[] { '\r', '\n' };


        //====================================================================================================
        // 拡張メソッド(削除系、トリム系)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【改行文字トリム】文字列の末尾から改行文字(CR や LF)を削除します。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <returns>
        /// トリム結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XTrimEndNewLineChars(this string target) => target.TrimEnd(NEWLINE_CHARS);


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列削除(序数比較)】文字列から指定された文字列をすべて削除します。
        /// </summary>
        /// <param name="target">      [in ]：対象インスタンス</param>
        /// <param name="removeString">[in ]：削除対象文字列(null と空文字列は不可)</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・削除対象文字列は序数比較で検索します。カルチャに依存せず、大文字と小文字・全角と半角・ひらがなとカタカナを区別します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XRemove(this string target, string removeString)
            => target.Replace(removeString, "");

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(XString) + ".XRemove(string)")]
        public static void ZZZ_XRemove_String(IAutoTestExecuter ifExecuter)
        {
            var testStr = "apple, peach, Apple, Peach, Ａｐｐｌｅ, Ｐｅａｃｈ, スモモ, もも肉, モカ";

            SubRoutine("a", "pple, pech, Apple, Pech, Ａｐｐｌｅ, Ｐｅａｃｈ, スモモ, もも肉, モカ", "正常系1");
            SubRoutine("apple", ", peach, Apple, Peach, Ａｐｐｌｅ, Ｐｅａｃｈ, スモモ, もも肉, モカ", "正常系2");
            SubRoutine("モモ", "apple, peach, Apple, Peach, Ａｐｐｌｅ, Ｐｅａｃｈ, ス, もも肉, モカ", "正常系3");

            SubRoutine(null, typeof(ArgumentNullException), "変換後文字列 = null");
            SubRoutine("", typeof(ArgumentException), "変換後文字列 = 空文字列");

            testStr = ""; SubRoutine("a", "", "空文字列に対する操作");
            testStr = ""; SubRoutine("モモ", "", "空文字列に対する操作");

#if CTRL_F5 // 中断対象例外のテスト。例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能
            testStr = null; SubRoutine("a", typeof(NullReferenceException), "null参照に対する操作");
#endif

            void SubRoutine(string removeString,                    // [in ]：削除対象文字列
                            AutoTestResultInfo<string> expectResult,// [in ]：予想結果(文字列 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.Test(XRemove, testStr, removeString, expectResult, ifExecuter, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字削除(序数比較)】文字列から指定された文字をすべて削除します。
        /// </summary>
        /// <param name="target">    [in ]：対象インスタンス</param>
        /// <param name="removeChar">[in ]：削除対象文字</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// ・削除対象文字列は序数比較で検索します。カルチャに依存せず、大文字と小文字・全角と半角・ひらがなとカタカナを区別します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XRemove(this string target, char removeChar)
            => target.Replace(removeChar.ToString(), "");

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(XString) + ".XRemove(char)")]
        public static void ZZZ_XRemove_Char(IAutoTestExecuter ifExecuter)
        {
            var testStr = "apple, peach, Apple, Peach, Ａｐｐｌｅ, Ｐｅａｃｈ, スモモ, もも肉, モカ";

            SubRoutine('a', "pple, pech, Apple, Pech, Ａｐｐｌｅ, Ｐｅａｃｈ, スモモ, もも肉, モカ", "半角英小文字");
            SubRoutine('Ｐ', "apple, peach, Apple, Peach, Ａｐｐｌｅ, ｅａｃｈ, スモモ, もも肉, モカ", "全角英大文字");
            SubRoutine('モ', "apple, peach, Apple, Peach, Ａｐｐｌｅ, Ｐｅａｃｈ, ス, もも肉, カ", "カタカナ");

            testStr = ""; SubRoutine('a', "", "空文字列に対する操作");

#if CTRL_F5 // 中断対象例外のテスト。例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能
            testStr = null; SubRoutine('a', typeof(NullReferenceException), "null参照に対する操作");
#endif

            void SubRoutine(char removeChar,                        // [in ]：削除対象文字
                            AutoTestResultInfo<string> expectResult,// [in ]：予想結果(文字列 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.Test(XRemove, testStr, removeChar, expectResult, ifExecuter, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字群削除(序数比較)】文字列から配列で指定された文字群をすべて削除します。
        /// </summary>
        /// <param name="target">     [in ]：対象インスタンス</param>
        /// <param name="removeChars">[in ]：削除対象文字群</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// ・削除対象文字列は序数比較で検索します。カルチャに依存せず、大文字と小文字・全角と半角・ひらがなとカタカナを区別します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XRemove(this string target, char[] removeChars)
        {
            //------------------------------------------------------------
            /// エラーチェック
            //------------------------------------------------------------
            if (target == null) throw new NullReferenceException();     //// 対象インスタンス = null の場合、null参照例外をスローする


            //------------------------------------------------------------
            /// 文字列から配列で指定された文字群をすべて削除する
            //------------------------------------------------------------
            var workStr = new StringBuilder(target);                    //// 文字列編集領域 = 対象インスタンスの内容 に初期化する

            foreach (var removeChar in removeChars)
            {                                                           //// 削除対象文字群を繰り返す
                workStr.Replace(removeChar.ToString(), "");             /////  削除対象文字を削除する
            }

            return workStr.ToString();                                  //// 戻り値 = 文字列編集領域 で関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(XString) + ".XRemove(char[])")]
        public static void ZZZ_XRemove_Chars(IAutoTestExecuter ifExecuter)
        {
            var testStr = "apple, peach, Apple, Peach, Ａｐｐｌｅ, Ｐｅａｃｈ, スモモ, もも肉, モカ";

            SubRoutine(new char[] { 'a', 'Ｐ', 'e', 'モ' }, "ppl, pch, Appl, Pch, Ａｐｐｌｅ, ｅａｃｈ, ス, もも肉, カ", "正常系");
            SubRoutine(new char[] { 'x', 'Ｙ', 'z', '文' }, "apple, peach, Apple, Peach, Ａｐｐｌｅ, Ｐｅａｃｈ, スモモ, もも肉, モカ", "削除対象なし");

            testStr = ""; SubRoutine(new char[] { 'a', 'p' }, "", "空文字列に対する操作");

#if CTRL_F5 // 中断対象例外のテスト。例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能
            testStr = null; SubRoutine(new char[] { 'a', 'p' }, typeof(NullReferenceException), "null参照に対する操作");
#endif

            void SubRoutine(char[] removeChars,                     // [in ]：削除対象文字群
                            AutoTestResultInfo<string> expectResult,// [in ]：予想結果(文字列 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.Test(XRemove, testStr, removeChars, expectResult, ifExecuter, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【改行文字群削除】文字列から改行文字群(CR と LF)を削除します。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XRemoveNewLineChars(this string target) => target.XRemove(NEWLINE_CHARS);


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【末尾削除(文字数指定)】文字列の末尾側から指定された文字数を削除します。
        /// </summary>
        /// <param name="target">      [in ]：対象インスタンス</param>
        /// <param name="removeLength">[in ]：削除文字数</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XRemoveEnd(
            this string target,
            int removeLength)
        {
            //------------------------------------------------------------
            /// エラーチェック
            //------------------------------------------------------------
            if (removeLength < 0) throw new ArgumentOutOfRangeException();  //// 削除文字数 < 0 の場合、引数範囲不正例外をスローする


            //------------------------------------------------------------
            /// 文字列の末尾側から指定された文字数を削除する
            //------------------------------------------------------------
            if (removeLength >= target.Length)
            {                                                           //// 削除文字数 >= 文字列の長さ の場合
                return "";                                              /////  戻り値 = 空文字列 で関数終了
            }
            else
            {                                                           //// 削除文字数 < 文字列の長さ の場合
                return target.XLeft(target.Length - removeLength);      /////  戻り値 = 末尾側から指定された文字数を削除した文字列 で関数終了
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【末尾削除(文字列指定)】文字列の末尾が指定された文字列と一致した場合に、その部分を削除します。
        /// </summary>
        /// <param name="target">      [in ]：対象インスタンス</param>
        /// <param name="removeString">[in ]：削除対象文字列</param>
        /// <param name="ignoreCase">  [in ]：大文字小文字無視フラグ</param>
        /// <param name="culture">     [in ]：カルチャ情報[null = 現在のカルチャ]</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・文字列から特定のサフィックスを削除するといった使い方ができます。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XRemoveEnd(
            this string target,
            string removeString,
            bool ignoreCase,
            CultureInfo culture)
        {
            //------------------------------------------------------------
            /// エラーチェック
            //------------------------------------------------------------
            if (removeString == null) throw new ArgumentNullException();    //// 削除対象文字列 = null の場合、引数null例外をスローする


            //------------------------------------------------------------
            /// 文字列の末尾が指定された文字列と一致した場合に、末尾部分を削除する
            //------------------------------------------------------------
            if (target.EndsWith(removeString, ignoreCase, culture))
            {                                                           //// 文字列の末尾が削除対象文字列と一致する場合
                return target.XRemoveEnd(removeString.Length);          /////  戻り値 = 末尾の削除対象文字列を削除した文字列 で関数終了
            }
            else
            {                                                           //// 文字列の末尾が削除対象文字列と一致しない場合
                return target;                                          /////  戻り値 = 文字列全体 で関数終了
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【先頭削除(文字数指定)】文字列の先頭側から指定された文字数を削除します。
        /// </summary>
        /// <param name="target">      [in ]：対象インスタンス</param>
        /// <param name="removeLength">[in ]：削除文字数</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XRemoveStart(
            this string target,
            int removeLength)
        {
            //------------------------------------------------------------
            /// エラーチェック
            //------------------------------------------------------------
            if (removeLength < 0) throw new ArgumentOutOfRangeException();  //// 削除文字数 < 0 の場合、引数範囲不正例外をスローする


            //------------------------------------------------------------
            /// 文字列の末尾側から指定された文字数を削除する
            //------------------------------------------------------------
            if (removeLength >= target.Length)
            {                                                           //// 削除文字数 >= 文字列の長さ の場合
                return "";                                              /////  戻り値 = 空文字列 で関数終了
            }
            else
            {                                                           //// 削除文字数 < 文字列の長さ の場合
                return target.XRight(target.Length - removeLength);     /////  戻り値 = 先頭側から指定された文字数を削除した文字列 で関数終了
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【先頭削除(文字列指定)】文字列の先頭が指定された文字列と一致した場合に、その部分を削除します。
        /// </summary>
        /// <param name="target">      [in ]：対象インスタンス</param>
        /// <param name="removeString">[in ]：削除対象文字列</param>
        /// <param name="ignoreCase">  [in ]：大文字小文字無視フラグ</param>
        /// <param name="culture">     [in ]：カルチャ情報[null = 現在のカルチャ]</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・文字列から特定のプリフィックスを削除するといった使い方ができます。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XRemoveStart(
            this string target,
            string removeString,
            bool ignoreCase,
            CultureInfo culture)
        {
            //------------------------------------------------------------
            /// エラーチェック
            //------------------------------------------------------------
            if (removeString == null) throw new ArgumentNullException();    //// 削除対象文字列 = null の場合、引数null例外をスローする


            //------------------------------------------------------------
            /// 文字列の先頭が指定された文字列と一致した場合に、先頭部分を削除する
            //------------------------------------------------------------
            if (target.StartsWith(removeString, ignoreCase, culture))
            {                                                           //// 文字列の先頭が削除対象文字列と一致する場合
                return target.XRemoveStart(removeString.Length);        /////  戻り値 = 先頭の削除対象文字列を削除した文字列 で関数終了
            }
            else
            {                                                           //// 文字列の先頭が削除対象文字列と一致しない場合
                return target;                                          /////  戻り値 = 文字列全体 で関数終了
            }
        }



#if DRAFT   //[-] 下書き版：作ってはみたものの使わなくなった。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列削除(比較方法指定)】文字列から指定された文字列をすべて削除します。
        /// </summary>
        /// <param name="target">          [in ]：対象インスタンス</param>
        /// <param name="removeString">    [in ]：削除対象文字列</param>
        /// <param name="stringComparison">[in ]：比較方法</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XRemove(this string target, string removeString,
                                     StringComparison stringComparison = StringComparison.Ordinal)
        {
            //------------------------------------------------------------
            /// 文字列から指定された文字列をすべて削除する
            //------------------------------------------------------------
            var result = target;                                        //// 処理結果文字列 = 対象インスタンスの内容 に初期化する
            var removeCount = removeString.Length;                      //// 削除文字数 = 削除文字列の文字数

            while (true)
            {                                                           //// 削除対象文字列が見つからなくなるまで繰り返す
                var findIndex =
                    result.IndexOf(removeString, stringComparison);     /////  削除対象文字列を検索する
                if (findIndex == -1)
                {                                                       /////  見つからなかった場合
                    return result;                                      //////   戻り値 = 処理結果文字列 で関数終了
                }

                result = result.Remove(findIndex, removeCount);         /////  発見位置の文字列を削除する
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字削除(比較方法指定)】文字列から指定された文字をすべて削除します。
        /// </summary>
        /// <param name="target">          [in ]：対象インスタンス</param>
        /// <param name="removeChar">      [in ]：削除対象文字</param>
        /// <param name="stringComparison">[in ]：比較方法</param>
        /// <returns>
        /// 削除結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// ・比較方法の既定値は序数検索です。カルチャに依存せず、大文字と小文字・全角と半角を区別します。
        /// ・サロゲート ペア文字(絵文字・異体字・結合文字など、複数の Char で表される Unicode 文字)を
        ///   削除する場合は、第一引数が string のバージョンを使用します。
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XRemove(this string target, char removeChar,
                                     StringComparison stringComparison = StringComparison.Ordinal)
            => XRemove(target, new string(removeChar, 1), stringComparison);
#endif


        //====================================================================================================
        // 拡張メソッド(置換系)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【改行文字群置換】文字列中の改行文字群(CR+LF, CR, LF)を別の文字列に置換します。
        /// </summary>
        /// <param name="target">  [in ]：対象インスタンス</param>
        /// <param name="newValue">[in ]：置換後文字列</param>
        /// <returns>
        /// 置換結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・CR+LF は２文字で１つの改行として扱いますが、LF+CRは２つの改行として扱います。<br></br>
        /// <br></br>
        /// 使用例<br></br>
        /// <code>
        /// ・strValue.XReplaceNewLineChars(" ")    … strValue 中の改行文字群(CR+LF, CR, LF)を半角スペースに置換して一行にまとめます。
        /// ・strValue.XReplaceNewLineChars("\r\n") … strValue 中の改行文字群(CR+LF, CR, LF)を CR+LF に統一します。
        /// ・strValue.XReplaceNewLineChars("\n")   … strValue 中の改行文字群(CR+LF, CR, LF)を LF に統一します。
        /// </code>
        /// 参考メモ：<see cref="ZZZ_Memo_CRLF"/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XReplaceNewLineChars(this string target, string newValue)
        {
            //------------------------------------------------------------
            /// エラーチェック
            //------------------------------------------------------------
            if (target == null) throw new NullReferenceException();     //// 対象インスタンス = null の場合、null参照例外をスローする


            //------------------------------------------------------------
            /// 文字列中の改行文字群(CR+LF, CR, LF)を別の文字列に置換する
            //------------------------------------------------------------
            var workStr = new StringBuilder();                          //// 置換結果文字列編集領域を生成する

            var afterCR = false;                                        //// CR直後フラグ = false に初期化する
            foreach (var chr in target)
            {                                                           //// 対象インスタンスの全文字を繰り返す
                switch (chr)
                {                                                       /////  文字の内容で分岐
                    case '\r':                                          //////   CR の場合
                        workStr.Append(newValue);                       ///////    置換結果文字列に置換後文字列を追加する
                        afterCR = true;                                 ///////    CR直後フラグ = true にセットする
                        break;

                    case '\n':                                          //////   LF の場合
                        if (afterCR == false)
                        {                                               ///////    CR直後でない場合(CR+LFの並びでない場合)
                            workStr.Append(newValue);                   ////////     置換結果文字列に置換後文字列を追加する
                        }
                        afterCR = false;                                ///////    CR直後フラグ = false にリセットする
                        break;

                    default:                                            //////   上記以外の場合
                        workStr.Append(chr);                            ///////    置換結果文字列に文字をそのまま追加する
                        afterCR = false;                                ///////    CR直後フラグ = false にリセットする
                        break;
                }
            }

            return workStr.ToString();                                  //// 戻り値 = 置換結果文字列 で関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod]
        public static void ZZZ_XReplaceNewLineChars(IAutoTestExecuter ifExecuter)
        {
            var testStr = "『古池や\r\n蛙飛びこむ\n水の音』\n\r松尾芭蕉\r-貞享三年-";

            SubRoutine("", "『古池や蛙飛びこむ水の音』松尾芭蕉-貞享三年-", "改行文字群を削除");
            SubRoutine("　", "『古池や　蛙飛びこむ　水の音』　　松尾芭蕉　-貞享三年-", "改行文字群を全角スペースに置換");
            SubRoutine("<改行>", "『古池や<改行>蛙飛びこむ<改行>水の音』<改行><改行>松尾芭蕉<改行>-貞享三年-", "改行文字群を<改行>に置換");

            SubRoutine("\r\n", "『古池や\r\n蛙飛びこむ\r\n水の音』\r\n\r\n松尾芭蕉\r\n-貞享三年-", "改行文字を CR+LF に統一");
            SubRoutine("\r", "『古池や\r蛙飛びこむ\r水の音』\r\r松尾芭蕉\r-貞享三年-", "改行文字を CR に統一");
            SubRoutine("\n", "『古池や\n蛙飛びこむ\n水の音』\n\n松尾芭蕉\n-貞享三年-", "改行文字を LF に統一");

            testStr = ""; SubRoutine("<改行>", "", "空文字列に対する操作");

#if CTRL_F5 // 中断対象例外のテスト。例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能
            testStr = null; SubRoutine("<改行>", typeof(NullReferenceException), "null参照に対する操作");
#endif

            void SubRoutine(string newValue,                        // [in ]：置換後文字列
                            AutoTestResultInfo<string> expectResult,// [in ]：予想結果(文字列 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.Test(XReplaceNewLineChars, testStr, newValue, expectResult, ifExecuter, testPattern);
            }
        }
#endif


        //====================================================================================================
        // 拡張メソッド(エスケープ処理系)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列エスケープ】エスケープコンバーターを使用して文字列中をエスケープします。
        /// </summary>
        /// <param name="sourceStr">      [in ]：変換元文字列</param>
        /// <param name="escapeConverter">[in ]：エスケープコンバーター[null = C#形式]</param>
        /// <returns>
        /// エスケープ結果文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・String オブジェクト主体で EscapeConverter.Escape を呼び出すユーティリティーメソッドです。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XEscape(this string sourceStr, EscapeConverter escapeConverter = null)
        {
            //------------------------------------------------------------
            /// 文字列中のエスケープ対象文字をエスケープする
            //------------------------------------------------------------
            EscapeConverter.Normalize(ref escapeConverter);             //// エスケープコンバーターを正規化する(null の場合は CSharp に差し替える)
            return escapeConverter.Escape(sourceStr);                   //// エスケープコンバーターでエスケープした結果を戻り値とし、関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【有効文字列チェック】指定された文字列が有効な文字列(null でもなく Empty 文字列でもない)かどうかを示します。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <returns>
        /// チェック結果[true = 有効な文字列である / false = 有効な文字列でない(null または Empty 文字列)]
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・String.IsNullOrEmpty の結果を反転させているだけですが、「有効な文字列かどうかをチェックする処理であること」を明示したい場合に使用します。
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static bool XIsValid(this string target) => !string.IsNullOrEmpty(target);

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod]
        public static void ZZZ_XIsValid(IAutoTestExecuter ifExecuter)
        {
            // ＜メモ＞中断対象例外のテストはない
            SubRoutine("ABC", true, "通常の文字列");
            SubRoutine("文字列", true, "通常の文字列");
            SubRoutine(" ", true, "半角スペース = 有効な文字列");
            SubRoutine("\0", true, "null文字 = 有効な文字列");
            SubRoutine("", false, "空文字列 = 有効な文字列でない");
            SubRoutine(string.Empty, false, "string.Empty = 有効な文字列でない");
            SubRoutine(null, false, "null = 有効な文字列でない");

            void SubRoutine(string strValue,                        // [in ]：文字列
                            AutoTestResultInfo<bool> expectResult,  // [in ]：予想結果(bool値 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]

            {
                AutoTest.Test(XIsValid, strValue, expectResult, ifExecuter, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列配列に分割】文字列を区切り文字で分割した配列を取得します。空の要素はスキップし、各要素はトリムします。
        /// </summary>
        /// <param name="target">   [in ]：対象インスタンス</param>
        /// <param name="separator">[in ]：区切り文字</param>
        /// <returns>
        /// 文字列配列
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string[] XSplit(this string target, char separator)
        {
            //------------------------------------------------------------
            /// 文字列を区切り文字で分割した配列を取得する
            //------------------------------------------------------------
            var result = target.Split(new char[] { separator },
                                StringSplitOptions.RemoveEmptyEntries); //// 文字列を区切り文字で分割する(空の要素はスキップ)

            for (var index = 0; index < result.Length; index++)
            {                                                           //// 分割結果文字列配列を繰り返す
                result[index] = result[index].Trim();                   /////  トリムする
            }

            return result;                                              //// 戻り値 = 分割結果文字列配列 で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【左側文字列取得】文字列の左側から取得文字数だけ取り出します。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <param name="length">[in ]：取得文字数(0以上)</param>
        /// <returns>
        /// 取得結果(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// 備考<br></br>
        /// ・取得文字数が文字列長以上の場合は文字列全体を返します。<br></br>
        /// ・文字列が空文字列の場合は、空文字列を返します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XLeft(this string target, int length)
        {
            //------------------------------------------------------------
            /// 文字列の左側から指定された文字数を取得する
            //------------------------------------------------------------
            if (length >= target.Length)
            {                                                           //// 取得文字数 >= 文字列長 の場合
                return target;                                          /////  戻り値 = 文字列全体 で関数終了
            }

            return target.Substring(0, length);                         //// 戻り値 = 左側から指定文字数だけ取り出した結果 で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【右側文字列取得】文字列の右側から取得文字数だけ取り出します。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <param name="length">[in ]：取得文字数(0以上)</param>
        /// <returns>
        /// 取得結果(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        /// <remarks>
        /// 備考<br></br>
        /// ・取得文字数が文字列長以上の場合は文字列全体を返します。<br></br>
        /// ・文字列が空文字列の場合は、空文字列を返します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XRight(this string target, int length)
        {
            //------------------------------------------------------------
            /// 文字列の右側から指定された文字数を取得する
            //------------------------------------------------------------
            if (length >= target.Length)
            {                                                           //// 取得文字数 >= 文字列長 の場合
                return target;                                          /////  戻り値 = 文字列全体 で関数終了
            }

            return target.Substring(target.Length - length);            //// 戻り値 = 右側から指定文字数だけ取り出した結果 で関数終了
        }

        //--------------------------------------------------------------------------------
        // テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod("XString.XLeft/XRight")]
        public static void ZZZ_XLeft_XRight(IAutoTestExecuter ifExecuter)
        {
            string testStr = "AB文字CD";

            SubRoutineL(0, "");
            SubRoutineL(1, "A");
            SubRoutineL(2, "AB");
            SubRoutineL(3, "AB文");
            SubRoutineL(4, "AB文字");
            SubRoutineL(5, "AB文字C");
            SubRoutineL(6, "AB文字CD");
            SubRoutineL(7, "AB文字CD");
            SubRoutineL(8, "AB文字CD");
            SubRoutineL(-1, typeof(ArgumentOutOfRangeException), "取得文字数が不正");

            SubRoutineR(0, "");
            SubRoutineR(1, "D");
            SubRoutineR(2, "CD");
            SubRoutineR(3, "字CD");
            SubRoutineR(4, "文字CD");
            SubRoutineR(5, "B文字CD");
            SubRoutineR(6, "AB文字CD");
            SubRoutineR(7, "AB文字CD");
            SubRoutineR(8, "AB文字CD");
            SubRoutineR(-1, typeof(ArgumentOutOfRangeException), "取得文字数が不正");

            testStr = "";
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
                AutoTest.Test(XLeft, testStr, length, expectResult, ifExecuter, testPattern);
            }

            void SubRoutineR(int length,                                // [in ]：文字数
                             AutoTestResultInfo<string> expectResult,   // [in ]：予想結果(文字列 または 例外の型情報)
                             string testPattern = null)                 // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.Test(XRight, testStr, length, expectResult, ifExecuter, testPattern);
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【マルチスペース抑制】連続した２つ以上の半角スペースを１つに抑制します。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <returns>
        /// マルチスペースを抑制した文字列(現在のインスタンスの内容を変更せずに、処理結果の新しいインスタンスを返します)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XSuppressMultiSpace(this string target)
        {
            //------------------------------------------------------------
            /// エラーチェック
            //------------------------------------------------------------
            if (target == null) throw new NullReferenceException();     //// 対象インスタンス = null の場合、null参照例外をスローする


            //------------------------------------------------------------
            /// 連続した２つ以上の半角スペースを１つに抑制する
            //------------------------------------------------------------
            var workStr = new System.Text.StringBuilder(target);        //// 文字列編集領域 = 対象インスタンスの内容 に初期化する

            while (true)
            {                                                           //// マルチスペースがなくなるまで繰り返す
                var befLen = workStr.Length;                            /////  処理前の文字列長を取得する
                workStr.Replace("  ", " ");                             /////  半角スペース２つを１つに置換する
                if (workStr.Length == befLen)
                {                                                       /////  処理前後で文字数が変わらない場合(マルチスペースがなかった場合)
                    break;                                              //////   繰り返し処理を抜ける
                }
            }

            return workStr.ToString();                                  //// 戻り値 = マルチスペースを抑制した文字列 で関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod()]
        public static void ZZZ_XSuppressMultiSpace(IAutoTestExecuter ifExecuter)
        {
            SubRoutine("ABC  DEF",
                       "ABC DEF", "正常系1");
            SubRoutine("     ABC     DEF     ",
                       " ABC DEF ", "正常系2");
            SubRoutine(" ABC DEF ",
                       " ABC DEF ", "抑制対象なし");

            SubRoutine("", "", "空文字列に対する操作");

#if CTRL_F5 // 中断対象例外のテスト。例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能
            SubRoutine(null, typeof(NullReferenceException), "null参照に対する操作");
#endif

            void SubRoutine(string strValue,                        // [in ]：文字列
                            AutoTestResultInfo<string> expectResult,// [in ]：予想結果(文字列 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                AutoTest.Test(XSuppressMultiSpace, strValue, expectResult, ifExecuter, testPattern);
            }
        }
#endif


        //====================================================================================================
        // static 公開メソッド(null対応)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列化(null対応)】文字列を取得します。文字列が null の場合は "(null)" を返します。
        /// </summary>
        /// <param name="str">[in ]：文字列</param>
        /// <returns>
        /// 文字列
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XToString(string strValue)
        {
            if (strValue == null)
            {
                return "(null)";
            }
            else
            {
                return strValue;
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ハッシュコード取得(null対応)】文字列のハッシュコードを取得します。null の場合は 0 を返します。
        /// </summary>
        /// <param name="str">[in ]：文字列</param>
        /// <returns>
        /// ハッシュコード
        /// </returns>
        //--------------------------------------------------------------------------------
        public static int XGetHashCode(string strValue)
        {
            if (strValue == null)
            {
                return 0;
            }
            else
            {
                return strValue.GetHashCode();
            }
        }

    }


    //====================================================================================================
    /// <summary>
    /// 【StringCollection 拡張メソッド】StringCollection クラスに拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XStringCollection
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コピー配列作成】StringCollection の全要素を新しい配列にコピーします。
        /// </summary>
        /// <param name="target">[in ]：対象インスタンス</param>
        /// <returns>
        /// 文字列配列
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string[] XToArray(this StringCollection target)
        {
            //------------------------------------------------------------
            /// StringCollection の全要素を新しい配列にコピーする
            //------------------------------------------------------------
            var array = new string[target.Count];                       //// 対象インスタンスの要素数で文字列配列を生成する
            target.CopyTo(array, 0);                                    //// 対象インスタンスの内容を文字列配列にコピーする
            return array;                                               //// 戻り値 = 文字列配列 で関数終了
        }
    }


#if DEBUG
    //====================================================================================================
    /// <summary>
    /// 【メモ】改行文字について
    /// <code>
    /// 改行文字   ：採用環境        ：テキストボックス：Debug.Print
    /// -----------：----------------：----------------：-----------
    /// CR+LF(\r\n)：マイクロソフト系：改行            ：改行
    /// CR   (\r  )：macOS           ：非表示          ：改行
    /// LF   (\n  )：Unix系、max OS X：非表示          ：改行
    /// LF+CR(\n\r)：なし            ：非表示          ：改行２つ
    /// </code>
    /// </summary>
    //====================================================================================================
    public static class ZZZ_Memo_CRLF { }
#endif
}
