// @(h)AutoTestOption.cs ver 0.01 ( '24.01.18 Nov-Lab ) 作成開始
// @(h)AutoTestOption.cs ver 0.21 ( '24.01.18 Nov-Lab ) アルファ版完成

// @(s)
// 　【テストオプション】自動テストの動作オプションを指定します。

using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace NovLab.DebugSupport
{
    //====================================================================================================
    /// <summary>
    /// 【テストオプション】自動テストの動作オプションを指定します。<br></br>
    /// テスト結果にテストパターン名を表示したり、戻り値や引数を定型文字列表記にすることができます。<br></br>
    /// </summary>
    //====================================================================================================
    public partial class AutoTestOptions
    {
        //====================================================================================================
        // 公開フィールド
        //====================================================================================================

        /// <summary>
        /// 【テストパターン名】null = なし
        /// </summary>
        public string testPattern = null;

        /// <summary>
        /// 【戻り値用定型文字列作成関数】null = 定型文字列を用いない。戻り値を定型文字列表記したい場合に指定します。
        /// </summary>
        public RegularStringFunc fncResultRegularString = null;

        /// <summary>
        /// 【対象インスタンス用定型文字列作成関数】null = 定型文字列を用いない。対象インスタンスを定型文字列表記したい場合に指定します。
        /// </summary>
        public RegularStringFunc fncInstanceRegularString = null;

        /// <summary>
        /// 【引数１用定型文字列作成関数】null = 定型文字列を用いない。引数１を定型文字列表記したい場合に指定します。
        /// </summary>
        public RegularStringFunc fncArg1RegularString = null;

        /// <summary>
        /// 【引数２用定型文字列作成関数】null = 定型文字列を用いない。引数２を定型文字列表記したい場合に指定します。
        /// </summary>
        public RegularStringFunc fncArg2RegularString = null;

        /// <summary>
        /// 【引数３用定型文字列作成関数】null = 定型文字列を用いない。引数３を定型文字列表記したい場合に指定します。
        /// </summary>
        public RegularStringFunc fncArg3RegularString = null;

        /// <summary>
        /// 【引数４用定型文字列作成関数】null = 定型文字列を用いない。引数４を定型文字列表記したい場合に指定します。
        /// </summary>
        public RegularStringFunc fncArg4RegularString = null;

        /// <summary>
        /// 【引数５用定型文字列作成関数】null = 定型文字列を用いない。引数５を定型文字列表記したい場合に指定します。
        /// </summary>
        public RegularStringFunc fncArg5RegularString = null;


        //====================================================================================================
        // コンストラクター・変換演算子
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デフォルトコンストラクター】既定の内容でテストオプションを生成します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public AutoTestOptions()
        {
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テストパターン名指定コンストラクター】テストパターン名を指定してテストオプションを生成します。
        /// </summary>
        /// <param name="testPattern">[in ]：テストパターン名</param>
        //--------------------------------------------------------------------------------
        public AutoTestOptions(string testPattern)
        {
            this.testPattern = testPattern;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【変換演算子(テストパターン名 -> テストオプション)】テストパターン名を、テストオプションに変換します。
        /// </summary>
        /// <param name="result">[in ]：戻り値</param>
        //--------------------------------------------------------------------------------
        public static implicit operator AutoTestOptions(string testPattern) => new AutoTestOptions(testPattern);

    } // class

} // namespace
