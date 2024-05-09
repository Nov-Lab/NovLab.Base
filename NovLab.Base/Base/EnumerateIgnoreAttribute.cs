// @(h)EnumerateIgnoreAttribute.cs ver 0.00 ( '24.05.06 Nov-Lab ) 作成開始
// @(h)EnumerateIgnoreAttribute.cs ver 0.51 ( '24.05.09 Nov-Lab ) ベータ版完成
// @(h)EnumerateIgnoreAttribute.cs ver 0.51a( '24.05.13 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【列挙処理対象外マーク属性】各種列挙処理の際に、この属性の付加対象を列挙処理から除外するように指示します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;


namespace NovLab
{
    // ＜メモ＞
    // ・列挙値以外(クラス、メソッド、フィールド、プロパティーなど)で使う可能性も考えておくこと。
    //====================================================================================================
    /// <summary>
    /// 【列挙処理対象外マーク属性】各種列挙処理の際に、この属性の付加対象を列挙処理から除外するように指示します。
    /// </summary>
    /// <remarks>
    /// 用途<br/>
    /// ・特定の要素を列挙処理の対象外にしたい場合に使います。<br/>
    /// <br/><br/><br/>
    /// 列挙型での使い方<br/>
    /// ・None、Unknown、Invalid など特殊な意味を持つ列挙値を、有効列挙値の対象外として扱いたい場合に使います。<br/>
    /// ・この属性が付加されているかどうかは <see cref="XEnum.XHasIgnore(Enum)"/> でチェックできます。<br/>
    /// ・Enum クラスの仕様により、同じ内部整数値を持つ列挙値が複数定義されている列挙型では注意が必要です。
    ///  (参考メモ：<see cref="Memo_DuplicateEnumValue"/>)<br/>
    /// <br/>
    /// 参照<br/>
    /// ・<see cref="XEnum.GetValidValues(Type, Enum[])"/><br/>
    /// ・<see cref="XEnum.XGetValidValues{TEnum}(TEnum[])"/><br/>
    /// </remarks>
    //====================================================================================================
    [AttributeUsage(AttributeTargets.All)]  // 属性適用対象 = すべて
    public partial class EnumerateIgnoreAttribute : Attribute
    {
    }

} // namespace
