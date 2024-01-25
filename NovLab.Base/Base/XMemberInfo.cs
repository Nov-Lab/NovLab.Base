// @(h)XMemberInfo.cs ver 0.00 ( '22.04.22 Nov-Lab ) 作成開始
// @(h)XMemberInfo.cs ver 0.21 ( '22.04.22 Nov-Lab ) アルファ版完成
// @(h)XMemberInfo.cs ver 0.21a( '24.01.26 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【MemberInfo 拡張メソッド】System.Reflection.MemberInfo クラスに拡張メソッドを追加します。

using System;
using System.Diagnostics;
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


        // 試しに一度作ってみたものの、よく考えたら不要だったもの
        //
        // ・GetCustomAttributes(Type attributeType, bool inherit) の記述簡素化版
        //   public static TAttribute[] XGetCustomAttributes<TAttribute>(this MemberInfo target, bool inherit) where TAttribute : Attribute
        //   
        //   ＜記述簡素化前＞変換の手間がかかり、変換前と変換後の変数の混同にも注意が必要
        //     var attributes =                                       //// ○○メソッド属性の配列を取得する <- 戻り値は object[]
        //         methodInfo.GetCustomAttributes(typeof(MyAttribute), false);
        //     foreach (var tmpAttr in attributes)
        //     {                                                      //// メソッド属性配列を繰り返す
        //         var myattr = (MyAttribute)tmpAttr;                 /////  ○○メソッド属性に変換する
        //           ：                                               /////  （属性を利用する処理）
        //     }
        //
        //   ＜記述簡素化後＞ジェネリックを利用して指定された型の配列を返すことで、型変換の手間や記述の冗長さを省こうとした
        //     var attributes =                                       //// ○○メソッド属性の配列を取得する <- 戻り値は MyAttribute[]
        //         methodInfo.XGetCustomAttributes<MyAttribute>(false);
        //     foreach (var tmpAttr in attributes)
        //     {                                                      /////  ○○メソッド属性配列を繰り返す
        //           ：                                               /////  （属性を利用する処理）
        //     }
        //
        //   ＜よく考えたら＞foreach ステートメント内でキャストすれば、記述を簡素化できる
        //     var attributes =                                       //// ○○メソッド属性の配列を取得する <- 戻り値は object[]
        //         methodInfo.GetCustomAttributes(typeof(MyAttribute), false);
        //     foreach (MyAttribute tmpAttr in attributes)            // (キャストは必ず成功する)
        //     {                                                      //// ○○メソッド属性配列を繰り返す
        //           ：                                               /////  （属性を利用する処理）
        //     }

    } // class

} // namespace
