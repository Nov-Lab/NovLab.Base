// @(h)XPipeStream.cs ver 0.00 ( '24.05.16 Nov-Lab ) 作成開始
// @(h)XPipeStream.cs ver 0.21 ( '24.05.16 Nov-Lab ) アルファ版完成
// @(h)XPipeStream.cs ver 0.22 ( '24.05.30 Nov-Lab ) 機能修正：キャンセル処理のエミュレート精度を向上させた
// @(h)XPipeStream.cs ver 0.22a( '24.06.04 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【PipeStream拡張メソッド】System.IO.Pipes.PipeStream クラスに拡張メソッドを追加します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;


namespace NovLab.IO.Pipes
{
    //====================================================================================================
    /// <summary>
    /// 【PipeStream拡張メソッド】System.IO.Pipes.PipeStream クラスに拡張メソッドを追加します。
    /// </summary>
    /// <remarks>
    /// キャンセル可能なタイムアウト付き非同期ストリーム読み書き<br/>
    /// <see cref="XPipeReadAsync(PipeStream, byte[], int, int, int, CancellationToken)"/><br/>
    /// <see cref="XPipeWriteAsync(PipeStream, byte[], int, int, int, CancellationToken)"/><br/>
    /// ・これらのメソッドを用意した経緯については <see cref="Memo_PipeStreamCanNotCancel"/> を参照してください。<br/>
    /// </remarks>
    //====================================================================================================
    public static partial class XPipeStream
    {
        //====================================================================================================
        // パイプストリーム用のタイムアウト付き非同期ストリーム読み書き
        // ・PipeStream ではキャンセルトークンが機能しないため、自力でキャンセル処理を行います。
        //====================================================================================================

#if DEBUG
        /// <summary>
        /// 【パイプストリームで、キャンセル処理やタイムアウト処理を自力で実装することにした経緯について】<br/>
        /// PipeStream に対する ReadAsync メソッド や WriteAsync メソッドは、CancellationToken によるキャンセルがほぼ機能しません(*1)(*2)。<br/>
        /// そのため、ReadAsync メソッド や WriteAsync メソッドへ CancellationToken を渡すのではなく、ほぼ同等のキャンセル処理を自力で実装しました。<br/>
        /// (*1).NET Framework 4.5.1 を対象フレームワークとするプロジェクトで、.NET Framework 4.8.04084 がインストールされているPCで確認しました。<br/>
        /// (*2)メソッドが呼び出された時点ですでに取り消し要求状態になっている場合のみ機能します。<br/>
        /// <br/>
        /// 実装方法<br/>
        /// ・キャンセルトークンが取り消し状態になったときは、パイプストリームをクローズしてエラーを発生させ、送受信処理を異常終了させます。<br/>
        /// ・送受信処理が異常終了したときは、終了要因がキャンセルなのかタイムアウトなのかを調査して適切な例外をスローします。<br/>
        /// <br/>
        /// 通常のキャンセル処理との相違点<br/>
        /// ・キャンセル時やタイムアウト時はパイプストリームを閉じるため、その後の送受信操作などはできなくなります。
        /// どのみち処理を継続することは困難なはずなので許容できるかと思います。<br/>
        /// </summary>
        /// <remarks>
        /// 補足<br/>
        /// ・.NET8 などでは PipeStream クラスで ReadAsync メソッド や WriteAsync メソッドをオーバーライドしているようですが、
        ///   .NET Framework 4.5.1 では Stream クラスのメソッドをそのまま利用しています。<br/>
        /// ・GitHub で公開されている .NET8.0.4 の Stream.ReadAsync / WriteAsync メソッドのソースを見ると、
        ///   キャンセルトークンを監視しているのは処理を始める前だけで、その後に呼び出される処理本体(BeginEndReadAsync や BeginEndWriteAsync)には
        ///   キャンセルトークンを渡してもいません。<br/>
        /// ・そのため、メソッドが呼び出された時点ですでに取り消し要求状態になっている場合以外は、キャンセルトークンが機能しないようです。<br/>
        /// </remarks>
        public static class Memo_PipeStreamCanNotCancel { }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【パイプストリーム用タイムアウト付き非同期ストリーム書き込み】
        /// 非同期でパイプストリームにバイト シーケンスを書き込み、書き込んだバイト数の分だけストリームの現在位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象パイプストリーム</param>
        /// <param name="buffer">             [in ]：バイト配列。このメソッドは、<paramref name="buffer"/> からストリームに、<paramref name="count"/> で指定されたバイト数だけコピーします。</param>
        /// <param name="offset">             [in ]：ストリームへのバイトのコピーを開始する位置を示す <paramref name="buffer"/> 内のバイト オフセット。インデックス番号は 0 から始まります。</param>
        /// <param name="count">              [in ]：ストリームに書き込むバイト数。</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XStream.XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XStream.XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// 戻り値を返さない非同期操作タスク
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XStream.XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XStream.XMLDOC.EX_TimeoutException"/></exception>
        /// <remarks>
        /// 注意<br/>
        /// ・キャンセル時やタイムアウト時はパイプストリームを閉じるため、その後の送受信操作などはできなくなります。<br/>
        /// ・キャンセル時やタイムアウト時にパイプストリームを閉じる理由や、このメソッドを用意した経緯については
        ///   <see cref="Memo_PipeStreamCanNotCancel"/> を参照してください。<br/>
        /// <br/>
        /// 補足<br/>
        /// ・<see cref="Stream.WriteAsync(byte[], int, int, CancellationToken)"/> のキャンセル処理を自力でエミュレートしつつ、タイムアウト処理を追加した拡張メソッドです。<br/>
        /// ・<see cref="XStream.XCallWriteAsync"/> メソッドを使うと、ストリームの種類に応じて適切なメソッドを呼び出すことができます。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static async Task XPipeWriteAsync(this PipeStream target,
                                                 byte[] buffer, int offset, int count,
                                                 int millisecondsTimeout, CancellationToken cancellationToken)
        {
            // ＜メモ＞
            // ・millisecondsTimeout 以外の引数チェックは target.WriteAsync に任せる。
            //------------------------------------------------------------
            /// 引数をチェックする
            //------------------------------------------------------------
            if (millisecondsTimeout < -1)
            {                                                           //// タイムアウト時間(ミリ秒)が -1 以外の負数の場合
                throw                                                   /////  引き数不正例外(値範囲)をスローする
                    new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }


            //------------------------------------------------------------
            /// タイムアウト処理付きで非同期パイプストリーム書き込み処理を行う
            //------------------------------------------------------------
            using (var ctsTimeout = new CancellationTokenSource())      //// using：タイムアウト用キャンセルトークンソースを生成する
            using (var ctsTotal =                                       //// using：引数で指定されたキャンセルトークンとタイムアウト用キャンセルトークンを結合して、総合キャンセルトークンソースを生成する
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ctsTimeout.Token))
            {
                ctsTotal.Token.Register(target.Close);                  /////  キャンセルされたらストリームを閉じるように総合キャンセルトークンを構成する

                try
                {                                                       /////  try開始
                    ctsTimeout.CancelAfter(millisecondsTimeout);        //////   タイムアウト用キャンセルトークンソースにタイムアウト時間を設定する
                    await target.WriteAsync(buffer, offset, count);     //////   非同期ストリーム書き込み処理を行う(キャンセル処理は自力で行うので、キャンセルトークンは指定しない)
                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する
                }
                catch
                {                                                       /////  catch：すべての例外
                    // ＜メモ＞
                    // ・キャンセル処理エミュレートによって発生した例外かそうでない例外かを例外の種類によって判別したかったが、
                    //   IOException だったり ObjectDisposedException だったり OperationCanceledException だったりと不安定で、
                    //   他にも別パターンがあるかもしれないので、単純にキャンセルトークンソースの状態だけで判定することにした。
                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する

                    if (ctsTotal.IsCancellationRequested)
                    {                                                   //////   取り消し要求による例外の場合(総合キャンセルトークンソースが取り消し要求状態の場合)
                        if (ctsTimeout.IsCancellationRequested)
                        {                                               ///////    タイムアウトによる取り消しの場合(タイムアウト用キャンセルトークンソースが取り消し要求状態の場合)
                            throw new TimeoutException();               ////////     タイムアウト例外をスローする
                        }
                        else
                        {                                               ///////    タイムアウトによる取り消しでない場合
                            throw new OperationCanceledException();     ////////     操作取り消し例外をスローする
                        }
                    }
                    else
                    {                                                   //////   取り消し要求による例外でない場合(総合キャンセルトークンソースが取り消し要求状態でない場合)
                        throw;                                          ///////    通常のI/Oエラーと判断して、例外を再スローする
                    }
                }
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【パイプストリーム用タイムアウト付き非同期ストリーム読み取り】
        /// 非同期でパイプストリームからバイト シーケンスを読み取り、読み取ったバイト数の分だけストリームの位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象パイプストリーム</param>
        /// <param name="buffer">             [in ]：バイト配列。このメソッドが戻るとき、指定したバイト配列の <paramref name="offset"/> から (<paramref name="offset"/> + <paramref name="count"/> -1) までの値が、ストリームから読み取られたバイトに置き換えられます。</param>
        /// <param name="offset">             [in ]：ストリームから読み取ったデータの格納を開始する位置を示す <paramref name="buffer"/> 内のバイト オフセット。インデックス番号は 0 から始まります。</param>
        /// <param name="count">              [in ]：ストリームから読み取る最大バイト数。</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XStream.XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XStream.XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// バッファーに読み取られた合計バイト数。要求しただけのバイト数を読み取ることができなかった場合、この値は要求したバイト数より小さくなります。ストリームの末尾に到達した場合は 0 (ゼロ) になることがあります。<br/>
        /// ※上記戻り値を返す非同期操作タスクを返します。<br/>
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XStream.XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XStream.XMLDOC.EX_TimeoutException"/></exception>
        /// <remarks>
        /// 注意<br/>
        /// ・キャンセル時やタイムアウト時はパイプストリームを閉じるため、その後の送受信操作などはできなくなります。<br/>
        /// ・キャンセル時やタイムアウト時にパイプストリームを閉じる理由や、このメソッドを用意した経緯については
        ///   <see cref="Memo_PipeStreamCanNotCancel"/> を参照してください。<br/>
        /// <br/>
        /// 補足<br/>
        /// ・<see cref="Stream.ReadAsync(byte[], int, int, CancellationToken)"/>  のキャンセル処理を自力でエミュレートしつつ、タイムアウト処理を追加した拡張メソッドです。<br/>
        /// ・<see cref="XStream.XCallReadAsync"/> メソッドを使うと、ストリームの種類に応じて適切なメソッドを呼び出すことができます。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static async Task<int> XPipeReadAsync(this PipeStream target,
                                                     byte[] buffer, int offset, int count,
                                                     int millisecondsTimeout, CancellationToken cancellationToken)
        {
            // ＜メモ＞
            // ・millisecondsTimeout 以外の引数チェックは target.ReadAsync に任せる。
            //------------------------------------------------------------
            /// 引数をチェックする
            //------------------------------------------------------------
            if (millisecondsTimeout < -1)
            {                                                           //// タイムアウト時間(ミリ秒)が -1 以外の負数の場合
                throw                                                   /////  引き数不正例外(値範囲)をスローする
                    new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }


            //------------------------------------------------------------
            /// タイムアウト処理付きで非同期パイプストリーム読み取り処理を行う
            //------------------------------------------------------------
            using (var ctsTimeout = new CancellationTokenSource())      //// using：タイムアウト用キャンセルトークンソースを生成する
            using (var ctsTotal =                                       //// using：引数で指定されたキャンセルトークンとタイムアウト用キャンセルトークンを結合して、総合キャンセルトークンソースを生成する
                        CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ctsTimeout.Token))
            {
                ctsTotal.Token.Register(target.Close);                  /////  キャンセルされたらストリームを閉じるように総合キャンセルトークンを構成する

                try
                {                                                       /////  try開始
                    ctsTimeout.CancelAfter(millisecondsTimeout);        //////   タイムアウト用キャンセルトークンソースにタイムアウト時間を設定する
                    var bytesReaded =
                        await target.ReadAsync(buffer, offset, count);  //////   非同期ストリーム読み取り処理を行い、読み取ったバイト数を取得する(キャンセル処理は自力で行うので、キャンセルトークンは指定しない)
                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する

                    // ＜メモ＞
                    // ・キャンセル処理エミュレートによる例外は発生したりしなかったりと不安定なので、
                    //   両方のパターンに対応しておいた。
                    if (bytesReaded < count)
                    {                                                   //////   読み取ったバイト数 < ストリームから読み取る最大バイト数 の場合(キャンセルやタイムアウトで中断された可能性がある場合)
                        if (ctsTotal.IsCancellationRequested)
                        {                                               ///////    取り消し要求による読み取り中断の場合(総合キャンセルトークンソースが取り消し要求状態の場合)
                            if (ctsTimeout.IsCancellationRequested)
                            {                                           ////////     タイムアウトによる取り消しの場合(タイムアウト用キャンセルトークンソースが取り消し要求状態の場合)
                                throw new TimeoutException();           /////////      タイムアウト例外をスローする
                            }
                            else
                            {                                           ////////     タイムアウトによる取り消しでない場合
                                throw new OperationCanceledException(); /////////      操作取り消し例外をスローする
                            }
                        }
                    }

                    return bytesReaded;                                 //////   戻り値 = 読み取ったバイト数 で関数終了
                }
                catch
                {                                                       /////  catch：すべての例外
                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する

                    if (ctsTotal.IsCancellationRequested)
                    {                                                   //////   取り消し要求による例外の場合(総合キャンセルトークンソースが取り消し要求状態の場合)
                        if (ctsTimeout.IsCancellationRequested)
                        {                                               ///////    タイムアウトによる取り消しの場合(タイムアウト用キャンセルトークンソースが取り消し要求状態の場合)
                            throw new TimeoutException();               ////////     タイムアウト例外をスローする
                        }
                        else
                        {                                               ///////    タイムアウトによる取り消しでない場合
                            throw new OperationCanceledException();     ////////     操作取り消し例外をスローする
                        }
                    }
                    else
                    {                                                   //////   取り消し要求による例外でない場合(総合キャンセルトークンソースが取り消し要求状態でない場合)
                        throw;                                          ///////    通常のI/Oエラーと判断して、例外を再スローする
                    }
                }
            }
        }

    } // class

} // namespace
