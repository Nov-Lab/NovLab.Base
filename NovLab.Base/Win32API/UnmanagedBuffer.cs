// @(h)UnmanagedBuffer.cs ver 0.00 ( '24.01.13 Nov-Lab ) 作成開始
// @(h)UnmanagedBuffer.cs ver 0.51 ( '24.01.16 Nov-Lab ) ベータ版完成
// @(h)UnmanagedBuffer.cs ver 0.51a( '24.01.20 Nov-Lab ) 仕変対応：AutoTest クラスの仕様変更に対応した。機能変更なし。
// @(h)UnmanagedBuffer.cs ver 0.51b( '24.01.23 Nov-Lab ) その他  ：コメントを微修正

// @(s)
// 　【アンマネージバッファー】アンマネージメモリ上にバッファーを作成する機能を提供します。

#if DEBUG
// #define VERBOSELOG  // 冗長ログ有効化
#endif

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using NovLab.DebugSupport;


namespace NovLab.Win32
{
    //====================================================================================================
    /// <summary>
    /// 【アンマネージバッファー】アンマネージメモリ上にバッファーを作成する機能を提供します。<br></br>
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・構造体用にバッファーを作成する場合は <see cref="UnmanagedBuffer{TStruct}"/> が便利です。<br></br>
    /// ・Marshal クラスによるメモリ操作を簡略化するためのユーティリティクラスです。<br></br>
    /// ・IntPtr ポインターを取得して API に渡したり、Marshal クラスのメモリ操作に使うことができます。<br></br>
    /// ・using ブロックで安全に使用できます。<br></br>
    /// ・割り当てられたメモリはゼロクリアされていないため、内容は未定義です。<br></br>
    /// </remarks>
    //====================================================================================================
    public partial class UnmanagedBuffer : DisposablePattern
    {
        //====================================================================================================
        // 内部フィールド
        //====================================================================================================

        /// <summary>
        /// 【メモリポインター】割り当てられたメモリへのポインター。IntPtr.Zero = 解放済み
        /// </summary>
        protected IntPtr m_hGlobal = IntPtr.Zero;

        /// <summary>
        /// 【バッファーサイズ(バイト数)】-1 = 解放済み
        /// </summary>
        protected int m_bufferSize;


        // ＜メモ＞
        // ・Min/Max の名前付けは SYSTEM_INFO 構造体を参考にした。
        /// <summary>
        /// 【バッファーの先頭アドレス】
        /// </summary>
        protected IntPtr m_minAddress;

        /// <summary>
        /// 【バッファーの終了アドレス】
        /// </summary>
        protected IntPtr m_maxAddress;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列形式作成】このインスタンスの内容を表す文字列を取得します。<br></br>
        /// </summary>
        /// <returns>文字列形式</returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・文字列形式は以下のような書式になります。
        /// <code>
        /// 書式：＜先頭アドレス＞(＜バイト数＞ bytes)[＜簡易ダンプ＞]
        /// 例  ：0x12345678(16 bytes)[000102030405060708090A0B0C0D0E0F]
        /// </code>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public override string ToString()
        {
            const int MAX_SIZE = 32;    // 簡易ダンプの最大サイズ

            //------------------------------------------------------------
            /// このインスタンスの内容を表す文字列を取得する
            //------------------------------------------------------------
            M_ThrowExceptionIfDisposed();                               //// オブジェクトを破棄済みの場合は例外をスローする

            string suffix = "";                                         //// サフィックス文字列 = 空文字列 に初期化する

            var dumpSize = Math.Min(m_bufferSize, MAX_SIZE);            //// ダンプサイズ = バッファーサイズと簡易ダンプの最大サイズとで小さい方
            if (dumpSize < m_bufferSize)
            {                                                           //// ダンプサイズ < バッファーサイズ の場合(簡易ダンプが切り捨てられる場合)
                suffix = "...";                                         /////  サフィックス文字列 = "..."
            }

            var dumpBuffer = new byte[dumpSize];                        //// ダンプサイズ分、簡易ダンプ領域を作成する
            Marshal.Copy(m_hGlobal, dumpBuffer, 0, dumpSize);           //// ダンプサイズ分、バッファーの内容を簡易ダンプ領域へコピーする

            return string.Format("0x{0:X8}({1} bytes)[{2}]"             //// 文字列形式を作成して戻り値とし、関数終了
                , m_hGlobal
                , m_bufferSize
                , HexString.FromBytes(dumpBuffer) + suffix);
        }


        //====================================================================================================
        // 公開プロパティー
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メモリポインター】アンマネージメモリ上に作成したバッファーへのポインター
        /// </summary>
        /// <exception cref="ObjectDisposedException">すでにメモリを解放済みです。</exception>
        //--------------------------------------------------------------------------------
        public IntPtr IntPtr
        {
            get
            {
                //------------------------------------------------------------
                /// メモリポインターを取得する
                //------------------------------------------------------------
                M_ThrowExceptionIfDisposed();                               //// オブジェクトを破棄済みの場合は例外をスローする
                return m_hGlobal;                                           //// 戻り値 = メモリポインター で関数終了
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【バッファーサイズ】アンマネージメモリ上に作成したバッファーのサイズ
        /// </summary>
        //--------------------------------------------------------------------------------
        public int Size => m_bufferSize;


        //====================================================================================================
        // コンストラクターと変換演算子
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【完全コンストラクター】アンマネージメモリ上にバッファーを作成します。
        /// </summary>
        /// <param name="bufferSize">[in ]：バッファーサイズ</param>
        //--------------------------------------------------------------------------------
        public UnmanagedBuffer(int bufferSize)
        {
            //------------------------------------------------------------
            /// アンマネージメモリ上にバッファーを作成する
            //------------------------------------------------------------
            if (bufferSize <= 0)
            {                                                           //// バッファーサイズ <= 0 の場合
                throw new                                               /////  引数不正例外をスローする
                    ArgumentException("バッファーサイズが０以下です。", nameof(bufferSize));
            }

            m_bufferSize = bufferSize;                                  //// バッファーサイズを内部フィールドに格納する
            m_hGlobal = Marshal.AllocHGlobal(bufferSize);               //// バッファーサイズ分メモリを割り当てて、ポインターを取得する

            m_minAddress = m_hGlobal;                                   //// バッファーの先頭アドレス = ポインター
            m_maxAddress = IntPtr.Add(m_minAddress, m_bufferSize - 1);  //// バッファーの最終アドレス = ポインター＋バッファサイズ－１

#if VERBOSELOG
            Debug.Print("Alloc Buffer 0x{0:X8}", m_hGlobal);
#endif
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【明示的な変換演算子(アンマネージバッファー -> IntPtr)】アンマネージバッファーを、IntPtr に変換します。
        /// </summary>
        /// <param name="target">[in ]：アンマネージバッファー</param>
        /// <exception cref="ObjectDisposedException">すでにメモリを解放済みです。</exception>
        //--------------------------------------------------------------------------------
        public static explicit operator IntPtr(UnmanagedBuffer target) => target.IntPtr;


        //====================================================================================================
        // DisposablePattern の実装
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【マネージリソース破棄】using ブロックを抜けるときに呼び出され、クリーンアップメソッドを実行します。
        /// </summary>
        //--------------------------------------------------------------------------------
        protected override void M_DisposeManagedResource()
        {
            //------------------------------------------------------------
            /// クリーンアップメソッドを実行する
            //------------------------------------------------------------
            if (m_hGlobal != IntPtr.Zero)
            {                                                           //// メモリを解放していない場合(メモリポインターが０でない場合)
                Free();                                                 /////  メモリ解放処理を行う
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【アンマネージリソース破棄】本クラスではアンマネージリソースは扱いません。
        /// </summary>
        //--------------------------------------------------------------------------------
        protected override void M_DisposeUnmanagedResource() { }


        //====================================================================================================
        // 内部メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【オブジェクトを破棄済みの場合は例外をスロー】
        /// オブジェクトが破棄済み(バッファーメモリを解放済み)の場合は例外をスローします。
        /// </summary>
        //--------------------------------------------------------------------------------
        protected void M_ThrowExceptionIfDisposed()
        {
            //------------------------------------------------------------
            /// オブジェクトが破棄済み(バッファーメモリを解放済み)の場合は例外をスローする
            //------------------------------------------------------------
            if (m_hGlobal == IntPtr.Zero)
            {                                                           //// メモリを解放済みの場合(メモリポインター = 0 の場合)
                throw new ObjectDisposedException(null);                /////  オブジェクト破棄済み例外をスローする
            }
        }


        //====================================================================================================
        // 公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【バッファーメモリ解放】アンマネージメモリ上に作成したバッファーを解放します。
        /// </summary>
        /// <exception cref="ObjectDisposedException">すでにメモリを解放済みです。</exception>
        //--------------------------------------------------------------------------------
        public void Free()
        {
            //------------------------------------------------------------
            /// アンマネージメモリ上に作成したバッファーを解放する
            //------------------------------------------------------------
            M_ThrowExceptionIfDisposed();                               //// オブジェクトを破棄済みの場合は例外をスローする

            if (m_hGlobal != IntPtr.Zero)
            {                                                           //// メモリを解放していない場合(メモリポインターが０でない場合)
#if VERBOSELOG
                Debug.Print("Free Buffer 0x{0:X8}", m_hGlobal);
#endif

                Marshal.FreeHGlobal(m_hGlobal);                         /////  メモリを解放する
                m_hGlobal = IntPtr.Zero;                                /////  メモリポインター = IntPtr.Zero にクリアする
                m_bufferSize = -1;                                      /////  バッファーサイズ = -1(解放済み) にクリアする
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ゼロクリア】バッファー全体をゼロクリアします。
        /// </summary>
        /// <exception cref="ObjectDisposedException">すでにメモリを解放済みです。</exception>
        //--------------------------------------------------------------------------------
        public void ZeroMemory()
        {
            //------------------------------------------------------------
            /// バッファー全体をゼロクリアする
            //------------------------------------------------------------
            M_ThrowExceptionIfDisposed();                               //// オブジェクトを破棄済みの場合は例外をスローする
            Win32API.RtlZeroMemory(m_hGlobal, (IntPtr)m_bufferSize);    //// バッファー全体をゼロクリアする
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【フィル】バッファー全体に指定されたバイト値を書き込みます。
        /// </summary>
        /// <param name="value">[in ]：書き込むバイト値</param>
        /// <exception cref="ObjectDisposedException">すでにメモリを解放済みです。</exception>
        //--------------------------------------------------------------------------------
        public void FillMemory(byte value)
        {
            //------------------------------------------------------------
            /// バッファー全体に指定されたバイト値を書き込む
            //------------------------------------------------------------
            M_ThrowExceptionIfDisposed();                               //// オブジェクトを破棄済みの場合は例外をスローする
            Win32API.RtlFillMemory(
                m_hGlobal, (IntPtr)m_bufferSize, value);                //// バッファー全体に指定されたバイト値を書き込む
        }


        //[-] 保留：必要になったら、他のデータ型に対応するRead/Write系メソッドも作成する
        //====================================================================================================
        // 公開メソッド(アドレス範囲チェック付きのデータ読み書き)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【オフセット位置がバッファー範囲外の場合は例外をスロー】
        /// オフセット位置がバッファー範囲外の場合は例外をスローします。
        /// </summary>
        /// <param name="offset">[in ]：オフセット位置</param>
        //--------------------------------------------------------------------------------
        protected void M_ThrowExceptionIfOutOfRange(int offset)
        {
            //------------------------------------------------------------
            /// オフセット位置がバッファー範囲外の場合は例外をスローする
            //------------------------------------------------------------
            IntPtr address = m_minAddress + offset;                     //// アドレス = バッファーの先頭アドレス＋オフセット
            var isInRange 
                = address.XIsInAddressRange(m_minAddress, m_maxAddress);//// アドレスが先頭アドレスから終了アドレスの範囲内にあるかチェック
            if(isInRange == false)
            {                                                           //// 範囲内でない場合
                throw new IndexOutOfRangeException(                     /////  オフセット範囲外例外をスローする
                    "オフセットがバッファーの範囲外です。");

            }
        }


#if false   //[-] 保留：作りかけ。ReadInt32 などバイトサイズ以外の読み書きメソッドを作る場合はこちらを使う予定
        protected void M_ThrowExceptionIfOutOfRange(int offset, int dataSize)
        {
            M_ThrowExceptionIfOutOfRange(offset);
            M_ThrowExceptionIfOutOfRange(offset + dataSize - 1);
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【１バイト読み取り】バッファーの先頭アドレス＋オフセットの位置から１バイトを読み取ります。
        /// </summary>
        /// <param name="offset">[in ]：オフセット位置</param>
        /// <returns>読み取ったバイト値</returns>
        /// <exception cref="ObjectDisposedException">すでにメモリを解放済みです。</exception>
        /// <exception cref="IndexOutOfRangeException">オフセットがバッファーの範囲外です。</exception>
        //--------------------------------------------------------------------------------
        public byte ReadByte(int offset)
        {
            //------------------------------------------------------------
            /// バッファーの先頭アドレス＋オフセットの位置から１バイトを読み取る
            //------------------------------------------------------------
            M_ThrowExceptionIfDisposed();                               //// オブジェクトを破棄済みの場合は例外をスローする
            M_ThrowExceptionIfOutOfRange(offset);                       //// オフセット位置がバッファー範囲外の場合は例外をスローする
            return Marshal.ReadByte(m_minAddress, offset);              //// バッファーの先頭アドレス＋オフセットの位置から１バイトを読み取り、その結果を戻り値として関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【１バイト書き込み】バッファーの先頭アドレス＋オフセットの位置へ１バイトを書き込みます。
        /// </summary>
        /// <param name="offset">[in ]：オフセット位置</param>
        /// <param name="value"> [in ]：バイト値</param>
        /// <exception cref="ObjectDisposedException">すでにメモリを解放済みです。</exception>
        /// <exception cref="IndexOutOfRangeException">オフセットがバッファーの範囲外です。</exception>
        //--------------------------------------------------------------------------------
        public void WriteByte(int offset, byte value)
        {
            //------------------------------------------------------------
            /// バッファーの先頭アドレス＋オフセットの位置へ１バイトを書き込む
            //------------------------------------------------------------
            M_ThrowExceptionIfDisposed();                               //// オブジェクトを破棄済みの場合は例外をスローする
            M_ThrowExceptionIfOutOfRange(offset);                       //// オフセット位置がバッファー範囲外の場合は例外をスローする
            Marshal.WriteByte(m_minAddress, offset, value);             //// バッファーの先頭アドレス＋オフセットの位置へ１バイトを書き込む
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド(ReadByte と WriteByte)
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod(nameof(UnmanagedBuffer) + "." + nameof(ReadByte) + "/" + nameof(WriteByte))]
        public static void ZZZ_ReadByte_WriteByte()
        {
            const int BUFFER_SIZE = 8;

            using (var buffer = new UnmanagedBuffer(BUFFER_SIZE))
            {
                for (var offset = 0; offset < buffer.Size; offset++)
                {
                    byte value = (byte)(offset * 10 + offset);
                    SubRoutine(buffer, offset, value, value);
                }

                SubRoutine(buffer, -1, 0, typeof(IndexOutOfRangeException), "オフセットが-1");
                SubRoutine(buffer, BUFFER_SIZE, 0, typeof(IndexOutOfRangeException), "オフセットが範囲外");
            }

            void SubRoutine(UnmanagedBuffer target,                 // [in ]：UnmanagedBuffer のインスタンス
                            int offset,                             // [in ]：オフセット位置
                            byte value,                             // [in ]：バイト値
                            AutoTestResultInfo<byte> expectResult,  // [in ]：予想結果(バイト値)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                       // テストオプション
                    fncResultRegularString = RegularString.ToHex1byte,  // ・戻り値は1バイト用16進数文字列表記
                };

                // WriteByte の自動テスト
                if (offset.XIsInRange(0, target.Size-1))
                {                                                   // バッファー範囲内：検証するものがないので普通に呼び出す
                    target.WriteByte(offset, value);
                }
                else
                {                                                   // バッファー範囲外：例外が発生することを検証する
                    AutoTest.Test(target, target.WriteByte, offset, value, typeof(IndexOutOfRangeException), testPattern);
                }

                // ReadByte の自動テスト
                AutoTest.Test(target.ReadByte, offset, expectResult, testOptions);
            }
        }
#endif

    } // class


    //====================================================================================================
    /// <summary>
    /// 【構造体用アンマネージバッファー】アンマネージメモリ上に構造体用のバッファーを作成する機能を提供します。<br></br>
    /// </summary>
    /// <typeparam name="TStruct">構造体の型</typeparam>
    /// <remarks>
    /// 補足<br></br>
    /// ・Marshal クラスによるメモリ操作を簡略化するためのユーティリティクラスです。<br></br>
    /// ・IntPtr ポインターを取得して API に渡したり、Marshal クラスのメモリ操作に使うことができます。<br></br>
    /// ・using ブロックで安全に使用できます。<br></br>
    /// ・割り当てられたメモリはゼロクリアされていないため、内容は未定義です。<br></br>
    /// </remarks>
    //====================================================================================================
    public partial class UnmanagedBuffer<TStruct> : UnmanagedBuffer where TStruct : struct
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デフォルトコンストラクター】アンマネージメモリ上に構造体用のバッファーを作成します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public UnmanagedBuffer() : base(Marshal.SizeOf(typeof(TStruct)))
        {
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【構造体へコピー】バッファの内容を構造体へコピーします。
        /// </summary>
        /// <returns>コピー先構造体</returns>
        /// <exception cref="ObjectDisposedException">すでにメモリを解放済みです。</exception>
        //--------------------------------------------------------------------------------
        public TStruct CopyToStructure()
        {
            //------------------------------------------------------------
            /// バッファの内容を構造体へコピーする
            //------------------------------------------------------------
            M_ThrowExceptionIfDisposed();                               //// オブジェクトを破棄済みの場合は例外をスローする
            return (TStruct)Marshal.PtrToStructure(                     //// バッファの内容をコピーした新しい構造体を生成して戻り値とし、関数終了
                m_hGlobal, typeof(TStruct));
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【構造体からコピー】構造体の内容をバッファへコピーします。
        /// </summary>
        /// <param name="source">[in ]：コピー元構造体</param>
        /// <exception cref="ObjectDisposedException">すでにメモリを解放済みです。</exception>
        //--------------------------------------------------------------------------------
        public void CopyFromStructure(TStruct source)
        {
            //------------------------------------------------------------
            /// 構造体の内容をバッファへコピーする
            //------------------------------------------------------------
            M_ThrowExceptionIfDisposed();                               //// オブジェクトを破棄済みの場合は例外をスローする
            Marshal.StructureToPtr(source, m_hGlobal, false);           //// 構造体の内容をバッファにコピーする
        }

    } // class




    // 手動テスト用のクラス
#if DEBUG
    //====================================================================================================
    // UnmanagedBuffer の手動テスト用クラス
    //====================================================================================================
    public partial class ZZZTest_UnmanagedBuffer
    {
        [ManualTestMethod("UnmanagedBuffer の総合的テスト")]
        public static void ZZZ_OverallTest()
        {
            Debug.Print("＜汎用バッファー＞");
            using (var buffer = new UnmanagedBuffer(32))
            {
                Debug.Print("  割り当て直後：" + buffer.ToString());

                buffer.ZeroMemory();
                Debug.Print("  ゼロクリア後：" + buffer.ToString());

                buffer.FillMemory(0xAB);
                Debug.Print("  0xABフィル後：" + buffer.ToString());

                for (var offset = 0; offset < buffer.Size; offset++)
                {
                    buffer.WriteByte(offset, (byte)offset);
                }

                Debug.Print("  データ設定後：" + buffer.ToString());

                buffer.Free();
            }
            Debug.Print("");


            Debug.Print("＜構造体用バッファー＞");
            using (var buffer = new UnmanagedBuffer<M_SYSTEMTIME>())
            {
                M_SYSTEMTIME localTime1 = new M_SYSTEMTIME();
                M_SYSTEMTIME localTime2 = new M_SYSTEMTIME();

                Debug.Print("・LocalTime1 に現在のローカル日時を取得");
                M_GetLocalTime(ref localTime1);
                Debug.Print("  LocalTime1：" + localTime1);
                Debug.Print("  LocalTime2：" + localTime2);
                Debug.Print("");


                Debug.Print("・LocalTime1 からバッファへコピー");
                Debug.Print("  コピー前：" + buffer.ToString());
                buffer.CopyFromStructure(localTime1);
                Debug.Print("  コピー後：" + buffer.ToString());
                Debug.Print("");

                Debug.Print("・バッファから LocalTime2 へコピー");
                localTime2 = buffer.CopyToStructure();
                Debug.Print("  LocalTime1：" + localTime1);
                Debug.Print("  LocalTime2：" + localTime2);

                //buffer.Free();    // 破棄済みの場合の例外を確認したい場合は、確認したい箇所に配置する
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【SYSTEMTIME構造体】
        /// </summary>
        //--------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential)]
        protected struct M_SYSTEMTIME   // typedef struct _SYSTEMTIME
        {                               // {
            public ushort wYear;        //     WORD wYear;
            public ushort wMonth;       //     WORD wMonth;
            public ushort wDayOfWeek;   //     WORD wDayOfWeek;
            public ushort wDay;         //     WORD wDay;
            public ushort wHour;        //     WORD wHour;
            public ushort wMinute;      //     WORD wMinute;
            public ushort wSecond;      //     WORD wSecond;
            public ushort wMilliseconds;//     WORD wMilliseconds;
                                        // } SYSTEMTIME, *PSYSTEMTIME, *LPSYSTEMTIME;

            public override string ToString()
            {
                return string.Format("{0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2}:{5:D2}.{6:D4}"
                    , wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【GetLocalTime API関数】現在のローカル日時を取得します。
        /// </summary>
        /// <param name="lpSystemTime">[ref]：SYSTEMTIME構造体</param>
        //--------------------------------------------------------------------------------
        [DllImport("Kernel32.dll", EntryPoint = "GetLocalTime")]
        protected static extern void M_GetLocalTime(    // VOID GetLocalTime(
            ref M_SYSTEMTIME lpSystemTime               //   LPSYSTEMTIME lpSystemTime
        );                                              // );

    } // class
#endif

} // namespace
