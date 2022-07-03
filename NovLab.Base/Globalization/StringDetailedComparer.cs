// @(h)StringDetailedComparer.cs ver 0.00 ( '22.06.30 Nov-Lab ) 作成開始
// @(h)StringDetailedComparer.cs ver 0.51 ( '22.07.03 Nov-Lab ) ベータ版完成

// @(s)
// 　【文字列詳細比較子】カルチャや文字列比較オプションを指定した詳細な文字列比較機能を提供します。

using System;
using System.Collections.Generic;
using System.Globalization;


namespace NovLab.Globalization
{
#if DEBUG
    //====================================================================================================
    /// <summary>
    /// 【メモ】文字列の等値比較子について
    /// <code>
    /// "Cow"と"cow"："Cow"と"Ｃｏｗ"："うし"と"ウシ"：適した Comparer                   ：備考
    /// ------------：---------------：--------------：----------------------------------：----------------------------------
    /// 区別する    ：区別する       ：区別する      ：EqualityComparer＜string＞.Default：完全序数比較
    /// 区別しない  ：区別する       ：区別する      ：StringComparer.OrdinalIgnoreCase  ：大文字と小文字を区別しない序数比較(ファイル名・アカウント名など)
    /// 区別しない  ：区別しない     ：区別しない    ：NovLab.TextComparer               ：テキスト比較
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
        // ・CompareOptions.IgnoreSymbols を指定すると、「co-op」だけでなく、「co op」や「co_op」までも、「coop」と同一視してしまう。
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

    }
}
