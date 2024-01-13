// @(h)FILEandLINE.cs ver 0.00 ( '24.01.19 Nov-Lab ) 作成開始
// @(h)FILEandLINE.cs ver 0.51 ( '24.01.20 Nov-Lab ) ベータ版完成

// @(s)
// 　【ソースファイル名と行番号】C言語/C++の事前定義済みマクロ __FILE__ や __LINE__ に相当する情報を取得する機能を提供します。

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace NovLab.DebugSupport
{
    //====================================================================================================
    /// <summary>
    /// 【ソースファイル名と行番号】C言語/C++の事前定義済みマクロ __FILE__ や __LINE__ に相当する情報を取得する機能を提供します。
    /// </summary>
    /// <remarks>
    /// 注意事項<br></br>
    /// ・これらの機能を使用すると、バイナリーファイルの中にフルパスソースファイル名が含まれますのでご注意ください。<br></br>
    /// <br></br>
    /// 補足<br></br>
    /// ・.NET Framework 4.5 以降が必要です。<br></br>
    /// </remarks>
    //====================================================================================================
    public partial class FILEandLINE
    {
        //====================================================================================================
        // 読み取り専用公開フィールド
        //====================================================================================================
        /// <summary>
        /// 【メンバー名】メソッド名またはプロパティ名
        /// </summary>
        public readonly string memberName;

        /// <summary>
        /// 【フルパスソースファイル名】
        /// </summary>
        public readonly string sourceFileName;

        /// <summary>
        /// 【ソースファイルの行番号】
        /// </summary>
        public readonly int sourceLineNumber;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列形式作成】このインスタンスの内容を表す文字列を取得します。<br></br>
        /// </summary>
        /// <returns>文字列形式</returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・文字列形式は以下のような形式になります。
        /// <code>
        /// 書式：＜メンバー名＞() in ＜パスなしソースファイル名＞[line:＜行番号＞]
        /// 例  ：MyMethod() in MyClass.cs[line:128]
        /// </code>
        /// メモ<br></br>
        /// ・<see cref="GetOutline(string, string, int)"/> からも使用しているので、書式変更は慎重に行うこと。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public override string ToString()
        {
            return string.Format("{0}() in {1}[line:{2}]",
                memberName,
                System.IO.Path.GetFileName(sourceFileName),
                sourceLineNumber);
        }


        //====================================================================================================
        // コンストラクターと公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コンストラクター】ソースファイル名や行番号などの情報をひとまとめに取得します。
        /// </summary>
        /// <param name="memberName">      [in ]：省略してください。</param>
        /// <param name="sourceFilePath">  [in ]：省略してください。</param>
        /// <param name="sourceLineNumber">[in ]：省略してください。</param>
        /// <remarks>
        /// 注意事項<br></br>
        /// ・このコンストラクターを使用すると、バイナリーファイルの中にフルパスソースファイル名が含まれますのでご注意ください。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public FILEandLINE([CallerMemberName] string memberName = "",
                           [CallerFilePath] string sourceFilePath = "",
                           [CallerLineNumber] int sourceLineNumber = 0)
        {
            this.memberName = memberName;
            this.sourceFileName = sourceFilePath;
            this.sourceLineNumber = sourceLineNumber;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【概要文字列取得】メンバー名、ソースファイル名、ソースファイル行番号をまとめて記述した概要文字列を取得します。
        /// </summary>
        /// <param name="memberName">      [in ]：省略してください。</param>
        /// <param name="sourceFilePath">  [in ]：省略してください。</param>
        /// <param name="sourceLineNumber">[in ]：省略してください。</param>
        /// <returns>
        /// 概要文字列
        /// </returns>
        /// <remarks>
        /// 注意事項<br></br>
        /// ・このメソッドを使用すると、バイナリーファイルの中にフルパスソースファイル名が含まれますのでご注意ください。<br></br>
        /// ・概要文字列の書式は <see cref="ToString"/> を参照してください。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string GetOutline([CallerMemberName] string memberName = "",
                                        [CallerFilePath] string sourceFilePath = "",
                                        [CallerLineNumber] int sourceLineNumber = 0)
            => new FILEandLINE(memberName, sourceFilePath, sourceLineNumber).ToString();


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メンバー名取得】メソッド名またはプロパティ名を取得します。
        /// </summary>
        /// <param name="memberName">[in ]：省略してください。</param>
        /// <returns>
        /// メンバー名(メソッド名またはプロパティ名)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string GetMemberName([CallerMemberName] string memberName = "") => memberName;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【フルパスソースファイル名取得】フルパスソースファイル名を取得します。C言語/C++の __FILE__ に相当します。
        /// </summary>
        /// <param name="sourceFilePath">[in ]：省略してください。</param>
        /// <returns>
        /// フルパスソースファイル名
        /// </returns>
        /// <remarks>
        /// 注意事項<br></br>
        /// ・このメソッドを使用すると、バイナリーファイルの中にフルパスソースファイル名が含まれますのでご注意ください。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string GetSourceFileName([CallerFilePath] string sourceFilePath = "") => sourceFilePath;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ソースファイル行番号取得】ソースファイルの行番号を取得します。C言語/C++の __LINE__ に相当します。
        /// </summary>
        /// <param name="sourceLineNumber">[in ]：省略してください。</param>
        /// <returns>
        /// ソースファイルの行番号
        /// </returns>
        //--------------------------------------------------------------------------------
        public static int GetSourceLineNumber([CallerLineNumber] int sourceLineNumber = 0) => sourceLineNumber;


#if DEBUG
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【関連Tips】<br></br>
        /// ・クラス名は <see cref="System.Reflection.MethodBase.GetCurrentMethod"/>.ReflectedType.Name で取得できます。<br></br>
        /// ・メンバー名だけなら <see cref="System.Reflection.MethodBase.GetCurrentMethod"/>.Name で取得できます。<br></br>
        /// ・.NET Framework 4.0以前では <see cref="StackFrame"/> クラスで取得できます。ただし、*.pdb ファイルがないとソースファイル名と行番号は取得できません。<br></br>
        /// </summary>
        //--------------------------------------------------------------------------------
        public static void ZZZTips() { }
#endif

    } // class


    // 手動テスト用のクラス(リリース版でも動作することを確認済み)
#if DEBUG
    //====================================================================================================
    // FILEandLINE の手動テスト用クラス
    //====================================================================================================
    public partial class ZZZTest_FILEandLINE
    {
        [ManualTestMethod(nameof(FILEandLINE) + " の総合的テスト")]
        public static void ZZZ_OverallTest()
        {
            Trace.WriteLine("概要文字列：" + FILEandLINE.GetOutline());
            Trace.WriteLine("メンバー名：" + FILEandLINE.GetMemberName());
            Trace.WriteLine("ファイル名：" + FILEandLINE.GetSourceFileName());
            Trace.WriteLine("行番号    ：" + FILEandLINE.GetSourceLineNumber());
            Trace.WriteLine("");


            // (参考).NET Framework 4.0以前での方法
            Trace.WriteLine("＜(参考).NET Framework 4.0以前での方法＞ただし *.pdb ファイルがないとソースファイル名と行番号は取得できない");
            var sf = new StackFrame(true);
            Trace.WriteLine(string.Format("概要文字列：{0}() in {1}[line:{2}]",
                            sf.GetMethod().Name,                            // リリース版でも取得可能
                            System.IO.Path.GetFileName(sf.GetFileName()),   // *.pdb ファイルがあればリリース版でも取得可能
                            sf.GetFileLineNumber()));                       // *.pdb ファイルがあればリリース版でも取得可能
            Trace.WriteLine("");


            // (参考)MethodBase クラスで取得する方法
            Trace.WriteLine("＜(参考)MethodBase クラスで取得する方法＞");
            Trace.WriteLine("クラス名  ：" + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name);
            Trace.WriteLine("メンバー名：" + System.Reflection.MethodBase.GetCurrentMethod().Name);


            // ＜手動テスト実行時の出力結果例＞
            //   ■FILEandLINE の総合的テスト
            //   概要文字列：ZZZ_OverallTest() in FILEandLINE.cs[line:185]
            //   メンバー名：ZZZ_OverallTest
            //   ファイル名：U:\NovLabRepos\NovLab.Base\NovLab.Base\DebugSupport\FILEandLINE.cs
            //   行番号    ：188
            //
            //   ＜(参考).NET Framework 4.0以前での方法＞ただし *.pdb ファイルがないとソースファイル名と行番号は取得できない
            //   概要文字列：ZZZ_OverallTest() in FILEandLINE.cs[line:194]
            //
            //   ＜(参考)MethodBase クラスで取得する方法＞
            //   クラス名  ：ZZZTest_FILEandLINE
            //   メンバー名：ZZZ_OverallTest
        }
    }
#endif

} // namespace
