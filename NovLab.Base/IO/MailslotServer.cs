// @(h)MailslotServer.cs ver 0.00 ( '22.03.24 Nov-Lab ) 作成開始
// @(h)MailslotServer.cs ver 0.51 ( '22.03.27 Nov-Lab ) ベータ版完成
// @(h)MailslotServer.cs ver 0.52 ( '22.04.29 Nov-Lab ) テスト  ：TestWin32Define を使用してWin32API定義が正しいことをテストした。
// @(h)MailslotServer.cs ver 0.53 ( '22.05.03 Nov-Lab ) 機能追加：while文で簡単に使えるように、GetMessageCount メソッドを追加した。
// @(h)MailslotServer.cs ver 0.53a( '22.05.24 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【メールスロットサーバー】プロセス間通信の機能の一つであるメールスロットの操作機能を提供します。

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NovLab.Win32;


namespace NovLab.IO.Mailslot
{
    // ＜メモ＞
    // ・Dispose-Finalizeパターンを実装する必要はありません。
    //   Closeの呼び出しを忘れた場合でも、SafeMailSlotHandle側でアンマネージリソースを破棄します。
    //====================================================================================================
    /// <summary>
    /// 【メールスロットサーバー】プロセス間通信の機能の一つであるメールスロットの操作機能を提供します。
    /// </summary>
    //====================================================================================================
    public class MailslotServer
    {
        //====================================================================================================
        // 内部フィールド
        //====================================================================================================
        /// <summary>
        /// 【メールスロット用セーフハンドル】
        /// </summary>
        protected SafeMailslotHandle m_hMailslot;

        /// <summary>
        /// 【メールスロット名】コンストラクターで指定したメールスロット名を保持します。エラーメッセージなどに使用します。
        /// </summary>
        protected string m_mailslotName;


        //====================================================================================================
        // Win32 API関連定義
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【CreateMailslot API関数(GetLastError対応)】メールスロットを作成します。
        /// </summary>
        /// <param name="lpName">              [in ]：メールスロット名</param>
        /// <param name="nMaxMessageSize">     [in ]：最大メッセージサイズ</param>
        /// <param name="lReadTimeout">        [in ]：読み取りタイムアウトの間隔(ミリ秒)</param>
        /// <param name="lpSecurityAttributes">[in ]：継承オプション</param>
        /// <returns>
        /// 関数が成功すると、メールスロットのハンドルが返ります。
        /// 関数が失敗すると、INVALID_HANDLE_VALUE(-1) が返ります。拡張エラー情報を取得するには GetLastError 関数を使います。
        /// </returns>
        //--------------------------------------------------------------------------------
        // [DllImport(TestWin32Define.DLLNAME, EntryPoint = "TestCreateMailslot", SetLastError = true, CharSet = CharSet.Auto)]    // Win32API定義をテストするときはこちら
        [DllImport("kernel32", EntryPoint = "CreateMailslot", SetLastError = true, CharSet = CharSet.Auto)]
        protected static extern
            SafeMailslotHandle M_API_CreateMailslot(    // HANDLE CreateMailslot(
            string lpName,                              //   LPCSTR lpName,                                // メールスロット名
            uint nMaxMessageSize,                       //   DWORD nMaxMessageSize,                        // 最大メッセージサイズ
            uint lReadTimeout,                          //   DWORD lReadTimeout,                           // 読み取りタイムアウトの間隔
            IntPtr lpSecurityAttributes                 //   LPSECURITY_ATTRIBUTES lpSecurityAttributes    // 継承オプション
        );                                              // );


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【GetMailslotInfo API関数(GetLastError対応)】指定されたメールスロットの情報を取得します。
        /// </summary>
        /// <param name="hMailslot">       [in ]：メールスロットのハンドル</param>
        /// <param name="lpMaxMessageSize">[out]：最大メッセージサイズ</param>
        /// <param name="lpNextSize">      [out]：次のメッセージのサイズ</param>
        /// <param name="lpMessageCount">  [out]：メッセージ数</param>
        /// <param name="lpReadTimeout">   [out]：読み取りタイムアウトの間隔</param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります。
        /// 関数が失敗すると、0 が返ります。拡張エラー情報を取得するには、GetLastError 関数を使います。
        /// </returns>
        //--------------------------------------------------------------------------------
        // [DllImport(TestWin32Define.DLLNAME, EntryPoint = "TestGetMailslotInfo", SetLastError = true, CharSet = CharSet.Auto)]   // Win32API定義をテストするときはこちら
        [DllImport("kernel32", EntryPoint = "GetMailslotInfo", SetLastError = true, CharSet = CharSet.Auto)]
        protected static extern bool M_API_GetMailslotInfo( // BOOL GetMailslotInfo(
            SafeMailslotHandle hMailslot,                   //   HANDLE hMailslot,         // メールスロットのハンドル
            out uint lpMaxMessageSize,                      //   LPDWORD lpMaxMessageSize, // 最大メッセージサイズ
            out uint lpNextSize,                            //   LPDWORD lpNextSize,       // 次のメッセージのサイズ
            out uint lpMessageCount,                        //   LPDWORD lpMessageCount,   // メッセージ数
            out uint lpReadTimeout                          //   LPDWORD lpReadTimeout     // 読み取りタイムアウトの間隔
        );                                                  // );


        //====================================================================================================
        // コンストラクターと公開メソッド(受信側用)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コンストラクター】メールスロットを作成します。
        /// </summary>
        /// <param name="mailslotName">[in ]：メールスロット名(\\.\mailslot\[path]name形式)</param>
        //--------------------------------------------------------------------------------
        public MailslotServer(string mailslotName)
        {
            //------------------------------------------------------------
            /// メールスロットを作成する
            //------------------------------------------------------------
            m_mailslotName = mailslotName;                              //// メールスロット名をフィールドに格納する

            m_hMailslot = M_API_CreateMailslot(mailslotName,            //// メールスロットを作成する
                0,              // 最大メッセージサイズ = 0(制限なし)
                0,              // 読み取りタイムアウトの間隔 = 0(メッセージが存在しないときは即座に制御を返す)
                IntPtr.Zero);   // ハンドルは継承できない
            if (m_hMailslot.IsInvalid)
            {                                                           //// 作成失敗の場合
                Marshal.ThrowExceptionForHR(                            /////  Win32エラーコードを使用して例外をスローする
                    Marshal.GetHRForLastWin32Error());
            }


            Debug.Print("hMailslot:0x" + m_hMailslot.DangerousGetHandle().ToString("X"));
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メールスロットクローズ】メールスロットを閉じます。
        /// </summary>
        //--------------------------------------------------------------------------------
        public void Close()
        {
            //------------------------------------------------------------
            /// メールスロットを閉じる
            //------------------------------------------------------------
            if (m_hMailslot.IsClosed == false)
            {                                                           //// メールスロットハンドルを閉じていない場合
                m_hMailslot.Close();                                    /////  メールスロットハンドルを閉じる
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メールスロットオープン状態(読み取り専用)】メールスロットを開いているかどうかを示す値を取得します。
        /// [true = 開いている / false = 開いていない]
        /// </summary>
        //--------------------------------------------------------------------------------
        public bool IsOpen => !m_hMailslot.IsClosed;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メールスロット情報取得】メールスロット情報を取得します。
        /// </summary>
        /// <returns>
        /// メールスロット情報
        /// </returns>
        //--------------------------------------------------------------------------------
        public MailslotInfo GetInfo()
        {
            MailslotInfo info;

            //------------------------------------------------------------
            /// メールスロット情報を取得する
            //------------------------------------------------------------
            var success = M_API_GetMailslotInfo(m_hMailslot,
                out info.maxMessageSize,
                out info.nextSize,
                out info.messageCount,
                out info.readTimeout);                                  //// メールスロット情報を取得する
            if (success == false)
            {                                                           //// 取得失敗の場合
                Marshal.ThrowExceptionForHR(                            /////  Win32エラーコードを使用して例外をスローする
                    Marshal.GetHRForLastWin32Error());
            }

            return info;                                                //// 戻り値 = メールスロット情報 で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メッセージ数取得】メールスロットに届いているメッセージ数を取得します。
        /// </summary>
        /// <returns>
        /// メッセージ数
        /// </returns>
        //--------------------------------------------------------------------------------
        public uint GetMessageCount() => GetInfo().messageCount;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列受信】メールスロットに届いているメッセージを文字列として読み取ります。
        /// </summary>
        /// <returns>
        /// 受信した文字列[null = メッセージなし]
        /// </returns>
        //--------------------------------------------------------------------------------
        public string ReceiveString()
        {
            //------------------------------------------------------------
            /// メールスロットに届いているメッセージを文字列として読み取る
            //------------------------------------------------------------
            var info = GetInfo();                                       //// メールスロット情報を取得する
            if (info.messageCount == 0)
            {                                                           //// メッセージ数 = 0 の場合
                return null;                                            /////  戻り値 = null(メッセージなし) で関数終了
            }
            if (info.nextSize == 0)
            {                                                           //// 次のメッセージのサイズ = 0 の場合
                return string.Empty;                                    /////  戻り値 = 空文字列 で関数終了
            }

            var buffer = new byte[info.nextSize];                       //// 次のメッセージのサイズ分、読み取りバッファーを確保する

            var success = Win32API.ReadFile(m_hMailslot,
                buffer, info.nextSize,
                out uint lpNumberOfBytesRead, IntPtr.Zero);             //// メールスロットからデータを読み取る
            if (success == false)
            {                                                           //// 処理失敗の場合
                Marshal.ThrowExceptionForHR(                            /////  Win32エラーコードを使用して例外をスローする
                    Marshal.GetHRForLastWin32Error());
            }

            return MailslotBase.ENCODING.GetString(buffer);             //// バッファーの内容を文字列にデコードして戻り値とし、関数終了
        }
    }
}
