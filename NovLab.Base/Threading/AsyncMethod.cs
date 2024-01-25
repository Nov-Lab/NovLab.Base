// @(h)AsyncMethod.cs ver 0.00 ( '24.01.25 Nov-Lab ) 作成開始
// @(h)AsyncMethod.cs ver 0.51 ( '24.01.27 Nov-Lab ) ベータ版完成

// @(s)
// 　【非同期メソッド】非同期メソッドを別スレッドで実行する機能を提供します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace NovLab.Threading
{
    // ＜作成動機＞
    // ・まずは：Thread クラスは戻り値や未ハンドル例外が受け取れないので、単発処理型の非同期メソッド呼び出しには不向き。
    //   一方で：Task クラスは戻り値や未ハンドル例外も受け取れる半面、未ハンドル例外のデバッグがしにくい(*1)
    //   なので：戻り値や未ハンドル例外を受け取れて、なおかつ未ハンドル例外のデバッグもしやすいクラスを作ってみた。
    //         ：ついでに引数の型指定ができるようにしてタイプセーフ化してみた。
    //
    // ＜機能比較＞
    //   機能          ：Task クラス      ：Thread クラス    ：AsyncMethod クラス
    //   --------------：-----------------：-----------------：------------------
    //   引数          ：object のみ渡せる：object のみ渡せる：型指定で渡せる
    //   戻り値        ：受け取れる       ：受け取れない     ：受け取れる
    //   未ハンドル例外：受け取れる       ：受け取れない     ：受け取れる
    //   例外のデバッグ：しにくい(*1)     ：しやすい         ：しやすい
    //
    // *1 呼び出し元でキャッチしても非同期メソッド内のデバッグはしにくいし、
    //    キャッチしないと Program.cs の Application.Run にまで戻されてしまって厄介。

    //[-] 保留：Task クラスのように HostProtectionAttribute を付けた方がいい？

    // ＜メモ＞
    // ・特長に挙げた２点が Task クラスで実現されれば、このクラスは不要になる。
    //====================================================================================================
    /// <summary>
    /// 【非同期メソッド】非同期メソッドを別スレッドで実行する機能を提供します。
    /// </summary>
    /// <remarks>
    /// ＜特長＞Task クラスを用いた非同期メソッド呼び出しと似ていますが、以下の特長があります。<br></br>
    /// ①非同期メソッド内で未ハンドル例外が発生した場合に、例外発生場所で即座にデバッグを中断できます。<br></br>
    ///   ・Task クラスのように呼び出し元でキャッチする仕組みだと、非同期メソッド内のデバッグがやりにくいです。<br></br>
    ///   ・既定では、デバッグ版では例外発生場所で即座に中断し、リリース版では呼び出し元へ再スローします。オプション設定で動作を指定することもできます。<br></br>
    /// ②引数の型を指定できます。<br></br>
    ///   ・Task クラスや Thread クラスのように object型だと、変換処理やチェック処理が必要になります。<br></br>
    /// </remarks>
    //====================================================================================================
    public class AsyncMethod
    {
        //====================================================================================================
        // 公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期メソッド実行】パラメーターなしの非同期アクションを別スレッドで実行し、
        /// 終了するまでスレッドをブロックせずに非同期で待機します。
        /// </summary>
        /// <param name="asyncAction">[in ]：パラメーターなしの非同期アクション</param>
        /// <param name="options">    [in ]：非同期メソッド実行オプション設定(null = 既定の設定)</param>
        //--------------------------------------------------------------------------------
        public async static Task RunWait(Action asyncAction, AsyncMethodOptions options = null)
        {
            await M_RunWaitCore<M_Void, M_Void>(CB_AsyncMethodInfoSet, default, options);

            void CB_AsyncMethodInfoSet(M_Worker<M_Void, M_Void> threadWorker)
            {
                threadWorker.AsyncActionNonParam = asyncAction;
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期メソッド実行】パラメーター付きの非同期アクションを別スレッドで実行し、
        /// 終了するまでスレッドをブロックせずに非同期で待機します。
        /// </summary>
        /// <typeparam name="TParam">非同期メソッドへ渡す引数の型</typeparam>
        /// <param name="asyncAction">[in ]：パラメーター付きの非同期アクション</param>
        /// <param name="param">      [in ]：非同期メソッドへ渡す引数</param>
        /// <param name="options">    [in ]：非同期メソッド実行オプション設定(null = 既定の設定)</param>
        //--------------------------------------------------------------------------------
        public async static Task RunWait<TParam>(Action<TParam> asyncAction, TParam param, AsyncMethodOptions options = null)
        {
            await M_RunWaitCore<TParam, M_Void>(CB_AsyncMethodInfoSet, param, options);

            void CB_AsyncMethodInfoSet(M_Worker<TParam, M_Void> threadWorker)
            {
                threadWorker.AsyncActionWithParam = asyncAction;
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期メソッド実行】パラメーターなしの非同期関数を別スレッドで実行し、
        /// 終了するまでスレッドをブロックせずに非同期で待機します。
        /// </summary>
        /// <typeparam name="TResult">非同期メソッドが返す戻り値の型</typeparam>
        /// <param name="asyncFunc">[in ]：パラメーターなしの非同期関数</param>
        /// <param name="options">  [in ]：非同期メソッド実行オプション設定(null = 既定の設定)</param>
        /// <returns>
        /// 非同期メソッドからの戻り値
        /// </returns>
        //--------------------------------------------------------------------------------
        public async static Task<TResult> RunWait<TResult>(Func<TResult> asyncFunc, AsyncMethodOptions options = null)
        {
            return await M_RunWaitCore<M_Void, TResult>(CB_AsyncMethodInfoSet, default, options);

            void CB_AsyncMethodInfoSet(M_Worker<M_Void, TResult> threadWorker)
            {
                threadWorker.AsyncFuncNonParam = asyncFunc;
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期メソッド実行】パラメーター付きの非同期関数を別スレッドで実行し、
        /// 終了するまでスレッドをブロックせずに非同期で待機します。
        /// </summary>
        /// <typeparam name="TParam"> 非同期メソッドへ渡す引数の型</typeparam>
        /// <typeparam name="TResult">非同期メソッドが返す戻り値の型</typeparam>
        /// <param name="asyncFunc">[in ]：パラメーター付きの非同期関数</param>
        /// <param name="param">    [in ]：非同期メソッドへ渡す引数</param>
        /// <param name="options">  [in ]：非同期メソッド実行オプション設定(null = 既定の設定)</param>
        /// <returns>
        /// 非同期メソッドからの戻り値
        /// </returns>
        //--------------------------------------------------------------------------------
        public async static Task<TResult> RunWait<TParam, TResult>(Func<TParam, TResult> asyncFunc, TParam param, AsyncMethodOptions options = null)
        {
            return await M_RunWaitCore<TParam, TResult>(CB_AsyncMethodInfoSet, param, options);

            void CB_AsyncMethodInfoSet(M_Worker<TParam, TResult> threadWorker)
            {
                threadWorker.AsyncFuncWithParam = asyncFunc;
            }
        }


        //====================================================================================================
        // 内部メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期メソッド実行コア】非同期メソッドを別スレッドで実行し、終了するまでスレッドをブロックせずに非同期で待機します。
        /// </summary>
        /// <typeparam name="TParam"> 非同期メソッドへ渡す引数の型(<see cref="M_Void"/> = 引数を使用しない)</typeparam>
        /// <typeparam name="TResult">非同期メソッドが返す戻り値の型(<see cref="M_Void"/> = 戻り値を使用しない)</typeparam>
        /// <param name="actAsyncMethodInfoSet">[in ]：非同期メソッド情報設定コールバック</param>
        /// <param name="param">                [in ]：非同期メソッドへ渡す引数(引数を使用しない場合は default を指定します)</param>
        /// <param name="options">              [in ]：非同期メソッド実行オプション設定(null = 既定の設定)</param>
        /// <returns>
        /// 非同期メソッドからの戻り値
        /// </returns>
        //--------------------------------------------------------------------------------
        protected async static Task<TResult> M_RunWaitCore<TParam, TResult>(Action<M_Worker<TParam, TResult>> actAsyncMethodInfoSet, TParam param, AsyncMethodOptions options = null)
        {
            //------------------------------------------------------------
            /// 非同期メソッドを別スレッドで実行する
            //------------------------------------------------------------
            if (options == null)
            {                                                           //// 非同期メソッド実行オプション設定 = null の場合(省略された場合)
                options = new AsyncMethodOptions();                     /////  既定値で非同期メソッド実行オプション設定を生成する
            }

            var threadWorker = new M_Worker<TParam, TResult>
            {                                                           //// 非同期メソッドワーカーを生成する
                ParamInfo = param,                                      /////  引数情報を設定する
                Options = options,                                      /////  非同期メソッド実行オプション設定を設定する
            };

            actAsyncMethodInfoSet(threadWorker);                        //// 非同期メソッド情報設定コールバック処理を呼び出す

            var thread = new Thread(threadWorker.RunWorker);            //// ワーカースレッドを生成する
            thread.IsBackground = true;                                 //// バックグラウンドスレッドにする(Task クラスと同様)

            thread.Start();                                             //// ワーカースレッドを開始する
            while (thread.IsAlive)
            {                                                           //// ワーカースレッドが実行されている間、繰り返す
                await Task.Delay(10);                                   /////  10ミリ秒待機する
            }

            if (threadWorker.ExceptionInfo != null)
            {                                                           //// 例外情報が設定されている場合(ワーカースレッドでハンドルされていない例外が発生した場合)
                throw threadWorker.ExceptionInfo;                       /////  呼び出し元スレッドで例外を再スローする
            }

            return threadWorker.ResultInfo;                             //// 戻り値 = 戻り値情報 で関数終了
        }


        //====================================================================================================
        // 内部クラス
        //====================================================================================================

        //================================================================================
        /// <summary>
        /// 【Void型】引数や戻り値を使用しないことを示すためのダミーの型です。
        /// </summary>
        //================================================================================
        protected struct M_Void { }


        //================================================================================
        /// <summary>
        /// 【非同期メソッドワーカー】非同期メソッドを別スレッド側で実行するためのワーカーです。
        /// 呼び出し元スレッドとの間で受け渡しをする情報の管理もします。
        /// </summary>
        /// <typeparam name="TParam"> 非同期メソッドへ渡す引数の型</typeparam>
        /// <typeparam name="TResult">非同期メソッドが返す戻り値の型</typeparam>
        //================================================================================
        protected class M_Worker<TParam, TResult>
        {
            // ＜メモ＞
            // ・呼び出し元スレッドとワーカー側スレッドで同時にアクセスしない前提なので、排他ロック制御は省略
            //================================================================================
            // 公開プロパティー(呼び出し元スレッドとワーカー側スレッドとの間で受け渡しをする情報)
            //================================================================================
            /// <summary>
            /// 【戻り値情報】非同期メソッドからの戻り値です。
            /// </summary>
            public TResult ResultInfo { get; set; }

            /// <summary>
            /// 【引数情報】非同期メソッドへ渡す引数です。
            /// </summary>
            public TParam ParamInfo { get; set; }

            /// <summary>
            /// 【例外情報】非同期メソッド内で発生した例外の情報です。(null = 例外なし)
            /// </summary>
            public Exception ExceptionInfo { get; set; }

            /// <summary>
            /// 【非同期メソッド実行オプション設定】非同期メソッドの実行処理に関するオプション設定です。
            /// </summary>
            public AsyncMethodOptions Options { get; set; }


            // ＜メモ＞
            // ・呼び出し元スレッドとワーカー側スレッドで同時にアクセスしない前提なので、排他ロック制御は省略
            // ・同時に複数セットしないように注意
            //================================================================================
            // 公開プロパティー(非同期メソッドのデリゲートインスタンス)
            //================================================================================
            /// <summary>
            /// 【パラメーターなし非同期アクション】ワーカースレッド側で実行する非同期メソッド(引数なし、戻り値なし)のデリゲートインスタンスです。
            /// </summary>
            public Action AsyncActionNonParam { get; set; }

            /// <summary>
            /// 【パラメーター付き非同期アクション】ワーカースレッド側で実行する非同期メソッド(引数あり、戻り値なし)のデリゲートインスタンスです。
            /// </summary>
            public Action<TParam> AsyncActionWithParam { get; set; }

            /// <summary>
            /// 【パラメーターなし非同期関数】ワーカースレッド側で実行する非同期メソッド(引数なし、戻り値あり)のデリゲートインスタンスです。
            /// </summary>
            public Func<TResult> AsyncFuncNonParam { get; set; }

            /// <summary>
            /// 【パラメーター付き非同期関数】ワーカースレッド側で実行する非同期メソッド(引数あり、戻り値あり)のデリゲートインスタンスです。
            /// </summary>
            public Func<TParam, TResult> AsyncFuncWithParam { get; set; }


            //================================================================================
            // メソッド
            //================================================================================

            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【ワーカー実行】非同期メソッドをワーカースレッドで実行する本体部分です。プロパティー設定に従って非同期メソッドを実行します。
            /// </summary>
            /// <remarks>
            /// 補足<br></br>
            /// ・<see cref="ThreadStart"/> デリゲートに適合しています。<br></br>
            /// </remarks>
            //--------------------------------------------------------------------------------
            public void RunWorker()
            {
                //------------------------------------------------------------
                /// プロパティー設定に従って非同期メソッドを実行する
                //------------------------------------------------------------
                if (Options.UnhandledExceptionPolicy.XDecisionize() == AsyncMethodUnhandledExceptionPolicy.RethrowToCaller)
                {                                                           //// 未ハンドル例外の処理ポリシー(確定値) = 呼び出し元へ再スロー の場合
                    M_RunMethodWithExceptionTrap();                         /////  例外トラップ付きで非同期メソッド実行処理を行う
                }
                else
                {                                                           //// 未ハンドル例外の処理ポリシー(確定値) = 呼び出し元へ再スロー でない場合
                    M_RunMethod();                                          /////  (例外トラップなしで)非同期メソッド実行処理を行う
                }
            }


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【非同期メソッド実行】(例外トラップなしで)非同期メソッドのデリゲートインスタンスを実行します。
            /// </summary>
            /// <remarks>
            /// 補足<br></br>
            /// ・例外をトラップしていない場合、ハンドルされていない例外が発生したときは例外発生場所で即座に中断されます。<br></br>
            /// </remarks>
            //--------------------------------------------------------------------------------
            protected void M_RunMethod()
            {
                //------------------------------------------------------------
                /// 非同期メソッドのデリゲートインスタンスを実行する
                ///-(引数や戻り値の有無に合わせていずれか一つだけが設定されている)
                //------------------------------------------------------------
                if (AsyncActionNonParam != null)                            //// パラメーターなし非同期アクションが設定されている場合は実行する
                {
                    AsyncActionNonParam();
                }

                if (AsyncActionWithParam != null)                           //// パラメーター付き非同期アクションが設定されている場合は実行する
                {
                    AsyncActionWithParam(ParamInfo);
                }

                if (AsyncFuncNonParam != null)                              //// パラメーターなし非同期関数が設定されている場合は実行して戻り値情報を取得する
                {
                    ResultInfo = AsyncFuncNonParam();
                }

                if (AsyncFuncWithParam != null)                             //// パラメーター付き非同期関数が設定されている場合は実行して戻り値情報を取得する
                {
                    ResultInfo = AsyncFuncWithParam(ParamInfo);
                }
            }


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【例外トラップ付きで非同期メソッド実行】例外トラップ付きで非同期メソッドのデリゲートインスタンスを実行します。
            /// </summary>
            //--------------------------------------------------------------------------------
            protected void M_RunMethodWithExceptionTrap()
            {
                //------------------------------------------------------------
                /// 例外トラップ付きで非同期メソッドのデリゲートインスタンスを実行する
                //------------------------------------------------------------
                try
                {                                                           /// try開始
                    M_RunMethod();                                          ////  非同期メソッド実行処理を行う
                }
                catch (Exception ex)
                {                                                           /// catch：すべての例外
                    ExceptionInfo = ex;                                     ////  例外情報を保持する
                }
            }

        } // class

    } // class

} // namespace
