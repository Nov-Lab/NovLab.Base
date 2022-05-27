// @(h)XMemberInfo.cs ver 0.00 ( '22.04.22 Nov-Lab ) 作成開始
// @(h)XMemberInfo.cs ver 0.21 ( '22.04.22 Nov-Lab ) アルファ版完成
// @(h)XMemberInfo.cs ver 0.21a( '22.05.24 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【MemberInfo 拡張メソッド】System.Reflection.MemberInfo クラスに拡張メソッドを追加します。

using System;
using System.Reflection;
using System.Collections.Generic;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【MemberInfo 拡張メソッド】System.Reflection.MemberInfo クラスに拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XMemberInfo
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【クラス名付きメンバー名取得】クラス名付きのメンバー名(例:"String.TrimEnd")を取得します。
        /// </summary>
        /// <param name="memberInfo">[in ]：メンバー情報</param>
        /// <returns>
        /// クラス名付きのメンバー名
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XGetName(this MemberInfo memberInfo)
        {
            return memberInfo.DeclaringType.Name + "." + memberInfo.Name;
        }

    }
}
