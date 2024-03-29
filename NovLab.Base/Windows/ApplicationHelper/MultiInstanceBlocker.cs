﻿// @(h)MultiInstanceBlocker.cs ver 0.00 ( '23.09.07 Nov-Lab ) プロトタイプを基に作成開始
// @(h)MultiInstanceBlocker.cs ver 0.51 ( '23.09.07 Nov-Lab ) ベータ版完成
// @(h)MultiInstanceBlocker.cs ver 0.51a( '23.09.14 Nov-Lab ) コメント修正

// @(s)
// 　【二重起動ブロッカー】アプリケーションの二重起動を防止し、単一インスタンスのアプリケーションとするための仕組みを提供します。

#if DEBUG
#define VERBOSELOG  // 冗長ログ有効化
#endif

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;


/// ＜関連リンク＞
/// 【メモ】namespace(名前空間)について <see cref="NovLab.ZZZ_Memo_namespace"/>
namespace NovLab.Windows.ApplicationHelper
{
    //====================================================================================================
    /// <summary>
    /// 【二重起動ブロッカー】
    /// アプリケーションの二重起動を防止し、単一インスタンスのアプリケーションとするための仕組みを提供します。
    /// </summary>
    /// <remarks>
    /// ・名前付きミューテックスを使用して二重起動を防止します。<br></br>
    /// ・using ステートメントと組み合わせて使用するようにデザインしてあります。<br></br>
    /// ・<see cref="ZZZUsage"/> を参考に、自動生成された Main メソッドを修正することで、
    ///   アプリケーションの二重起動を防止できるようになります。<br></br>
    /// 
    /// ・https://learn.microsoft.com/ja-jp/troubleshoot/developer/visualstudio/csharp/language-compilers/single-instance-application-crashes によると、
    ///   Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase.IsSingleInstance でチェックする方法だと、
    ///   IP 仮想化が有効になっている環境でクラッシュするとのことです。<br></br>
    /// </remarks>
    //====================================================================================================
    public class MultiInstanceBlocker : DisposablePattern
    {
        //====================================================================================================
        // 内部フィールド
        //====================================================================================================
        /// <summary>
        /// ミューテックスオブジェクト
        /// </summary>
        protected Mutex m_mutex;


        //====================================================================================================
        // 公開フィールド
        //====================================================================================================
        /// <summary>
        /// 【単一インスタンス状態】
        /// 二重起動ではなく単一インスタンスとして起動されたかどうかを示す値を取得します。
        /// </summary>
        public readonly bool isSingleInstance;


        //====================================================================================================
        // 使い方サンプル
        //====================================================================================================
#if DEBUG
        public static void ZZZUsage()
        {
#if false   // 自動生成された Main メソッドを以下のように修正します(using NovLab.Base; も追加します)
            using (var blocker = new MultiInstanceBlocker())
            {                                                           //// using：二重起動ブロッカーを生成する
                if (blocker.isSingleInstance)
                {                                                       /////  単一インスタンスの場合、アプリケーション本体を実行する
                    //------------------------------------------------------------
                    // 自動生成された部分
                    //------------------------------------------------------------
                    Application.EnableVisualStyles();                       // この部分は
                    Application.SetCompatibleTextRenderingDefault(false);   // アプリケーションの種類などによって
                    Application.Run(new Form1());                           // 内容が変わります
                }

                blocker.Close();                                        /////  二重起動ブロッカーを閉じる
            }
#endif
        }
#endif


        //====================================================================================================
        // コンストラクターと公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【二重起動ブロッカー コンストラクター】
        /// アプリケーションの二重起動を防止し、単一インスタンスのアプリケーションとするための仕組みを提供します。
        /// </summary>
        /// <param name="mutexName">[in ]：ミューテックス名(nullまたは空文字列 = プロジェクトのGUIDを用いる)</param>
        /// <remarks>
        /// ・ミューテックス名を指定しなかった場合はプロジェクトのGUIDを用います。<br></br>
        ///   プロジェクトのGUIDは [アセンブリ情報] ダイアログ ボックスに設定されています。<br></br>
        ///   GUIDが同じならば、実行ファイル名を変更しても同じアプリケーションとして扱います。<br></br>
        ///   GUIDが同じならば、デバッグ版もリリース版も同じアプリケーションとして扱います。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public MultiInstanceBlocker(string mutexName = null)
        {
            //------------------------------------------------------------
            /// ミューテックス名が指定されなかった場合はプロジェクトのGUIDを用いる
            //------------------------------------------------------------
            if (string.IsNullOrEmpty(mutexName))
            {                                                           //// ミューテックス名が null または空文字列の場合
                GuidAttribute guidAttribute = (GuidAttribute)Attribute.GetCustomAttribute(
                    System.Reflection.Assembly.GetEntryAssembly(),
                    typeof(GuidAttribute));                             /////  エントリポイントアセンブリのGuid属性を取得する
                mutexName = guidAttribute.Value;                        /////  ミューテックス名 = エントリポイントアセンブリのGUID(= プロジェクトのGUID)
            }

            NovLabDraft.VerboseDebug.Print("mutexName=" + mutexName);


            //------------------------------------------------------------
            /// ミューテックスの所有権を取得する
            //------------------------------------------------------------
            bool hasMutex = false;                                      //// ミューテックス所有フラグ = false に初期化する
            m_mutex = new Mutex(false, mutexName);                      //// ミューテックスオブジェクトを生成する

            try
            {                                                           //// try開始
                hasMutex = m_mutex.WaitOne(0, false);                   /////  ミューテックスの所有権取得を試行し、結果をミューテックス所有フラグに反映する
            }
            catch (AbandonedMutexException)
            {                                                           //// catch：AbandonedMutexException(別のスレッドが解放せずに終了することによって放棄されたミューテックスが残っており、その所有権を取得した場合)
                hasMutex = true;                                        /////  ミューテックス所有フラグ = true (放棄されたミューテックスが残っていたことを検出するための例外であり、所有権は取得できている)
            }


            //------------------------------------------------------------
            /// 所有権を取得できたかどうかで、単一インスタンスか二重起動かを判定する
            //------------------------------------------------------------
            if (hasMutex == true)
            {                                                           //// ミューテックスの所有権を取得できた場合(単一インスタンスとして起動された場合)
                NovLabDraft.VerboseDebug.Print("ミューテックスの所有権を取得できました(二重起動でない)。");
                isSingleInstance = true;                                /////  単一インスタンス状態 = true
                return;                                                 /////  関数終了
            }
            else
            {                                                           //// ミューテックスの所有権を取得できなかった場合(二重起動された場合)
                NovLabDraft.VerboseDebug.Print("ミューテックスの所有権を取得できませんでした(二重起動された)。");
                isSingleInstance = false;                               /////  単一インスタンス状態 = false
                m_mutex.Close();                                        /////  ミューテックスオブジェクトを閉じる
                m_mutex = null;                                         /////  ミューテックスオブジェクトを破棄する
                return;                                                 /////  関数終了
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【クローズ】
        /// 現在のインスタンスによって使用されているすべてのリソースを解放し、二重起動の防止処理を終了します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public void Close()
        {
            /// 明示的リソース破棄処理を行う(リソースを破棄する = ミューテックスを解放する = 二重起動の防止処理を終了する)
            Dispose();
        }


        //====================================================================================================
        // Dispose-Finalizeパターン(DisposablePattern 抽象メソッドの実装)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【マネージリソース破棄】
        /// マネージリソースを破棄します。
        /// ミューテックスオブジェクトを生成してある場合、ミューテックスを解放してから閉じます。
        /// </summary>
        /// <remarks>
        /// ・ミューテックスの所有権を取得できなかった場合はその時点で閉じて破棄しているため、
        ///   「オブジェクトを生成してある = 所有権を取得してある = 解放や破棄をしなければならない」
        ///   となります。
        /// </remarks>
        //--------------------------------------------------------------------------------
        protected override void M_DisposeManagedResource()
        {
            //------------------------------------------------------------
            /// マネージリソースを破棄する
            //------------------------------------------------------------
            if (m_mutex != null)
            {                                                           //// ミューテックスオブジェクトを生成してある場合
                m_mutex.ReleaseMutex();                                 /////  ミューテックスを解放する
                m_mutex.Close();                                        /////  ミューテックスオブジェクトを閉じる
                m_mutex = null;                                         /////  ミューテックスオブジェクトを破棄する
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【アンマネージリソース破棄】このクラスはアンマネージリソースを扱いません。
        /// </summary>
        //--------------------------------------------------------------------------------
        protected override void M_DisposeUnmanagedResource() { }
    }

}
