﻿// @(h)Referrable.cs ver 0.00 ( '22.05.01 Nov-Lab ) 作成開始
// @(h)Referrable.cs ver 1.01 ( '22.05.01 Nov-Lab ) 初版完成

// @(s)
// 　【参照可能型】値型変数を参照型と同様に扱えるようにします。

using System;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【参照可能型】値型変数を参照型と同様に扱えるようにします。<br></br>
    /// ・同時利用性：１つの実体を複数個所から参照し、利用できます。<br></br>
    /// ・事後利用性：例えば、コンストラクターでint変数への参照を保持しておくと、
    ///   後で呼び出されるメソッドやデストラクターからその変数を参照できるようになります。<br></br>
    /// </summary>
    /// <typeparam name="T">値型変数の型</typeparam>
    /// <remarks>
    /// ・使用例は <see cref="RecursionBlocker"/> を参考にしてください。<br></br>
    /// </remarks>
    //====================================================================================================
    public class Referrable<T> where T : struct
    {
        /// <summary>
        /// 【値型変数値】値型変数の実体です。
        /// </summary>
        public T Value { get; set; } = default;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コンストラクター】新しいインスタンスを初期化します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public Referrable() { }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コンストラクター】初期値を指定して新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="initialValue">[in ]：初期値</param>
        //--------------------------------------------------------------------------------
        public Referrable(T initialValue)
        {
            Value = initialValue;
        }
    }
}
