// @(h)DebugData.cs ver 0.00 ( '22.04.18 Nov-Lab ) 作成開始
// @(h)DebugData.cs ver 0.51 ( '22.04.19 Nov-Lab ) ベータ版完成
// @(h)DebugData.cs ver 1.02 ( '22.05.05 Nov-Lab ) 初版完成
// @(h)DebugData.cs ver 1.03 ( '22.05.09 Nov-Lab ) 機能変更：GetOutline。１行表示用に改行文字群を ", " に置換するようにした
// @(h)DebugData.cs ver 1.04 ( '22.05.10 Nov-Lab ) 機能追加：カテゴリ名に対応した。
// @(h)DebugData.cs ver 1.04b( '24.05.12 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【デバッグデータ】デバッグステーションアプリケーションへ送信するデバッグデータを管理します。

using System;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Xml.Serialization;


namespace NovLab.DebugStation
{
    //====================================================================================================
    /// <summary>
    /// 【デバッグデータ】デバッグステーションアプリケーションへ送信するデバッグデータを管理します。
    /// </summary>
    /// <remarks>
    /// 参照<br></br>
    /// ・<see cref="DebugStationTraceListener"/><br></br>
    /// ・<see cref="NLDebug"/><br></br>
    /// </remarks>
    //====================================================================================================
    public class DebugData
    {
        //====================================================================================================
        /// <summary>
        /// 【デバッグデータヘッダー情報】デバッグデータの必須情報をひとまとめに管理します。
        /// </summary>
        /// <remarks>
        /// ・デバッグデータ本体部と明確に区別するためにクラス化しています。<br></br>
        /// </remarks>
        //====================================================================================================
        public class HeaderInfo
        {
            // ＜メモ＞
            // ・TraceEventCache は必要なメンバーを直接保持します(シリアル化に対応していないため)。
            // ・TraceEventType  は文字列形式で保持・シリアル化します(内部整数値=0の列挙値がなく、シリアル化に対応していないため)。
            //====================================================================================================
            // 公開プロパティー(基本部)
            //====================================================================================================
            /// <summary>
            /// 【デバッグ情報種別】
            /// </summary>
            [DefaultValue(DebugInfoKind.MessageInfo)]
            public DebugInfoKind DebugInfoKind { get; set; }

            /// <summary>
            /// 【インデントレベル】<see cref="TraceListener.IndentLevel"/>
            /// </summary>
            [DefaultValue(0)]
            public int IndentLevel { get; set; }

            /// <summary>
            /// 【イベントソース】
            /// </summary>
            public string EventSource { get; set; }

            /// <summary>
            /// 【イベントカテゴリ名】[null = カテゴリ名なし]フォーム名や処理名などを設定しておくことで、絞り込みや検索に利用することができます。
            /// </summary>
            public string EventCategory { get; set; }

            /// <summary>
            /// 【プロセスID】<see cref="TraceEventCache.ProcessId"/>
            /// </summary>
            public int ProcessId { get; set; }

            /// <summary>
            /// 【スレッドID】<see cref="TraceEventCache.ThreadId"/>
            /// </summary>
            public string ThreadId { get; set; }

            /// <summary>
            /// 【イベント発生日時(UTC)】<see cref="TraceEventCache.DateTime"/>
            /// </summary>
            public DateTime EventDateTime { get; set; }

            /// <summary>
            /// 【コールスタック文字列】<see cref="TraceEventCache.Callstack"/>
            /// </summary>
            public string Callstack { get; set; }


            //====================================================================================================
            // 公開プロパティー(トレース情報固有部)
            //====================================================================================================
            /// <summary>
            /// 【イベント種類】
            /// </summary>
            [XmlIgnore, EnumStr]    // 文字列形式で保持・シリアル化する列挙値
            public TraceEventType EventType
            {
                get => XEnum.XStrParse(XML_EventTypeStr, TraceEventType.Information);
                set => XML_EventTypeStr = value.XStrMake(TraceEventType.Information);
            }

            [XmlElement("EventTypeStr"), DefaultValue(null)]
            public string XML_EventTypeStr { get; set; } = null;


            /// <summary>
            /// 【イベント識別子】
            /// </summary>
            [DefaultValue(0)]
            public int EventId { get; set; }


            //====================================================================================================
            // 公開メソッド(インスタンス生成)
            //====================================================================================================

            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【デバッグデータヘッダー情報生成(完全)】すべての内容を指定して新しいインスタンスを生成します。
            /// </summary>
            /// <param name="debugInfoKind">[in ]：デバッグ情報種別</param>
            /// <param name="indentLevel">  [in ]：インデントレベル</param>
            /// <param name="eventCache">   [in ]：トレース イベント データ</param>
            /// <param name="source">       [in ]：イベントソース</param>
            /// <param name="eventType">    [in ]：イベント種類</param>
            /// <param name="id">           [in ]：イベント識別子</param>
            /// <param name="category">     [in ]：イベントカテゴリ名</param>
            /// <returns>
            /// 生成したデバッグデータヘッダー情報
            /// </returns>
            /// <remarks>
            /// ・プロパティーやフィールドを追加した場合はまずこのメソッドを修正します。
            ///   すると引数不一致により CreateFor 系メソッドがエラーになるので、それらも修正します。
            /// </remarks>
            //--------------------------------------------------------------------------------
            public static HeaderInfo CreateCompletely(
                DebugInfoKind debugInfoKind,
                int indentLevel,
                TraceEventCache eventCache,
                string source,
                TraceEventType eventType,
                int id,
                string category)
            {
                return new HeaderInfo()
                {
                    DebugInfoKind = debugInfoKind,
                    IndentLevel = indentLevel,
                    EventSource = source,
                    EventType = eventType,
                    EventId = id,
                    EventCategory = category,
                    ProcessId = eventCache.ProcessId,
                    ThreadId = eventCache.ThreadId,
                    EventDateTime = eventCache.DateTime,
                    Callstack = eventCache.Callstack,
                };
            }


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【デバッグデータヘッダー情報生成(メッセージ情報用)】メッセージ情報用に新しいインスタンスを生成します。
            /// </summary>
            /// <param name="indentLevel">[in ]：インデントレベル</param>
            /// <param name="category">   [in ]：カテゴリ名[null = カテゴリなし]</param>
            /// <returns>
            /// 生成したデバッグデータヘッダー情報
            /// </returns>
            /// <remarks>
            /// ・参考：<see cref="TraceListener.WriteLine(string, string)"/>
            /// </remarks>
            //--------------------------------------------------------------------------------
            public static HeaderInfo CreateForMessageInfo(int indentLevel, string category)
            {
                return CreateCompletely(DebugInfoKind.MessageInfo, indentLevel, new TraceEventCache(), CurrentProcessName, TraceEventType.Information, 0, category);
            }


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【デバッグデータヘッダー情報生成(トレース情報用)】トレース情報用に新しいインスタンスを生成します。
            /// </summary>
            /// <param name="indentLevel">[in ]：インデントレベル</param>
            /// <param name="eventCache"> [in ]：トレース イベント データ</param>
            /// <param name="source">     [in ]：イベントソース</param>
            /// <param name="eventType">  [in ]：イベント種類</param>
            /// <param name="id">         [in ]：イベント識別子</param>
            /// <returns>
            /// 生成したデバッグデータヘッダー情報
            /// </returns>
            /// <remarks>
            /// ・参考：<see cref="TraceListener.TraceEvent(TraceEventCache, string, TraceEventType, int)"/>
            /// </remarks>
            //--------------------------------------------------------------------------------
            public static HeaderInfo CreateForTraceInfo(
                int indentLevel,
                TraceEventCache eventCache,
                string source,
                TraceEventType eventType,
                int id)
            {
                return CreateCompletely(DebugInfoKind.TraceInfo, indentLevel, eventCache, source, eventType, id, null);
            }


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【デバッグデータヘッダー情報生成(フェイル情報用)】フェイル情報用に新しいインスタンスを生成します。
            /// </summary>
            /// <param name="indentLevel">[in ]：インデントレベル</param>
            /// <returns>
            /// 生成したデバッグデータヘッダー情報
            /// </returns>
            /// <remarks>
            /// ・参考：<see cref="TraceListener.Fail(string)"/>
            /// </remarks>
            //--------------------------------------------------------------------------------
            public static HeaderInfo CreateForFailInfo(int indentLevel)
            {
                return CreateCompletely(DebugInfoKind.FailInfo, indentLevel, new TraceEventCache(), CurrentProcessName, TraceEventType.Error, 0, null);
            }


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【デバッグデータヘッダー情報生成(シグナル情報用)】デバッグステーションシグナル情報用に新しいインスタンスを生成します。
            /// </summary>
            /// <param name="indentLevel">[in ]：インデントレベル</param>
            /// <returns>
            /// 生成したデバッグデータヘッダー情報
            /// </returns>
            //--------------------------------------------------------------------------------
            public static HeaderInfo CreateForSignal(int indentLevel)
            {
                return CreateCompletely(DebugInfoKind.DebugStationSignal, indentLevel, new TraceEventCache(), CurrentProcessName, TraceEventType.Information, 0, null);
            }


            //====================================================================================================
            // コンストラクター
            //====================================================================================================

            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【プライベート コンストラクター】XMLシリアル化対応のために用意しているパラメーターなしのコンストラクターです。
            /// </summary>
            //--------------------------------------------------------------------------------
            private HeaderInfo() { }
        }


        //====================================================================================================
        // 公開プロパティー
        //====================================================================================================
        //============================================================
        // ヘッダ部
        //============================================================
        /// <summary>
        /// 【デバッグデータヘッダ情報】
        /// </summary>
        public HeaderInfo Header { get; set; }


        //============================================================
        // データ部
        //============================================================

        /// <summary>
        /// 【デバッグステーションシグナル種別】[NONE = なし]
        /// </summary>
        [DefaultValue(DebugStationSignalKind.NONE)]
        public DebugStationSignalKind DebugStationSignalKind { get; set; }

        /// <summary>
        /// 【メッセージ文字列】[null = メッセージなし]
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 【詳細メッセージ文字列】[null = 詳細メッセージなし]
        /// </summary>
        public string DetailMessage { get; set; }


#if false   //[-] 保留：詳細は DebugStationTraceListener のコメントを参照
        /// <summary>
        /// 【トレースデータ配列】[null = トレースデータなし]
        /// </summary>
        public object[] TraceDatas { get; set; }
#endif


        //====================================================================================================
        // 基本操作
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列形式作成】このインスタンスの内容を表す文字列を取得します。
        /// </summary>
        /// <returns>文字列形式</returns>
        //--------------------------------------------------------------------------------
        public override string ToString()
        {
            return Header.EventSource + "[" + Header.ProcessId + "]:" + Message;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【概要文字列取得】このデバッグデータの概要を説明する簡単な文字列を取得します。
        /// </summary>
        /// <returns>
        /// 概要文字列
        /// </returns>
        /// <remarks>
        /// ・デバッグステーションアプリケーションでの一行表示に使用します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public string GetOutline()
        {
            var outline = "";                                           ///  概要文字列 = 空文字列 に初期化する


            //------------------------------------------------------------
            /// デバッグステーションシグナル種別を追加する
            //------------------------------------------------------------
            if (DebugStationSignalKind != DebugStationSignalKind.NONE)
            {                                                           //// デバッグステーションシグナル種別 = なし でない場合
                outline += DebugStationSignalKind.ToString();           /////  概要文字列にデバッグステーションシグナル種別を追加する
            }


            // メッセージ文字列：デバッグ情報種別：概要文字列への反映内容
            //-----------------------------------------------------------
            // null            ：N/A             ：反映しない
            // 空文字列        ：フェイル情報    ：「アサート失敗」を追加する
            // 空文字列        ：フェイル情報以外：反映しない
            // 空文字列でない  ：N/A             ：メッセージ文字列を追加する
            //------------------------------------------------------------
            /// メッセージ文字列を追加する
            //------------------------------------------------------------
            if (Message != null)
            {                                                           //// メッセージ文字列 = null(なし) でない場合
                if (Message != "")
                {                                                       /////  メッセージ文字列が空文字列でない場合
                    outline += Message;                                 //////   概要文字列にメッセージ文字列を追加する
                }
                else
                {                                                       /////  メッセージ文字列が空文字列の場合
                    if (Header.DebugInfoKind == DebugInfoKind.FailInfo)
                    {                                                   //////   フェイル情報の場合(Assert のメッセージなし版が呼び出された場合)
                        outline += "アサート失敗";                      ///////    概要文字列に「アサート失敗」を追加する
                    }
                }
            }


            //------------------------------------------------------------
            /// 詳細メッセージ文字列を追加する
            //------------------------------------------------------------
            if (DetailMessage.XIsValid())
            {                                                           //// 詳細メッセージ文字列が有効な内容の場合
                outline += " - " + DetailMessage;                       /////  概要文字列に詳細メッセージ文字列を追加する
            }


            return outline.XReplaceNewLineChars(", ");                  ///  戻り値 = 概要文字列 で関数終了
        }


        //====================================================================================================
        // static 公開プロパティー
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【カレントプロセス名】
        /// 既定では Process.GetCurrentProcess().ProcessName の内容ですが、
        /// 任意の文字列に差し替えることもできます。
        /// </summary>
        //--------------------------------------------------------------------------------
        public static string CurrentProcessName
        {
            get
            {
                //------------------------------------------------------------
                /// カレントプロセス名を取得する
                //------------------------------------------------------------
                if (bf_currentProcessName == null)
                {                                                           //// バッキングフィールドに取得していない場合
                    bf_currentProcessName =                                 /////  カレントプロセス名を取得してバッキングフィールドに格納する
                        Process.GetCurrentProcess().ProcessName + ".exe";
                }
                return bf_currentProcessName;                               //// 戻り値 = カレントプロセス名 で関数終了
            }
            set => bf_currentProcessName = value;
        }
        protected static string bf_currentProcessName;


        //====================================================================================================
        // 公開メソッド(インスタンス生成)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デバッグデータ生成(メッセージ情報)】メッセージ情報用に新しいインスタンスを生成します。
        /// </summary>
        /// <param name="indentLevel">[in ]：インデントレベル</param>
        /// <param name="message">    [in ]：メッセージ文字列</param>
        /// <param name="category">   [in ]：カテゴリ名[null = カテゴリなし]</param>
        /// <returns>
        /// 生成したデバッグデータ
        /// </returns>
        /// <remarks>
        /// ・参考：<see cref="TraceListener.WriteLine(string)"/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static DebugData CreateForMessageInfo(int indentLevel, string message, string category)
        {
            //------------------------------------------------------------
            /// メッセージ情報用に新しいインスタンスを生成する
            //------------------------------------------------------------
            return new DebugData()
            {                                                           //// メッセージ情報用に新しいインスタンスを生成して戻り値とし、関数終了
                Header =HeaderInfo.CreateForMessageInfo(
                    indentLevel, category),                             /////  ヘッダー情報 = メッセージ情報用
                Message = message,                                      /////  メッセージ文字列 = 引数で指定された内容
            };
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デバッグデータ生成(トレース情報)】トレース情報用に新しいインスタンスを生成します。
        /// </summary>
        /// <param name="indentLevel">[in ]：インデントレベル</param>
        /// <param name="eventCache"> [in ]：トレース イベント データ</param>
        /// <param name="source">     [in ]：イベントソース</param>
        /// <param name="eventType">  [in ]：イベントの種類</param>
        /// <param name="id">         [in ]：イベント識別子</param>
        /// <param name="message">    [in ]：メッセージ文字列</param>
        /// <returns>
        /// 生成したデバッグデータ
        /// </returns>
        //--------------------------------------------------------------------------------
        public static DebugData CreateForTraceInfo(int indentLevel, TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            //------------------------------------------------------------
            /// トレース情報用に新しいインスタンスを生成する
            //------------------------------------------------------------
            return new DebugData()
            {                                                           //// トレース情報用に新しいインスタンスを生成して戻り値とし、関数終了
                Header = HeaderInfo.CreateForTraceInfo(
                    indentLevel, eventCache, source, eventType, id),    /////  ヘッダー情報 = トレース情報用
                Message = message,                                      /////  メッセージ文字列 = 引数で指定された内容
            };
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デバッグデータ生成(フェイル情報用)】フェイル情報用に新しいインスタンスを生成します。
        /// </summary>
        /// <param name="indentLevel">  [in ]：インデントレベル</param>
        /// <param name="message">      [in ]：エラー メッセージ</param>
        /// <param name="detailMessage">[in ]：詳細エラー メッセージ</param>
        /// <returns>
        /// 生成したデバッグデータ
        /// </returns>
        /// <remarks>
        /// ・参考：<see cref="TraceListener.Fail(string)"/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static DebugData CreateForFailInfo(int indentLevel, string message, string detailMessage)
        {
            //------------------------------------------------------------
            /// フェイル情報用に新しいインスタンスを生成する
            //------------------------------------------------------------
            return new DebugData()
            {                                                           //// フェイル情報用に新しいインスタンスを生成して戻り値とし、関数終了
                Header = HeaderInfo.CreateForFailInfo(indentLevel),     /////  ヘッダー情報 = フェイル情報用
                Message = message,                                      /////  メッセージ文字列 = エラーメッセージ
                DetailMessage = detailMessage,                          /////  詳細メッセージ文字列 = 詳細エラーメッセージ
            };
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デバッグデータ生成(シグナル用)】デバッグステーションシグナル用に新しいインスタンスを生成します。
        /// </summary>
        /// <param name="indentLevel">           [in ]：インデントレベル</param>
        /// <param name="debugStationSignalKind">[in ]：デバッグステーションシグナル種別</param>
        /// <returns>
        /// 生成したデバッグデータ
        /// </returns>
        //--------------------------------------------------------------------------------
        public static DebugData CreateForSignal(int indentLevel, DebugStationSignalKind debugStationSignalKind)
        {
            //------------------------------------------------------------
            /// デバッグステーションシグナル用に新しいインスタンスを生成する
            //------------------------------------------------------------
            return new DebugData()
            {                                                           //// 新しいインスタンスを生成して戻り値とし、関数終了
                Header = HeaderInfo.CreateForSignal(indentLevel),       /////  ヘッダー情報 = シグナル情報用
                DebugStationSignalKind = debugStationSignalKind,        /////  デバッグステーションシグナル種別 = 引数で指定された内容
            };
        }


        //====================================================================================================
        // コンストラクター
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【プライベート コンストラクター】XMLシリアル化対応のために用意しているパラメーターなしのコンストラクターです。
        /// </summary>
        //--------------------------------------------------------------------------------
        private DebugData() { }


        //====================================================================================================
        // XML文字列操作
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【XML文字列解析】XML文字列を解析してデバッグデータに変換します。
        /// </summary>
        /// <param name="xmlString">[in ]：XML文字列</param>
        /// <returns>
        /// デバッグデータ[null = 解析失敗]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static DebugData ParseXmlString(string xmlString) => XmlUtil.ParseXmlString<DebugData>(xmlString);


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【XML文字列作成】このインスタンスの内容からXML文字列を作成します。
        /// </summary>
        /// <returns>
        /// XML文字列
        /// </returns>
        //--------------------------------------------------------------------------------
        public string ToXmlString() => XmlUtil.ToXmlString(this);


        //====================================================================================================
        // その他の操作
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デバッグデータ送信】このデバッグデータをデバッグステーションアプリケーションへ送信します。
        /// </summary>
        /// <param name="debugStationClient">[in ]：デバッグステーションクライアント</param>
        /// <remarks>
        /// ・debugData.SendTo(debugStationClient) のように直感的に記述するためのユーティリティーメソッドです。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public void SendTo(DebugStationClient debugStationClient) => debugStationClient.Send(this);
    }


    //====================================================================================================
    /// <summary>
    /// 【デバッグ情報種別】デバッグ情報の種類を表します。
    /// </summary>
    //====================================================================================================
    public enum DebugInfoKind
    {
        //================================================================================
        // トレースリスナー関連(Debug, Trace, TraceSource から、DebugStationTraceListener を介して送信される情報)
        //================================================================================
        /// <summary>
        /// 【メッセージ情報】Debug/Trace の Write系メソッド や Debug.Print で出力されたメッセージ文字列です。
        /// </summary>
        MessageInfo = 0,

        /// <summary>
        /// 【フェイル情報】Debug/Trace の Fail や Assert で出力されたフェイル情報です。
        /// </summary>
        FailInfo,

        /// <summary>
        /// 【トレース情報】Trace の TraceData, TraceEvent, TraceTransfer, TraceInformation, TraceWarning, TraceError で出力されたトレース情報です。
        /// </summary>
        TraceInfo,


        //================================================================================
        // DebugStation関連(NLDebug を介して送信されるデバッグステーション独自の情報)
        //================================================================================
        /// <summary>
        /// 【デバッグステーションシグナル情報】NLDebug を介して出力されるデバッグステーション向けのシグナルです。
        /// </summary>
        DebugStationSignal,
    }


    //====================================================================================================
    /// <summary>
    /// 【デバッグステーションシグナル種別】デバッグステーションシグナルの種類を表します。
    /// </summary>
    //====================================================================================================
    public enum DebugStationSignalKind
    {
        /// <summary>
        /// 【デバッグステーションシグナル種別：なし】
        /// </summary>
        NONE = 0,

        /// <summary>
        /// 【デバッグステーションシグナル種別：プロセス開始】プロセスが開始されたときに送出するシグナルです。
        /// </summary>
        /// <remarks>
        /// ・プロセス開始時にこのイベントを送信しておくと、デバッグ情報などを送信する前から
        ///   デバッグステーションアプリケーションがプロセスを認識できるようになります。<br></br>
        /// </remarks>
        ProcessStart,

        /// <summary>
        /// 【デバッグステーションシグナル種別：プロセス終了】プロセスが終了されるときに送出するシグナルです。
        /// </summary>
        ProcessExit,
    }
}
