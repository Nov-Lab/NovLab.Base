// @(h)XMutex.cs ver 0.00 ( '24.06.26 Nov-Lab ) プロトタイプを元に作成開始
// @(h)XMutex.cs ver 0.51 ( '24.06.26 Nov-Lab ) ベータ版完成

// @(s)
// 　【Mutex 拡張メソッド】Mutex クラスに拡張メソッドを追加します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NovLab.DebugSupport;

#if DEBUG
using System.Collections.Concurrent;
#endif


namespace NovLab.Threading
{
    //====================================================================================================
    /// <summary>
    /// 【Mutex 拡張メソッド】Mutex クラスに拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XMutex
    {
        //[-] 保留：XMutex.XWaitOne。必要になったら他のオーバーロードも作成する。

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ミューテックス所有権取得】ミューテックスの所有権を取得します。
        /// 別プロセスや別スレッドに所有権がある場合は、ミューテックスの所有権を取得できるか、タイムアウトするまで、現在のスレッドをブロックします。
        /// </summary>
        /// <param name="target">                         [in ]：対象ミューテックス</param>
        /// <param name="millisecondsTimeout">            [in ]：タイムアウト時間(ミリ秒)。無期限に待機する場合は <see cref="Timeout.Infinite"/>(-1)</param>
        /// <param name="abandonedMutexDetectionCallback">[in ]：放棄ミューテックス検出コールバック[null = ワーニングトレース出力]</param>
        /// <param name="callbackArg">                    [in ]：コールバック引数</param>
        /// <returns>
        /// 処理結果[true = 取得成功 / false = 取得失敗(タイムアウト時間内に所有権を取得できなかった)]
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="WaitHandle.WaitOne(int)"/> について、<see cref="AbandonedMutexException"/>例外への対応を簡略化したバージョンです。<br/>
        /// ・別プロセスや別スレッドが解放せずに終了することによって放棄されたミューテックスが
        ///   残っていたことを検出することはバグ発見に役立ちますが、多くの場合、検出した時点でできることは特段ありません。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static bool XWaitOne(this Mutex target, int millisecondsTimeout,
                                    Action<AbandonedMutexException, object> abandonedMutexDetectionCallback = null, object callbackArg = null)
        {
            //------------------------------------------------------------
            /// ミューテックスの所有権を取得する
            //------------------------------------------------------------
            try
            {                                                           //// try開始
                return target.WaitOne(millisecondsTimeout);             /////  ミューテックスの所有権を取得してその結果を戻り値とし、関数終了
            }
            catch (AbandonedMutexException ex)
            {                                                           //// catch：放棄されたミューテックス例外
                                                                        ////-(別プロセスや別スレッドが解放せずに終了することによって放棄されたミューテックスが残っており、その所有権を取得した場合)
                if (abandonedMutexDetectionCallback != null)
                {                                                       /////  放棄ミューテックス検出コールバックが指定されている場合
                    abandonedMutexDetectionCallback(ex, callbackArg);   //////   コールバック処理を行う
                }
                else
                {                                                       /////  放棄ミューテックス検出コールバックが指定されていない場合
                    M_Warning_AbandonedMutexDetected(ex);               //////   放棄ミューテックス検出ワーニング出力処理を行う
                }
                return true;                                            /////  戻り値 = true(取得成功) で関数終了
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【放棄ミューテックス検出ワーニング出力】
        /// 放棄されたミューテックスを検出したことを知らせるワーニングトレースを出力します。
        /// </summary>
        /// <param name="ex">[in ]：AbandonedMutexException例外</param>
        //--------------------------------------------------------------------------------
        private static void M_Warning_AbandonedMutexDetected(AbandonedMutexException ex)
        {
            Trace.TraceWarning($"Abandoned Mutex Detected:{ex.Message}");
        }

    } // class


#if DEBUG
    //====================================================================================================
    // 手動テスト用クラス
    //====================================================================================================
    public static partial class XMutexTest
    {
        /// <summary>
        /// 【テスト用ミューテックス】
        /// </summary>
        /// <remarks>
        /// 補足<br/>
        /// ・複数のスレッドから同時にアクセスすることはないので排他ロック制御は省略しています。<br/>
        /// </remarks>
        private readonly static Mutex testMutex = new Mutex(false, "AbandonedMutexTest");


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【XMutex.XWaitOne の手動テスト】
        /// </summary>
        //--------------------------------------------------------------------------------
        [ManualTestMethod(nameof(XMutex) + "." + nameof(XMutex.XWaitOne))]
        public static void ZZZ_XWaitOneTest()
        {
            bool success = false;   // 取得成功フラグ


            //------------------------------------------------------------
            Debug.Print("●通常パターン");

            Debug.Print("ミューテックスの所有権を取得");
            success = testMutex.XWaitOne(0);                            //// テスト用ミューテックスの所有権を取得する
            PostProcess();                                              //// 所有権取得後処理を行う


            //------------------------------------------------------------
            Debug.Print("");
            Debug.Print("●放棄されたミューテックスの所有権を取得するパターン(コールバックなし)");
            M_PrepareAbandonMutex();                                    //// 放棄ミューテックス準備処理を行う

            Debug.Print("ミューテックスの所有権を取得");
            success = testMutex.XWaitOne(0);                            //// テスト用ミューテックスの所有権を取得する
            PostProcess();                                              //// 所有権取得後処理を行う


            //------------------------------------------------------------
            Debug.Print("");
            Debug.Print("●放棄されたミューテックスの所有権を取得するパターン(コールバックあり)");
            M_PrepareAbandonMutex();                                    //// 放棄ミューテックス準備処理を行う

            Debug.Print("ミューテックスの所有権を取得");
            success = testMutex.XWaitOne(0,                             //// テスト用ミューテックスの所有権を取得する
                                MyWarning, "テスト用ミューテックス");
            PostProcess();                                              //// 所有権取得後処理を行う


            //------------------------------------------------------------
            /// 【ローカル関数】所有権取得後処理
            //------------------------------------------------------------
            void PostProcess()
            {
                if (success)
                {                                                           //// 取得成功の場合
                    testMutex.ReleaseMutex();                               /////  テスト用ミューテックスの所有権を解放する
                    Debug.Print("ミューテックス所有権取得成功");            /////  デバッグ出力(所有権取得成功)
                }
                else
                {                                                           //// 取得失敗の場合
                    Debug.Print("ミューテックス所有権取得失敗");            /////  デバッグ出力(所有権取得失敗)
                }
            }


            //------------------------------------------------------------
            /// 【ローカル関数】放棄ミューテックス検出コールバック
            //------------------------------------------------------------
            void MyWarning(AbandonedMutexException ex, object callbackArg)
            {
                Debug.Print($"放棄されたミューテックスを検出しました：{ex.Message}[{callbackArg}]");
                System.Media.SystemSounds.Hand.Play();
            }
        }


        //====================================================================================================
        // テスト準備関連
        //====================================================================================================

        // ＜メモ＞
        // ・複数のスレッドから同時にアクセスすることはないが、念のため ConcurrentQueue を使用している。
        /// <summary>
        /// 【デバッグ出力キュー】
        /// </summary>
        /// <remarks>
        /// 補足<br/>
        /// ・別スレッドで動作するワーカーからは <c>Debug.Print</c> を直接呼び出すことができないため(2024年6月現在。機会があれば後日修正する)、
        ///   デバッグ出力内容をキューに蓄積しておき、メインスレッド側に戻ってからデバッグ出力します。
        /// </remarks>
        private readonly static ConcurrentQueue<string> m_debugOutputQueue = new ConcurrentQueue<string>();


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【放棄ミューテックス準備】テストのために、放棄されたミューテックスを用意します。
        /// </summary>
        //--------------------------------------------------------------------------------
        private static void M_PrepareAbandonMutex()
        {
            //------------------------------------------------------------
            /// 放棄されたミューテックスを用意する
            //------------------------------------------------------------
            var abandonMutexThread = new Thread(M_AbandonMutexWorker);  //// ミューテックス放棄ワーカースレッドを生成する
            abandonMutexThread.Start();                                 //// スレッドを開始する
            abandonMutexThread.Join();                                  //// スレッドが終了するまで待機する

            while (m_debugOutputQueue.TryDequeue(out string debugOutput))
            {                                                           //// デバッグ出力キューから情報を取り出せる間、繰り返す
                Debug.Print(debugOutput);                               /////  デバッグ出力する
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ミューテックス放棄ワーカー】テスト用ミューテックスの所有権を取得して、解放しないまま終了します。
        /// </summary>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="AbandonedMutexException"/>例外の原因となる
        ///   「別プロセスや別スレッドが解放せずに終了することによって放棄されたミューテックス」を意図的に作り出すための処理です。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        private static void M_AbandonMutexWorker()
        {
            //------------------------------------------------------------
            /// テスト用ミューテックスの所有権を取得して、解放しないまま終了する
            //------------------------------------------------------------
            m_debugOutputQueue.Enqueue("放棄されたミューテックスの用意");

            var success = testMutex.WaitOne(0);
            if (success)
            {
                m_debugOutputQueue.Enqueue("  ミューテックス所有権取得成功");
            }
            m_debugOutputQueue.Enqueue("  所有権を解放しないままスレッド終了");
        }

    } // class
#endif

} // namespace
