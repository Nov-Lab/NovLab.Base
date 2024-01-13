// @(h)AutoTest.cs ver 0.00 ( '22.04.22 Nov-Lab ) 作成開始
// @(h)AutoTest.cs ver 0.21 ( '22.05.18 Nov-Lab ) アルファ版完成
// @(h)AutoTest.cs ver 0.22 ( '22.05.19 Nov-Lab ) 機能修正：コレクションI/Fに対応した(AutoTest.ToDisplayString, AutoTestResultInfo.Matches)
// @(h)AutoTest.cs ver 0.23 ( '22.05.24 Nov-Lab ) 機能修正：例外メッセージも通知するようにした
// @(h)AutoTest.cs ver 0.24 ( '24.01.16 Nov-Lab ) 機能修正：実行前後のインスタンスの内容も確認できるようにした
// @(h)AutoTest.cs ver 0.25 ( '24.01.16 Nov-Lab ) 機能修正：テスト可能なメソッドの形式を拡充した
// @(h)AutoTest.cs ver 0.26 ( '24.01.19 Nov-Lab ) 機能修正：テストオプションで定型文字列作成関数を指定できるようにした
// @(h)AutoTest.cs ver 0.37 ( '24.01.21 Nov-Lab ) 仕様変更：リスナーを登録制とすることで、毎回引数で渡さなくてもいいようにした

// @(s)
// 　【自動テスト】メソッドの単体テストを自動化する機能を提供します。

#if DEBUG
//#define CTRL_F5 // Ctrl+F5テスト：中断対象例外もテストします。中断対象例外は、例外設定を調整するか、Ctrl+F5(デバッグなしで開始)で中断なしにテスト可能です。
#endif

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;


#if DEBUG   // DEBUGビルドのみ使用可能
namespace NovLab.DebugSupport
{
    //====================================================================================================
    /// <summary>
    /// 【自動テスト結果リスナーI/F】自動テスト結果を受け取るリスナークラスに必要な機能を定義します。
    /// </summary>
    //====================================================================================================
    public interface IAutoTestResultListener
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果通知】自動テスト結果リスナーにテスト結果を通知します。
        /// </summary>
        /// <param name="autoTestResult">  [in ]：自動テスト結果種別</param>
        /// <param name="testDescription"> [in ]：テスト内容文字列</param>
        /// <param name="testPattern">     [in ]：テストパターン名[null = 省略]</param>
        /// <param name="execResult">      [in ]：実行結果文字列(戻り値 または 例外の型情報)</param>
        /// <param name="expectResult">    [in ]：予想結果文字列(戻り値 または 例外の型情報)</param>
        /// <param name="exceptionMessage">[in ]：例外メッセージ</param>
        /// <param name="befContent">      [in ]：実行前のインスタンス内容文字列(null = 静的メソッド)</param>
        /// <param name="aftContent">      [in ]：実行後のインスタンス内容文字列(null = 静的メソッド)</param>
        //--------------------------------------------------------------------------------
        void NoticeTestResult(AutoTestResultKind autoTestResult,
                              string testDescription, string testPattern,
                              string execResult, string expectResult,
                              string exceptionMessage,
                              string befContent,
                              string aftContent);

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メッセージ書き込み】自動テスト結果リスナーにメッセージを書き込みます。
        /// </summary>
        /// <param name="message">[in ]：メッセージ文字列</param>
        //--------------------------------------------------------------------------------
        void Print(string message);
    }


    // ＜今後の予定＞
    //[-] 保留：インスタンス内容文字列：インスタンス.ToString() が「System.OrdinalComparer」のように型名だけを返す場合は抑制したい


    // ＜メモ＞
    // ・自動テスト用メソッドの作成状況
    //   引数の個数              ：なし：１つ：２つ：３つ：
    //   通常メソッド、戻り値なし： 〇 ： 〇 ： 〇 ：<未>：
    //   通常メソッド、戻り値あり： 〇 ： 〇 ： 〇 ： 〇 ：
    //   拡張メソッド、戻り値なし：<未>： 〇 ：<未>：<未>：
    //   拡張メソッド、戻り値あり： 〇 ： 〇 ： 〇 ：<未>：
    //====================================================================================================
    /// <summary>
    /// 【自動テスト】メソッドの単体テストを自動化する機能を提供します。
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・テストデータとテストコードを常備しておくことで、いつでも誰でも何度でも同じテストを実行することができ、
    ///   ロジック修正時などのテストを効率化できます。<br></br>
    /// ・特に、機能追加やロジック変更の際に、処理結果が変わってしまっていないかどうかを確認するのに効果的です。<br></br>
    /// </remarks>
    //====================================================================================================
    public static partial class AutoTest
    {
        //====================================================================================================
        // リスナー関連
        //====================================================================================================

        /// <summary>
        /// 【自動テスト結果リスナーリスト】自動テストの結果を受け取るリスナーを管理します。
        /// </summary>
        private static List<IAutoTestResultListener> m_ifListeners = new List<IAutoTestResultListener>();


        //[-] 保留：必要になったら AddListener時の重複チェック, RemoveListener, ClearListener なども作る。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト結果リスナー追加】自動テスト結果リスナーを追加登録します。
        /// </summary>
        /// <param name="ifListener">[in ]：自動テスト結果リスナー</param>
        //--------------------------------------------------------------------------------
        public static void AddListener(IAutoTestResultListener ifListener)
        {
            //------------------------------------------------------------
            /// 自動テスト結果リスナーを追加登録する
            //------------------------------------------------------------
            lock (m_ifListeners)
            {                                                           //// クリティカルセクション：自動テスト結果リスナーリストを排他ロック
                m_ifListeners.Add(ifListener);                          /////  自動テスト結果リスナーをリストに追加する
            }
        }


        //====================================================================================================
        // 自動テスト制御メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メッセージ書き込み】自動テスト結果リスナーにメッセージを書き込みます。
        /// </summary>
        /// <param name="message">[in ]：メッセージ文字列</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・自動テスト結果リスナーにだけメッセージを書き込みたいときは、Debug.Print ではなくこちらを使います。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static void Print(string message)
        {
            //------------------------------------------------------------
            /// 自動テスト結果リスナーにメッセージを書き込む
            //------------------------------------------------------------
            foreach (var tmpListener in m_ifListeners)
            {                                                           //// 自動テスト結果リスナーリストを繰り返す
                tmpListener.Print(message);                             /////  メッセージ書き込み処理を行う
            }
        }


        //====================================================================================================
        // 自動テスト実施メソッド(通常メソッド、戻り値なし用)
        //====================================================================================================

        // ＜メモ＞
        // ・引数 targetInstance はなくすことも可能だが、型引数を推論させるのに有効なので残している。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(通常メソッド、戻り値なし、引数なし)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：instance.Action() 形式のメソッド(対象インスタンス操作系など)。<br></br>
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <param name="targetInstance">[in ]：対象インスタンス</param>
        /// <param name="actTestTarget"> [in ]：テスト対象Action</param>
        /// <param name="expectResult">  [in ]：予想結果</param>
        /// <param name="testOptions">   [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には、予想される実行結果を格納したインスタンス、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TTarget>(
                                TTarget targetInstance,
                                Action actTestTarget,
                                    AutoTestResultInfo<TTarget> expectResult, 
                                AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription
                = actTestTarget.Method.XGetName() + "()";               //// テスト内容文字列を作成する

            M_DoTest(testDescription, targetInstance                    //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Actionテスト本体
            //------------------------------------------------------------
            void CB_TestBody() => actTestTarget();                      //// テスト対象Actionを呼び出す
        }


        // ＜メモ＞
        // ・引数 targetInstance はなくすことも可能だが、型引数を推論させるのに有効なので残している。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(通常メソッド、戻り値なし、引数１つ)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：instance.Action(param1) 形式のメソッド(対象インスタンス操作系など)。<br></br>
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <typeparam name="TArg1">引数１の型</typeparam>
        /// <param name="targetInstance">[in ]：対象インスタンス</param>
        /// <param name="actTestTarget"> [in ]：テスト対象Action</param>
        /// <param name="inArg1">        [in ]：引数１</param>
        /// <param name="expectResult">  [in ]：予想結果</param>
        /// <param name="testOptions">   [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には、予想される実行結果を格納したインスタンス、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TTarget, TArg1>(
                                TTarget targetInstance,
                                Action<TArg1> actTestTarget,
                                    TArg1 inArg1,
                                    AutoTestResultInfo<TTarget> expectResult,
                                AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription = actTestTarget.Method.XGetName() + "(" +
                ToDisplayString(inArg1, testOptions.fncArg1RegularString) +
                ")";                                                    //// テスト内容文字列を作成する

            M_DoTest(testDescription, targetInstance                    //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Actionテスト本体
            //------------------------------------------------------------
            void CB_TestBody() => actTestTarget(inArg1);                //// テスト対象Actionを呼び出す
        }


        // ＜メモ＞
        // ・引数 targetInstance はなくすことも可能だが、型引数を推論させるのに有効なので残している。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(通常メソッド、戻り値なし、引数２つ)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：instance.Action(param1, param2) 形式のメソッド(対象インスタンス操作系など)。<br></br>
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <typeparam name="TArg1">引数１の型</typeparam>
        /// <typeparam name="TArg2">引数２の型</typeparam>
        /// <param name="targetInstance">[in ]：対象インスタンス</param>
        /// <param name="actTestTarget"> [in ]：テスト対象Action</param>
        /// <param name="inArg1">        [in ]：引数１</param>
        /// <param name="inArg2">        [in ]：引数２</param>
        /// <param name="expectResult">  [in ]：予想結果</param>
        /// <param name="testOptions">   [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には、予想される実行結果を格納したインスタンス、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TTarget, TArg1, TArg2>(
                                TTarget targetInstance,
                                Action<TArg1, TArg2> actTestTarget,
                                    TArg1 inArg1,
                                    TArg2 inArg2,
                                    AutoTestResultInfo<TTarget> expectResult,
                                AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription = actTestTarget.Method.XGetName() + "(" +
                ToDisplayString(inArg1, testOptions.fncArg1RegularString) + ", " +
                ToDisplayString(inArg2, testOptions.fncArg2RegularString) +
                ")";                                                    //// テスト内容文字列を作成する

            M_DoTest(testDescription, targetInstance                    //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Actionテスト本体
            //------------------------------------------------------------
            void CB_TestBody() => actTestTarget(inArg1, inArg2);        //// テスト対象Actionを呼び出す
        }


        //====================================================================================================
        // 自動テスト実施メソッド(通常メソッド、戻り値あり用)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(通常メソッド、戻り値あり、引数なし)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：result = instance.Func() 形式のメソッド(文字列操作系など)。<br></br>
        /// </summary>
        /// <typeparam name="TResult">戻り値の型</typeparam>
        /// <param name="fncTestTarget">[in ]：テスト対象Func</param>
        /// <param name="expectResult"> [in ]：予想結果</param>
        /// <param name="testOptions">  [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TResult>(
                                Func<TResult> fncTestTarget,
                                    AutoTestResultInfo<TResult> expectResult,
                                AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription 
                = fncTestTarget.Method.XGetName() + "()";               //// テスト内容文字列を作成する

            M_DoTest(testDescription, fncTestTarget.Target              //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Funcテスト本体
            //------------------------------------------------------------
            TResult CB_TestBody() => fncTestTarget();                   //// テスト対象Funcを呼び出す
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(通常メソッド、戻り値あり、引数１つ)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：result = instance.Func(param1) 形式のメソッド(文字列判定系など)。<br></br>
        /// </summary>
        /// <typeparam name="TArg1">引数１の型</typeparam>
        /// <typeparam name="TResult">戻り値の型</typeparam>
        /// <param name="fncTestTarget">[in ]：テスト対象Func</param>
        /// <param name="inArg1">       [in ]：引数１</param>
        /// <param name="expectResult"> [in ]：予想結果</param>
        /// <param name="testOptions">  [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TArg1, TResult>(
                                Func<TArg1, TResult> fncTestTarget,
                                    TArg1 inArg1,
                                    AutoTestResultInfo<TResult> expectResult, 
                                AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription = fncTestTarget.Method.XGetName() + "(" +
                ToDisplayString(inArg1, testOptions.fncArg1RegularString) +
                ")";                                                    //// テスト内容文字列を作成する

            M_DoTest(testDescription, fncTestTarget.Target              //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Funcテスト本体
            //------------------------------------------------------------
            TResult CB_TestBody() => fncTestTarget(inArg1);             //// テスト対象Funcを呼び出す
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(通常メソッド、戻り値あり、引数２つ)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：result = instance.Func(param1, param2) 形式のメソッド(文字列操作系など)。<br></br>
        /// </summary>
        /// <typeparam name="TArg1">引数１の型</typeparam>
        /// <typeparam name="TArg2">引数２の型</typeparam>
        /// <typeparam name="TResult">戻り値の型</typeparam>
        /// <param name="fncTestTarget">[in ]：テスト対象Func</param>
        /// <param name="inArg1">       [in ]：引数１</param>
        /// <param name="inArg2">       [in ]：引数２</param>
        /// <param name="expectResult"> [in ]：予想結果</param>
        /// <param name="testOptions">  [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TArg1, TArg2, TResult>(
                                Func<TArg1, TArg2, TResult> fncTestTarget,
                                    TArg1 inArg1, 
                                    TArg2 inArg2,
                                    AutoTestResultInfo<TResult> expectResult,
                                AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription = fncTestTarget.Method.XGetName() + "(" +
                ToDisplayString(inArg1, testOptions.fncArg1RegularString) + ", " +
                ToDisplayString(inArg2, testOptions.fncArg2RegularString) +
                ")";                                                    //// テスト内容文字列を作成する

            M_DoTest(testDescription, fncTestTarget.Target              //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Funcテスト本体
            //------------------------------------------------------------
            TResult CB_TestBody() => fncTestTarget(inArg1, inArg2);     //// テスト対象Funcを呼び出す
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(通常メソッド、戻り値あり、引数３つ)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：result = instance.Func(param1, param2, param3) 形式のメソッド。<br></br>
        /// </summary>
        /// <typeparam name="TArg1">引数１の型</typeparam>
        /// <typeparam name="TArg2">引数２の型</typeparam>
        /// <typeparam name="TArg3">引数３の型</typeparam>
        /// <typeparam name="TResult">戻り値の型</typeparam>
        /// <param name="fncTestTarget">[in ]：テスト対象Func</param>
        /// <param name="inArg1">       [in ]：引数１</param>
        /// <param name="inArg2">       [in ]：引数２</param>
        /// <param name="inArg3">       [in ]：引数３</param>
        /// <param name="expectResult"> [in ]：予想結果</param>
        /// <param name="testOptions">  [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TArg1, TArg2, TArg3, TResult>(
                                Func<TArg1, TArg2, TArg3, TResult> fncTestTarget,
                                    TArg1 inArg1, 
                                    TArg2 inArg2, 
                                    TArg3 inArg3,
                                    AutoTestResultInfo<TResult> expectResult, 
                                AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription = fncTestTarget.Method.XGetName() + "(" +
                ToDisplayString(inArg1, testOptions.fncArg1RegularString) + ", " +
                ToDisplayString(inArg2, testOptions.fncArg2RegularString) + ", " +
                ToDisplayString(inArg3, testOptions.fncArg3RegularString) + 
                ")";                                                    //// テスト内容文字列を作成する

            M_DoTest(testDescription, fncTestTarget.Target              //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Funcテスト本体
            //------------------------------------------------------------
            TResult CB_TestBody()
                => fncTestTarget(inArg1, inArg2, inArg3);               //// テスト対象Funcを呼び出す
        }


        //====================================================================================================
        // 自動テスト実施メソッド(拡張メソッド、戻り値なし用)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(拡張メソッド、戻り値なし、引数１つ)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：target.Func(param1) 形式の拡張メソッド(対象インスタンス操作系など)。<br></br>
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <typeparam name="TArg1">引数１の型</typeparam>
        /// <param name="actTestTarget"> [in ]：テスト対象Action</param>
        /// <param name="targetInstance">[in ]：対象インスタンス</param>
        /// <param name="inArg1">        [in ]：引数１</param>
        /// <param name="expectResult">  [in ]：予想結果</param>
        /// <param name="testOptions">   [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には、予想される実行結果を格納したインスタンス、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void TestX<TTarget, TArg1>(
                                Action<TTarget, TArg1> actTestTarget,
                                    TTarget targetInstance,
                                    TArg1 inArg1,
                                    AutoTestResultInfo<TTarget> expectResult, 
                                AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription = "(拡張メソッド)" + actTestTarget.Method.XGetName() + "(" +
                ToDisplayString(targetInstance, testOptions.fncInstanceRegularString) + ", " +
                ToDisplayString(inArg1, testOptions.fncArg1RegularString) +
                ")";                                                    //// テスト内容文字列を作成する

            M_DoTest(testDescription, targetInstance                    //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Actionテスト本体
            //------------------------------------------------------------
            void CB_TestBody() 
                => actTestTarget(targetInstance, inArg1);               //// テスト対象Actionを呼び出す
        }


        //====================================================================================================
        // 自動テスト実施メソッド(拡張メソッド、戻り値あり用)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(拡張メソッド、戻り値あり、引数なし)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：result = target.Func() 形式の拡張メソッド(文字列操作系など)。<br></br>
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <typeparam name="TResult">戻り値の型</typeparam>
        /// <param name="fncTestTarget"> [in ]：テスト対象Func</param>
        /// <param name="targetInstance">[in ]：対象インスタンス</param>
        /// <param name="expectResult">  [in ]：予想結果</param>
        /// <param name="testOptions">   [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void TestX<TTarget, TResult>(
                                 Func<TTarget, TResult> fncTestTarget,
                                    TTarget targetInstance,
                                    AutoTestResultInfo<TResult> expectResult,
                                 AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription = "(拡張メソッド)" + fncTestTarget.Method.XGetName() + "(" +
                ToDisplayString(targetInstance, testOptions.fncInstanceRegularString) +
                ")";                                                    //// テスト内容文字列を作成する

            M_DoTest(testDescription, targetInstance                    //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Funcテスト本体
            //------------------------------------------------------------
            TResult CB_TestBody() => fncTestTarget(targetInstance);     //// テスト対象Funcを呼び出す
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(拡張メソッド、戻り値あり、引数１つ)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：result = target.Func(param1) 形式の拡張メソッド。<br></br>
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <typeparam name="TArg1">引数１の型</typeparam>
        /// <typeparam name="TResult">戻り値の型</typeparam>
        /// <param name="fncTestTarget"> [in ]：テスト対象Func</param>
        /// <param name="targetInstance">[in ]：対象インスタンス</param>
        /// <param name="inArg1">        [in ]：引数１</param>
        /// <param name="expectResult">  [in ]：予想結果</param>
        /// <param name="testOptions">   [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void TestX<TTarget, TArg1, TResult>(
                                 Func<TTarget, TArg1, TResult> fncTestTarget,
                                    TTarget targetInstance,
                                    TArg1 inArg1,
                                    AutoTestResultInfo<TResult> expectResult,
                                 AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription = "(拡張メソッド)" + fncTestTarget.Method.XGetName() + "(" +
                ToDisplayString(targetInstance, testOptions.fncInstanceRegularString) + ", " +
                ToDisplayString(inArg1, testOptions.fncArg1RegularString) +
                ")";                                                    //// テスト内容文字列を作成する

            M_DoTest(testDescription, targetInstance                    //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Funcテスト本体
            //------------------------------------------------------------
            TResult CB_TestBody() 
                => fncTestTarget(targetInstance, inArg1);               //// テスト対象Funcを呼び出す
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(拡張メソッド、戻り値あり、引数２つ)】メソッドをテストして結果を自動テスト結果リスナーへ通知します。<br></br>
        /// テスト対象メソッド：result = target.Func(param1, param2) 形式の拡張メソッド。<br></br>
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <typeparam name="TArg1">引数１の型</typeparam>
        /// <typeparam name="TArg2">引数２の型</typeparam>
        /// <typeparam name="TResult">戻り値の型</typeparam>
        /// <param name="fncTestTarget"> [in ]：テスト対象Func</param>
        /// <param name="targetInstance">[in ]：対象インスタンス</param>
        /// <param name="inArg1">        [in ]：引数１</param>
        /// <param name="inArg2">        [in ]：引数２</param>
        /// <param name="expectResult">  [in ]：予想結果</param>
        /// <param name="testOptions">   [in ]：テストパターン名またはテストオプション[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void TestX<TTarget, TArg1, TArg2, TResult>(
                                 Func<TTarget, TArg1, TArg2, TResult> fncTestTarget,
                                    TTarget targetInstance,
                                    TArg1 inArg1,
                                    TArg2 inArg2,
                                    AutoTestResultInfo<TResult> expectResult,
                                 AutoTestOptions testOptions = null)
        {
            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (testOptions == null)
            {                                                           //// テストパターン名もテストオプションも指定されなかった場合
                testOptions = new AutoTestOptions();                    /////  テストオプション = 空のテストオプション
            }

            var testDescription = "(拡張メソッド)" + fncTestTarget.Method.XGetName() + "(" +
                ToDisplayString(targetInstance, testOptions.fncInstanceRegularString) + ", " +
                ToDisplayString(inArg1, testOptions.fncArg1RegularString) + ", " +
                ToDisplayString(inArg2, testOptions.fncArg2RegularString) +
                ")";                                                    //// テスト内容文字列を作成する

            M_DoTest(testDescription, targetInstance                    //// テスト実行処理を行う
                , expectResult, testOptions, CB_TestBody);

            //------------------------------------------------------------
            /// 【ローカル関数】Funcテスト本体
            //------------------------------------------------------------
            TResult CB_TestBody()                                       //// テスト対象Funcを呼び出す
                => fncTestTarget(targetInstance, inArg1, inArg2);
        }


        //====================================================================================================
        // 自動テスト実施用内部メソッド
        //====================================================================================================

        // ＜メモ＞
        // ・引数の数によって異なる部分(テスト内容文字列を作成するロジックと、テスト本体を実行するロジック)以外の共通部分。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト実行(戻り値なし版)】戻り値なしのメソッドをテストします。検証対象は TTarget型のインスタンスです。
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <param name="testDescription">[in ]：テスト内容文字列</param>
        /// <param name="targetInstance"> [in ]：対象インスタンス(通常メソッドの場合は fncTestTarget.Target)</param>
        /// <param name="expectResult">   [in ]：予想結果</param>
        /// <param name="testOptions">    [in ]：テストオプション</param>
        /// <param name="actTestBody">    [in ]：Actionテスト本体</param>
        //--------------------------------------------------------------------------------
        private static void M_DoTest<TTarget>(
            string testDescription,
            TTarget targetInstance, AutoTestResultInfo<TTarget> expectResult,
            AutoTestOptions testOptions,
            Action actTestBody)
        {
            AutoTestResultInfo<TTarget> execResult; // 実行結果(TTarget型のインスタンス または 例外の型情報)
            AutoTestResultKind resultKind;          // 自動テスト結果
            string exceptionMessage = null;         // 例外メッセージ
            string befContent = null;               // 実行前のインスタンス内容文字列
            string aftContent = null;               // 実行後のインスタンス内容文字列


            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (targetInstance != null)
            {                                                           //// 対象インスタンス = null でない場合(静的メソッドでない場合)
                befContent =                                            /////  実行前のインスタンス内容文字列を取得する
                    ToDisplayString(targetInstance, testOptions.fncInstanceRegularString);
            }

            try
            {                                                           //// try開始
                actTestBody();                                          /////  Actionテスト本体を呼び出す
                execResult = targetInstance;                            /////  実行結果 = 対象インスタンス
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外
                execResult = ex.GetType();                              /////  実行結果 = 例外の型情報
                exceptionMessage = ex.Message;                          /////  例外メッセージを取得する
            }

            if (targetInstance != null)
            {                                                           //// 対象インスタンス = null でない場合(静的メソッドでない場合)
                aftContent =                                            /////  実行後のインスタンス内容文字列を取得する
                    ToDisplayString(targetInstance, testOptions.fncInstanceRegularString);
            }


            //------------------------------------------------------------
            /// テスト結果を判定し、自動テスト結果リスナーへ通知する
            //------------------------------------------------------------
            if (expectResult.IsEqual(execResult))
            {                                                           //// 実行結果と予想結果が一致する場合
                resultKind = AutoTestResultKind.Succeeded;              /////  自動テスト結果 = 成功
            }
            else
            {                                                           //// 実行結果と予想結果が一致しない場合
                resultKind = AutoTestResultKind.Failed;                 /////  自動テスト結果 = 失敗
            }


            foreach (var tmpListener in m_ifListeners)
            {                                                           //// 自動テスト結果リスナーリストを繰り返す
                tmpListener.NoticeTestResult(                           /////  テスト結果通知処理を行う
                    resultKind, testDescription, testOptions.testPattern,
                    ToDisplayString(execResult.value, testOptions.fncResultRegularString),
                    ToDisplayString(expectResult.value, testOptions.fncResultRegularString),
                    exceptionMessage,
                    befContent, aftContent);
            }
        }


        // ＜メモ＞
        // ・引数の数によって異なる部分(テスト内容文字列を作成するロジックと、テスト本体を実行するロジック)以外の共通部分。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト実行(戻り値あり版)】戻り値ありのメソッドをテストします。検証対象は TResult型の戻り値です。
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <typeparam name="TResult">戻り値の型</typeparam>
        /// <param name="testDescription">[in ]：テスト内容文字列</param>
        /// <param name="targetInstance"> [in ]：対象インスタンス(通常メソッドの場合は fncTestTarget.Target)</param>
        /// <param name="expectResult">   [in ]：予想結果</param>
        /// <param name="testOptions">    [in ]：テストオプション</param>
        /// <param name="fncTestBody">    [in ]：Funcテスト本体</param>
        //--------------------------------------------------------------------------------
        private static void M_DoTest<TTarget, TResult>(
            string testDescription,
            TTarget targetInstance, AutoTestResultInfo<TResult> expectResult,
            AutoTestOptions testOptions,
            Func<TResult> fncTestBody)
        {
            AutoTestResultInfo<TResult> execResult; // 実行結果(TResult型の戻り値 または 例外の型情報)
            AutoTestResultKind resultKind;          // 自動テスト結果
            string exceptionMessage = null;         // 例外メッセージ
            string befContent = null;               // 実行前のインスタンス内容文字列
            string aftContent = null;               // 実行後のインスタンス内容文字列


            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            if (targetInstance != null)
            {                                                           //// 対象インスタンス = null でない場合(静的メソッドでない場合)
                befContent =                                            /////  実行前のインスタンス内容文字列を取得する
                    ToDisplayString(targetInstance, testOptions.fncInstanceRegularString);
            }

            try
            {                                                           //// try開始
                execResult = fncTestBody();                             /////  Funcテスト本体を呼び出し、戻り値を実行結果に格納する
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外
                execResult = ex.GetType();                              /////  実行結果 = 例外の型情報
                exceptionMessage = ex.Message;                          /////  例外メッセージを取得する
            }

            if (targetInstance != null)
            {                                                           //// 対象インスタンス = null でない場合(静的メソッドでない場合)
                aftContent =                                            /////  実行後のインスタンス内容文字列を取得する
                    ToDisplayString(targetInstance, testOptions.fncInstanceRegularString);
            }


            //------------------------------------------------------------
            /// テスト結果を判定し、自動テスト結果リスナーへ通知する
            //------------------------------------------------------------
            if (expectResult.IsEqual(execResult))
            {                                                           //// 実行結果と予想結果が一致する場合
                resultKind = AutoTestResultKind.Succeeded;              /////  自動テスト結果 = 成功
            }
            else
            {                                                           //// 実行結果と予想結果が一致しない場合
                resultKind = AutoTestResultKind.Failed;                 /////  自動テスト結果 = 失敗
            }

            foreach (var tmpListener in m_ifListeners)
            {                                                           //// 自動テスト結果リスナーリストを繰り返す
                tmpListener.NoticeTestResult(                          /////  テスト結果通知処理を行う
                    resultKind, testDescription, testOptions.testPattern,
                    ToDisplayString(execResult.value, testOptions.fncResultRegularString),
                    ToDisplayString(expectResult.value, testOptions.fncResultRegularString),
                    exceptionMessage,
                    befContent, aftContent);
            }
        }


        //====================================================================================================
        // ユーティリティーメソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【表示用文字列化】オブジェクトからテスト結果表示用文字列を作成します。
        /// </summary>
        /// <param name="target">          [in ]：対象オブジェクト</param>
        /// <param name="fncRegularString">[in ]：定型文字列作成関数(null = なし)</param>
        /// <returns>
        /// テスト結果表示用文字列
        /// <code>
        /// ①null値の場合は "&lt;null&gt;"
        /// ②対象オブジェクトが型情報の場合は型情報の名前
        /// ③定型文字列作成関数が指定されている場合は定型文字列を作成。作成失敗時(float型に対して16進数文字列作成を指定した場合など)は次へ
        /// ④上記以外の場合は、オブジェクトの種類に応じて作成
        /// 
        ///   オブジェクトの種類：テスト結果表示用文字列の書式           ：例
        ///   ------------------：---------------------------------------：---------
        ///   string型          ："＜文字列＞":＜文字数＞                ："ABCあいう":6
        ///   StringBuilder型   ："＜文字列＞":＜文字数＞                ："ABCあいう":6
        ///   char型            ：'＜文字＞'                             ：'A'
        ///   コレクション      ：{＜要素１＞, ＜要素２＞, … ＜要素ｎ＞}：{123, 456, -1}
        ///   上記以外          ：ToString() の結果
        /// </code>
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string ToDisplayString(object target, RegularStringFunc fncRegularString = null)
        {
            //------------------------------------------------------------
            /// ①対象オブジェクトが null の場合は「＜null＞」を返す
            //------------------------------------------------------------
            if (target is null)
            {                                                           //// 対象オブジェクト = null の場合
                return "<null>";                                        /////  戻り値 = "<null>" で関数終了
            }


            //------------------------------------------------------------
            /// ②対象オブジェクトが型情報の場合は型情報の名前を返す
            //------------------------------------------------------------
            if (target is Type)
            {                                                           //// 対象オブジェクトが型情報の場合
                return target.ToString();                               /////  対象オブジェクトを文字列化して戻り値とし、関数終了
            }


            //------------------------------------------------------------
            /// ③定型文字列作成関数が指定されている場合は定型文字列を作成して返す
            //------------------------------------------------------------
            if (fncRegularString != null)
            {                                                           //// 定型文字列作成関数が指定されている場合(null でない場合)
                try
                {                                                       /////  try開始
                    return fncRegularString(target);                    //////   定型文字列を作成して戻り値とし、関数終了
                }
                catch { }                                               /////  catch：すべての例外。作成失敗時は次へ(float型に対して16進数文字列作成を指定した場合など)
            }


            //------------------------------------------------------------
            /// ④オブジェクトの種類に応じてテスト結果表示用文字列を作成する
            //------------------------------------------------------------
            if (target is string strValue)
            {                                                           //// 対象オブジェクトが string 型の場合
                return "\"" + strValue.XEscape(EscapeConverter.CcVisualization) + "\":" +
                       strValue.Length;                                 /////  string 型用に表示用文字列を作成して戻り値とし、関数終了
            }

            if (target is StringBuilder strBuilder)
            {                                                           //// 対象オブジェクトが StringBuilder 型の場合
                return "\"" + strBuilder.ToString().XEscape(EscapeConverter.CcVisualization) + "\":" +
                       strBuilder.Length;                               /////  StringBuilder 型用に表示用文字列を作成して戻り値とし、関数終了
            }

            if (target is char charValue)
            {                                                           //// 対象オブジェクトが char 型の場合
                return "'" + charValue + "'";                           /////  char 型用に表示用文字列を作成して戻り値とし、関数終了
            }

            if (target is IEnumerable)
            {                                                           //// 対象オブジェクトが IEnumerable I/F を持つコレクションの場合
                var itemStrs = new Collection<string>();                /////  要素文字列コレクションを生成する

                foreach (var item in target as IEnumerable)
                {                                                       /////  コレクションの全要素を繰り返す
                    itemStrs.Add(ToDisplayString(item));                //////   コレクション要素に対する表示用文字列を取得する
                }

                return "{" + string.Join(", ", itemStrs) + "}:" +
                       itemStrs.Count;                                  /////  コレクション用に表示用文字列を作成して戻り値とし、関数終了
            }

            return target.ToString();                                   //// 上記以外の場合、対象オブジェクトを文字列化して戻り値とし、関数終了
        }


        //[-] 保留：適切なテスト対象メソッドがないパターンは保留中
        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド自身の自動テスト
        //--------------------------------------------------------------------------------
        [AutoTestMethod("AutoTest 自身の総合的テスト")]
        public static void ZZZ_AutoTestSelf()
        {
            const string REM_STRING = "(実行前後で内容が変わらないのは String クラスの仕様なのでOK)";

            //--------------- 0....*....1....*....2....*....3
            string testStr = "AutoTest for AutoTest";
            ZZZRefString refString;


            //------------------------------------------------------------
            AutoTest.Print("＜戻り値ありの通常メソッド＞");

            // 正常終了パターン
            AutoTest.Test(testStr.ToUpper, "AUTOTEST FOR AUTOTEST", "戻り値あり、引数なし" + REM_STRING);
            AutoTest.Test(testStr.Substring, 9, "for AutoTest", "戻り値あり、引数１つ" + REM_STRING);
            AutoTest.Test(testStr.Insert, 13, "The ", "AutoTest for The AutoTest", "戻り値あり、引数２つ" + REM_STRING);

            // 例外発生パターン
            // ・保留：引数なしの例外発生パターン
            AutoTest.Test(testStr.Substring, 99, typeof(ArgumentOutOfRangeException), "戻り値あり、引数１つ、引数不正例外");
            AutoTest.Test(testStr.Insert, 99, "The ", typeof(ArgumentOutOfRangeException), "戻り値あり、引数２つ、引数不正例外");


            //------------------------------------------------------------
            AutoTest.Print("");
            AutoTest.Print("＜戻り値なしの通常メソッド＞");

            // 正常終了パターン
            ResetInstance(); AutoTest.Test(refString, refString.ToUpper, (ZZZRefString)"AUTOTEST FOR AUTOTEST", "戻り値なし、引数なし");
            ResetInstance(); AutoTest.Test(refString, refString.SubString, 9, (ZZZRefString)"for AutoTest", "戻り値なし、引数１つ");
            ResetInstance(); AutoTest.Test(refString, refString.Insert, 13, "The ", (ZZZRefString)"AutoTest for The AutoTest", "戻り値なし、引数２つ");

            // 例外発生パターン
            // ・保留：引数なしの例外発生パターン
            ResetInstance(); AutoTest.Test(refString, refString.SubString, 99, typeof(ArgumentOutOfRangeException), "戻り値なし、引数１つ、引数不正例外");
            ResetInstance(); AutoTest.Test(refString, refString.Insert, 99, "The ", typeof(ArgumentOutOfRangeException), "戻り値なし、引数２つ、引数不正例外");


            // 保留：戻り値なしの拡張メソッド


            //------------------------------------------------------------
            AutoTest.Print("");
            AutoTest.Print("＜戻り値ありの拡張メソッド＞");

            // 正常終了パターン
            AutoTest.TestX(XString.XIsValid, testStr, true, "拡張メソッド、戻り値あり、引数なし" + REM_STRING);
            AutoTest.TestX(XString.XLeft, testStr, 5, "AutoT", "拡張メソッド、戻り値あり、引数１つ" + REM_STRING);
            // ・保留：引数２つの正常終了パターン

            // 例外発生パターン
            // ・保留：引数なしの例外発生パターン
            AutoTest.TestX(XString.XLeft, testStr, -1, typeof(ArgumentOutOfRangeException), "拡張メソッド、戻り値あり、引数１つ、引数不正例外");
            // ・保留：引数２つの例外発生パターン


            void ResetInstance()
            {
                //---------- 0....*....1....*....2....*....3
                refString = "AutoTest for AutoTest";
            }
        }

    } // class




    // ＜メモ＞
    // ・AutoTest で戻り値なしパターンのテストをしたい
    //   しかし：インスタンスの内容を操作するメソッドは、インスタンス自身を戻り値として返すことが多いため、戻り値なしパターンのテストに適したメソッドがなかなか見当たらない
    //   なので：テスト用クラスを作成した
    //====================================================================================================
    // AutoTest の自動テスト用クラス：参照型の(インスタンス自身の内容を直接書き換える)文字列型
    //====================================================================================================
    public class ZZZRefString
    {
        protected string m_content;

        public override string ToString() => AutoTest.ToDisplayString(m_content);

        public override int GetHashCode() => m_content.GetHashCode();

        public override bool Equals(object other) => Equals((ZZZRefString)other);

        public bool Equals(ZZZRefString other) => m_content.Equals(other.m_content);

        public ZZZRefString() => m_content = null;
        public ZZZRefString(string value) => m_content = value;

        public static implicit operator ZZZRefString(string value) => new ZZZRefString(value);

        // 公開メソッド
        public void ToUpper() => m_content = m_content.ToUpper();
        public void SubString(int startIndex) => m_content = m_content.Substring(startIndex);
        public void Insert(int startIndex, string value) => m_content = m_content.Insert(startIndex, value);
    } // class

} // namespace
#endif
