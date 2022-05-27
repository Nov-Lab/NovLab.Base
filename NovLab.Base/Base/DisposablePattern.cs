﻿// @(h)DisposablePattern.cs ver 0.00 ( '22.05.01 Nov-Lab ) 作成開始
// @(h)DisposablePattern.cs ver 1.01 ( '22.05.01 Nov-Lab ) 初版完成
// @(h)DisposablePattern.cs ver 1.01a( '22.05.05 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【Disposableパターン】Dispose-Finalizeパターンを実装するための抽象基本クラスです。

#if DEBUG           // DEBUGビルドのみ有効
//#define VERBOSELOG  // 冗長ログ有効化
#endif

using System;
using NovLab.DebugStation.DebugUseBlocker;  // Debug/Traceクラス使用ブロッカー(DebugStationTraceListener の呼び出し先では、Debug クラスと Trace クラスは使用禁止)


//====================================================================================================
/// <summary>
/// 【Disposableパターン】Dispose-Finalizeパターンを実装するための抽象基本クラスです。<br></br>
/// 「アンマネージ リソースをクリーンアップするための Finalize および Dispose の実装」に基づく定型テンプレートを提供します。
/// </summary>
//====================================================================================================
public abstract class DisposablePattern : IDisposable
{
    //====================================================================================================
    // 派生クラス固有の処理を記述するための抽象メソッド定義
    //====================================================================================================

    //--------------------------------------------------------------------------------
    /// <summary>
    /// 【マネージリソース破棄】マネージリソースを破棄します。
    /// </summary>
    //--------------------------------------------------------------------------------
    protected abstract void M_DisposeManagedResource();


    //--------------------------------------------------------------------------------
    /// <summary>
    /// 【アンマネージリソース破棄】アンマネージリソースを破棄します
    /// </summary>
    //--------------------------------------------------------------------------------
    protected abstract void M_DisposeUnmanagedResource();


    //====================================================================================================
    // 内部プロパティー
    //====================================================================================================
    /// <summary>
    /// 【Dispose忘れ警告フラグ】Disposeメソッドを呼び出さないままオブジェクトが破棄される場合にそれを警告するかどうかを示します。
    /// </summary>
    /// <remarks>
    /// ・警告を抑制したい場合は派生クラスで false を設定します。
    /// </remarks>
    protected bool M_WarningFinalizeWithoutDispose { get; set; } = true;


    //====================================================================================================
    // Disposableパターンの実装
    //====================================================================================================
    /// <summary>
    /// 【リソース破棄済みフラグ】
    /// </summary>
    protected bool m_disposed = false;


    //--------------------------------------------------------------------------------
    /// <summary>
    /// 【Finalize デストラクター】アンマネージリソースを破棄します。
    /// Disposeメソッドを呼び出さないままオブジェクトが破棄される場合にリソースをクリーンアップするための安全装置です。
    /// </summary>
    /// <remarks>
    /// ・Dispose メソッドを呼び出してある場合は、Finalize デストラクターは呼び出されません。<br></br>
    /// ・C# では Finalize をオーバーライドする代わりにデストラクターを提供します。<br></br>
    /// ・C# では基本クラスの Finalize 呼び出しは暗黙的に行われます。<br></br>
    /// </remarks>
    //--------------------------------------------------------------------------------
    ~DisposablePattern()
    {
#if VERBOSELOG
        DebugX.Print("Finalize:" + ToString());
#endif

        // ＜メモ＞
        // ・Disposeメソッドを呼び出さないままガベージコレクタによってオブジェクトが破棄される場合、
        //   ガベージコレクタはFinalizeデストラクターを呼び出します。
        //   この場合、マネージリソースの破棄はガベージコレクタが自動的に行います。
        //------------------------------------------------------------
        /// アンマネージリソースを破棄する
        //------------------------------------------------------------
        if (M_WarningFinalizeWithoutDispose)
        {                                                           //// Dispose忘れ警告フラグ = true の場合
            if (m_disposed == false)
            {                                                       /////  リソース破棄済みフラグ = false の場合(Disposeメソッドを呼び出さないままオブジェクトが破棄された場合)
                DebugX.Print(                                       //////   デバッグログ出力
                    "Finalize without dispose:" + ToString());
            }
        }

        M_Dispose(false);                                           //// リソース破棄処理を行う(マネージリソースの破棄はガベージコレクタが自動的に行う)
    }


    //--------------------------------------------------------------------------------
    /// <summary>
    /// 【明示的リソース破棄】すべてのリソースを破棄します(IDisposable.Dispose の実装)
    /// </summary>
    //--------------------------------------------------------------------------------
    public void Dispose()
    {
#if VERBOSELOG
        DebugX.Print("Dispose:" + ToString());
#endif

        //------------------------------------------------------------
        /// すべてのリソースを破棄する
        //------------------------------------------------------------
        M_Dispose(true);                                            //// リソース破棄処理を行う(マネージリソースも破棄)
        GC.SuppressFinalize(this);                                  //// ファイナライザー呼び出しを抑制する(明示的に破棄したので)
    }


    //--------------------------------------------------------------------------------
    /// <summary>
    /// 【リソース破棄】状況等に応じて適切にリソースを破棄します。
    /// </summary>
    /// <param name="disposing">[in ]：Disposeメソッド明示的呼び出しフラグ</param>
    //--------------------------------------------------------------------------------
    protected virtual void M_Dispose(bool disposing)
    {
#if VERBOSELOG
        DebugX.Print("M_Dispose(" + disposing + "):" + ToString());
#endif

        //------------------------------------------------------------
        /// すでに破棄済みの場合は何もしない
        //------------------------------------------------------------
        if (m_disposed)
        {                                                           //// 破棄済みフラグ = true の場合
            return;                                                 /////  何もせずに関数終了
        }


        // ＜メモ＞
        // ・Disposeメソッドを呼び出さないままガベージコレクタによってオブジェクトが破棄される場合、
        //   このメソッドはFinalizeデストラクターから呼び出されます。
        //   この場合、マネージリソースの破棄はガベージコレクタが自動的に行います。
        //------------------------------------------------------------
        /// マネージリソースを破棄する
        //------------------------------------------------------------
        if (disposing)
        {                                                           //// Disposeメソッド明示的呼び出しフラグ = true の場合(ガベージコレクタによるFinalizeデストラクターからの呼び出しでない場合)
            M_DisposeManagedResource();                             /////  マネージリソース破棄処理を行う
        }


        //------------------------------------------------------------
        /// アンマネージリソースを破棄する
        //------------------------------------------------------------
        M_DisposeUnmanagedResource();                               //// アンマネージリソース破棄処理を行う


        //------------------------------------------------------------
        /// 破棄済みとしてマークする
        //------------------------------------------------------------
        m_disposed = true;                                          //// 破棄済みフラグ = true にセットする
    }
}
