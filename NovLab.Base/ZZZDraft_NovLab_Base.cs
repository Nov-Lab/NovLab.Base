// 下書き用

#if DEBUG
//#define CTRL_F5 // Ctrl+F5テスト：中断対象例外もテストします。中断対象例外は、例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能です。
#endif

using System;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text;
using System.Reflection;
using System.Globalization;

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using NovLab.DebugSupport;


/// ＜関連リンク＞
/// 【メモ】namespace(名前空間)について <see cref="NovLab.ZZZ_Memo_namespace"/>
namespace NovLabDraft
{

    // ＜仕様検討メモ＞
    // ・冗長デバッグ出力
    //   デバッグビルドでのみ有効で、リリースビルドでは除去される
    //   必要なときだけ出力したい。普段は出力したくない
    //
    // ・DebugStation で出力の有効化／無効化を制御できるようにしたい(再ビルドせずに設定を制御したい)
    //   現在は仮仕様として条件付きコンパイル シンボルで制御している
    //
    // ＜使い分け方＞
    // ・デバッグ出力    ：Debug.Print         デバッグ版のみ有効で、常に出力する
    // ・冗長デバッグ出力：VerboseDebug.Print  デバッグ版のみ有効で、出力するかどうかを制御可能
    // ・トレース出力    ：Trace.WriteLine     リリース版でも有効で、常に出力する

    //====================================================================================================
    /// <summary>
    /// 【冗長デバッグ】
    /// 条件付きコンパイル シンボル "VERBOSELOG" が定義されている場合にだけ有効となる Debug メソッドを提供します。
    /// </summary>
    /// <remarks>
    /// ・DEBUG シンボルと Debug クラスの仕様により、既定ではデバッグビルドの場合にだけ有効です。<br></br>
    /// </remarks>
    //====================================================================================================
    public class VerboseDebug
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【冗長Debug.Print】
        /// 条件付きコンパイル シンボル "VERBOSELOG" が定義されている場合にだけ有効となる Debug.Print メソッドです。
        /// </summary>
        /// <param name="message">[in ]：書き込むメッセージ。</param>
        /// <remarks>
        /// ・DEBUG シンボルと Debug クラスの仕様により、既定ではデバッグビルドの場合にだけ有効です。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("VERBOSELOG")]
        public static void Print(string message) => Debug.Print(message);
    }

}
