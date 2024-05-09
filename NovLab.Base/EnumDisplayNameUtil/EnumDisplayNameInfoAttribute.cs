// @(h)EnumDisplayNameInfoAttribute.cs ver 0.00 ( '24.05.06 Nov-Lab ) 作成開始
// @(h)EnumDisplayNameInfoAttribute.cs ver 0.51 ( '24.05.09 Nov-Lab ) ベータ版完成
// @(h)EnumDisplayNameInfoAttribute.cs ver 0.51a( '24.05.11 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【列挙値表示名情報属性】列挙値に対して表示名情報を付加する属性クラスです。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using NovLab.DebugSupport;


namespace NovLab.EnumDisplayNameUtil
{
#if DEBUG
    //====================================================================================================
    /// <summary>
    /// 
    /// 【概要説明】<br/>
    ///   EnumDisplayNameInfoAttribute は、列挙値に対して表示名情報を直接付加するための属性クラスです。<br/>
    ///   この属性で付加した列挙値表示名は、EnumDisplayName クラスを通じて取得することができます。<br/>
    /// <br/>
    /// ・編集のできる独自の列挙体に表示名を付加する場合にこの属性を使用します。<br/>
    /// ・編集できない既存の列挙体に表示名を後付けする場合や、この属性で付加した表示名を上書きしたい場合は、
    ///   <see cref="About_EnumDisplayNameProviderAttribute"/> を参照してください。<br/>
    /// ・なお、NovLab.Windows.Forms クラスライブラリには、リストボックスやコンボボックスに列挙体表示名を表示する機能などが用意されています。<br/>
    ///   
    /// </summary>
    //====================================================================================================
    public partial class About_EnumDisplayNameInfoAttribute : ZZZ
    {
        //
        // 【使い方①】
        //   以下のように、EnumDisplayNameInfoAttribute 属性で各列挙値に列挙値表示名を付加します。
        //
        //====================================================================================================
        /// <summary>
        /// 【リモートワーク種別】リモートワークの種類を示します。
        /// </summary>
        //====================================================================================================
        public enum RemoteWorkKind
        {
            /// <summary>
            /// 【リモートワーク種別：なし】
            /// </summary>
            [EnumDisplayNameInfo("なし(オフィス出社)")]
            None = 0,

            /// <summary>
            /// 【リモートワーク種別：一部リモートワーク】
            /// </summary>
            [EnumDisplayNameInfo("一部リモートワーク")]
            Partial,

            /// <summary>
            /// 【リモートワーク種別：基本リモートワーク】
            /// </summary>
            [EnumDisplayNameInfo("基本リモートワーク")]
            Basic,

            /// <summary>
            /// 【リモートワーク種別：完全リモートワーク】
            /// </summary>
            [EnumDisplayNameInfo("完全リモートワーク")]
            Full,
        }


        //
        // 【使い方②】
        //   以下のように、EnumDisplayName クラスを使うことで属性で付加した列挙値表示名を取得することができます。
        //   （このサンプルは NovLab テスト画面のテスト項目リストから実際に実行できます）
        //
        [ManualTestMethod("サンプル：EnumDisplayNameInfoAttribute")]
        public static void Sample1()
        {
            // 拡張メソッドを使って直感的に記述する場合
            Debug.Print("希望する就業場所=" + RemoteWorkKind.Full.XToDisplayName());

            // 拡張メソッドを使わずに記述する場合
            Debug.Print("希望する就業場所=" + EnumDisplayName.GetFrom(RemoteWorkKind.Full));

            // 【実行結果の出力例】
            //      希望する就業場所=完全リモートワーク
            //      希望する就業場所=完全リモートワーク
        }


        //====================================================================================================
        // サンプル列挙体を利用して EnumDisplayNameInfoAttribute の自動テスト
        //====================================================================================================
        [AutoTestMethod(nameof(EnumDisplayName) + "." + nameof(EnumDisplayName.XToDisplayName)
                      + " & " + nameof(EnumDisplayNameInfoAttribute))]
        public static void ZZZ_OverallTest()
        {
            SubRoutine(RemoteWorkKind.None, "なし(オフィス出社)", "列挙体で定義されている列挙値");
            SubRoutine(RemoteWorkKind.Partial, "一部リモートワーク", "列挙体で定義されている列挙値");
            SubRoutine(RemoteWorkKind.Basic, "基本リモートワーク", "列挙体で定義されている列挙値");
            SubRoutine(RemoteWorkKind.Full, "完全リモートワーク", "列挙体で定義されている列挙値");
            SubRoutine((RemoteWorkKind)65535, "65535", "列挙体で定義されていない内部整数値");


            void SubRoutine(RemoteWorkKind remoteWorkKind,  // [in ]：リモートワーク種別
                            string expectResult,            // [in ]：予想結果文字列
                            string testPattern = null)      // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                           // テストオプション
                    fncInstanceRegularString = RegularString.WithTypeName,  // ・対象インスタンスは型名併記
                };

                AutoTest.TestX(EnumDisplayName.XToDisplayName, (Enum)remoteWorkKind, expectResult, testOptions);
            }
        }

    } // class
#endif


    // ＜メモ＞
    // ・System.ComponentModel.DisplayNameAttribute 属性を流用しようかとも思ったが、列挙値には適用できないので断念した。
    //====================================================================================================
    /// <summary>
    /// 【列挙値表示名情報属性】列挙値に対して表示名情報を付加する属性クラスです。<br/>
    /// この属性で付加した列挙値表示名は、<see cref="EnumDisplayName"/> クラスを通じて取得することができます。<br/>
    /// </summary>
    /// <remarks>
    /// 補足<br/>
    /// ・Enum クラスの仕様により、同じ内部整数値を持つ列挙値が複数定義されている列挙型には適しません。
    ///  (参考メモ：<see cref="Memo_DuplicateEnumValue"/>)<br/>
    /// </remarks>
    //====================================================================================================
    [AttributeUsage(AttributeTargets.Field)]    // 属性適用対象 = フィールド(列挙値もフィールド扱い)
    public partial class EnumDisplayNameInfoAttribute : Attribute
    {
        /// <summary>
        /// 【列挙値表示名(読み取り専用)】列挙値に付加された表示名を取得します。
        /// </summary>
        public readonly string DisplayName;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【完全コンストラクター】すべての内容を指定して列挙値表示名情報属性の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="displayName">[in ]：列挙値表示名</param>
        //--------------------------------------------------------------------------------
        public EnumDisplayNameInfoAttribute(string displayName)
        {
            DisplayName = displayName;
        }

    } // class

} // namespace
