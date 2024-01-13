// @(h)StringDetailedComparer.cs ver 0.00 ( '22.06.30 Nov-Lab ) 作成開始
// @(h)StringDetailedComparer.cs ver 0.51 ( '22.07.03 Nov-Lab ) ベータ版完成
// @(h)StringDetailedComparer.cs ver 0.51a( '24.01.20 Nov-Lab ) 仕変対応：AutoTest クラスの仕様変更に対応した。機能変更なし。

// @(s)
// 　【文字列詳細比較子】カルチャや文字列比較オプションを指定した詳細な文字列比較機能を提供します。

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

using NovLab.DebugSupport;


namespace NovLab.Globalization
{
#if DEBUG
    //====================================================================================================
    /// <summary>
    /// 【メモ】文字列の等値比較子について
    /// <code>
    /// よく使うComparer                  ："Cow"と"cow"："Cow"と"Ｃｏｗ"："うし"と"ウシ"：備考
    /// ----------------------------------：------------：---------------：--------------：----------------------------------
    /// EqualityComparer＜string＞.Default：区別する    ：区別する       ：区別する      ：完全序数比較
    /// StringComparer.OrdinalIgnoreCase  ：同一視する  ：区別する       ：区別する      ：大文字と小文字を区別しない序数比較(ファイル名・アカウント名など)
    /// NovLab.TextComparer               ：同一視する  ：同一視する     ：同一視する    ：カレントカルチャ依存でテキスト比較
    ///
    /// カルチャによる合字の扱い："Æ"と"AE" ：備考
    /// ------------------------：----------：--------------------------------------------
    /// 序数比較                ：区別する  ：
    /// 日本語や英語(en-US)     ：同一視する：IgnoreCase でない場合は、"Æ"と"ae"は区別する
    /// デンマーク語(da-DK)     ：区別する  ：
    /// 
    /// ・"辻"(二点しんにょう)と"辻󠄀"(一点しんにょう)は、常に(序数比較であっても)同一視する。
    /// </code>
    /// </summary>
    //====================================================================================================
    public static class ZZZ_Memo_EqualityComparer { }
#endif


    //====================================================================================================
    /// <summary>
    /// 【テキスト比較子】テキスト比較用の比較子です。
    /// 大文字と小文字・ひらがなとカタカナ・半角と全角を区別せず、カルチャに依存します。
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・「みかん」と「ミカン」や、「Apple」と「ＡＰＰＬＥ」を同一視します。<br></br>
    /// ・「coop」と「co-op」は区別します。<br></br>
    /// <br></br>
    /// 参考メモ：<see cref="ZZZ_Memo_EqualityComparer"/>
    /// </remarks>
    //====================================================================================================
    public class TextComparer : StringDetailedComparer
    {
        // ＜メモ＞
        // ・CompareOptions.IgnoreSymbols を指定すると、「coop」と「co-op」を同一視するだけでなく、「co op」や「co_op」までも「coop」と同一視してしまう。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【既定のコンストラクター】テキスト比較子を生成します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public TextComparer() : base(CompareOptions.IgnoreCase |        // 大文字と小文字を区別しない
                                     CompareOptions.IgnoreKanaType |    // ひらがなとカタカナを区別しない
                                     CompareOptions.IgnoreWidth)        // 半角と全角を区別しない
        { }
    }


    //====================================================================================================
    /// <summary>
    /// 【文字列詳細比較子】カルチャや文字列比較オプションを指定した詳細な文字列比較機能を提供します。
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・<see cref="StringComparer"/> とは違い、ひらがなとカタカナの区別や、半角と全角の区別なども詳細に指定できます。<br></br>
    /// <br></br>
    /// 参考メモ：<see cref="ZZZ_Memo_EqualityComparer"/>
    /// </remarks>
    //====================================================================================================
    public class StringDetailedComparer : IEqualityComparer<string>,    // 等値比較子(キー値による検索やキー値の重複チェックなどで使用する)
                                          IComparer<string>             // 順序比較子(ソート処理などで使用する)
    {
        //====================================================================================================
        // 公開フィールド
        //====================================================================================================
        /// <summary>
        /// 【CompareInfo】この比較子が使用する CompareInfo です。
        /// </summary>
        public readonly CompareInfo compareInfo;

        /// <summary>
        /// 【文字列比較オプション】この比較子が使用する文字列比較オプションです。
        /// </summary>
        public readonly CompareOptions compareOptions;


        //====================================================================================================
        // コンストラクター
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【基本コンストラクター】文字列比較オプションを指定して文字列詳細比較子を生成します。
        /// カルチャは Thread.CurrentCulture の値を使用します。
        /// </summary>
        /// <param name="compareOptions">[in ]：文字列比較オプション</param>
        //--------------------------------------------------------------------------------
        public StringDetailedComparer(CompareOptions compareOptions)
        {
            compareInfo = CultureInfo.CurrentCulture.CompareInfo;
            this.compareOptions = compareOptions;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【完全コンストラクター】カルチャと文字列比較オプションを指定して文字列詳細比較子を生成します。
        /// </summary>
        /// <param name="cultureInfo">   [in ]：カルチャ</param>
        /// <param name="compareOptions">[in ]：文字列比較オプション</param>
        //--------------------------------------------------------------------------------
        public StringDetailedComparer(CultureInfo cultureInfo, CompareOptions compareOptions)
        {
            compareInfo = cultureInfo.CompareInfo;
            this.compareOptions = compareOptions;
        }


        //====================================================================================================
        // IComparer<string> I/F の実装
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【比較】２つの文字列を比較します。
        /// </summary>
        /// <param name="info1">[in ]：文字列１</param>
        /// <param name="info2">[in ]：文字列２</param>
        /// <returns>
        /// 比較結果値[0より小さい = 情報１＜情報２、0 = 情報１＝情報２、0より大きい = 情報１＞情報２]
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・IComparer&lt;string&gt;.Compare(T, T) の実装です。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        int IComparer<string>.Compare(string info1, string info2)
        {
            return compareInfo.Compare(info1, info2, compareOptions);
        }


        //====================================================================================================
        // IEqualityComparer<string> I/F の実装
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【等値チェック】２つの文字列が等しいかどうかを判断します。
        /// </summary>
        /// <param name="info1">[in ]：文字列１</param>
        /// <param name="info2">[in ]：文字列２</param>
        /// <returns>
        /// チェック結果[true = 等しい / false = 等しくない]
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・IEqualityComparer&lt;string&gt;.Equals(T, T) の実装です。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        bool IEqualityComparer<string>.Equals(string info1, string info2)
        {
            //------------------------------------------------------------
            /// ２つの文字列が等しいかどうかを判断する
            //------------------------------------------------------------
            var sortkey1 = compareInfo.GetSortKey(info1, compareOptions);   //// 文字列１から並べ替えキーを取得する
            var sortkey2 = compareInfo.GetSortKey(info2, compareOptions);   //// 文字列２から並べ替えキーを取得する
            return sortkey1.Equals(sortkey2);                               //// 戻り値 = 並べ替えキーによる等値チェックの結果 で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ハッシュコード取得】文字列に対するハッシュコードを取得します。
        /// </summary>
        /// <param name="obj">[in ]：文字列</param>
        /// <returns>
        /// ハッシュコード
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・IEqualityComparer&lt;string&gt;.GetHashCode(T) の実装です。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        int IEqualityComparer<string>.GetHashCode(string obj)
        {
            /// 戻り値 = 文字列から取得した並べ替えキーに対応するハッシュコード で関数終了
            return compareInfo.GetSortKey(obj, compareOptions).GetHashCode();
        }


        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod]
        public static void ZZZ_Equals()
        {
            // ＜メモ＞中断対象例外のテストはない
            // ＜メモ＞日本語環境前提でのテスト
            var textComparer = new TextComparer();
            AutoTest.Print("＜NovLab.Globalization.TextComparer で比較：テキスト比較＞");
            SubRoutine(textComparer, "Cow", "cow", true, "大文字と小文字は同一視");
            SubRoutine(textComparer, "Cow", "Ｃｏｗ", true, "半角と全角は同一視");
            SubRoutine(textComparer, "うし", "ウシ", true, "ひらがなとカタカナは同一視");
            SubRoutine(textComparer, "coop", "co-op", false, "記号を含むものは区別");
            SubRoutine(textComparer, "Æ", "AE", true, "合字(日本語では同一視する)");
            SubRoutine(textComparer, "辻さん", "辻󠄀さん", true, "異体字(日本語では同一視する)");
            AutoTest.Print("");

            AutoTest.Print("＜EqualityComparer<string>.Default で比較：大文字と小文字を区別する序数比較＞");
            SubRoutine(EqualityComparer<string>.Default, "Cow", "cow", false, "大文字と小文字は区別");
            SubRoutine(EqualityComparer<string>.Default, "Cow", "Ｃｏｗ", false, "半角と全角は区別");
            SubRoutine(EqualityComparer<string>.Default, "うし", "ウシ", false, "ひらがなとカタカナは区別");
            SubRoutine(EqualityComparer<string>.Default, "coop", "co-op", false, "記号を含むものは区別");
            SubRoutine(EqualityComparer<string>.Default, "Æ", "AE", false, "合字(序数比較では区別する)");
            SubRoutine(textComparer, "辻さん", "辻󠄀さん", true, "異体字(序数比較でも同一視する)");
            AutoTest.Print("");

            AutoTest.Print("＜StringComparer.OrdinalIgnoreCase で比較：大文字と小文字を同一視する序数比較＞");
            SubRoutine(StringComparer.OrdinalIgnoreCase, "Cow", "cow", true, "大文字と小文字は同一視");
            SubRoutine(StringComparer.OrdinalIgnoreCase, "Cow", "Ｃｏｗ", false, "半角と全角は区別");
            SubRoutine(StringComparer.OrdinalIgnoreCase, "うし", "ウシ", false, "ひらがなとカタカナは区別");
            SubRoutine(StringComparer.OrdinalIgnoreCase, "coop", "co-op", false, "記号を含むものは区別");
            SubRoutine(StringComparer.OrdinalIgnoreCase, "Æ", "AE", false, "合字(序数比較では区別する)");
            SubRoutine(textComparer, "辻さん", "辻󠄀さん", true, "異体字(序数比較でも同一視する)");
            AutoTest.Print("");

            AutoTest.Print("＜StringComparer.CurrentCultureIgnoreCase で比較：大文字と小文字を同一視するカルチャベース比較＞");
            SubRoutine(StringComparer.CurrentCultureIgnoreCase, "Cow", "cow", true, "大文字と小文字は同一視");
            SubRoutine(StringComparer.CurrentCultureIgnoreCase, "Cow", "Ｃｏｗ", false, "半角と全角は区別");
            SubRoutine(StringComparer.CurrentCultureIgnoreCase, "うし", "ウシ", false, "ひらがなとカタカナは区別");
            SubRoutine(StringComparer.CurrentCultureIgnoreCase, "coop", "co-op", false, "記号を含むものは区別");
            SubRoutine(StringComparer.CurrentCultureIgnoreCase, "Æ", "AE", true, "合字(日本語では同一視する)");
            SubRoutine(textComparer, "辻さん", "辻󠄀さん", true, "異体字(日本語では同一視する)");
            AutoTest.Print("");

            AutoTest.Print("＜カルチャによる違い＞");
            var myComparer = new StringDetailedComparer(new CultureInfo("en-US"), CompareOptions.None);
            SubRoutine(myComparer, "Æ", "AE", true, "合字(英語では同一視する)");
            SubRoutine(myComparer, "辻さん", "辻󠄀さん", true, "異体字(英語でも同一視する)");

            myComparer = new StringDetailedComparer(new CultureInfo("da-DK"), CompareOptions.None);
            SubRoutine(myComparer, "Æ", "AE", false, "合字(デンマーク語では区別する)");
            SubRoutine(myComparer, "辻さん", "辻󠄀さん", true, "異体字(デンマーク語でも同一視する)");

            void SubRoutine(IEqualityComparer<string> ifComparer,   // [in ]：文字列
                            string strValue1,                       // [in ]：文字列1
                            string strValue2,                       // [in ]：文字列2
                            AutoTestResultInfo<bool> expectResult,  // [in ]：予想結果(bool値 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]

            {
                AutoTest.Test(ifComparer.Equals, strValue1, strValue2, expectResult, testPattern);
            }
        }
#endif

    } // class

} // namespace
