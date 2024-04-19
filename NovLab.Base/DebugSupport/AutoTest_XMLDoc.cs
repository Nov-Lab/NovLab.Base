// @(h)AutoTest_XMLDoc.cs ver 0.00 ( '24.04.21 Nov-Lab ) 作成開始
// @(h)AutoTest_XMLDoc.cs ver 0.11 ( '24.04.22 Nov-Lab ) アルファ版完成
// @(h)AutoTest_XMLDoc.cs ver 0.11a( '24.04.23 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【AutoTestクラス用XMLコメント継承元】<inheritdoc> タグで継承するための XML ドキュメント コメントを用意したダミークラスです。

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// ＜メモ＞
// ・inheritdoc を使って共通部分を継承する場合
//   メリット  ：同じ説明文を個別に修正する必要がないのでメンテナンスしやすい。
//   デメリット：各要素をポイントしなければ説明が表示されないため、ひと目ではわかりにくい。印刷物上では説明文がまったくわからない。
//
// ・inheritdoc を使わない場合
//   メリット  ：ソースコードをひと目見ただけでわかりやすい。
//   デメリット：メンテナンスがしにくい。
//
// ・XMLファイルに出力してそれを <include> すれば不要なダミークラスがバイナリーファイルに含まれなくて済みそうだが、こちらの方がメンテナンスがしやすそう。


#pragma warning disable IDE0060 // 未使用のパラメーターを削除します

#if DEBUG   // DEBUGビルドのみ使用可能
namespace NovLab.DebugSupport
{
    public static partial class AutoTest
    {
        // ＜メモ＞
        // ・AutoTest クラス専用なので内部クラスとして作成している。
        //====================================================================================================
        /// <summary>
        /// 【XMLコメント継承元】AutoTestクラス用<br/>
        /// &lt;inheritdoc&gt; タグで継承するための XML ドキュメント コメントを用意したダミークラスです。
        /// </summary>
        //====================================================================================================
        private partial class XMLDOC
        {
            //====================================================================================================
            /// <summary>
            /// 【XMLコメント継承元】Test / TextX / TestAsync / TestXAsync メソッドのオーバーロード用<br/>
            /// &lt;inheritdoc&gt; タグで継承するための XML ドキュメント コメントを用意したダミークラスです。
            /// </summary>
            //====================================================================================================
            private partial class Test
            {
                //====================================================================================================
                // 引数以外の共通部分(引数違い、および通常メソッド／拡張メソッドのすべてのオーバーロードで継承する部分)
                // ・returns と remarks の XML ドキュメントをここで用意する。
                // ・summary は各オーバーロード側で固有の説明を記述する。
                // ・typeparam と param は各要素ごとに用意したダミーメソッドから継承する。
                //====================================================================================================

                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Common_Action_Sync"/>
                //--------------------------------------------------------------------------------
                // 【XMLコメント継承元】Test / TextX のうち、戻り値なしAction用オーバーロードの共通部分
                // ・同期的メソッド用のテストメソッドなので戻り値はない。
                //--------------------------------------------------------------------------------
                /// <remarks>
                /// 補足<br/>
                /// ・予想結果には、予想される実行結果を格納したインスタンス、または例外の型情報を指定します。<br/>
                /// </remarks>
                //--------------------------------------------------------------------------------
                public void Common_Action_Sync() { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Common_Func_Sync"/>
                //--------------------------------------------------------------------------------
                // 【XMLコメント継承元】Test / TextX のうち、戻り値ありFunc用オーバーロードの共通部分
                // ・同期的メソッド用のテストメソッドなので戻り値はない。
                //--------------------------------------------------------------------------------
                /// <remarks>
                /// 補足<br/>
                /// ・予想結果には <typeparamref name="TResult"/> 型の戻り値、または例外の型情報を指定します。<br/>
                /// </remarks>
                //--------------------------------------------------------------------------------
                public void Common_Func_Sync<TResult>() { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Common_Action_Async"/>
                //--------------------------------------------------------------------------------
                // 【XMLコメント継承元】TestAsync / TextXAsync のうち、戻り値なしAction用オーバーロードの共通部分
                // ・非同期メソッド用のテストメソッドなので Task 型の戻り値を返す。
                //--------------------------------------------------------------------------------
                /// <returns>
                /// 戻り値を返さない非同期操作タスク
                /// </returns>
                /// <remarks>
                /// 補足<br/>
                /// ・予想結果には、予想される実行結果を格納したインスタンス、または例外の型情報を指定します。<br/>
                /// </remarks>
                //--------------------------------------------------------------------------------
                public async Task Common_Action_Async() { await Task.Yield(); }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Common_Func_Async"/>
                //--------------------------------------------------------------------------------
                // 【XMLコメント継承元】TestAsync / TextXAsync のうち、戻り値ありFunc用オーバーロードの共通部分
                // ・非同期メソッド用のテストメソッドなので Task 型の戻り値を返す。
                //--------------------------------------------------------------------------------
                /// <returns>
                /// 戻り値を返さない非同期操作タスク
                /// </returns>
                /// <remarks>
                /// 補足<br/>
                /// ・予想結果には <typeparamref name="TResult"/> 型の戻り値、または例外の型情報を指定します。<br/>
                /// </remarks>
                //--------------------------------------------------------------------------------
                public async Task Common_Func_Async<TResult>() { await Task.Yield(); }


                //====================================================================================================
                // 引数説明用ダミーメソッド
                // ・型パラメーター名や引数名は同じでなくても良いが、合わせておいた方がわかりやすい。
                //====================================================================================================

                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.TargetInstance"/>
                /// <typeparam name="TTarget">   対象インスタンスの型</typeparam>
                /// <param name="targetInstance">[in ]：対象インスタンス</param>
                public void TargetInstance<TTarget>(TTarget targetInstance) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.ExpectResult"/>
                /// <typeparam name="TResult"> テスト対象Funcが返す戻り値の型</typeparam>
                /// <param name="expectResult">[in ]：予想結果</param>
                public void ExpectResult<TResult>(TResult expectResult) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.ActTestTarget"/>
                /// <param name="actTestTarget">[in ]：テスト対象Action</param>
                public void ActTestTarget(int actTestTarget) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.FncTestTarget"/>
                /// <param name="fncTestTarget">[in ]：テスト対象Func</param>
                public void FncTestTarget(int fncTestTarget) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.TestOptions"/>
                /// <param name="testOptions">[in ]：テストパターン名またはテストオプション[null = 省略]</param>
                public void TestOptions(int testOptions) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Arg1"/>
                /// <typeparam name="T">引数１の型</typeparam>
                /// <param name="param">[in ]：引数１</param>
                public void Arg1<T>(T param) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Arg2"/>
                /// <typeparam name="T">引数２の型</typeparam>
                /// <param name="param">[in ]：引数２</param>
                public void Arg2<T>(T param) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Arg3"/>
                /// <typeparam name="T">引数３の型</typeparam>
                /// <param name="param">[in ]：引数３</param>
                public void Arg3<T>(T param) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Arg4"/>
                /// <typeparam name="T">引数４の型</typeparam>
                /// <param name="param">[in ]：引数４</param>
                public void Arg4<T>(T param) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Arg5"/>
                /// <typeparam name="T">引数５の型</typeparam>
                /// <param name="param">[in ]：引数５</param>
                public void Arg5<T>(T param) { }


                // 【継承先の構文】<inheritdoc cref="XMLDOC.Test.Arg6"/>
                /// <typeparam name="T">引数６の型</typeparam>
                /// <param name="param">[in ]：引数６</param>
                public void Arg6<T>(T param) { }

            } // class Test

        } // class XMLDoc

    } // class AutoTest

} // namespace
#endif

#pragma warning restore IDE0060 // 未使用のパラメーターを削除します
