// @(h)XStream_XMLDoc.cs ver 0.00 ( '24.04.24 Nov-Lab ) 作成開始
// @(h)XStream_XMLDoc.cs ver 0.11 ( '24.04.24 Nov-Lab ) アルファ版完成
// @(h)XStream_XMLDoc.cs ver 0.11a( '24.05.22 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【XStreamクラス用XMLコメント継承元】<inheritdoc> タグで継承するための XML ドキュメント コメントを用意したダミークラスです。

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#pragma warning disable IDE0060 // 未使用のパラメーターを削除します

namespace NovLab.IO
{
    public static partial class XStream
    {
        // ＜メモ＞
        // ・XStream クラス(およびその系列クラス)専用なので内部クラスとして作成している。
        //====================================================================================================
        /// <summary>
        /// 【XMLコメント継承元】XStream クラスおよび Stream 関連クラス用<br/>
        /// &lt;inheritdoc&gt; タグで継承するための XML ドキュメント コメントを用意したダミークラスです。
        /// </summary>
        //====================================================================================================
        private partial class XMLDOC
        {
            //====================================================================================================
            // 引数説明用ダミーメソッド
            // ・型パラメーター名や引数名は正確でなくても良いが、合わせておいた方がわかりやすい。
            //====================================================================================================

            /// <param name="millisecondsTimeout">[in ]：タイムアウト時間(ミリ秒)。無期限に待機する場合は <see cref="Timeout.Infinite"/>(-1)</param>
            public void MillisecondsTimeout(int millisecondsTimeout) { }

            /// <param name="cancellationToken">[in ]：キャンセルトークン。取り消し要求機能を使わない場合は <see cref="CancellationToken.None"/></param>
            public void CancellationToken(CancellationToken cancellationToken) { }


            //====================================================================================================
            // 例外説明用ダミーメソッド
            // ・例外クラス名は正確でなくても良いが、合わせておいた方がわかりやすい。
            //====================================================================================================

            /// <exception cref="OperationCanceledException">
            /// 操作取り消し例外。キャンセルトークンによって取り消し要求が通知されました。
            /// </exception>
            public void EX_OperationCanceledException() { }

            /// <exception cref="TimeoutException">
            /// タイムアウト例外。タイムアウト時間内に処理が完了しませんでした。
            /// </exception>
            public void EX_TimeoutException() { }

        } // class XMLDOC

    } // class XStream

} // namespace

#pragma warning restore IDE0060 // 未使用のパラメーターを削除します
