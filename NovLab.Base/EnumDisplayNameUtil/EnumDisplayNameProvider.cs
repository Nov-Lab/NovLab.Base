// @(h)EnumDisplayNameProvider.cs ver 0.00 ( '24.05.06 Nov-Lab ) 作成開始
// @(h)EnumDisplayNameProvider.cs ver 0.51 ( '24.05.10 Nov-Lab ) ベータ版完成
// @(h)EnumDisplayNameProvider.cs ver 0.51a( '24.05.13 Nov-Lab ) その他  ：機能変更なし。概要説明、サンプル、自動テストを追加した。
// @(h)EnumDisplayNameProvider.cs ver 0.51b( '24.05.14 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【列挙値表示名供給関数属性】メソッドを列挙値表示名供給関数としてマークし、EnumDisplayName クラスが自動的に認識できるようにします。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using NovLab.DebugSupport;

#if DEBUG
using System.Threading;
#endif


namespace NovLab.EnumDisplayNameUtil
{
#if DEBUG
    //====================================================================================================
    /// <summary>
    /// 
    /// 【概要説明】<br/>
    ///   EnumDisplayNameProviderAttribute は、メソッドを列挙値表示名供給関数としてマークするための属性クラスです。<br/>
    ///   この属性を付加したメソッドが供給する列挙値表示名は、EnumDisplayName クラスを通じて取得することができます。<br/>
    /// <br/>
    /// ・編集できない既存の列挙体に表示名を後付けする場合や、下記属性で列挙値に直接付加された表示名を上書きする場合に、この属性を使用します。<br/>
    /// ・編集のできる独自の列挙体に表示名を付加する場合は、
    ///   <see cref="About_EnumDisplayNameInfoAttribute"/> を参照してください。<br/>
    /// ・なお、NovLab.Windows.Forms クラスライブラリには、リストボックスやコンボボックスに列挙体表示名を表示する機能などが用意されています。<br/>
    ///   
    /// </summary>
    //====================================================================================================
    public partial class About_EnumDisplayNameProviderAttribute : ZZZ
    {
        //
        // 【使い方①】
        //   以下のように、列挙値から表示名を取得するメソッドを作成し、EnumDisplayNameProviderAttribute 属性を付加します。
        //   これにより、編集できない既存の列挙体に表示名を後付けすることができます。
        //
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ApartmentState 用列挙値表示名取得】列挙値に対応する表示名を取得します。<br/>
        /// </summary>
        /// <param name="enumValue">[in ]：列挙値</param>
        /// <returns>
        /// 列挙値表示名[null = 取得失敗]
        /// </returns>
        //--------------------------------------------------------------------------------
        [EnumDisplayNameProvider(typeof(ApartmentState), EnumDisplayNameProviderPriority.NovLab)]   // 列挙値表示名供給関数
        public static string EnumDisplayNameForApartmentState(Enum enumValue)
        {
            //------------------------------------------------------------
            /// 列挙値に対応する列挙値表示名を取得する
            //------------------------------------------------------------
            if (enumValue is ApartmentState specificEnum)
            {                                                           //// 列挙値を固有の列挙型に変換できる場合(正しい呼び出しの場合)
                switch (specificEnum)
                {                                                       /////  ApartmentState 列挙値で分岐
                    case ApartmentState.MTA:                            //////   ・MTA の場合、"マルチスレッド アパートメント" を返す
                        return "マルチスレッド アパートメント";

                    case ApartmentState.STA:                            //////   ・STA の場合、"シングルスレッド アパートメント" を返す
                        return "シングルスレッド アパートメント";

                    case ApartmentState.Unknown:                        //////   ・Unknown の場合、"不明" を返す
                        return "不明";

                    default:                                            //////   ・上記以外の場合(列挙体で定義されていない内部整数値の場合)
                        return null;                                    ///////    戻り値 = null(取得失敗) で関数終了
                }
            }
            else
            {                                                           //// 列挙値を固有の列挙型に変換できない場合(不正な呼び出しの場合)
                return null;                                            /////  戻り値 = null(取得失敗) で関数終了
            }
        }


        //
        // 【使い方②】
        //   以下のように EnumDisplayName クラスで定義されている XToDisplayName() 拡張メソッドを使うことで、
        //   列挙値表示名供給関数が供給する列挙値表示名を取得することができます。
        //   （このサンプルは NovLab テスト画面のテスト項目リストから実際に実行できます）
        //
        [ManualTestMethod("サンプル：EnumDisplayNameProviderAttribute")]
        public static void Sample1()
        {
            // 列挙定数名と列挙値表示名を表示する
            foreach (Enum tmpEnum in Enum.GetValues(typeof(ApartmentState)))
            {
                Debug.Print(tmpEnum.ToString() + " = " + tmpEnum.XToDisplayName());
            }

            // 【実行結果の出力例】
            //      STA = シングルスレッド アパートメント
            //      MTA = マルチスレッド アパートメント
            //      Unknown = 不明
        }

    } // class

#endif


    //====================================================================================================
    /// <summary>
    /// 【列挙値表示名供給関数属性】メソッドを列挙値表示名供給関数としてマークし、<see cref="EnumDisplayName"/> クラスが自動的に認識できるようにします。
    /// </summary>
    /// <remarks>
    /// 補足<br/>
    /// ・この属性を付加するメソッドは、<see cref="EnumDisplayNameProvider"/> デリゲートに適合する public なメソッドでなければなりません。<br/>
    /// ・この属性を付加するメソッドには、特に理由がない限り GetEnumDisplayName を含めておくと、オブジェクト ブラウザーで検索しやすくなって便利です。<br/>
    /// ・使い方やサンプルは <see cref="About_EnumDisplayNameProviderAttribute"/> を参照してください。<br/>
    /// ・実装例は <see cref="NovLabEnumDisplayNameProvider"/> クラスを参照してください。<br/>
    /// </remarks>
    //====================================================================================================
    [AttributeUsage(AttributeTargets.Method)]   // 属性適用対象 = メソッド
    public class EnumDisplayNameProviderAttribute : Attribute
    {
        /// <summary>
        /// 【列挙体の型】属性適用対象メソッドが列挙値表示名を供給する列挙体の型です。
        /// </summary>
        public readonly Type enumType;

        /// <summary>
        /// 【列挙値表示名供給関数優先度】属性適用対象メソッドの優先度です。
        /// </summary>
        public readonly EnumDisplayNameProviderPriority priority;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【完全コンストラクター】すべての内容を指定して列挙値表示名供給関数属性の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="enumType">[in ]：列挙体の型</param>
        /// <param name="priority">[in ]：列挙値表示名供給関数優先度</param>
        //--------------------------------------------------------------------------------
        public EnumDisplayNameProviderAttribute(Type enumType, EnumDisplayNameProviderPriority priority = EnumDisplayNameProviderPriority.Normal)
        {
            // ＜メモ＞
            // ・EnumDisplayNameProviderAttribute<TEnum> : Attribute where TEnum : struct, Enum のようにしてコンパイル時に型制約をかけられると良かったが、
            //   属性クラスをジェネリックにすることはできないので断念した。
            //------------------------------------------------------------
            /// 引数をチェックする
            //------------------------------------------------------------
            if (enumType.IsEnum == false)
            {                                                           //// 列挙体の型として指定された型が列挙型でない場合
                throw new AttributeException($"{nameof(enumType)} に指定された {enumType.FullName} は列挙型(Enum の派生型)ではありません。",
                    nameof(EnumDisplayNameProviderAttribute));          /////  属性不正例外をスローする
            }


            //------------------------------------------------------------
            /// フィールドに内容を設定する
            //------------------------------------------------------------
            this.enumType = enumType;
            this.priority = priority;
        }


        //--------------------------------------------------------------------------------
        // テストコード：enumType は列挙型(Enum の派生型)でなければならない
        //--------------------------------------------------------------------------------
#if TESTCODE_WRONGTYPE  // テストするときだけ有効化すること
        [EnumDisplayNameProvider(typeof(int))]
        public static string WrongType(int value) => value.ToString();
#endif

    } // class


    //====================================================================================================
    /// <summary>
    /// 【列挙値表示名供給関数デリゲート】列挙値から列挙値表示名を取得する関数のデリゲートです。
    /// </summary>
    /// <param name="enumValue">[in ]：列挙値</param>
    /// <returns>
    /// 列挙値表示名
    /// </returns>
    //====================================================================================================
    public delegate string EnumDisplayNameProvider(Enum enumValue);


    // ＜メモ＞
    // ・内部整数値が大きいほど優先度が高くなる。値そのものに意味はないので変更しても構わないが、大小比較は正しく行える必要がある。
    //====================================================================================================
    /// <summary>
    /// 【列挙値表示名供給関数優先度】列挙値表示名供給関数の優先度を示します。
    /// </summary>
    /// <remarks>
    /// 同じ列挙体に対して複数の列挙値表示名供給関数がある場合、以下の順番で優先されます。<br/>
    /// ①最高 <see cref="Highest"/> … アプリケーション独自の表示名など、他のどれよりも優先したい場合に用います。<br/>
    /// ②高い <see cref="High"/> … 開発プロジェクト固有のライブラリなど、他のライブラリが提供する表示名を上書きしたい場合に適しています。<br/>
    /// ③通常 <see cref="Normal"/> … 通常はこの優先度を指定します。社内共通ライブラリなどに適しています。<br/>
    /// ④低い <see cref="Low"/> … 第三者に提供するクラスライブラリなど、提供先が容易に上書きできるようにしたい場合に適しています。<br/>
    /// <br/>
    /// 補足<br/>
    /// ・<see cref="NovLab"/> は使用しないでください。<br/>
    /// ・内部整数値は将来変更する可能性も考えられますので、列挙定数名だけを使うようにしてください。<br/>
    /// ・同じ列挙体に対して同じ優先度の列挙値表示名供給関数が複数定義されている場合、どの供給関数が使われるかは未定義であり不定です。<br/>
    /// <br/>
    /// 参照<br/>
    /// ・<see cref="EnumDisplayName"/><br/>
    /// ・<see cref="EnumDisplayNameProviderAttribute"/><br/>
    /// </remarks>
    //====================================================================================================
    public enum EnumDisplayNameProviderPriority
    {
        /// <summary>
        /// 【列挙値表示名供給関数優先度・NovLab提供版】使用しないでください。NovLab クラスライブラリで提供する表示名であり、他のどれよりも優先度が低いことを示します。
        /// </summary>
        [EnumDisplayNameInfo("NovLab提供版")]
        NovLab = int.MinValue,

        /// <summary>
        /// 【列挙値表示名供給関数優先度・低い】
        /// </summary>
        [EnumDisplayNameInfo("低い優先度")]
        Low = -100,

        /// <summary>
        /// 【列挙値表示名供給関数優先度・通常】
        /// </summary>
        [EnumDisplayNameInfo("通常優先度")]
        Normal = 0,

        /// <summary>
        /// 【列挙値表示名供給関数優先度・高い】
        /// </summary>
        [EnumDisplayNameInfo("高い優先度")]
        High = 100,

        /// <summary>
        /// 【列挙値表示名供給関数優先度・最高】
        /// </summary>
        [EnumDisplayNameInfo("最高優先度")]
        Highest = 200,
    }


#if DEBUG
    // ＜メモ＞
    // ・EnumDisplayName クラスのテストとも言えるが、EnumDisplayNameProviderPriority 列挙体との関連性が強いためこのソースファイルに収録している。
    //====================================================================================================
    /// <summary>
    /// EnumDisplayName フォールバック処理のテスト(機械的)<br/>
    /// </summary>
    //====================================================================================================
    public partial class ZZZTest_EnumDisplayName_Fallback_Mechanical
    {
        //--------------------------------------------------------------------------------
        // 自動テスト
        //--------------------------------------------------------------------------------
        [AutoTestMethod("EnumDisplayName フォールバック処理のテスト(機械的)")]
        public static void Test()
        {
            SubRoutine(FallbackTest.Invalid, "Invalid", "表示名を取得できないパターン");
            SubRoutine(FallbackTest.LastResort, "LastResort", "表示名を取得できないパターン");
            SubRoutine(FallbackTest.ProvideByAttributeMetadata, "属性によって付加された表示名です");
            SubRoutine(FallbackTest.ProvideByNovLab, "列挙値表示名供給関数(NovLab優先度)によって供給された表示名です");
            SubRoutine(FallbackTest.ProvideByLow, "列挙値表示名供給関数(低い優先度)によって供給された表示名です");
            SubRoutine(FallbackTest.ProvideByNormal, "列挙値表示名供給関数(通常優先度)によって供給された表示名です");
            SubRoutine(FallbackTest.ProvideByHigh, "列挙値表示名供給関数(高い優先度)によって供給された表示名です");
            SubRoutine(FallbackTest.ProvideByHighest, "列挙値表示名供給関数(最高優先度)によって供給された表示名です");


            //------------------------------------------------------------
            // 【ローカル関数】
            //------------------------------------------------------------
            void SubRoutine(FallbackTest enumValue,     // [in ]：列挙値
                            string displayString,       // [in ]：予想結果(列挙値表示名)
                            string testPattern = null)  // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                           // テストオプション
                    fncInstanceRegularString = RegularString.WithTypeName,  // ・対象インスタンスは型名併記
                };

                AutoTest.TestX<Enum, string>(EnumDisplayName.XToDisplayName, enumValue, displayString, testOptions);
            }
        }


        //
        // テスト用列挙体
        //
        public enum FallbackTest
        {
            // 列挙処理から除外されるパターン
            [EnumerateIgnore]
            Invalid = 0,

            // 表示名を取得できないパターン
            LastResort,

            // 属性から取得するパターン
            [EnumDisplayNameInfo("属性によって付加された表示名です")]
            ProvideByAttributeMetadata,

            // NovLab優先度の列挙値表示名供給関数で上書きするパターン
            [EnumDisplayNameInfo("属性によって付加された表示名です")]
            ProvideByNovLab,

            // 低い優先度の列挙値表示名供給関数で上書きするパターン
            [EnumDisplayNameInfo("属性によって付加された表示名です")]
            ProvideByLow,

            // 通常優先度の列挙値表示名供給関数で上書きするパターン
            [EnumDisplayNameInfo("属性によって付加された表示名です")]
            ProvideByNormal,

            // 高い優先度の列挙値表示名供給関数で上書きするパターン
            [EnumDisplayNameInfo("属性によって付加された表示名です")]
            ProvideByHigh,

            // 最高優先度の列挙値表示名供給関数で上書きするパターン
            [EnumDisplayNameInfo("属性によって付加された表示名です")]
            ProvideByHighest,
        }

        //
        // 列挙値表示名供給関数(NovLab優先度)
        //
        [EnumDisplayNameProvider(typeof(FallbackTest), EnumDisplayNameProviderPriority.NovLab)]
        public static string EnumDisplayNameProvider_NovLab(Enum enumValue)
        {
            if (enumValue is FallbackTest specificEnum)
            {
                switch (specificEnum)
                {
                    case FallbackTest.ProvideByNovLab:
                    case FallbackTest.ProvideByLow:
                    case FallbackTest.ProvideByNormal:
                    case FallbackTest.ProvideByHigh:
                    case FallbackTest.ProvideByHighest:
                        return "列挙値表示名供給関数(NovLab優先度)によって供給された表示名です";

                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        //
        // 列挙値表示名供給関数(低い優先度)
        //
        [EnumDisplayNameProvider(typeof(FallbackTest), EnumDisplayNameProviderPriority.Low)]
        public static string EnumDisplayNameProvider_Low(Enum enumValue)
        {
            if (enumValue is FallbackTest specificEnum)
            {
                switch (specificEnum)
                {
                    case FallbackTest.ProvideByLow:
                    case FallbackTest.ProvideByNormal:
                    case FallbackTest.ProvideByHigh:
                    case FallbackTest.ProvideByHighest:
                        return "列挙値表示名供給関数(低い優先度)によって供給された表示名です";

                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        //
        // 列挙値表示名供給関数(通常優先度)
        //
        [EnumDisplayNameProvider(typeof(FallbackTest), EnumDisplayNameProviderPriority.Normal)]
        public static string EnumDisplayNameProvider_Normal(Enum enumValue)
        {
            if (enumValue is FallbackTest specificEnum)
            {
                switch (specificEnum)
                {
                    case FallbackTest.ProvideByNormal:
                    case FallbackTest.ProvideByHigh:
                    case FallbackTest.ProvideByHighest:
                        return "列挙値表示名供給関数(通常優先度)によって供給された表示名です";

                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        //
        // 列挙値表示名供給関数(高い優先度)
        //
        [EnumDisplayNameProvider(typeof(FallbackTest), EnumDisplayNameProviderPriority.High)]
        public static string EnumDisplayNameProvider_High(Enum enumValue)
        {
            if (enumValue is FallbackTest specificEnum)
            {
                switch (specificEnum)
                {
                    case FallbackTest.ProvideByHigh:
                    case FallbackTest.ProvideByHighest:
                        return "列挙値表示名供給関数(高い優先度)によって供給された表示名です";

                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        //
        // 列挙値表示名供給関数(最高優先度)
        //
        [EnumDisplayNameProvider(typeof(FallbackTest), EnumDisplayNameProviderPriority.Highest)]
        public static string EnumDisplayNameProvider_Highest(Enum enumValue)
        {
            if (enumValue is FallbackTest specificEnum)
            {
                switch (specificEnum)
                {
                    case FallbackTest.ProvideByHighest:
                        return "列挙値表示名供給関数(最高優先度)によって供給された表示名です";

                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

    } // class
#endif


#if DEBUG
    // ＜メモ＞
    // ・EnumDisplayName クラスのテストとも言えるが、EnumDisplayNameProviderPriority 列挙体との関連性が強いためこのソースファイルに収録している。
    //====================================================================================================
    /// <summary>
    /// EnumDisplayName フォールバック処理のテスト(実践的)<br/>
    /// <br/>
    /// ＜想定条件＞<br/>
    /// ①列挙体の定義は既存のクラスライブラリにあるため編集できない。<br/>
    /// ②汎用ライブラリで日本語表示名を供給する。<br/>
    /// ③英語圏向けアドオンで、英語圏向け表示名(LCID=1033:en-US)で上書きする。<br/>
    /// ④さらに、イギリス向けアドオンで、イギリス英語の表示名(LCID=2057:en-GB)を差分で供給する。<br/>
    /// </summary>
    //====================================================================================================
    public partial class ZZZTest_EnumDisplayName_Fallback_Practical
    {
        //--------------------------------------------------------------------------------
        // 自動テスト
        //--------------------------------------------------------------------------------
        [AutoTestMethod("EnumDisplayName フォールバック処理のテスト(実践的)")]
        public static void Test()
        {
            SubRoutine(TrafficSignalKind.GreenLight, "green light", "英語圏向けアドオンで上書きされたパターン");
            SubRoutine(TrafficSignalKind.YellowLight, "amber light", "イギリス向けアドオンで上書きされたパターン");
            SubRoutine(TrafficSignalKind.RedLight, "red light", "英語圏向けアドオンで上書きされたパターン");


            //------------------------------------------------------------
            // 【ローカル関数】
            //------------------------------------------------------------
            void SubRoutine(TrafficSignalKind enumValue,    // [in ]：交通信号種別
                            string displayString,           // [in ]：予想結果(列挙値表示名)
                            string testPattern = null)      // [in ]：テストパターン名[null = 省略]
            {
                var testOptions = new AutoTestOptions(testPattern)
                {                                                           // テストオプション
                    fncInstanceRegularString = RegularString.WithTypeName,  // ・対象インスタンスは型名併記
                };

                AutoTest.TestX<Enum, string>(EnumDisplayName.XToDisplayName, enumValue, displayString, testOptions);
            }
        }


        //================================================================================
        /// <summary>
        /// 【交通信号種別】
        /// </summary>
        //================================================================================
        public enum TrafficSignalKind
        {
            GreenLight,
            YellowLight,
            RedLight,
        }


        //================================================================================
        /// <summary>
        /// 【交通信号種別表示名】交通信号種別の日本語版列挙値表示名を供給します。
        /// </summary>
        //================================================================================
        public static partial class TrafficSignalKindDisplayName
        {
            [EnumDisplayNameProvider(typeof(TrafficSignalKind))]    // 列挙値表示名供給関数(通常優先度)
            public static string TestEnumDisplayNameProvider(Enum enumValue)
            {
                if (enumValue is TrafficSignalKind specificEnum)
                {
                    switch (specificEnum)
                    {
                        case TrafficSignalKind.GreenLight:
                            return "青信号";

                        case TrafficSignalKind.YellowLight:
                            return "黄色信号";

                        case TrafficSignalKind.RedLight:
                            return "赤信号";

                        default:
                            return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        } // class


        //================================================================================
        /// <summary>
        /// 【英語圏向け交通信号種別表示名】交通信号種別の英語圏向け列挙値表示名を供給します。<br/>
        /// 英語圏向けの表示名を、日本語版をすべて上書きする形で供給します。
        /// </summary>
        //================================================================================
        public static partial class TrafficSignalKindDisplayNameForEnglish
        {
            [EnumDisplayNameProvider(typeof(TrafficSignalKind), EnumDisplayNameProviderPriority.High)]  // 列挙値表示名供給関数(高い優先度)
            public static string TestEnumDisplayNameProvider(Enum enumValue)
            {
                if (enumValue is TrafficSignalKind specificEnum)
                {
                    switch (specificEnum)
                    {
                        case TrafficSignalKind.GreenLight:
                            return "green light";

                        case TrafficSignalKind.YellowLight:
                            return "yellow light";

                        case TrafficSignalKind.RedLight:
                            return "red light";

                        default:
                            return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        } // class


        //================================================================================
        /// <summary>
        /// 【イギリス向け交通信号種別表示名】交通信号種別のイギリス向け列挙値表示名を供給します。<br/>
        /// イギリス向けの表示名を、英語圏版に対する差分の形で供給します。
        /// </summary>
        //================================================================================
        public static partial class TrafficSignalKindDisplayNameForEnGB
        {
            [EnumDisplayNameProvider(typeof(TrafficSignalKind), EnumDisplayNameProviderPriority.Highest)]   // 列挙値表示名供給関数(最高優先度)
            public static string TestEnumDisplayNameProvider(Enum enumValue)
            {
                if (enumValue is TrafficSignalKind specificEnum)
                {
                    switch (specificEnum)
                    {
                        case TrafficSignalKind.YellowLight:
                            return "amber light";

                        default:
                            return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        } // class

    } // class
#endif

} // namespace
