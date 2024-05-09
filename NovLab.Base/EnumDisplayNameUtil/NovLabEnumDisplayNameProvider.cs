// @(h)NovLabEnumDisplayNameProvider.cs ver 0.00 ( '24.05.07 Nov-Lab ) 作成開始
// @(h)NovLabEnumDisplayNameProvider.cs ver 0.21 ( '24.05.08 Nov-Lab ) アルファ版完成
// @(h)NovLabEnumDisplayNameProvider.cs ver 0.21a( '24.05.14 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【NovLab列挙値表示名供給関数群】.NET Framework で定義されている既存の列挙体について、列挙値に対応する表示名を供給する機能を提供します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Globalization;


namespace NovLab.EnumDisplayNameUtil
{
    //====================================================================================================
    /// <summary>
    /// 【NovLab提供版・列挙値表示名供給関数群】
    /// .NET Framework で定義されている既存の列挙体について、列挙値に対応する表示名を供給する機能を提供します。
    /// </summary>
    /// <remarks>
    /// 補足<br/>
    /// ・<see cref="EnumDisplayName"/> クラスを使うことで、これらの関数が供給する列挙値表示名を取得することができます。<br/>
    /// <code>
    /// | Debug.Print("可燃ごみの日=" + DayOfWeek.Tuesday.XToDisplayName());
    /// </code>
    /// </remarks>
    //====================================================================================================
    public static partial class NovLabEnumDisplayNameProvider
    {
        //================================================================================
        /// <summary>
        /// 【EventLogEntryType 列挙体用列挙値表示名供給関数群】
        /// 列挙値に対応する表示名を供給する機能を提供します。
        /// </summary>
        //================================================================================
        public static partial class ForEventLogEntryType
        {
            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【EventLogEntryType 用列挙値表示名取得】列挙値に対応する表示名を取得します。<br/>
            /// </summary>
            /// <param name="enumValue">[in ]：列挙値</param>
            /// <returns>
            /// 列挙値表示名[null = 取得失敗]
            /// </returns>
            /// <remarks>
            /// 補足<br/>
            /// ・<see cref="EnumDisplayName"/> クラスを用いて列挙値表示名を取得するための列挙値表示名供給関数です。<br/>
            /// ・列挙体で定義されている値でない場合(<c>(EventLogEntryType)(-1)</c> でキャストした列挙値など)は、取得に失敗します。<br/>
            /// ・フォールバック機構が必要でない場合は直接呼び出して使うこともできますが、取得失敗時は null を返しますのでご注意ください。<br/>
            /// </remarks>
            //--------------------------------------------------------------------------------
            [EnumDisplayNameProvider(typeof(EventLogEntryType), EnumDisplayNameProviderPriority.NovLab)]    // 列挙値表示名供給関数(NovLab提供版)
            public static string GetEnumDisplayName(Enum enumValue)
            {
                //------------------------------------------------------------
                /// 列挙値に対応する列挙値表示名を取得する
                //------------------------------------------------------------
                if (enumValue is EventLogEntryType specificEnum)
                {                                                           //// 列挙値を固有の列挙型に変換できる場合(正しい呼び出しの場合)
                    switch (specificEnum)
                    {                                                       /////  EventLogEntryType 列挙値で分岐
                        case EventLogEntryType.Error:                       //////   ・Error の場合
                            return "エラーイベント";                        ///////    戻り値 = "エラーイベント" で関数終了

                        case EventLogEntryType.Warning:                     //////   ・Warning の場合
                            return "警告イベント";                          ///////    戻り値 = "警告イベント" で関数終了

                        case EventLogEntryType.Information:                 //////   ・Information の場合
                            return "情報イベント";                          ///////    戻り値 = "情報イベント" で関数終了

                        case EventLogEntryType.SuccessAudit:                //////   ・SuccessAudit の場合
                            return "成功した監査イベント";                  ///////    戻り値 = "成功した監査イベント" で関数終了

                        case EventLogEntryType.FailureAudit:                //////   ・FailureAudit の場合
                            return "監査エラーイベント";                    ///////    戻り値 = "監査エラーイベント" で関数終了

                        default:                                            //////   ・上記以外の場合
                            return null;                                    ///////    戻り値 = null(取得失敗) で関数終了
                    }
                }
                else
                {                                                           //// 列挙値を固有の列挙型に変換できない場合(不正な呼び出しの場合)
                    return null;                                            /////  戻り値 = null(取得失敗) で関数終了
                }
            }
        } // class


        //================================================================================
        /// <summary>
        /// 【DayOfWeek 列挙体用列挙値表示名供給関数群】
        /// 列挙値に対応する表示名を供給する機能を提供します。
        /// </summary>
        //================================================================================
        public static partial class ForDayOfWeek
        {
            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【DayOfWeek 用列挙値表示名取得】列挙値に対応する表示名を取得します。<br/>
            /// 固定の文字列ではなく、現在のカルチャに基づく曜日の完全名を、<see cref="DateTimeFormatInfo.GetDayName(DayOfWeek)"/> で取得して返します。<br/>
            /// </summary>
            /// <param name="enumValue">[in ]：列挙値</param>
            /// <returns>
            /// 列挙値表示名[null = 取得失敗]
            /// </returns>
            /// <remarks>
            /// 補足<br/>
            /// ・<see cref="EnumDisplayName"/> クラスを用いて列挙値表示名を取得するための列挙値表示名供給関数です。<br/>
            /// ・列挙体で定義されている値でない場合(<c>(DayOfWeek)(-1)</c> でキャストした列挙値など)は、取得に失敗します。<br/>
            /// ・フォールバック機構が必要でない場合は直接呼び出して使うこともできますが、取得失敗時は null を返しますのでご注意ください。<br/>
            /// </remarks>
            //--------------------------------------------------------------------------------
            [EnumDisplayNameProvider(typeof(DayOfWeek), EnumDisplayNameProviderPriority.NovLab)]    // 列挙値表示名供給関数(NovLab提供)
            public static string GetEnumDisplayName(Enum enumValue)
            {
                //------------------------------------------------------------
                /// 列挙値に対応する列挙値表示名を取得する
                //------------------------------------------------------------
                if (enumValue is DayOfWeek specificEnum)
                {                                                           //// 列挙値を固有の列挙型に変換できる場合(正しい呼び出しの場合)
                    if (Enum.IsDefined(typeof(DayOfWeek), specificEnum))
                    {                                                       /////  列挙体で定義されている値の場合
                        return                                              //////   現在のカルチャに基づく曜日の完全名を取得して戻り値とし、関数終了
                            CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(specificEnum);
                    }
                    else
                    {                                                       /////  列挙体で定義されている値でない場合((DayOfWeek)(-1) でキャストした列挙値など)
                        return null;                                        //////   戻り値 = null(取得失敗) で関数終了
                    }
                }
                else
                {                                                           //// 列挙値を固有の列挙型に変換できない場合(不正な呼び出しの場合)
                    return null;                                            /////  戻り値 = null(取得失敗) で関数終了
                }
            }


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【DayOfWeek 用列挙値表示名(日本語１文字)取得】列挙値に対応する表示名(日本語１文字)を取得します。<br/>
            /// </summary>
            /// <param name="enumValue">[in ]：列挙値</param>
            /// <returns>
            /// 列挙値表示名[null = 取得失敗]
            /// </returns>
            /// <remarks>
            /// 補足<br/>
            /// ・<see cref="EnumDisplayName"/> クラスを用いずに直接呼び出して使うための列挙値表示名供給関数です。<br/>
            /// ・列挙体で定義されている値でない場合(<c>(DayOfWeek)(-1)</c> でキャストした列挙値など)は、取得に失敗します。<br/>
            /// ・取得失敗時は null を返しますのでご注意ください。<br/>
            /// </remarks>
            //--------------------------------------------------------------------------------
            public static string GetEnumDisplayNameByJP1Char(Enum enumValue)
            {
                //------------------------------------------------------------
                /// 列挙値に対応する列挙値表示名を取得する
                //------------------------------------------------------------
                if (enumValue is DayOfWeek specificEnum)
                {                                                           //// 列挙値を固有の列挙型に変換できる場合(正しい呼び出しの場合)
                    switch (specificEnum)
                    {                                                       /////  DayOfWeek 列挙値で分岐
                        case DayOfWeek.Sunday:                              //////   ・Sunday の場合、戻り値 = "日" で関数終了
                            return "日";

                        case DayOfWeek.Monday:                              //////   ・Monday の場合、戻り値 = "月" で関数終了
                            return "月";

                        case DayOfWeek.Tuesday:                             //////   ・Tuesday の場合、戻り値 = "火" で関数終了
                            return "火";

                        case DayOfWeek.Wednesday:                           //////   ・Wednesday の場合、戻り値 = "水" で関数終了
                            return "水";

                        case DayOfWeek.Thursday:                            //////   ・Thursday の場合、戻り値 = "木" で関数終了
                            return "木";

                        case DayOfWeek.Friday:                              //////   ・Friday の場合、戻り値 = "金" で関数終了
                            return "金";

                        case DayOfWeek.Saturday:                            //////   ・Saturday の場合、戻り値 = "土" で関数終了
                            return "土";

                        default:                                            //////   ・上記以外の場合(列挙体で定義されていない内部整数値の場合)
                            return null;                                    ///////    戻り値 = null(取得失敗) で関数終了
                    }
                }
                else
                {                                                           //// 列挙値を固有の列挙型に変換できない場合(不正な呼び出しの場合)
                    return null;                                            /////  戻り値 = null(取得失敗) で関数終了
                }
            }

        } // class ForDayOfWeek

    } // class

} // namespace
