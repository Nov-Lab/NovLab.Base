// @(h)XStream.cs ver 0.00 ( '24.04.10 Nov-Lab ) 作成開始
// @(h)XStream.cs ver 0.21 ( '24.04.25 Nov-Lab ) アルファ版完成
// @(h)XStream.cs ver 0.22 ( '24.05.19 Nov-Lab ) 機能追加：.NET Framework 4.5.1 の PipeStream でキャンセルトークンが機能しない問題に対応した
// @(h)XStream.cs ver 0.22a( '24.05.22 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【Stream 拡張メソッド】Stream の派生クラスに拡張メソッドを追加します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.IO;
using System.IO.Pipes;

using NovLab.IO.Pipes;


namespace NovLab.IO
{
    //====================================================================================================
    /// <summary>
    /// 【Stream 拡張メソッド】<see cref="Stream"/> の派生クラスに拡張メソッドを追加します。<br/>
    /// <br/>
    /// タイムアウト付きでの非同期ストリーム読み書き<br/>
    /// <see cref="XReadAsync(Stream, byte[], int, int, int, CancellationToken)"/><br/>
    /// <see cref="XWriteAsync(Stream, byte[], int, int, int, CancellationToken)"/><br/>
    /// ・<see cref="Stream"/> クラスの非同期メソッドにタイムアウト処理を追加したメソッドです。<br/>
    /// ・ストリームがタイムアウトをサポートせず、<see cref="Stream.ReadTimeout"/> や <see cref="Stream.WriteTimeout"/> に対応していなくても、
    /// タイムアウト処理を行うことができます。<br/>
    /// <br/>
    /// Int32値の読み書き<br/>
    /// <see cref="XReadInt32Async(Stream, int, CancellationToken)"/><br/>
    /// <see cref="XWriteInt32Async(Stream, int, int, CancellationToken)"/><br/>
    /// </summary>
    /// <remarks>
    /// 補足<br/>
    /// ・データチャンク形式の読み書きを行う拡張メソッドは <see cref="XStream_Chunk"/> クラスで定義しています。<br/>
    /// </remarks>
    //====================================================================================================
    public static partial class XStream
    {
        // ＜メモ＞
        // ・WriteAsync / ReadAsync メソッドでは、キャンセルトークンを使って処理を取り消すことができる。
        //   しかし：PipeStream クラスの WriteAsync / ReadAsync メソッドでは、キャンセルトークンが機能しない(詳しくは XPipeStream.cs を参照)。
        //   なので：キャンセル処理を自力で行う拡張メソッドを追加した。
        //   ただし：ストリームの種類に応じて、適切な WriteAsync / ReadAsync メソッドを呼び出す必要がある。
        //   なので：ユーティリティーメソッドを用意した。
        //
        // ＜.NET Framework 4.5.1 時点での WriteAsync / ReadAsync メソッドの派生クラスでのオーバーライド状況＞
        //   × Microsoft.JScript.COMCharStream
        //   × System.Data.OracleClient.OracleBFile
        //   × System.Data.OracleClient.OracleLob
        //   × System.Data.SqlTypes.SqlFileStream
        //   ○ System.IO.BufferedStream
        //   × System.IO.Compression.DeflateStream
        //   × System.IO.Compression.GZipStream
        //   ○ System.IO.FileStream
        //   ○ System.IO.MemoryStream
        // *1× System.IO.Pipes.PipeStream
        //   × System.IO.UnmanagedMemoryStream
        //   × System.Net.Security.AuthenticatedStream
        //   × System.Net.Sockets.NetworkStream
        //   × System.Printing.PrintQueueStream
        //   ○ System.Security.Cryptography.CryptoStream
        // *1：キャンセル機能が使えるように拡張メソッドを用意し、ユーティリティーメソッドも対応させた。
        //====================================================================================================
        // WriteAsync / ReadAsync メソッド呼び出し用のユーティリティーメソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期ストリーム書き込み呼び出し】ストリームの種類に応じて、適切な WriteAsync メソッドを呼び出すユーティリティーメソッドです。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">           [in ]：対象ストリーム</param>
        /// <param name="buffer">           [in ]：バイト配列。このメソッドは、<paramref name="buffer"/> からストリームに、<paramref name="count"/> で指定されたバイト数だけコピーします。</param>
        /// <param name="offset">           [in ]：ストリームへのバイトのコピーを開始する位置を示す <paramref name="buffer"/> 内のバイト オフセット。インデックス番号は 0 から始まります。</param>
        /// <param name="count">            [in ]：ストリームに書き込むバイト数。</param>
        /// <param name="cancellationToken"><inheritdoc cref="XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// 戻り値を返さない非同期操作タスク
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XMLDOC.EX_TimeoutException"/></exception>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="Stream.WriteAsync(byte[], int, int, CancellationToken)"/> メソッドの代わりに呼び出すことで、
        ///   ストリームの種類に応じた適切なメソッドを呼び出すことができます。<br/>
        /// <br/>
        /// 動作概要<br/>
        /// ・<see cref="PipeStream"/> の場合は、<see cref="XPipeStream.XPipeWriteAsync"/> を呼び出します。<br/>
        /// ・上記以外の場合は WriteAsync メソッドを呼び出します。<br/>
        /// （派生クラスでオーバーライドしている WriteAsync メソッドか、オーバーライドしていない場合は基本クラスの Stream.WriteAsync が呼び出されます。）<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static async Task XCallWriteAsync(this Stream target,
                                                 byte[] buffer, int offset, int count,
                                                 CancellationToken cancellationToken)
        {
            // ＜メモ＞
            // ・拡張メソッドで派生や継承を疑似的に再現するための工夫
            // ・階層の深い派生クラスから順番に呼び出すこと
            //------------------------------------------------------------
            /// 派生クラス用の拡張メソッドが用意されている場合はそちらを呼び出す
            //------------------------------------------------------------
            if (target is PipeStream pipeStream)
            {                                                           //// PipeStream クラスの場合は XPipeWriteAsync 拡張メソッド を呼び出す
                await pipeStream.XPipeWriteAsync(
                            buffer, offset, count, 
                            Timeout.Infinite, cancellationToken);
                return;
            }


            //------------------------------------------------------------
            /// 上記以外の場合は WriteAsync メソッドを呼び出す
            ///-(派生クラスでオーバーライドしている場合はそちらが呼び出される)
            //------------------------------------------------------------
            await target.WriteAsync(buffer, offset, count, cancellationToken);
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期ストリーム読み取り呼び出し】ストリームの種類に応じて、適切な ReadAsync メソッドを呼び出すユーティリティーメソッドです。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">           [in ]：対象ストリーム</param>
        /// <param name="buffer">           [in ]：バイト配列。このメソッドが戻るとき、指定したバイト配列の <paramref name="offset"/> から (<paramref name="offset"/> + <paramref name="count"/> -1) までの値が、ストリームから読み取られたバイトに置き換えられます。</param>
        /// <param name="offset">           [in ]：ストリームから読み取ったデータの格納を開始する位置を示す <paramref name="buffer"/> 内のバイト オフセット。インデックス番号は 0 から始まります。</param>
        /// <param name="count">            [in ]：ストリームから読み取る最大バイト数。</param>
        /// <param name="cancellationToken"><inheritdoc cref="XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// バッファーに読み取られた合計バイト数。要求しただけのバイト数を読み取ることができなかった場合、この値は要求したバイト数より小さくなります。ストリームの末尾に到達した場合は 0 (ゼロ) になることがあります。<br/>
        /// ※上記戻り値を返す非同期操作タスクを返します。<br/>
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XMLDOC.EX_TimeoutException"/></exception>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="Stream.ReadAsync(byte[], int, int, CancellationToken)"/> メソッドの代わりに呼び出すことで、
        ///   ストリームの種類に応じた適切なメソッドを呼び出すことができます。<br/>
        /// <br/>
        /// 動作概要<br/>
        /// ・<see cref="PipeStream"/> の場合は、<see cref="XPipeStream.XPipeReadAsync"/> を呼び出します。<br/>
        /// ・上記以外の場合は ReadAsync メソッドを呼び出します。<br/>
        /// （派生クラスでオーバーライドしている ReadAsync メソッドか、オーバーライドしていない場合は基本クラスの Stream.ReadAsync が呼び出されます。）<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static async Task<int> XCallReadAsync(this Stream target,
                                                     byte[] buffer, int offset, int count,
                                                     CancellationToken cancellationToken)
        {
            // ＜メモ＞
            // ・拡張メソッドで派生や継承を疑似的に再現するための工夫
            // ・階層の深い派生クラスから順番に呼び出すこと
            //------------------------------------------------------------
            /// 派生クラス用の拡張メソッドが用意されている場合はそちらを呼び出す
            //------------------------------------------------------------
            if (target is PipeStream pipeStream)
            {                                                           //// PipeStream クラスの場合は XPipeReadAsync 拡張メソッド を呼び出す
                return await pipeStream.XPipeReadAsync(buffer, offset, count, Timeout.Infinite, cancellationToken);
            }


            //------------------------------------------------------------
            /// 上記以外の場合は ReadAsync メソッドを呼び出す
            ///-(派生クラスでオーバーライドしている場合はそちらが呼び出される)
            //------------------------------------------------------------
            return await target.ReadAsync(buffer, offset, count, cancellationToken);
        }


        //====================================================================================================
        // タイムアウト付きでの非同期ストリーム読み書き
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【タイムアウト付き非同期ストリーム書き込み】
        /// 非同期でストリームにバイト シーケンスを書き込み、書き込んだバイト数の分だけストリームの現在位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象ストリーム</param>
        /// <param name="buffer">             [in ]：バイト配列。このメソッドは、<paramref name="buffer"/> からストリームに、<paramref name="count"/> で指定されたバイト数だけコピーします。</param>
        /// <param name="offset">             [in ]：ストリームへのバイトのコピーを開始する位置を示す <paramref name="buffer"/> 内のバイト オフセット。インデックス番号は 0 から始まります。</param>
        /// <param name="count">              [in ]：ストリームに書き込むバイト数。</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// 戻り値を返さない非同期操作タスク
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XMLDOC.EX_TimeoutException"/></exception>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="Stream.WriteAsync(byte[], int, int, CancellationToken)"/> へタイムアウト処理を追加した拡張メソッドです。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static async Task XWriteAsync(this Stream target,
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
            /// タイムアウト処理付きで非同期ストリーム書き込み処理を行う
            //------------------------------------------------------------
            using (var ctsTimeout = new CancellationTokenSource())      //// using：タイムアウト用キャンセルトークンソースを生成する
            using (var ctsTotal =                                       //// using：引数で指定されたキャンセルトークンとタイムアウト用キャンセルトークンを結合して、総合キャンセルトークンソースを生成する
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ctsTimeout.Token))
            {
                try
                {                                                       /////  try開始
                    ctsTimeout.CancelAfter(millisecondsTimeout);        //////   タイムアウト用キャンセルトークンソースにタイムアウト時間を設定する
                    await target.XCallWriteAsync(buffer, offset, count
                                          , ctsTotal.Token);            //////   非同期ストリーム書き込み呼び出し処理を行う
                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する
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
        /// 【タイムアウト付き非同期ストリーム読み取り】
        /// 非同期でストリームからバイト シーケンスを読み取り、読み取ったバイト数の分だけストリームの位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象ストリーム</param>
        /// <param name="buffer">             [in ]：バイト配列。このメソッドが戻るとき、指定したバイト配列の <paramref name="offset"/> から (<paramref name="offset"/> + <paramref name="count"/> -1) までの値が、ストリームから読み取られたバイトに置き換えられます。</param>
        /// <param name="offset">             [in ]：ストリームから読み取ったデータの格納を開始する位置を示す <paramref name="buffer"/> 内のバイト オフセット。インデックス番号は 0 から始まります。</param>
        /// <param name="count">              [in ]：ストリームから読み取る最大バイト数。</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// バッファーに読み取られた合計バイト数。要求しただけのバイト数を読み取ることができなかった場合、この値は要求したバイト数より小さくなります。ストリームの末尾に到達した場合は 0 (ゼロ) になることがあります。<br/>
        /// ※上記戻り値を返す非同期操作タスクを返します。<br/>
        /// </returns>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XMLDOC.EX_TimeoutException"/></exception>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="Stream.ReadAsync(byte[], int, int, CancellationToken)"/> へタイムアウト処理を追加した拡張メソッドです。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static async Task<int> XReadAsync(this Stream target,
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
            /// タイムアウト処理付きで非同期ストリーム読み取り処理を行う
            //------------------------------------------------------------
            using (var ctsTimeout = new CancellationTokenSource())      //// using：タイムアウト用キャンセルトークンソースを生成する
            using (var ctsTotal =                                       //// using：引数で指定されたキャンセルトークンとタイムアウト用キャンセルトークンを結合して、総合キャンセルトークンソースを生成する
                        CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ctsTimeout.Token))
            {
                try
                {                                                       /////  try開始
                    ctsTimeout.CancelAfter(millisecondsTimeout);        //////   タイムアウト用キャンセルトークンソースにタイムアウト時間を設定する
                    var bytesReaded = await target.XCallReadAsync(
                        buffer, offset, count, ctsTotal.Token);         //////   非同期ストリーム読み取り呼び出し処理を行い、読み取ったバイト数を取得する
                    ctsTimeout.CancelAfter(Timeout.Infinite);           //////   タイムアウト用キャンセルトークンソースのタイムアウト時間を解除する

                    return bytesReaded;                                 //////   戻り値 = 読み取ったバイト数 で関数終了
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
        // ・BinaryWriter は、閉じたときに基になるストリームも閉じてしまうので、単発操作には向かない。
        //====================================================================================================
        // Int32値の読み書き
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期Int32値書き込み】非同期でInt32値をストリームへ書き込み、書き込んだバイト数の分だけストリームの位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象ストリーム</param>
        /// <param name="value">              [in ]：ストリームへ書き込む Int32値</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// 戻り値を返さない非同期操作タスク
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・このメソッドは、Int32値をリトルエンディアン形式で書き込みます。<br/>
        /// </remarks>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XMLDOC.EX_TimeoutException"/></exception>
        //--------------------------------------------------------------------------------
        public async static Task XWriteInt32Async(this Stream target,
                                                  int value,
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


            // ＜メモ＞
            // ・リトルエンディアン形式固定とするために MemoryStream と BinaryWriter を使っている。
            // ・BitConverter の方がシンプルだが、エンディアン形式がプラットフォーム依存になるため、
            //   たとえば Windows と Mac とでネットワーク経由でストリーム接続するような場合に、
            //   送信側と受信側のエンディアンの違いによる転送ミスが考えられる。
            //------------------------------------------------------------
            /// 非同期でInt32値をストリームへ書き込む
            //------------------------------------------------------------
            //----------------------------------------
            // Int32値をバイト配列へ変換
            //----------------------------------------
            byte[] buffer;  // 書き込みバッファー

            using (var memoryStream = new MemoryStream(4))              //// using：書き込みバッファー作成用にメモリーストリームを生成する
            using (var binaryWriter = new BinaryWriter(memoryStream))   //// using：メモリーストリームに対するバイナリーライターを生成する
            {
                binaryWriter.Write(value);                              /////  バイナリーライターを通じてメモリーストリームにInt32値を書き込む
                buffer = memoryStream.ToArray();                        /////  メモリーストリームの内容(バイト配列)を書き込みバッファーに取得する
            }

            //----------------------------------------
            // ストリームへ書き込む
            //----------------------------------------
            await target.XWriteAsync(buffer, 0, buffer.Length           //// 非同期で書き込みバッファーの内容をストリームへ書き込む
                    , millisecondsTimeout, cancellationToken);
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【非同期Int32読み取り】非同期でInt32値をストリームから読み取り、読み取ったバイト数の分だけストリームの位置を進めます。<br/>
        /// ※上記処理を行う非同期操作タスクを生成します。<br/>
        /// </summary>
        /// <param name="target">             [in ]：対象ストリーム</param>
        /// <param name="millisecondsTimeout"><inheritdoc cref="XMLDOC.MillisecondsTimeout"/></param>
        /// <param name="cancellationToken">  <inheritdoc cref="XMLDOC.CancellationToken"/></param>
        /// <returns>
        /// 読み取ったInt32値。<br/>
        /// ※上記戻り値を返す非同期操作タスクを返します。<br/>
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・このメソッドは、Int32値をリトルエンディアン形式で読み取ります。<br/>
        /// </remarks>
        /// <exception cref="OperationCanceledException"><inheritdoc cref="XMLDOC.EX_OperationCanceledException"/></exception>
        /// <exception cref="TimeoutException">          <inheritdoc cref="XMLDOC.EX_TimeoutException"/></exception>
        /// <exception cref="EndOfStreamException">      ストリーム終了例外。Int32値の読み取り中にストリームが末尾に到達しました。</exception>
        //--------------------------------------------------------------------------------
        public async static Task<int> XReadInt32Async(this Stream target,
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


            // ＜メモ＞
            // ・リトルエンディアン形式固定とするために MemoryStream と BinaryReader を使っている。
            // ・BitConverter の方がシンプルだが、エンディアン形式がプラットフォーム依存になるため、
            //   たとえば Windows と Mac とでネットワーク経由でストリーム接続するような場合に、
            //   送信側と受信側のエンディアンの違いによる転送ミスが考えられる。
            //------------------------------------------------------------
            /// 非同期でInt32値をストリームから読み取る
            //------------------------------------------------------------
            //----------------------------------------
            // ストリームから読み取る
            //----------------------------------------
            var buffer = new byte[sizeof(int)];                         //// Int32値のサイズ分、読み取りバッファーを生成する
            var readSize = await target.XReadAsync(                     //// 入力ストリームから読み取りバッファーへデータを非同期で読み取る
                            buffer, 0, buffer.Length
                            , millisecondsTimeout, cancellationToken);
            if (readSize != buffer.Length)
            {                                                           //// 読み取りサイズがバッファーサイズと一致しない場合
                throw                                                   /////  ストリーム終了例外をスローする
                    new EndOfStreamException("Int32値の読み取り中にストリームが末尾に到達しました。");
            }                                                           // PipeStream:パイプが途中で切断された場合、FileStream:ファイルが途中で終了した場合、などに発生する

            //----------------------------------------
            // バイト配列からInt32値へ変換
            //----------------------------------------
            int result; // Int32値

            using (var memoryStream = new MemoryStream(buffer))         //// using：読み取りバッファーからメモリーストリームを生成する
            using (var binaryReader = new BinaryReader(memoryStream))   //// using：メモリーストリームに対するバイナリーリーダーを生成する
            {
                result = binaryReader.ReadInt32();                      /////  バイナリーリーダーを通じてメモリーストリームからInt32値を読み取る
            }

            return result;                                              //// 戻り値 = Int32値 で関数終了
        }

    } // class

} // namespace
