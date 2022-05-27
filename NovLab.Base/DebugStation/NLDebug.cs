// @(h)NLDebug.cs ver 0.00 ( '22.03.25 Nov-Lab ) 作成開始
// @(h)NLDebug.cs ver 0.51 ( '22.05.05 Nov-Lab ) ベータ版完成
// @(h)NLDebug.cs ver 0.51a( '22.05.10 Nov-Lab ) 微修正  ：DebugData.CreateForMessageInfo の引数追加に対応した。機能変更なし。
// @(h)NLDebug.cs ver 0.51b( '22.05.24 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【デバッグ支援】デバッグステーションアプリケーションを用いたデバッグ支援機能を提供します。

using System;
using System.Diagnostics;
using System.Collections.Generic;


namespace NovLab.DebugStation
{
    // ＜メモ＞
    // ・"Debug."  で検索：Debug クラス呼び出しとNLDebug クラス呼び出しの両方を検索できます。
    // ・" Debug." で検索：Debug クラス呼び出しだけを検索できます。
    // ・"NLDebug."で検索：NLDebug クラス呼び出しだけを検索できます。

    //====================================================================================================
    /// <summary>
    /// 【デバッグ支援】デバッグステーションアプリケーションを用いたデバッグ支援機能を提供します。
    /// </summary>
    /// <remarks>
    /// 関連クラスの使い分け方<br></br>
    /// ・<see cref="NLDebug"/> クラスを使うと、任意の情報メッセージやイベントをデバッグステーションへ送ることができます。<br></br>
    /// ・<see cref="DebugStationTraceListener"/> クラスを使うと、Debug クラスや Trace クラスの出力をデバッグステーションへ送ることができます。<br></br>
    /// ・<see cref="DebugStationClient"/> クラスを直接使うと、複数のデバッグステーションを使い分けることができます。<br></br>
    /// <br></br>
    /// Tips<br></br>
    /// ・連携対象は既定のデバッグステーションアプリケーションです。
    ///   連携対象を変更したい場合は、あらかじめ <see cref="DebugStationClient.OpenDefault(string)"/> を、
    ///   メールスロット名を指定して呼び出しておきます。<br></br>
    /// </remarks>
    //====================================================================================================
    public static class NLDebug
    {
        //====================================================================================================
        // 内部メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デバッグステーションクライアント取得】デバッグステーションクライアントを取得します。未オープンの場合は、既定の宛先でオープンします。
        /// </summary>
        /// <returns>
        /// デバッグステーションクライアント
        /// </returns>
        //--------------------------------------------------------------------------------
        private static DebugStationClient M_GetOrOpenClient() => DebugStationClient.GetOrOpenDefault();


        //====================================================================================================
        // インデント操作
        //====================================================================================================

        /// <summary>
        /// 【インデントレベル】インデント(字下げ)の深さです。
        /// </summary>
        public static int IndentLevel
        {
            get
            {
                if (bf_IndentLevel < 0)
                {
                    return 0;
                }
                else
                {
                    return bf_IndentLevel;
                }
            }
            set
            {
                if (value < 0)
                {
                    bf_IndentLevel = 0;
                }
                else
                {
                    bf_IndentLevel = value;
                }
            }
        }
        private static int bf_IndentLevel = 0;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【インデント開始】インデントレベルを１増やします。
        /// </summary>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Indent() => IndentLevel++;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【インデント終了】インデントレベルを１減らします。
        /// </summary>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Unindent() => IndentLevel--;


        //====================================================================================================
        // 送信操作
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【プロセス開始シグナル送信】デバッグステーションアプリケーションへプロセス開始シグナルを送信します。
        /// </summary>
        /// <remarks>
        /// ・DEBUG シンボルが定義されていない場合、呼び出しはコンパイルされません。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void SendProcessStart() => DebugData.CreateForSignal(IndentLevel, DebugStationSignalKind.ProcessStart).SendTo(M_GetOrOpenClient());


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【プロセス終了シグナル送信】デバッグステーションアプリケーションへプロセス終了シグナルを送信します。
        /// </summary>
        /// <remarks>
        /// ・DEBUG シンボルが定義されていない場合、呼び出しはコンパイルされません。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void SendProcessExit() => DebugData.CreateForSignal(IndentLevel, DebugStationSignalKind.ProcessExit).SendTo(M_GetOrOpenClient());


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メッセージ書き込み】デバッグステーションアプリケーションへ情報メッセージ文字列を送信します。
        /// </summary>
        /// <param name="message">[in ]：メッセージ文字列</param>
        /// <remarks>
        /// ・DebugStationTraceListener を介した Debug.Print とは違い、デバッグステーションにだけ出力します。<br></br>
        /// ・DEBUG シンボルが定義されていない場合、呼び出しはコンパイルされません。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Print(string message) => DebugData.CreateForMessageInfo(IndentLevel, message, null).SendTo(M_GetOrOpenClient());
    }
}
