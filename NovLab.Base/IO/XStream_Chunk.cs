// @(h)XStream_Chunk.cs ver 0.00 ( '24.04.10 Nov-Lab ) 作成開始
// @(h)XStream_Chunk.cs ver 0.21 ( '24.04.25 Nov-Lab ) アルファ版完成
// @(h)XStream_Chunk.cs ver 0.21a( '24.05.22 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【Stream 拡張メソッド(データチャンク読み書き)】Stream の派生クラスに、データチャンクの読み書きを行う拡張メソッドを追加します。

//[-] 保留：XStream_Chunk。将来対応予定。可能になったらデバッグトレースに対応させる。
#if DEBUG           // DEBUGビルドのみ有効
#define VERBOSELOG  // 冗長ログ有効化
#endif

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.IO;


namespace NovLab.IO
{
    // ＜メモ＞
    // ・BinaryWriter や StreamWriter は、閉じたときに基になるストリームも閉じてしまうので、単発操作には向かない。
    //====================================================================================================
    /// <summary>
    /// 【Stream 拡張メソッド(データチャンク読み書き)】<see cref="Stream"/> の派生クラスに、データチャンクの読み書きを行う拡張メソッドを追加します。<br/>
    /// ・データチャンクとは、サイズ情報を付加したデータのかたまりのことです。
    ///   サイズ情報を付加することで、可変長データの読み書きを簡単に行うことができます。<br/>
    /// <br/>
    /// データチャンクの読み書き<br/>
    /// <see cref="XReadChunkAsync(Stream, int, CancellationToken)"/><br/>
    /// <see cref="XWriteChunkAsync(Stream, byte[], int, CancellationToken)"/><br/>
    /// ・バイト配列をデータチャンク形式で読み書きします。<br/>
    /// <br/>
    /// 文字列チャンクの読み書き<br/>
    /// <see cref="XReadStringChunkAsync(Stream, int, CancellationToken)"/><br/>
    /// <see cref="XWriteStringChunkAsync(Stream, string, int, CancellationToken)"/><br/>
    /// ・文字列をデータチャンク形式で読み書きします。クラスインスタンスや構造体をXML文字列形式で読み書きするのに適しています。<br/>
    /// </summary>
    /// <remarks>
    /// 補足<br/>
    /// ・基本的な拡張メソッドは <see cref="XStream"/> クラスで定義しています。<br/>
    /// <br/>
    /// データチャンク形式の構造は以下のようになっています。<br/>
    /// 1.チャンクサイズ(リトルエンディアン形式の Int32値)<br/>
    /// 2.チャンクデータ本体(チャンクサイズの長さのByte配列)<br/>
    /// </remarks>
    //====================================================================================================
    public static partial class XStream_Chunk
    {
        //[-] 保留：必要になったら大きなチャンクを小さく分割して送受信する機能。(予定メソッド名：XWriteLargeChunkAsync / XReadLargeChunkAsync)
        //          「ネットワーク経由での名前付きパイプへの書き込み操作では、上限は 65,535 バイトです。」
        //====================================================================================================
        // サイズ情報付きデータチャンクの読み書き
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期データチャンク書き込み】
        /// 非同期でストリームにデータチャンクを書き込み、書き込んだバイト数の分だけストリームの現在位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象ストリーム</param>
        /// <param name="chunkData">          [in ]：チャンクデータ(バイト配列)</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XStream.XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XStream.XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// 戻り値を返さない非同期操作タスク
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XStream.XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XStream.XMLDOC.EX_TimeoutException"/></exception>
        //--------------------------------------------------------------------------------
        public async static Task XWriteChunkAsync(this Stream target,
                                                  byte[] chunkData,
                                                  int millisecondsTimeout, CancellationToken cancellationToken)
        {
            //------------------------------------------------------------
            /// 引数をチェックする
            //------------------------------------------------------------
            if (millisecondsTimeout < -1)
            {                                                           //// タイムアウト時間(ミリ秒)が -1 以外の負数の場合
                throw                                                   /////  引き数不正例外(値範囲)をスローする
                    new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }


            //------------------------------------------------------------
            /// 非同期でデータチャンクを書き込む
            //------------------------------------------------------------
#if VERBOSELOG
            Debug.Print($"{nameof(XWriteChunkAsync)}:チャンクサイズ:" + chunkData.Length);
#endif

            using (var ctsTimeout = new CancellationTokenSource())      //// using：タイムアウト用キャンセルトークンソースを生成する
            using (var ctsTotal =                                       //// using：引数で指定されたキャンセルトークンとタイムアウト用キャンセルトークンを結合して、総合キャンセルトークンソースを生成する
                        CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ctsTimeout.Token))
            {
                try
                {                                                       /////  try開始
                    ctsTimeout.CancelAfter(millisecondsTimeout);        //////   タイムアウト用キャンセルトークンソースにタイムアウト時間を設定する

                    await target.XWriteInt32Async(chunkData.Length
                                , Timeout.Infinite, ctsTotal.Token);    //////   非同期でチャンクサイズを書き込む
                    await target.XWriteAsync(chunkData, 0, chunkData.Length
                                , Timeout.Infinite, ctsTotal.Token);    //////   非同期でチャンクデータ本体を書き込む

                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する

                    return;                                             //////   関数終了
                }
                catch (OperationCanceledException)
                {                                                       /////  catch：操作取り消し例外
                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する

                    if (ctsTimeout.IsCancellationRequested)
                    {                                                   //////   タイムアウトによる取り消しの場合(タイムアウト用キャンセルトークンソースが取り消し要求状態の場合)
                        throw new TimeoutException();                   ///////    タイムアウト例外をスローする
                    }
                    else
                    {                                                   //////   タイムアウトによる取り消しでない場合
                        throw;                                          ///////    例外を再スローする
                    }
                }                                                       /////  上記以外の例外は処理せずに呼び出し元へ伝える
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期データチャンク読み取り】
        /// 非同期でストリームからデータチャンクを読み取り、読み取ったバイト数の分だけストリームの位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象ストリーム</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XStream.XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XStream.XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// 読み取ったチャンクデータ(バイト配列)。
        /// ※上記戻り値を返す非同期操作タスクを返します。<br/>
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XStream.XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XStream.XMLDOC.EX_TimeoutException"/></exception>
        /// <exception cref="EndOfStreamException">      ストリーム終了例外。チャンクの読み取り中にストリームが末尾に到達しました。</exception>
        //--------------------------------------------------------------------------------
        public async static Task<byte[]> XReadChunkAsync(this Stream target,
                                                         int millisecondsTimeout, CancellationToken cancellationToken)
        {
            //------------------------------------------------------------
            /// 引数をチェックする
            //------------------------------------------------------------
            if (millisecondsTimeout < -1)
            {                                                           //// タイムアウト時間(ミリ秒)が -1 以外の負数の場合
                throw                                                   /////  引き数不正例外(値範囲)をスローする
                    new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }


            //------------------------------------------------------------
            /// 非同期でデータチャンクを読み取る
            //------------------------------------------------------------
            using (var ctsTimeout = new CancellationTokenSource())      //// using：タイムアウト用キャンセルトークンソースを生成する
            using (var ctsTotal =                                       //// using：引数で指定されたキャンセルトークンとタイムアウト用キャンセルトークンを結合して、総合キャンセルトークンソースを生成する
                        CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ctsTimeout.Token))
            {
                try
                {                                                       /////  try開始
                    //----------------------------------------
                    // 前準備
                    //----------------------------------------
                    ctsTimeout.CancelAfter(millisecondsTimeout);        //////   タイムアウト用キャンセルトークンソースにタイムアウト時間を設定する


                    //----------------------------------------
                    // チャンクサイズ
                    //----------------------------------------
                    var chunkSize =                                     //////   非同期でチャンクサイズを読み取る
                        await target.XReadInt32Async(Timeout.Infinite, cancellationToken);

#if VERBOSELOG
                    Debug.Print($"{nameof(XReadChunkAsync)}:チャンクサイズ:" + chunkSize);
#endif


                    //----------------------------------------
                    // チャンクデータ本体
                    //----------------------------------------
                    var chunkData = new byte[chunkSize];                //////   チャンクサイズ分、チャンクデータバッファを生成する

                    var readSize = await target.XReadAsync(
                                        chunkData, 0, chunkSize,
                                        Timeout.Infinite,
                                        cancellationToken);             //////   非同期でチャンクデータ本体を読み取る

#if VERBOSELOG
                    Debug.Print($"{nameof(XReadChunkAsync)}:読み取りサイズ:" + readSize);
#endif
                    if (readSize != chunkSize)
                    {                                                   //////   読み取りサイズがチャンクサイズと一致しない場合
                        throw                                           ///////    ストリーム終了例外をスローする
                            new EndOfStreamException("チャンクの読み取り中にストリームが末尾に到達しました。");
                    }                                                   // PipeStream:パイプが途中で切断された場合、FileStream:ファイルが途中で終了した場合、などに発生する


                    //----------------------------------------
                    // 後始末
                    //----------------------------------------
                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する

                    return chunkData;                                   //////   戻り値 = チャンクデータ で関数終了
                }
                catch (OperationCanceledException)
                {                                                       /////  catch：操作取り消し例外
                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する

                    if (ctsTimeout.IsCancellationRequested)
                    {                                                   //////   タイムアウトによる取り消しの場合(タイムアウト用キャンセルトークンソースが取り消し要求状態の場合)
                        throw new TimeoutException();                   ///////    タイムアウト例外をスローする
                    }
                    else
                    {                                                   //////   タイムアウトによる取り消しでない場合
                        throw;                                          ///////    例外を再スローする
                    }
                }                                                       /////  上記以外の例外は処理せずに呼び出し元へ伝える
            }
        }


        // ＜メモ＞
        // ・BinaryWriter.Write(string) / BinaryReader.ReadString は文字列長プリフィックス付きで文字列の読み書きができるが、非同期版のメソッドが用意されていない。
        // ・StreamWriter.WriteLineAsync(string) / StreamReader.ReadLineAsync は行単位での読み書きで、しかもキャンセルトークンに対応していない。
        //====================================================================================================
        // 文字列チャンクの読み書き
        //====================================================================================================

        //[-] 保留：必要になったらエンコーディングの指定機能を付ける。：XWriteStringChunkAsync / XReadStringChunkAsync
        /// <summary>
        /// 【エンコーディング】サイズ情報付き文字列チャンクで用いるエンコーディング(UTF-8)
        /// </summary>
        private static readonly Encoding m_encoding = new UTF8Encoding();


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期文字列チャンク書き込み】
        /// 非同期でストリームに文字列チャンクを書き込み、書き込んだバイト数の分だけストリームの現在位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象ストリーム</param>
        /// <param name="value">              [in ]：ストリームへ書き込む文字列</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XStream.XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XStream.XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// 戻り値を返さない非同期操作タスク
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XStream.XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XStream.XMLDOC.EX_TimeoutException"/></exception>
        //--------------------------------------------------------------------------------
        public async static Task XWriteStringChunkAsync(this Stream target,
                                                        string value,
                                                        int millisecondsTimeout, CancellationToken cancellationToken)
        {
            //------------------------------------------------------------
            /// 非同期で文字列チャンクを書き込む
            //------------------------------------------------------------
            var chunkData = m_encoding.GetBytes(value);                 //// 文字列をバイトシーケンスにエンコードしてチャンクデータを作成する
            await target.XWriteChunkAsync(chunkData                     //// 非同期データチャンク書き込み処理を行う
                                , millisecondsTimeout, cancellationToken);
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期文字列チャンク読み取り】
        /// 非同期でストリームから文字列チャンクを読み取り、読み取ったバイト数の分だけストリームの位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象ストリーム</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XStream.XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XStream.XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// 読み取った文字列。<br/>
        /// ※上記戻り値を返す非同期操作タスクを返します。<br/>
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XStream.XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XStream.XMLDOC.EX_TimeoutException"/></exception>
        //--------------------------------------------------------------------------------
        public async static Task<string> XReadStringChunkAsync(this Stream target,
                                                               int millisecondsTimeout, CancellationToken cancellationToken)
        {
            //------------------------------------------------------------
            /// 非同期で文字列チャンクを読み取る
            //------------------------------------------------------------
            var chunkData =                                             //// 非同期データチャンクを読み取り処理を行う
                await target.XReadChunkAsync(millisecondsTimeout, cancellationToken);
            return m_encoding.GetString(chunkData);                     //// 読み取ったバイトシーケンスを文字列にデコードして戻り値とし、関数終了
        }

    } // class

} // namespace
