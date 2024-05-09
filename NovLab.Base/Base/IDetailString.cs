// @(h)IDetailString.cs ver 0.00 ( '24.05.11 Nov-Lab ) 作成開始
// @(h)IDetailString.cs ver 0.51 ( '24.05.11 Nov-Lab ) ベータ版完成
// @(h)IDetailString.cs ver 0.51a( '24.05.14 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【詳細文字列インターフェイス】オブジェクトから詳細文字列を取得する機能を提供します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【詳細文字列インターフェイス】オブジェクトから詳細文字列を取得する機能を提供します。<br/>
    /// このI/Fによって提供される詳細文字列は、<see cref="DetailString"/> クラスで簡単に取得することができます。<br/>
    /// </summary>
    /// <remarks>
    /// 補足<br/>
    /// ・ToString() とは別に、より詳しい詳細文字列を提供したい場合に使用します。<br/>
    /// </remarks>
    //====================================================================================================
    public interface IDetailString
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【詳細文字列取得】オブジェクトから詳細文字列を取得します。
        /// </summary>
        /// <returns>
        /// 詳細文字列
        /// </returns>
        //--------------------------------------------------------------------------------
        string GetDetailString();

    } // interface


    //====================================================================================================
    /// <summary>
    /// 【詳細文字列操作】<see cref="IDetailString"/> I/Fを通じてオブジェクトから詳細文字列を取得するための機能を提供します。
    /// </summary>
    //====================================================================================================
    public partial class DetailString
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【詳細文字列取得】対象オブジェクトから詳細文字列を取得します。<br/>
        /// 対象オブジェクトが <see cref="IDetailString"/> I/Fを実装していない場合は、ToString() の結果を返します。<br/>
        /// </summary>
        /// <param name="target">[in ]：対象オブジェクト</param>
        /// <returns>
        /// 詳細文字列
        /// </returns>
        /// <exception cref="ArgumentNullException">引数不正例外(null値)。<paramref name="target"/> を null にすることはできません。</exception>
        //--------------------------------------------------------------------------------
        public static string Get(object target)
        {
            //------------------------------------------------------------
            /// 引数をチェックする
            //------------------------------------------------------------
            if (target == null)
            {                                                           //// 対象オブジェクト = null の場合
                throw new ArgumentNullException(nameof(target));        /////  引数不正例外(null)をスローする
            }


            //------------------------------------------------------------
            /// 対象オブジェクトから詳細文字列を取得する
            //------------------------------------------------------------
            if (target is IDetailString detailString)
            {                                                           //// 対象オブジェクトが IDetailString I/F を実装している場合
                return detailString.GetDetailString();                  /////  詳細文字列を取得して戻り値とし、関数終了
            }
            else
            {                                                           //// 対象オブジェクトが IDetailString I/F を実装していない場合
                return target.ToString();                               /////  戻り値 = ToString() の結果 で関数終了
            }
        }

    } // class


#if DEBUG
    //====================================================================================================
    /// <summary>
    /// IDetailString のテストコード
    /// </summary>
    //====================================================================================================
    public partial class ZZZTest_IDetailString
    {
        [AutoTestMethod(nameof(IDetailString) + " の総合的テスト")]
        public static void ZZZ_OverallTest()
        {
            //--------------------------------------------------------
            // 正常系のテスト
            //--------------------------------------------------------
            AutoTest.Print($"＜正常系・{nameof(IDetailString)} I/Fなし＞");

            SubRoutine("ABC", "ABC", $"string型");
            SubRoutine((int)123, "123", $"int型");
            SubRoutine(DayOfWeek.Saturday, "Saturday", $"DayOfWeek 列挙体");


            AutoTest.Print("");
            AutoTest.Print($"＜正常系・{nameof(IDetailString)} I/Fあり＞");

            SubRoutine(new M_Test("果物", "メロン", 12), "果物-メロン 12個");


            //--------------------------------------------------------
            // 異常系のテスト
            //--------------------------------------------------------
            AutoTest.Print("");
            AutoTest.Print("＜異常系のテスト＞");

            SubRoutine(null, typeof(ArgumentNullException), "対象オブジェクトがnull");


            //--------------------------------------------------------
            // 【ローカル関数】
            //--------------------------------------------------------
            void SubRoutine(object target,                          // [in ]：対象オブジェクト
                            AutoTestResultInfo<string> expectResult,// [in ]：予想結果(文字列 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                       // テストオプション
                    fncArg1RegularString = RegularString.WithTypeName,  // ・引数１は型名併記
                };

                AutoTest.Test(DetailString.Get, target, expectResult, testOptions);
            }
        }


        //------------------------------------------------------------
        // 自動テストで使う内部クラス
        //------------------------------------------------------------
        protected class M_Test : IDetailString
        {
            public string itemCategory; // アイテム分類名
            public string itemName;     // アイテム名
            public int numOfStock;      // 在庫数

            public override string ToString() => itemName;

            public string GetDetailString() => $"{itemCategory}-{itemName} {numOfStock}個";

            public M_Test(string itemCategory, string itemName, int numOfStock)
            {
                this.itemCategory = itemCategory;
                this.itemName = itemName;
                this.numOfStock = numOfStock;
            }
        }

    } // class
#endif

} // namespace
