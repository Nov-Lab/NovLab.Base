// @(h)AsyncMethodOptions.cs ver 0.00 ( '24.01.25 Nov-Lab ) 作成開始
// @(h)AsyncMethodOptions.cs ver 0.51 ( '24.01.27 Nov-Lab ) ベータ版完成

// @(s)
// 　【非同期メソッド実行オプション設定】非同期メソッドの実行処理に関するオプション設定を管理します。

using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace NovLab.Threading
{
    //====================================================================================================
    /// <summary>
    /// 【非同期メソッド実行オプション設定】非同期メソッドの実行処理に関するオプション設定を管理します。
    /// </summary>
    //====================================================================================================
    public class AsyncMethodOptions
    {
        /// <summary>
        /// 【未ハンドル例外の処理ポリシー】非同期メソッド内でハンドルされていない例外が発生した場合の処理方法を指定します。
        /// </summary>
        /// <remarks>
        /// 補足<br></br>
        /// ・既定値は <see cref="AsyncMethodUnhandledExceptionPolicy.Automatic"/> (自動)です。<br></br>
        /// </remarks>
        public AsyncMethodUnhandledExceptionPolicy UnhandledExceptionPolicy { get; set; } = AsyncMethodUnhandledExceptionPolicy.Automatic;

    } // class


    //====================================================================================================
    /// <summary>
    /// 【未ハンドル例外の処理ポリシー】非同期メソッド内でハンドルされていない例外が発生した場合の処理方法を示します。
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・実際の動作内容を決定するときは <see cref="XAsyncMethodUnhandledExceptionPolicy.XDecisionize(AsyncMethodUnhandledExceptionPolicy)"/> で確定値化します。<br></br>
    /// </remarks>
    //====================================================================================================
    public enum AsyncMethodUnhandledExceptionPolicy
    {
        /// <summary>
        /// 【自動(既定値)】デバッグ版では「非同期メソッド内で中断」になり、リリース版では「呼び出し元へ再スロー」になります。
        /// </summary>
        Automatic = 0,

        /// <summary>
        /// 【非同期メソッド内で中断】Thread クラスと同様に例外発生場所で即座に中断します。<br></br>
        /// 非同期メソッド内で発生した未ハンドル例外をデバッグすることができます。<br></br>
        /// デバッグ版の既定の動作です。<br></br>
        /// </summary>
        BreakInAsyncMethod,

        /// <summary>
        /// 【呼び出し元へ再スロー】Task クラスと同様に呼び出し元スレッドで再スローします。<br></br>
        /// 呼び出し元での例外処理をデバッグすることができます。<br></br>
        /// リリース版の既定の動作です。<br></br>
        /// </summary>
        RethrowToCaller,

    }
    // ↑の拡張メソッドがこちら↓
    //====================================================================================================
    /// <summary>
    /// 【AsyncMethodUnhandledExceptionPolicy 拡張メソッド】AsyncMethodUnhandledExceptionPolicy 列挙体に拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XAsyncMethodUnhandledExceptionPolicy
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【確定値化】未ハンドル例外の処理ポリシー(設定値)から、確定値(ビルド構成に合わせて「自動」の動作内容を決定したもの)を取得します。
        /// </summary>
        /// <param name="target">[in ]：未ハンドル例外の処理ポリシー(設定値)</param>
        /// <returns>
        /// 未ハンドル例外の処理ポリシー(確定値)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static AsyncMethodUnhandledExceptionPolicy XDecisionize(this AsyncMethodUnhandledExceptionPolicy target)
        {
            //------------------------------------------------------------
            /// 未ハンドル例外の処理ポリシー(設定値)から確定値を取得する
            //------------------------------------------------------------
            switch (target)
            {                                                                       //// 未ハンドル例外の処理ポリシー(設定値)で分岐
                case AsyncMethodUnhandledExceptionPolicy.Automatic:                 /////  ・「自動」の場合
#if DEBUG                                                                           //////   デバッグ版の場合
                    return AsyncMethodUnhandledExceptionPolicy.BreakInAsyncMethod;  ///////    戻り値 = 「非同期メソッド内で中断」 で関数終了
#else                                                                               //////   リリース版の場合
                    return AsyncMethodUnhandledExceptionPolicy.RethrowToCaller;     //////     戻り値 = 「呼び出し元へ再スロー」 で関数終了
#endif

                case AsyncMethodUnhandledExceptionPolicy.BreakInAsyncMethod:        /////  ・「非同期メソッド内で中断」の場合
                case AsyncMethodUnhandledExceptionPolicy.RethrowToCaller:           /////  ・「呼び出し元へ再スロー」の場合
                    return target;                                                  //////   設定値をそのまま戻り値とし、関数終了

                default:                                                            /////  ・上記以外の場合
                    throw new InvalidOperationException(                            //////   操作不正例外をスローする
                        $"Invalid {nameof(AsyncMethodUnhandledExceptionPolicy)}:{target}");
            }
        }

    } // class

} // namespace
