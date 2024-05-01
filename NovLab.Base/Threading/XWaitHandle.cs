// @(h)XWaitHandle.cs ver 0.00 ( '24.04.19 Nov-Lab ) プロトタイプを元に作成開始
// @(h)XWaitHandle.cs ver 0.51 ( '24.04.23 Nov-Lab ) ベータ版完成
// @(h)XWaitHandle.cs ver 0.52 ( '24.05.02 Nov-Lab ) バグ修正：XWaitAnyAsync と XWaitOneAsync が無期限のタイムアウト時間に対応していなかった不具合を修正した

// @(s)
// 　【WaitHandle 拡張メソッド】WaitHandle クラスに拡張メソッドを追加します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using NovLab.DebugSupport;


namespace NovLab.Threading
{
    //====================================================================================================
    /// <summary>
    /// 【WaitHandle 拡張メソッド】WaitHandle クラスに拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XWaitHandle
    {
        //[-] 保留：必要になったら。XWaitHandle.XWaitAnyAsync 系列メソッドの作成状況 (1/5)
        //   未 XWaitAnyAsync(CancellationToken cancellationToken, params WaitHandle[] waitHandles)
        //   ○ XWaitAnyAsync(int timeoutMsec, CancellationToken cancellationToken, params WaitHandle[] waitHandles)
        //   未 XWaitAnyAsync(TimeSpan timeout, CancellationToken cancellationToken, params WaitHandle[] waitHandles)
        // * 未 XWaitAnyAsync(int timeoutMsec, Boolean exitContext, CancellationToken cancellationToken, params WaitHandle[] waitHandles)
        //   未 XWaitAnyAsync(TimeSpan timeout, Boolean exitContext, CancellationToken cancellationToken, params WaitHandle[] waitHandles)
        // 将来的には * のメソッドを作ってコアとし、他のメソッドはそれを呼び出すようにする予定

        //[-] 保留：必要になったら。XWaitHandle.XWaitOneAsync 系列メソッドの作成状況 (1/5)
        //   未 WaitOneAsync(CancellationToken cancellationToken)
        //   ○ WaitOneAsync(int timeoutMsec, CancellationToken cancellationToken)
        //   未 WaitOneAsync(TimeSpan timeout, CancellationToken cancellationToken)
        // * 未 WaitOneAsync(int timeoutMsec, Boolean exitContext, CancellationToken cancellationToken)
        //   未 WaitOneAsync(TimeSpan timeout, Boolean exitContext, CancellationToken cancellationToken)
        // 将来的には * のメソッドを作ってコアとし、他のメソッドはそれを呼び出すようにする予定

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期ハンドル待機】待機ハンドルがシグナル受信状態になるまで、指定時間だけキャンセル付きで待機します。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">           [in ]：対象インスタンス</param>
        /// <param name="timeoutMsec">      [in ]：タイムアウト時間(ミリ秒)。無期限に待機する場合は <see cref="Timeout.Infinite"/>(-1)</param>
        /// <param name="cancellationToken">[in ]：キャンセルトークン。取り消し要求機能を使わない場合は <see cref="CancellationToken.None"/></param>
        /// <returns>
        /// 待機結果[true = シグナル受信 / false = タイムアウト]<br/>
        /// ※上記戻り値を返す非同期操作タスクを返します。<br/>
        /// </returns>
        /// <exception cref="OperationCanceledException">操作取り消し例外。キャンセルトークンによって取り消し要求が通知されました。</exception>
        /// <exception cref="ArgumentOutOfRangeException">引き数不正例外(値範囲)。timeoutMsec が -1 以外の負数です(-1 は無期限を表します)。</exception>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="WaitHandle.WaitOne(int)"/> にキャンセルトークン監視処理を追加して、なおかつ非同期化した拡張メソッドです。<br/>
        /// ・タイムアウト時間に 0 を指定した場合は、待機ハンドルの状態をテストして、すぐに制御を戻します。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public async static Task<bool> XWaitOneAsync(this WaitHandle target, int timeoutMsec, CancellationToken cancellationToken)
        {
            //------------------------------------------------------------
            /// 待機ハンドルがシグナル受信状態になるまで、指定時間だけキャンセル付きで待機する
            //------------------------------------------------------------
            var idxWaitHandle =
                await XWaitAnyAsync(timeoutMsec, cancellationToken, target);//// 配列指定版の非同期ハンドル待機を行う
            if (idxWaitHandle == 0)
            {                                                               //// シグナル受信状態の場合(インデックス0の待機ハンドルがシグナル受信状態の場合)
                return true;                                                /////  戻り値 = true(シグナル受信) で関数終了
            }
            else
            {                                                               //// タイムアウトした場合
                return false;                                               /////  戻り値 = false(タイムアウト) で関数終了
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期ハンドル待機】いずれかの待機ハンドルがシグナル受信状態になるまで、指定時間だけキャンセル付きで待機します。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="waitHandles">      [in ]：待機ハンドル配列</param>
        /// <param name="timeoutMsec">      [in ]：タイムアウト時間(ミリ秒)。無期限に待機する場合は <see cref="Timeout.Infinite"/>(-1)</param>
        /// <param name="cancellationToken">[in ]：キャンセルトークン。取り消し要求機能を使わない場合は <see cref="CancellationToken.None"/></param>
        /// <returns>
        /// シグナルを受信した待機ハンドル配列のインデックス
        /// (複数の待機ハンドルが同時にシグナル状態になった場合は、シグナル状態になったものの中で一番小さいインデックス)。<br/>
        /// [<see cref="WaitHandle.WaitTimeout"/> = タイムアウト時間内にシグナル状態にならなかった。]<br/>
        /// ※上記戻り値を返す非同期操作タスクを返します。<br/>
        /// </returns>
        /// <exception cref="OperationCanceledException">操作取り消し例外。キャンセルトークンによって取り消し要求が通知されました。</exception>
        /// <exception cref="ArgumentOutOfRangeException">引き数不正例外(値範囲)。timeoutMsec が -1 以外の負数です(-1 は無期限を表します)。</exception>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="WaitHandle.WaitAny(WaitHandle[], int)"/> にキャンセルトークン監視処理を追加して、なおかつ非同期化した拡張メソッドです。<br/>
        /// ・また、<paramref name="waitHandles"/> を可変個引数にして呼び出しやすくしています。<br/>
        /// ・タイムアウト時間に 0 を指定した場合は、待機ハンドルの状態をテストして、すぐに制御を戻します。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public async static Task<int> XWaitAnyAsync(int timeoutMsec, CancellationToken cancellationToken, params WaitHandle[] waitHandles)
        {
            //------------------------------------------------------------
            /// 引数をチェックする(waitHandles 引数のチェックは WaitHandle.WaitAny に任せる)
            //------------------------------------------------------------
            if (timeoutMsec < -1)
            {                                                           //// タイムアウト時間(ミリ秒)が -1 以外の負数の場合
                throw                                                   /////  引き数不正例外(値範囲)をスローする
                    new ArgumentOutOfRangeException(nameof(timeoutMsec));
            }


            //------------------------------------------------------------
            /// すでに取り消し状態の場合は即座に例外をスローする
            //------------------------------------------------------------
            cancellationToken.ThrowIfCancellationRequested();           //// キャンセルトークンをチェックし、取り消し状態の場合はOperationCanceledException例外をスローする


            //------------------------------------------------------------
            /// いずれかの待機ハンドルがシグナル受信状態になるまで、指定時間だけキャンセル付きで待機する
            //------------------------------------------------------------
            var swTimeout = TimeoutStopwatch.StartNew(timeoutMsec);     //// タイムアウトストップウォッチを生成して開始する
            var spinner = new SpinWait();                               //// スピン待機オブジェクトを生成する

            do
            {                                                           //// タイムアウトするまで、最低一回は繰り返す
                //----------------------------------------
                // 待機ハンドルのテスト
                //----------------------------------------
                var idxWaitHandle = WaitHandle.WaitAny(waitHandles, 0); //////   待機ハンドル配列のシグナル受信状態をテストして、シグナルを受信した待機ハンドル配列のインデックスを取得する
                if (idxWaitHandle != WaitHandle.WaitTimeout)
                {                                                       //////   いずれかの待機ハンドルがシグナルを受信した場合
                    return idxWaitHandle;                               ///////    戻り値 = シグナルを受信した待機ハンドル配列のインデックス で関数終了
                }

                if (timeoutMsec == 0)
                {                                                       //////   タイムアウト時間 = 0 の場合
                    break;                                              ///////    繰り返し処理を抜ける(テストだけして待機はしない)
                }


                //----------------------------------------
                // ごく短時間だけ非同期で待機
                //----------------------------------------
                spinner.SpinOnce();                                     //////   単一スピン待機する
                await Task.Yield();                                     //////   他の非同期操作タスクに実行権を譲る
                cancellationToken.ThrowIfCancellationRequested();       //////   キャンセルトークンをチェックし、取り消し状態の場合はOperationCanceledException例外をスローする
            } while (swTimeout.IsTimeout == false);

            return WaitHandle.WaitTimeout;                              //// タイムアウトした場合(ループを抜けた場合)、戻り値 = タイムアウト で関数終了
        }




        //====================================================================================================
        // 自動テスト用のユーティリティーメソッド
        //====================================================================================================
#if DEBUG
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【定型文字列作成(WaitAny戻り値文字列)】
        /// オブジェクトからWaitAny戻り値文字列を作成します。<br/>
        /// <see cref="WaitHandle.WaitTimeout"/>(258) がタイムアウトを意味する戻り値に使用します。<br/>
        /// </summary>
        /// <param name="target">[in ]：対象オブジェクト(タイムアウトの場合は <see cref="WaitHandle.WaitTimeout"/>(258))</param>
        /// <returns>
        /// WaitAny戻り値文字列
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="RegularStringFunc"/> デリゲートに合致しています。<br/>
        /// ・対象オブジェクトが int 型でない場合、対象オブジェクトの文字列形式を返します。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string ZZZ_WaitAnyResultString(object target)
        {
            //------------------------------------------------------------
            /// オブジェクトからWaitAny戻り値文字列を作成する
            //------------------------------------------------------------
            if (target is int intResult)
            {                                                           //// 対象オブジェクトがint型の場合
                if (intResult == WaitHandle.WaitTimeout)
                {                                                       /////  WaitAny戻り値 = WaitHandle.WaitTimeout(タイムアウト) の場合
                    return "WaitHandle.WaitTimeout";                    //////   戻り値 = "WaitHandle.WaitTimeout" で関数終了
                }
                else
                {                                                       /////  WaitAny戻り値 = WaitHandle.WaitTimeout(タイムアウト) でない場合
                    return intResult.ToString();                        //////   戻り値 = WaitAny戻り値の文字列形式 で関数終了
                }
            }
            else
            {                                                           //// 対象オブジェクトがint型でない場合
                return target.ToString();                               /////  戻り値 = 対象オブジェクトの文字列形式 で関数終了
            }
        }
#endif


        //====================================================================================================
        // 自動テスト用メソッド
        //====================================================================================================
#if DEBUG
        // 自動テスト用定数定義(タイミングによる揺らぎがあっても常に安定した結果が出るように値を調整しておくこと)
        private const int SIGNAL0_AFTER_MSEC = 120; // イベント0をシグナル状態にするまでの時間(ミリ秒)         :システムクロック=約15ミリ秒の4倍以上5倍未満
        private const int SIGNAL1_AFTER_MSEC = 80;  // イベント1をシグナル状態にするまでの時間(ミリ秒)         :システムクロック=約15ミリ秒の3倍以上4倍未満
        private const int CANCEL_REQUEST_MSEC = 40; // キャンセルトークンを取り消し状態にするまでの時間(ミリ秒):システムクロック=約15ミリ秒の2倍以上3倍未満
        private const int TIMEOUT_MSEC = 20;        // タイムアウトをテストする場合のタイムアウト時間(ミリ秒)  :システムクロック=約15ミリ秒の1倍以上2倍未満

        // ＜メモ＞
        // ・System.Threading.Timer クラスのタイマー精度はシステムクロックの精度(Windows では約15ミリ秒)。

        // イベントをシグナル状態にするタイマーコールバック
        private static void ZZZ_TimerCallback(object objManualResetEventSlim)
        {
            var target = (ManualResetEventSlim)objManualResetEventSlim;
            target.Set();
        }


        //--------------------------------------------------------------------------------
        // XWaitAnyAsync の自動テスト
        //--------------------------------------------------------------------------------
        [AutoTestMethod(nameof(XWaitHandle) + "." + nameof(XWaitAnyAsync))]
        public async static Task ZZZ_XWaitAnyAsync()
        {
            var timerEvent0 = new ManualResetEventSlim();   // インデックス0用の待機ハンドル
            var timerEvent1 = new ManualResetEventSlim();   // インデックス1用の待機ハンドル

            var waitHandles1 = new WaitHandle[]             // 待機ハンドルが１つの配列
            {
                timerEvent0.WaitHandle,
            };

            var waitHandles2 = new WaitHandle[]             // 待機ハンドルが２つの配列
            {
                timerEvent0.WaitHandle,
                timerEvent1.WaitHandle,
            };


            //--------------------------------------------------------
            //--------------------------------------------------------
            AutoTest.Print("＜XWaitAnyAsync(int, CancellationToken, params WaitHandle[]) オーバーロード＞");

            await SubRoutine(Timeout.Infinite, CancellationToken.None, waitHandles1, 0, "待機ハンドル１つ、１つめ(インデックス0)が無期限待機中にシグナル状態");
            await SubRoutine(Timeout.Infinite, CancellationToken.None, waitHandles2, 1, "待機ハンドル２つ、２つめ(インデックス1)が無期限待機中にシグナル状態");

            await SubRoutine(1000, CancellationToken.None, waitHandles1, 0, "待機ハンドル１つ、１つめ(インデックス0)が時間内にシグナル状態");
            await SubRoutine(1000, CancellationToken.None, waitHandles2, 1, "待機ハンドル２つ、２つめ(インデックス1)が時間内にシグナル状態");

            await SubRoutine(TIMEOUT_MSEC, CancellationToken.None, waitHandles1, WaitHandle.WaitTimeout, "待機ハンドル１つ、タイムアウト");
            await SubRoutine(TIMEOUT_MSEC, CancellationToken.None, waitHandles2, WaitHandle.WaitTimeout, "待機ハンドル２つ、タイムアウト");

            using (var cts = new CancellationTokenSource(CANCEL_REQUEST_MSEC))
            {
                await SubRoutine(Timeout.Infinite, cts.Token, waitHandles1, typeof(OperationCanceledException), "待機ハンドル１つ、無期限待機中にキャンセルトークンで取り消し");
            }

            using (var cts = new CancellationTokenSource(CANCEL_REQUEST_MSEC))
            {
                await SubRoutine(Timeout.Infinite, cts.Token, waitHandles2, typeof(OperationCanceledException), "待機ハンドル２つ、無期限待機中にキャンセルトークンで取り消し");
            }

            using (var cts = new CancellationTokenSource(CANCEL_REQUEST_MSEC))
            {
                await SubRoutine(1000, cts.Token, waitHandles1, typeof(OperationCanceledException), "待機ハンドル１つ、時間内にキャンセルトークンで取り消し");
            }

            using (var cts = new CancellationTokenSource(CANCEL_REQUEST_MSEC))
            {
                await SubRoutine(1000, cts.Token, waitHandles2, typeof(OperationCanceledException), "待機ハンドル２つ、時間内にキャンセルトークンで取り消し");
            }

            await SubRoutine(-2, CancellationToken.None, waitHandles1, typeof(ArgumentOutOfRangeException), "タイムアウト時間に-1以外の負数を指定");


            // 他のオーバーロード XWaitAnyAsync(TimeSpan, CancellationToken, params WaitHandle[]) などを作ったらここに追加


            //--------------------------------------------------------
            // XWaitAnyAsync(int, CancellationToken, params WaitHandle[]) のテスト用サブルーチン
            //--------------------------------------------------------
            async Task SubRoutine(int timeoutMsec,                      // [in ]：タイムアウト時間(ミリ秒)
                                  CancellationToken cancellationToken,  // [in ]：キャンセルトークン
                                  WaitHandle[] waitHandles,             // [in ]：待機ハンドル配列
                                  AutoTestResultInfo<int> expectResult, // [in ]：予想結果(int値 または 例外の型情報)
                                  string testPattern = null)            // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {
                    fncArg1RegularString = RegularString.TimeoutMsecString, // 引数１はタイムアウト時間文字列で表示
                    fncResultRegularString = ZZZ_WaitAnyResultString,       // 戻り値はWaitAny戻り値文字列で表示
                };

                timerEvent0.Reset();
                timerEvent1.Reset();

                using (Timer timer0 = new Timer(ZZZ_TimerCallback, timerEvent0
                                              , SIGNAL0_AFTER_MSEC, Timeout.Infinite),
                             timer1 = new Timer(ZZZ_TimerCallback, timerEvent1
                                              , SIGNAL1_AFTER_MSEC, Timeout.Infinite))  // タイマーでシグナル状態にする
                {
                    await AutoTest.TestAsync(XWaitAnyAsync, timeoutMsec, cancellationToken, waitHandles
                                           , expectResult, testOptions);
                }
            }
        }


        //--------------------------------------------------------------------------------
        // XWaitOneAsync の自動テスト
        //--------------------------------------------------------------------------------
        [AutoTestMethod(nameof(XWaitHandle) + "." + nameof(XWaitOneAsync))]
        public async static Task ZZZ_XWaitOneAsync()
        {
            var timerEvent0 = new ManualResetEventSlim();


            //--------------------------------------------------------
            //--------------------------------------------------------
            AutoTest.Print("＜XWaitOneAsync(int, CancellationToken) オーバーロード＞");

            await SubRoutine(Timeout.Infinite, CancellationToken.None, true, "無期限待機中にシグナル状態");
            await SubRoutine(1000, CancellationToken.None, true, "時間内にシグナル状態");
            await SubRoutine(TIMEOUT_MSEC, CancellationToken.None, false, "タイムアウト");

            using (var cts = new CancellationTokenSource(CANCEL_REQUEST_MSEC))
            {
                await SubRoutine(Timeout.Infinite, cts.Token, typeof(OperationCanceledException), "無期限待機中にキャンセルトークンで取り消し");
            }

            using (var cts = new CancellationTokenSource(CANCEL_REQUEST_MSEC))
            {
                await SubRoutine(1000, cts.Token, typeof(OperationCanceledException), "時間内にキャンセルトークンで取り消し");
            }

            await SubRoutine(-2, CancellationToken.None, typeof(ArgumentOutOfRangeException), "タイムアウト時間に-1以外の負数を指定");


            // 他のオーバーロード XWaitOneAsync(TimeSpan, CancellationToken) などを作ったらここに追加


            //--------------------------------------------------------
            // XWaitOneAsync(int, CancellationToken) のテスト用サブルーチン
            //--------------------------------------------------------
            async Task SubRoutine(int timeoutMsec,                      // [in ]：タイムアウト時間(ミリ秒)
                                  CancellationToken cancellationToken,  // [in ]：キャンセルトークン
                                  AutoTestResultInfo<bool> expectResult,// [in ]：予想結果(bool値 または 例外の型情報)
                                  string testPattern = null)            // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {
                    fncArg1RegularString = RegularString.TimeoutMsecString, // 引数１はタイムアウト時間文字列で表示
                };

                timerEvent0.Reset();

                using (Timer timer0 = new Timer(ZZZ_TimerCallback, timerEvent0
                                              , SIGNAL0_AFTER_MSEC, Timeout.Infinite))  // タイマーでシグナル状態にする
                {
                    await AutoTest.TestXAsync(XWaitOneAsync, timerEvent0.WaitHandle, timeoutMsec, cancellationToken
                                            , expectResult, testOptions);
                }
            }
        }
#endif


        //====================================================================================================
        // 手動テスト用メソッド(実行に時間がかかる＆目視確認も行うため自動テストには適さない)
        //====================================================================================================
#if DEBUG

        // ＜メモ＞
        // ・各種待機方法を比較検討した結果、SpinWait.SpinOnce() を採用することにした。
        //
        // 【待機方法          ：呼び出し元のスレッド：CPU負荷：最小待機時間】
        //   Task.Delay(0)     ：ブロックしない      ：極小   ：待機しない
        //   Task.Delay(1)     ：ブロックしない      ：極小   ：約15ミリ秒(HPの日本語版ドキュメントに、システムクロックに依存すると明記されている)
        //   Thread.Sleep(1)   ：ブロックする        ：極小   ：約15ミリ秒(英語版のヘルプに、システムクロックに依存するような記述がある)
        //   SpinWait.SpinOnce ：ブロックする        ：極小   ：約0.8ミリ秒(テスト環境での実験結果。ただし、明確な規定はなさそう)
        //   Thread.SpinWait(1)：ブロックする        ：極小   ：ほぼゼロ
        //
        [ManualTestMethod("非同期ハンドル待機処理における待機方法の調査検討用")]
        public async static Task ZZZ_WaitMethodTestAsync()
        {
            const int TIMEOUT_MSEC = 3000;  // タイムアウト時間(ミリ秒) = テスト実行時間

            Debug.Print("実行中：CPU使用率グラフもチェックすること");


            //--------------------------------------------------------
            /// 前準備
            //--------------------------------------------------------
            var waitHandles = new WaitHandle[]
            {                                                       //// 待機ハンドル配列を生成する(テスト用ダミーであり、シグナル状態になることはない)
                (new ManualResetEventSlim()).WaitHandle,
            };

            var ctsDummy = new CancellationTokenSource();
            var cancellationToken = ctsDummy.Token;                 //// キャンセルトークンを用意する(テスト用ダミーであり、取り消し状態になることはない)


            //--------------------------------------------------------
            /// テスト本体
            //--------------------------------------------------------
            int repeatCounter = 0;                                  //// 反復回数カウンタ = 0 に初期化する
            var swTimeout = TimeoutStopwatch.StartNew(TIMEOUT_MSEC);//// タイムアウトストップウォッチを生成して開始する
            var spinner = new SpinWait();                           //// スピン待機オブジェクトを生成する

            do
            {                                                       //// タイムアウト時間を経過するまで、最低一回は繰り返す
#if true        // 各種待機方法の純粋な待機時間だけを測定するときは無効化する
                WaitHandle.WaitAny(waitHandles, 0);                 /////  待機ハンドル配列をテストする(テスト用ダミーなので結果は見ない)
                await Task.Yield();                                 /////  他の非同期操作タスクに実行権を譲る
                cancellationToken.ThrowIfCancellationRequested();   /////  キャンセルトークンをチェックする(テスト用ダミーなのでキャンセルは発生しない)
#endif


                // ＜メモ＞
                // ・Task.Delay 以外は呼び出し元スレッドをブロックするため、別途 await Task.Yield(); が必要
                // ・SpinWait.SpinOnce は、システムクロック(約15ミリ秒)よりも短く、
                //   なおかつ短すぎない時間(だいたい1ミリ秒弱？)だけ待機する。
                // ・いずれの待機方法も、待機処理自体のCPU負荷はほぼゼロ。
                //   テスト時にCPU負荷が高くなるのは待機処理以外の部分によるもの。
                //----------------------------------------
                // 各種待機方法のテスト
                // (テストしたいパターンをコメントアウトする)
                //----------------------------------------

                spinner.SpinOnce();                                 // CPU負荷はほぼゼロ。待機時間はごくわずか -> 丁度いい(参考:テスト環境で約0.8ミリ秒)
                // Thread.SpinWait(1);                                 // CPU負荷は高め    。待機時間はほぼゼロ   -> 短すぎる(参考:テスト環境で約0.00017ミリ秒)
                // await Task.Delay(0);                                // CPU負荷は高め    。待機なし             -> 短すぎる(参考:テスト環境で約0.00014ミリ秒)
                // await Task.Delay(1);                                // CPU負荷はほぼゼロ。待機時間は約15ミリ秒 -> 長すぎる
                // Thread.Sleep(1);                                    // CPU負荷はほぼゼロ。待機時間は約15ミリ秒 -> 長すぎる

                repeatCounter++;                                    /////  反復回数カウンタに１加算する
            } while (swTimeout.IsTimeout == false);


            //--------------------------------------------------------
            /// 結果表示
            //--------------------------------------------------------
            var waitTimeMsec = (float)TIMEOUT_MSEC / repeatCounter;

            Debug.Print($"{TIMEOUT_MSEC} ミリ秒の間に {repeatCounter} 回繰り返したので、");
            Debug.Print($"１ループあたりの平均所要時間は {waitTimeMsec} ミリ秒。");
        }
#endif

    } // class

} // namespace
