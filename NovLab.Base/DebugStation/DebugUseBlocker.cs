// @(h)DebugUseBlocker.cs ver 0.00 ( '22.03.30 Nov-Lab ) 作成開始
// @(h)DebugUseBlocker.cs ver 0.51 ( '22.03.30 Nov-Lab ) ベータ版完成
// @(h)DebugUseBlocker.cs ver 0.51a( '22.05.24 Nov-Lab ) その他  ：コメント整理

// @(s)
//   【Debug/Traceクラス使用ブロッカー】
//     Debug クラスと Trace クラスの使用を防ぎ、代替手段を提供します。
//     ・DebugStationTraceListener クラスおよびその呼び出し先で使用します。
//     ・DebugStationTraceListener をリスナーに設定しているときに、Debug クラスや Trace クラスの使用によって
//       自己再帰呼び出しループが発生してしまうのを防ぐための仕組みです。

using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace NovLab.DebugStation.DebugUseBlocker
{
    //====================================================================================================
    /// <summary>
    /// 【Debug クラス使用ブロッカー】System.Diagnostics.Debug クラスの呼び出しを防ぐためのダミークラスです。
    /// </summary>
    /// <remarks>
    /// ・DebugStationTraceListener クラスおよびその呼び出し先でデバッグ出力をする場合は、代わりに DebugX クラスを使用してください。
    /// </remarks>
    //====================================================================================================
    public static class Debug { }


    //====================================================================================================
    /// <summary>
    /// 【Trace クラス使用ブロッカー】System.Diagnostics.Trace クラスの呼び出しを防ぐためのダミークラスです。
    /// </summary>
    /// <remarks>
    /// ・DebugStationTraceListener クラスおよびその呼び出し先でトレース出力をする場合は、代わりに TraceX クラスを使用してください。
    /// </remarks>
    //====================================================================================================
    public static class Trace { }


    //====================================================================================================
    /// <summary>
    /// 【代替 Debug クラス】Debug クラスの代替機能を提供します。
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・DebugStationTraceListener クラスおよびその呼び出し先で使用します。<br></br>
    /// ・DebugStationTraceListener をリスナーに設定しているときに、Debug クラスや Trace クラスの使用によって
    ///   自己再帰呼び出しループが発生してしまうのを防ぐための仕組みです。<br></br>
    /// </remarks>
    //====================================================================================================
    public static class DebugX
    {
        // 必要に応じて他の代替メソッドも追加する


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【代替 Debug.Print メソッド】
        /// DebugStationTraceListener以外のリスナーにデバッグメッセージを書き込みます。
        /// </summary>
        /// <param name="message">[in ]：メッセージ文字列</param>
        /// <remarks>
        /// ・DEBUG シンボルが定義されてされていない場合、呼び出しはコンパイルされません。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Print(string message)
        {
            //------------------------------------------------------------
            /// DebugStationTraceListener以外のリスナーにデバッグメッセージを書き込む
            //------------------------------------------------------------
            foreach (TraceListener listener in System.Diagnostics.Debug.Listeners)
            {                                                           //// リスナーコレクションを繰り返す
                if (listener is DebugStationTraceListener == false)
                {                                                       /////  DebugStationTraceListener型でない場合
                    listener.WriteLine(message);                        //////   メッセージ文字列を書き込む
                }
            }
        }
    }
}
