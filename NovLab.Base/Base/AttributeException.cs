// @(h)AttributeException.cs ver 0.00 ( '24.05.13 Nov-Lab ) 作成開始
// @(h)AttributeException.cs ver 0.21 ( '24.05.13 Nov-Lab ) アルファ版完成
// @(h)AttributeException.cs ver 0.21a( '24.05.14 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【属性不正例外】属性の使い方が不正な場合にスローされる例外です。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【属性不正例外】属性の使い方が不正な場合にスローされる例外です。<br/>
    /// この例外の多くは、ビルド段階でコンパイルエラーとすべき単純なコーディングミスでありながら、
    /// 実行時でなければ検出できないものを示します。<br/>
    /// </summary>
    /// <remarks>
    /// ヒント<br/>
    /// ・属性クラスの定義に <see cref="AttributeUsageAttribute"/> 属性を付加することで、属性の適用対象要素を制限することができます。<br/>
    /// <br/>
    /// 用途例<br/>
    /// ・Action(int) デリゲートに適合するメソッドに付加すべき属性を、適合しないメソッドに付加した。<br/>
    /// ・string 型のプロパティーに付加すべき属性を、別の型のプロパティーに付加した。<br/>
    /// </remarks>
    //====================================================================================================
    public class AttributeException : Exception
    {
        //====================================================================================================
        // 内部定数定義
        //====================================================================================================
        /// <summary>
        /// 【既定の例外メッセージ】
        /// </summary>
        private const string DEFAULT_MESSAGE = "属性の使い方が不正です。";


        //====================================================================================================
        // 公開プロパティー
        //====================================================================================================

        /// <summary>
        /// 【属性クラス名】この例外の原因である属性クラスの名前を取得します。
        /// </summary>
        public virtual string AttributeClassName => bf_attributeClassName;
        protected readonly string bf_attributeClassName = "";


        /// <summary>
        /// 【例外メッセージ(読み取り専用)】例外の内容を説明するメッセージを取得します。
        /// </summary>
        public override string Message
        {
            get
            {
                //------------------------------------------------------------
                /// 例外メッセージを取得する
                //------------------------------------------------------------
                var message = base.Message;                                 //// 例外メッセージ(コンストラクターで設定した基本部分)を取得する
                if (message == null)
                {                                                           //// 例外メッセージ = null の場合
                    message = DEFAULT_MESSAGE;                              /////  例外メッセージ = 既定の例外メッセージ
                }

                if (string.IsNullOrEmpty(AttributeClassName) == false)
                {                                                           //// 属性クラス名に有効文字列が設定されている場合
                    message += "\n属性クラス名:" + AttributeClassName;      /////  例外メッセージに属性クラス名情報を追加する
                }

                return message;                                             //// 戻り値 = 例外メッセージ で関数終了
            }
        }


        //====================================================================================================
        // コンストラクター
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デフォルトコンストラクター】属性不正例外の新しいインスタンスを生成します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public AttributeException() : base(DEFAULT_MESSAGE) { }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コンストラクター】エラーメッセージを指定して、属性不正例外の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="message">[in ]：エラーメッセージ</param>
        //--------------------------------------------------------------------------------
        public AttributeException(string message) : base(message) { }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コンストラクター】エラーメッセージと、属性クラス名を指定して、属性不正例外の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="message">           [in ]：エラーメッセージ</param>
        /// <param name="attributeClassName">[in ]：属性クラス名(通常は <c>nameof(&lt;属性クラス&gt;)</c></param>
        //--------------------------------------------------------------------------------
        public AttributeException(string message, string attributeClassName) : base(message)
        {
            bf_attributeClassName = attributeClassName;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コンストラクター】エラーメッセージと、この例外の原因である内部例外への参照を指定して、属性不正例外の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="message">       [in ]：エラーメッセージ</param>
        /// <param name="innerException">[in ]：内部例外</param>
        //--------------------------------------------------------------------------------
        public AttributeException(string message, Exception innerException) : base(message, innerException) { }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コンストラクター】エラーメッセージ、属性クラス名、およびこの例外の原因である内部例外への参照を指定して、属性不正例外の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="message">           [in ]：エラーメッセージ</param>
        /// <param name="attributeClassName">[in ]：属性クラス名(通常は <c>nameof(&lt;属性クラス&gt;)</c></param>
        /// <param name="innerException">    [in ]：内部例外</param>
        //--------------------------------------------------------------------------------
        public AttributeException(string message, string attributeClassName, Exception innerException) : base(message, innerException)
        {
            bf_attributeClassName = attributeClassName;
        }


        //====================================================================================================
        // エラーメッセージ作成用ユーティリティーメソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【デリゲート不適合エラーメッセージ作成】
        /// 属性を付加されているメソッドが、要求するデリゲートに適合しない場合のエラーメッセージを作成するユーティリティーメソッドです。
        /// </summary>
        /// <typeparam name="TDelegate">デリゲートの型</typeparam>
        /// <param name="methodInfo">[in ]：属性を付加されているメソッドの <see cref="MethodInfo"/></param>
        /// <returns>
        /// メッセージ文字列
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・作成されるメッセージは、以下のような書式になります。<br/>
        /// <code>
        /// 書式：＜クラス名＞.＜メソッド名＞ は ＜デリゲート名＞ に適合しません。[＜追加メッセージ＞]
        /// 例  ：MyClass.MyMethod は MyDelegate デリゲートに適合しません。
        /// </code>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string MakeDelegateMismatchMessage<TDelegate>(MethodInfo methodInfo) where TDelegate : Delegate
        {
            return methodInfo.DeclaringType.FullName + "." + methodInfo.Name +
                   " は、" + typeof(TDelegate).FullName + " デリゲートに適合しません。";
        }


        //--------------------------------------------------------------------------------
        // テストコード：以下を有効化して var tmp = DayOfWeek.Monday.XToDisplayName(); を呼び出すことでテストできます
        //--------------------------------------------------------------------------------
#if TESTCODE_DELEGATEMISMATCH
        [EnumDisplayNameProvider(typeof(PlatformID))]
        public static void MismatchMethod() { }
#endif

    } // class

} // namespace
