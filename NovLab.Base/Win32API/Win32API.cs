// @(h)Win32API.cs ver 0.00 ( '22.03.25 Nov-Lab ) 作成開始
// @(h)Win32API.cs ver 0.51 ( '22.03.27 Nov-Lab ) ベータ版完成
// @(h)Win32API.cs ver 0.52 ( '22.04.20 Nov-Lab ) 機能追加：CloseHandle を追加した。
// @(h)Win32API.cs ver 0.52a( '22.04.29 Nov-Lab ) テスト  ：TestWin32Define を使用してWin32API定義が正しいことをテストした。
// @(h)Win32API.cs ver 0.53 ( '24.01.16 Nov-Lab ) 機能追加：RtlZeroMemory, RtlFillMemory を追加した。

// @(s)
// 　【Win32API】Win32 API の関数・定数・構造体などを定義します。

using System;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;


// ＜メモ＞
// Wtypes.h の
// アンマネージ型  C#型名  マネージ クラス名   説明
// --------------------------------------------------------
// HANDLE          N/A     System.IntPtr       32 ビット Windows オペレーティング システムの場合は 32 ビット、64 ビット Windows オペレーティング システムの場合は 64 ビット。
// BYTE            byte    System.Byte         符号なし 8 ビット整数
// SHORT           short   System.Int16        符号付き 16 ビット整数
// WORD            ushort  System.UInt16       符号なし 16 ビット整数
// INT             int     System.Int32        符号付き 32 ビット整数
// UINT            uint    System.UInt32       符号なし 32 ビット整数
// LONG            int     System.Int32        符号付き 32 ビット整数
// BOOL            int     System.Int32        符号付き 32 ビット整数
// DWORD           uint    System.UInt32       符号なし 32 ビット整数
// ULONG           uint    System.UInt32       符号なし 32 ビット整数
// CHAR            sbyte   System.Char         ANSI により装飾。符号付き 8 ビット整数
// WCHAR           char    System.Char         Unicode により装飾。Unicode(16ビット)文字
// LPSTR                   System.String(*)    ANSI により装飾
// LPCSTR                  System.String(*)    ANSI により装飾
// LPWSTR                  System.String(*)    Unicode により装飾
// LPCWSTR                 System.String(*)    Unicode により装飾
// FLOAT           float   System.Single       単精度浮動小数点数(32 ビット)
// DOUBLE          double  System.Double       倍精度浮動小数点数(64 ビット)
// SIZE_T          N/A     System.IntPtr       typedef ULONG_PTR SIZE_T, *PSIZE_T;
// 
// (*) または System.Text.StringBuilder
// 
// ・参考：文字列のマーシャリング


// ・Marshal クラスにさまざまな機能が用意されているので、まずはそちらが使えないか確認すること。


// 【DllImportAttribute】
//   C#での既定値
//   true          BestFitMapping        Unicode 文字を ANSI 文字に変換するときの、最適マッピング動作のオン/オフを切り替えます。 
//   Winapi        CallingConvention     エントリ ポイントの呼び出し規約を示します。 
//   Ansi          CharSet               文字列パラメーターをメソッドにマーシャリングし、名前マングルを制御する方法を示します。 
//                 EntryPoint            呼び出す DLL エントリ ポイントの名前または序数を指定します。
//   false         ExactSpelling         DllImportAttribute.CharSet フィールドで、指定された名前以外のエントリ ポイント名をアンマネージ DLL から共通言語ランタイムに検索させるかどうかを制御します。 
//   true          PreserveSig           戻り値が HRESULT または retval であるアンマネージ メソッドを直接変換するか、戻り値 HRESULT または retval を自動的に例外に変換するかを示します。 
//   false         SetLastError          属性付きメソッドから戻る前に、呼び出し先が SetLastError Win32 API 関数を呼び出すかどうかを示します。 
//   false         ThrowOnUnmappableChar マップできない Unicode 文字 (ANSI の "?" に変換される文字) が見つかったときに、例外をスローするかどうかを指定します。 


namespace NovLab.Win32
{
    //====================================================================================================
    /// <summary>
    /// 【Win32API】Win32 API の関数・定数・構造体などを定義します。
    /// </summary>
    //====================================================================================================
    public static partial class Win32API
    {
        //====================================================================================================
        // Win32API関連定義
        //====================================================================================================
        /// <summary>
        /// 【アクセスモード：読み取りアクセス】
        /// </summary>
        public const uint GENERIC_READ = 0x80000000;       // WinNT.h：#define GENERIC_READ (0x80000000L)

        /// <summary>
        /// 【アクセスモード：書き込みアクセス】
        /// </summary>
        public const uint GENERIC_WRITE = 0x40000000;      // WinNT.h：#define GENERIC_WRITE (0x40000000L)

        /// <summary>
        /// 【共有モード：読み取り許可】
        /// </summary>
        public const uint FILE_SHARE_READ = 0x1;           // WinNT.h：#define FILE_SHARE_READ 0x00000001

        /// <summary>
        /// 【共有モード：書き込み許可】
        /// </summary>
        public const uint FILE_SHARE_WRITE = 0x2;          // WinNT.h：#define FILE_SHARE_WRITE 0x00000002

        /// <summary>
        /// 【ファイル作成モード：既存のファイルを開く】
        /// </summary>
        public const uint OPEN_EXISTING = 3;               // fileapi.h：#define OPEN_EXISTING 3

        /// <summary>
        /// 【ファイル属性：通常のファイル】
        /// </summary>
        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;    // WinNT.h：#define FILE_ATTRIBUTE_NORMAL 0x00000080


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【CloseHandle API関数(GetLastError対応)】オブジェクトハンドルを閉じます。
        /// </summary>
        /// <param name="hObject">[in ]：オブジェクトのハンドル</param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります。
        /// 関数が失敗すると、0 が返ります。拡張エラー情報を取得するには、GetLastError 関数を使います。
        /// </returns>
        //--------------------------------------------------------------------------------
        // [DllImport(TestWin32Define.DLLNAME, EntryPoint = "TestCloseHandle", SetLastError = true, CharSet = CharSet.Auto)]   // Win32API定義をテストするときはこちら
        [DllImport("kernel32", EntryPoint = "CloseHandle", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(  // BOOL CloseHandle(
            IntPtr hObject                      //   HANDLE hObject    // オブジェクトのハンドル
        );                                      // );


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【CreateFile API関数(GetLastError対応)】ファイルを作成するか開きます。
        /// </summary>
        /// <param name="lpFileName">           [in ]：ファイル名</param>
        /// <param name="dwDesiredAccess">      [in ]：アクセスモード</param>
        /// <param name="dwShareMode">          [in ]：共有モード</param>
        /// <param name="lpSecurityAttributes"> [in ]：セキュリティ記述子</param>
        /// <param name="dwCreationDisposition">[in ]：作成方法</param>
        /// <param name="dwFlagsAndAttributes"> [in ]：ファイル属性</param>
        /// <param name="hTemplateFile">        [in ]：テンプレートファイルのハンドル</param>
        /// <returns>
        /// 関数が成功すると、ファイルのハンドルが返ります。
        /// 関数が失敗すると、INVALID_HANDLE_VALUE(-1) が返ります。拡張エラー情報を取得するには GetLastError 関数を使います。
        /// </returns>
        //--------------------------------------------------------------------------------
        // [DllImport(TestWin32Define.DLLNAME, EntryPoint = "TestCreateFile", SetLastError = true, CharSet = CharSet.Auto)]    // Win32API定義をテストするときはこちら
        [DllImport("kernel32", EntryPoint = "CreateFile", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile( // HANDLE CreateFile(
            string lpFileName,                          //   LPCTSTR lpFileName,                           // ファイル名
            uint dwDesiredAccess,                       //   DWORD dwDesiredAccess,                        // アクセスモード
            uint dwShareMode,                           //   DWORD dwShareMode,                            // 共有モード
            IntPtr lpSecurityAttributes,                //   LPSECURITY_ATTRIBUTES lpSecurityAttributes,   // セキュリティ記述子
            uint dwCreationDisposition,                 //   DWORD dwCreationDisposition,                  // 作成方法
            uint dwFlagsAndAttributes,                  //   DWORD dwFlagsAndAttributes,                   // ファイル属性
            IntPtr hTemplateFile                        //   HANDLE hTemplateFile                          // テンプレートファイルのハンドル
        );                                              // );


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ReadFile API関数(GetLastError対応)】ファイルからデータを読み取ります。
        /// ※データバッファは、読み取り対象のバイト数以上のサイズをあらかじめ確保しておかなければなりません。
        /// </summary>
        /// <param name="hFile">               [in ]：ファイルのハンドル</param>
        /// <param name="byteBuffer">          [ref]：データバッファ</param>
        /// <param name="nNumberOfBytesToRead">[in ]：読み取り対象のバイト数</param>
        /// <param name="lpNumberOfBytesRead"> [in ]：読み取ったバイト数</param>
        /// <param name="lpOverlapped">        [in ]：オーバーラップ構造体のバッファ</param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります。
        /// 関数が失敗すると、0 が返ります。拡張エラー情報を取得するには、GetLastError 関数を使います。
        /// </returns>
        //--------------------------------------------------------------------------------
        // [DllImport(TestWin32Define.DLLNAME, EntryPoint = "TestReadFile", SetLastError = true, CharSet = CharSet.Auto)]  // Win32API定義をテストするときはこちら
        [DllImport("kernel32", EntryPoint = "ReadFile", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool ReadFile(     // BOOL ReadFile(
            SafeHandle hFile,                   //   HANDLE hFile,                  // ファイルのハンドル
            byte[] byteBuffer,                  //   LPCVOID lpBuffer,              // データバッファ
            uint nNumberOfBytesToRead,          //   DWORD nNumberOfBytesToRead,    // 読み取り対象のバイト数
            out uint lpNumberOfBytesRead,       //   LPDWORD lpNumberOfBytesRead,   // 読み取ったバイト数
            IntPtr lpOverlapped                 //   LPOVERLAPPED lpOverlapped      // オーバーラップ構造体のバッファ
        );                                      // );


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【WriteFile API関数(GetLastError対応)】ファイルにデータを書き込みます。
        /// </summary>
        /// <param name="hFile">                 [in ]：ファイルのハンドル</param>
        /// <param name="byteBuffer">            [in ]：データバッファ</param>
        /// <param name="nNumberOfBytesToWrite"> [in ]：書き込み対象のバイト数</param>
        /// <param name="lpNumberOfBytesWritten">[in ]：書き込んだバイト数</param>
        /// <param name="lpOverlapped">          [in ]：オーバーラップ構造体のバッファ</param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります。
        /// 関数が失敗すると、0 が返ります。拡張エラー情報を取得するには、GetLastError 関数を使います。
        /// </returns>
        //--------------------------------------------------------------------------------
        // [DllImport(TestWin32Define.DLLNAME, EntryPoint = "TestWriteFile", SetLastError = true, CharSet = CharSet.Auto)] // Win32API定義をテストするときはこちら
        [DllImport("kernel32", EntryPoint = "WriteFile", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool WriteFile(    // BOOL WriteFile(
            SafeHandle hFile,                   //   HANDLE hFile,                     // ファイルのハンドル
            byte[] byteBuffer,                  //   LPCVOID lpBuffer,                 // データバッファ
            uint nNumberOfBytesToWrite,         //   DWORD nNumberOfBytesToWrite,      // 書き込み対象のバイト数
            out uint lpNumberOfBytesWritten,    //   LPDWORD lpNumberOfBytesWritten,   // 書き込んだバイト数
            IntPtr lpOverlapped                 //   LPOVERLAPPED lpOverlapped         // オーバーラップ構造体のバッファ
        );                                      // );


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【RtlZeroMemory API関数(GetLastError非対応)】指定されたメモリ範囲に０を書き込みます。
        /// </summary>
        /// <param name="Destination">[in ]：先頭アドレス</param>
        /// <param name="Length">     [in ]：バイト数</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・C言語の memset((Destination),0,(Length)) と同等です。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [DllImport("kernel32", EntryPoint = "RtlZeroMemory", SetLastError = false)]
        public static extern void RtlZeroMemory(    // VOID RtlZeroMemory(
            IntPtr Destination,                     //     IN VOID UNALIGNED *Destination,  // 先頭アドレス
            IntPtr Length                           //     IN SIZE_T          Length        // バイト数
        );                                          // );


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【RtlFillMemory API関数(GetLastError非対応)】指定されたメモリ範囲に指定されたバイト値を書き込みます。
        /// </summary>
        /// <param name="Destination">[in ]：先頭アドレス</param>
        /// <param name="Length">     [in ]：バイト数</param>
        /// <param name="Fill">       [in ]：書き込むバイト値</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・C言語の memset((Destination),(Fill),(Length)) と同等です。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [DllImport("kernel32", EntryPoint = "RtlFillMemory", SetLastError = false)]
        public static extern void RtlFillMemory(    // VOID RtlFillMemory(
            IntPtr Destination,                     //     IN VOID UNALIGNED *Destination,  // 先頭アドレス
            IntPtr Length,                          //     IN SIZE_T          Length        // バイト数
            byte Fill                               //     IN UCHAR           Fill          // 書き込むバイト値
        );                                          // );

    } // class

} // namespace
