// @(h)TimeoutStopwatch.cs ver 0.00 ( '24.05.01 Nov-Lab ) 作成開始
// @(h)TimeoutStopwatch.cs ver 0.51 ( '24.05.01 Nov-Lab ) ベータ版完成
// @(h)TimeoutStopwatch.cs ver 0.51a( '24.05.02 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【タイムアウトストップウォッチ】タイムアウト時間の計測と判定に用いるストップウォッチです。


using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【タイムアウトストップウォッチ】タイムアウト時間の計測と判定に用いるストップウォッチです。
    /// </summary>
    /// <remarks>
    /// 補足<br/>
    /// ・タイムアウト判定処理を簡素化するのに適したユーティリティークラスです。<br/>
    /// ・タイムアウト時間に <see cref="Timeout.Infinite"/>(-1) を指定することで、無期限に待機することもできます。<br/>
    /// <br/>
    /// 使用例<br/>
    /// <code>
    /// ｜var swTimeout = TimeoutStopwatch.StartNew(timeoutMsec);     //// タイムアウトストップウォッチを生成して開始する
    /// ｜
    /// ｜do
    /// ｜{                                                           //// タイムアウトするまで、最低一回は繰り返す
    /// ｜    ＜処理本体＞
    /// ｜        ：
    /// ｜    if (finished)
    /// ｜    {                                                       /////  処理が完了した場合
    /// ｜        return;                                             //////   関数終了
    /// ｜    }
    /// ｜    cancellationToken.ThrowIfCancellationRequested();       /////  キャンセルトークンをチェックし、取り消し状態の場合は例外をスローする
    /// ｜} while (swTimeout.IsTimeout == false);
    /// </code>
    /// </remarks>
    //====================================================================================================
    public partial class TimeoutStopwatch
    {
        //====================================================================================================
        // 内部フィールド
        //====================================================================================================

        /// <summary>
        /// 【ストップウォッチ】経過時間の計測に使います。
        /// </summary>
        protected readonly Stopwatch m_stopwatch;


        //====================================================================================================
        // 公開プロパティー
        //====================================================================================================

        /// <summary>
        /// 【タイムアウト時間(ミリ秒)】無期限に待機する場合は <see cref="Timeout.Infinite"/>(-1)
        /// </summary>
        protected int TimeoutMsec { get; set; }


        /// <summary>
        /// 【タイムアウト状態(読み取り専用)】タイムアウトしたかどうかをチェックして取得します。<br/>
        /// [true = タイムアウトした / false = タイムアウトしていない]
        /// </summary>
        public bool IsTimeout
        {
            get
            {
                //------------------------------------------------------------
                /// タイムアウトしたかどうかをチェックする
                //------------------------------------------------------------
                if (TimeoutMsec == Timeout.Infinite)
                {                                                           //// タイムアウト時間 = 無期限に待機 の場合
                    return false;                                           /////  戻り値 = false(タイムアウトしていない) で関数終了
                }

                if (m_stopwatch.ElapsedMilliseconds <= TimeoutMsec)
                {                                                           //// 経過時間 <= タイムアウト時間 の場合
                    return false;                                           /////  戻り値 = false(タイムアウトしていない) で関数終了
                }
                else
                {                                                           //// 経過時間 > タイムアウト時間 の場合
                    return true;                                            /////  戻り値 = true(タイムアウトした) で関数終了
                }
            }
        }


        //====================================================================================================
        // コンストラクターと static 公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【新規開始】タイムアウトストップウォッチを新規生成して計測を開始します。
        /// </summary>
        /// <param name="timeoutMsec"><inheritdoc cref="TimeoutStopwatch(int)" path="/param[@name='timeoutMsec']"/></param>
        /// <returns>
        /// 生成したインスタンス
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・このメソッドは、<see cref="TimeoutStopwatch(int)"/> コンストラクターを呼び出してから、新しいインスタンスで <see cref="Start"/> を呼び出すのと同じです。
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static TimeoutStopwatch StartNew(int timeoutMsec)
        {
            //------------------------------------------------------------
            /// タイムアウトストップウォッチを新規生成して開始する
            //------------------------------------------------------------
            var newInstance = new TimeoutStopwatch(timeoutMsec);        //// 新しいインスタンスを生成する
            newInstance.Start();                                        //// 経過時間の計測を開始する
            return newInstance;                                         //// 戻り値 = 生成したインスタンス で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コンストラクター】必要な情報を指定してタイムアウトストップウォッチの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="timeoutMsec">[in ]：タイムアウト時間(ミリ秒)。無期限に待機する場合は <see cref="Timeout.Infinite"/>(-1)</param>
        /// <remarks>
        /// 補足<br/>
        /// ・経過時間の計測をすぐに開始する場合は <see cref="StartNew"/> を呼び出す方が便利です。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public TimeoutStopwatch(int timeoutMsec)
        {
            //------------------------------------------------------------
            /// 引数をチェックする
            //------------------------------------------------------------
            if (timeoutMsec < -1)
            {                                                           //// タイムアウト時間(ミリ秒)が -1 以外の負数の場合
                throw                                                   /////  引き数不正例外(値範囲)をスローする
                    new ArgumentOutOfRangeException(nameof(timeoutMsec));
            }


            //------------------------------------------------------------
            /// 新しいインスタンスを生成する
            //------------------------------------------------------------
            m_stopwatch = new Stopwatch();                              //// ストップウォッチを生成する
            TimeoutMsec = timeoutMsec;                                  //// タイムアウト時間をプロパティーに設定する
        }


        //====================================================================================================
        // Stopwatch のメソッドやプロパティーを透過するもの
        //====================================================================================================

        /// <summary>
        /// 【計測開始】経過時間の計測を開始または再開します。
        /// </summary>
        public void Start() => m_stopwatch.Start();

        /// <summary>
        /// 【計測停止】経過時間の計測を停止します。
        /// </summary>
        public void Stop() => m_stopwatch.Stop();

        /// <summary>
        /// 【リセット】経過時間の計測を停止して、経過時間をゼロにリセットします。
        /// </summary>
        public void Reset() => m_stopwatch.Reset();

        /// <summary>
        /// 【リスタート】経過時間の計測を停止し、経過時間をゼロにリセットして、経過時間の計測を再度開始します。
        /// </summary>
        public void Restart() => m_stopwatch.Restart();

        /// <summary>
        /// 【計測中状態(読み取り専用)】経過時間を計測中かどうかを取得します。[true = 計測中 / false = 停止中]
        /// </summary>
        public bool IsRunning => m_stopwatch.IsRunning;

        /// <summary>
        /// 【経過時間(ミリ秒)(読み取り専用)】経過時間をミリ秒単位で取得します。
        /// </summary>
        public long ElapsedMilliseconds => m_stopwatch.ElapsedMilliseconds;


        //--------------------------------------------------------------------------------
        // 手動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [ManualTestMethod(nameof(TimeoutStopwatch) + " の総合的テスト")]
        public static void ZZZ_TimeoutStopwatch()
        {
            Debug.Print("＜正常系＞");
            Test(1);                // タイムアウト時間 = 1ミリ秒 でテスト
            Test(100);              // タイムアウト時間 = 100ミリ秒 でテスト
            Test(1000);             // タイムアウト時間 = 1000ミリ秒 でテスト
            Test(Timeout.Infinite); // タイムアウト時間 = 無期限 でテスト

            Debug.Print("＜引数不正パターン＞");
            try
            {
                Test(-2);           // タイムアウト時間 = -2 でテスト(引数不正パターン)
            }
            catch (Exception ex)
            {
                Debug.Print("例外：" + ex.Message);
                Debug.Unindent();
                Debug.Print("");
            }

            Debug.Print("テスト終了");


            //------------------------------------------------------------
            /// 【ローカル関数】テスト本体
            //------------------------------------------------------------
            void Test(int timeoutMsec)  // [in ]：タイムアウト時間(ミリ秒)
            {
                const int TEST_LIMIT_MSEC = 3000;   // テスト実行期限(ミリ秒)

                Debug.Print($"・タイムアウト時間 = {RegularString.TimeoutMsecString(timeoutMsec)} でテスト開始");
                Debug.Indent();

                var swTimeout = TimeoutStopwatch.StartNew(timeoutMsec); //// タイムアウトストップウォッチを新規開始する
                PrintStatus(swTimeout);                                 //// タイムアウトストップウォッチ状態表示処理を行う

                var spinner = new SpinWait();                           //// スピン待機オブジェクトを生成する
                while(swTimeout.IsTimeout == false)
                {                                                       //// タイムアウトするまで繰り返す
                    spinner.SpinOnce();                                 /////  単一スピン待機する
                    if (swTimeout.ElapsedMilliseconds > TEST_LIMIT_MSEC)
                    {                                                   /////  テスト実行期限を過ぎた場合
                        Debug.Print("テスト実行期限を過ぎてもタイムアウトしなかったのでループ終了");
                        break;                                          //////   繰り返し処理を抜ける
                    }
                }
                swTimeout.Stop();                                       //// タイムアウトストップウォッチを停止する
                PrintStatus(swTimeout);                                 //// タイムアウトストップウォッチ状態表示処理を行う

                Debug.Unindent();
                Debug.Print("");
            }


            //------------------------------------------------------------
            /// 【ローカル関数】タイムアウトストップウォッチ状態表示
            //------------------------------------------------------------
            void PrintStatus(TimeoutStopwatch target)
            {
                Debug.Print($"タイムアウト状態：{target.IsTimeout}, " + 
                            $"タイムアウト時間：{RegularString.TimeoutMsecString(target.TimeoutMsec)}, " +
                            $"経過時間：{target.ElapsedMilliseconds}ms");
            }
        }
#endif

    } // class

} // namespace
