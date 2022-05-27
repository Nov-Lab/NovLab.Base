// @(h)AutoTest.cs ver 0.00 ( '22.04.22 Nov-Lab ) 作成開始
// @(h)AutoTest.cs ver 0.21 ( '22.05.18 Nov-Lab ) アルファ版完成
// @(h)AutoTest.cs ver 0.22 ( '22.05.19 Nov-Lab ) 機能修正：コレクションI/Fに対応した(AutoTest.ToDisplayString, AutoTestResultInfo.Matches)
// @(h)AutoTest.cs ver 0.23 ( '22.05.24 Nov-Lab ) 機能修正：例外メッセージも通知するようにした

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


namespace NovLab.DebugSupport
{

#if DEBUG   // DEBUGビルドのみ使用可能
    //====================================================================================================
    /// <summary>
    /// 【自動テスト実行者I/F】自動テストを実行するクラスに必要な機能を定義します。
    /// </summary>
    //====================================================================================================
    public interface IAutoTestExecuter
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果通知】テスト結果を自動テスト実行者に通知します。
        /// </summary>
        /// <param name="autoTestResult">  [in ]：自動テスト結果種別</param>
        /// <param name="testDescription"> [in ]：テスト内容文字列</param>
        /// <param name="testPattern">     [in ]：テストパターン名[null = 省略]</param>
        /// <param name="execResult">      [in ]：実行結果(戻り値 または 例外の型情報)</param>
        /// <param name="expectResult">    [in ]：予想結果(戻り値 または 例外の型情報)</param>
        /// <param name="exceptionMessage">[in ]：例外メッセージ</param>
        //--------------------------------------------------------------------------------
        void NoticeTestResult(AutoTestResultKind autoTestResult,
                              string testDescription, string testPattern,
                              object execResult, object expectResult,
                              string exceptionMessage);
    }


    //====================================================================================================
    /// <summary>
    /// 【自動テスト】メソッドの単体テストを自動化する機能を提供します。
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・テストデータとテストコードを常備しておくことで、いつでも誰でも何度でも同じテストを実行することができ、
    ///   ロジック修正時などのテストを効率化できます。<br></br>
    /// </remarks>
    //====================================================================================================
    public static class AutoTest
    {
        //====================================================================================================
        // テスト実施メソッド(関数用)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(引数１つの関数)】メソッドをテストして結果を自動テスト実行者I/Fへ通知します。<br></br>
        /// テスト対象メソッド：result = Func(param1) 形式のメソッド(文字列判定系など)。<br></br>
        /// </summary>
        /// <typeparam name="TParam1">引数１の型</typeparam>
        /// <param name="testTarget">  [in ]：テスト対象メソッド</param>
        /// <param name="inParam1">    [in ]：引数１</param>
        /// <param name="expectResult">[in ]：予想結果</param>
        /// <param name="ifExecuter">  [in ]：自動テスト実行者I/F</param>
        /// <param name="testPattern"> [in ]：テストパターン名[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TParam1, TResult>(Func<TParam1, TResult> testTarget,
                                TParam1 inParam1,
                                AutoTestResultInfo<TResult> expectResult, IAutoTestExecuter ifExecuter,
                                string testPattern = null)
        {
            AutoTestResultInfo<TResult> execResult; // 実行結果(TResult型の戻り値 または 例外の型情報)
            AutoTestResultKind resultKind;          // 自動テスト結果
            string exceptionMessage = null;         // 例外メッセージ


            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            var testDescription = testTarget.Method.XGetName() + "(" +
                ToDisplayString(inParam1) + ")";                        //// テスト内容文字列を作成する

            try
            {                                                           //// try開始
                execResult = testTarget(inParam1);                      /////  テスト対象メソッドを実行し、実行結果 = 戻り値 とする
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外
                execResult = ex.GetType();                              /////  実行結果 = 例外の型情報
                exceptionMessage = ex.Message;                          /////  例外メッセージを取得する
            }


            //------------------------------------------------------------
            /// テスト結果を判定し、自動テスト実行者I/Fへ通知する
            //------------------------------------------------------------
            if (expectResult.IsEqual(execResult))
            {                                                           //// 実行結果と予想結果が一致する場合
                resultKind = AutoTestResultKind.Succeeded;              /////  自動テスト結果 = 成功
            }
            else
            {                                                           //// 実行結果と予想結果が一致しない場合
                resultKind = AutoTestResultKind.Failed;                 /////  自動テスト結果 = 失敗
            }

            ifExecuter.NoticeTestResult(                                //// テスト結果を通知する
                resultKind, testDescription, testPattern,
                execResult.value, expectResult.value, exceptionMessage);
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(引数２つの関数)】メソッドをテストして結果を自動テスト実行者I/Fへ通知します。<br></br>
        /// テスト対象メソッド：result = Func(param1, param2) 形式のメソッド(文字列操作系など)。<br></br>
        /// </summary>
        /// <typeparam name="TParam1">引数１の型</typeparam>
        /// <typeparam name="TParam2">引数２の型</typeparam>
        /// <param name="testTarget">  [in ]：テスト対象メソッド</param>
        /// <param name="inParam1">    [in ]：引数１</param>
        /// <param name="inParam2">    [in ]：引数２</param>
        /// <param name="expectResult">[in ]：予想結果</param>
        /// <param name="ifExecuter">  [in ]：自動テスト実行者I/F</param>
        /// <param name="testPattern"> [in ]：テストパターン名[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TParam1, TParam2, TResult>(Func<TParam1, TParam2, TResult> testTarget,
                                TParam1 inParam1, TParam2 inParam2,
                                AutoTestResultInfo<TResult> expectResult, IAutoTestExecuter ifExecuter,
                                string testPattern = null)
        {
            AutoTestResultInfo<TResult> execResult; // 実行結果(TResult型の戻り値 または 例外の型情報)
            AutoTestResultKind resultKind;          // 自動テスト結果
            string exceptionMessage = null;         // 例外メッセージ


            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            var testDescription = testTarget.Method.XGetName() + "(" +
                ToDisplayString(inParam1) + ", " +
                ToDisplayString(inParam2) + ")";                        //// テスト内容文字列を作成する

            try
            {                                                           //// try開始
                execResult = testTarget(inParam1, inParam2);            /////  テスト対象メソッドを実行し、実行結果 = 戻り値 とする
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外
                execResult = ex.GetType();                              /////  実行結果 = 例外の型情報
                exceptionMessage = ex.Message;                          /////  例外メッセージを取得する
            }


            //------------------------------------------------------------
            /// テスト結果を判定し、自動テスト実行者I/Fへ通知する
            //------------------------------------------------------------
            if (expectResult.IsEqual(execResult))
            {                                                           //// 実行結果と予想結果が一致する場合
                resultKind = AutoTestResultKind.Succeeded;              /////  自動テスト結果 = 成功
            }
            else
            {                                                           //// 実行結果と予想結果が一致しない場合
                resultKind = AutoTestResultKind.Failed;                 /////  自動テスト結果 = 失敗
            }

            ifExecuter.NoticeTestResult(                                //// テスト結果を通知する
                resultKind, testDescription, testPattern,
                execResult.value, expectResult.value, exceptionMessage);
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(引数３つの関数)】メソッドをテストして結果を自動テスト実行者I/Fへ通知します。<br></br>
        /// テスト対象メソッド：result = Func(param1, param2, param3) 形式のメソッド。<br></br>
        /// </summary>
        /// <typeparam name="TParam1">引数１の型</typeparam>
        /// <typeparam name="TParam2">引数２の型</typeparam>
        /// <typeparam name="TParam3">引数３の型</typeparam>
        /// <param name="testTarget">  [in ]：テスト対象メソッド</param>
        /// <param name="inParam1">    [in ]：引数１</param>
        /// <param name="inParam2">    [in ]：引数２</param>
        /// <param name="inParam3">    [in ]：引数３</param>
        /// <param name="expectResult">[in ]：予想結果</param>
        /// <param name="ifExecuter">  [in ]：自動テスト実行者I/F</param>
        /// <param name="testPattern"> [in ]：テストパターン名[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void Test<TParam1, TParam2, TParam3, TResult>(Func<TParam1, TParam2, TParam3, TResult> testTarget,
                                TParam1 inParam1, TParam2 inParam2, TParam3 inParam3,
                                AutoTestResultInfo<TResult> expectResult, IAutoTestExecuter ifExecuter,
                                string testPattern = null)
        {
            AutoTestResultInfo<TResult> execResult; // 実行結果(TResult型の戻り値 または 例外の型情報)
            AutoTestResultKind resultKind;          // 自動テスト結果
            string exceptionMessage = null;         // 例外メッセージ


            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            var testDescription = testTarget.Method.XGetName() + "(" +
                ToDisplayString(inParam1) + ", " +
                ToDisplayString(inParam2) + ", " +
                ToDisplayString(inParam3) + ")";                        //// テスト内容文字列を作成する

            try
            {                                                           //// try開始
                execResult = testTarget(inParam1, inParam2, inParam3);  /////  テスト対象メソッドを実行し、実行結果 = 戻り値 とする
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外
                execResult = ex.GetType();                              /////  実行結果 = 例外の型情報
                exceptionMessage = ex.Message;                          /////  例外メッセージを取得する
            }


            //------------------------------------------------------------
            /// テスト結果を判定し、自動テスト実行者I/Fへ通知する
            //------------------------------------------------------------
            if (expectResult.IsEqual(execResult))
            {                                                           //// 実行結果と予想結果が一致する場合
                resultKind = AutoTestResultKind.Succeeded;              /////  自動テスト結果 = 成功
            }
            else
            {                                                           //// 実行結果と予想結果が一致しない場合
                resultKind = AutoTestResultKind.Failed;                 /////  自動テスト結果 = 失敗
            }

            ifExecuter.NoticeTestResult(                                //// テスト結果を通知する
                resultKind, testDescription, testPattern,
                execResult.value, expectResult.value, exceptionMessage);
        }


        //====================================================================================================
        // テスト実施メソッド(拡張メソッド用)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト(引数１つ、戻り値なしの拡張メソッド)】メソッドをテストして結果を自動テスト実行者I/Fへ通知します。<br></br>
        /// テスト対象メソッド：target.Func(param1) 形式の拡張メソッド(対象インスタンス操作系など)。<br></br>
        /// </summary>
        /// <typeparam name="TTarget">対象インスタンスの型</typeparam>
        /// <typeparam name="TParam1">引数１の型</typeparam>
        /// <param name="testTarget">  [in ]：テスト対象メソッド</param>
        /// <param name="target">      [in ]：対象インスタンス</param>
        /// <param name="inParam1">    [in ]：引数１</param>
        /// <param name="expectResult">[in ]：予想結果</param>
        /// <param name="ifExecuter">  [in ]：自動テスト実行者I/F</param>
        /// <param name="testPattern"> [in ]：テストパターン名[null = 省略]</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・予想結果には TResult型の戻り値、または例外の型情報を指定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [Conditional("DEBUG")]
        public static void TestX<TTarget, TParam1>(Action<TTarget, TParam1> testTarget,
                                 TTarget target, TParam1 inParam1,
                                 AutoTestResultInfo<TTarget> expectResult, IAutoTestExecuter ifExecuter,
                                 string testPattern = null)
        {
            AutoTestResultInfo<TTarget> execResult; // 実行結果(TResult型の戻り値 または 例外の型情報)
            AutoTestResultKind resultKind;          // 自動テスト結果
            string exceptionMessage = null;         // 例外メッセージ


            //------------------------------------------------------------
            /// メソッドをテストする
            //------------------------------------------------------------
            var testDescription = "(拡張メソッド)" + testTarget.Method.XGetName() + "(" +
                ToDisplayString(target) + ", " +
                ToDisplayString(inParam1) + ")";                        //// テスト内容文字列を作成する

            try
            {                                                           //// try開始
                testTarget(target, inParam1);                           /////  テスト対象メソッドを実行する
                execResult = target;                                    /////  実行結果 = 対象インスタンス
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外
                execResult = ex.GetType();                              /////  実行結果 = 例外の型情報
                exceptionMessage = ex.Message;                          /////  例外メッセージを取得する
            }


            //------------------------------------------------------------
            /// テスト結果を判定し、自動テスト実行者I/Fへ通知する
            //------------------------------------------------------------
            if (expectResult.IsEqual(execResult))
            {                                                           //// 実行結果と予想結果が一致する場合
                resultKind = AutoTestResultKind.Succeeded;              /////  自動テスト結果 = 成功
            }
            else
            {                                                           //// 実行結果と予想結果が一致しない場合
                resultKind = AutoTestResultKind.Failed;                 /////  自動テスト結果 = 失敗
            }

            ifExecuter.NoticeTestResult(                                //// テスト結果を通知する
                resultKind, testDescription, testPattern,
                execResult.value, expectResult.value, exceptionMessage);
        }


        //====================================================================================================
        // ユーティリティーメソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【表示用文字列化】オブジェクトからテスト結果表示用文字列を作成します。
        /// </summary>
        /// <param name="obj">[in ]：オブジェクト</param>
        /// <returns>
        /// 表示用文字列<br></br>
        /// <code>
        /// オブジェクトの種類：テスト結果表示用文字列の書式           ：例
        /// ------------------：---------------------------------------：---------
        /// string型          ："＜文字列＞":＜文字数＞                ："ABCあいう":6
        /// char型            ：'＜文字＞'                             ：'A'
        /// 配列              ：{＜要素１＞, ＜要素２＞, … ＜要素ｎ＞}：{123, 456, -1}
        /// null値            ："&lt;null&gt;"
        /// 上記以外          ：ToString() の結果
        /// </code>
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string ToDisplayString(object obj)
        {
            //------------------------------------------------------------
            /// オブジェクトからテスト結果表示用文字列を作成する
            //------------------------------------------------------------
            if (obj is null)
            {                                                           //// オブジェクト = null の場合
                return "<null>";                                        /////  戻り値 = "<null>" で関数終了
            }

            if (obj is string strValue)
            {                                                           //// オブジェクトが string 型の場合
                return "\"" + strValue.XEscape(EscapeConverter.CcVisualization) + "\":" +
                       strValue.Length;                                 /////  string 型用に表示用文字列を作成して戻り値とし、関数終了
            }

            if (obj is StringBuilder strBuilder)
            {                                                           //// オブジェクトが StringBuilder 型の場合
                return "\"" + strBuilder.ToString().XEscape(EscapeConverter.CcVisualization) + "\":" +
                       strBuilder.Length;                               /////  StringBuilder 型用に表示用文字列を作成して戻り値とし、関数終了
            }

            if (obj is char charValue)
            {                                                           //// オブジェクトが char 型の場合
                return "'" + charValue + "'";                           /////  char 型用に表示用文字列を作成して戻り値とし、関数終了
            }

            if (obj is IEnumerable)
            {                                                           //// オブジェクトが IEnumerable I/F を持つコレクションの場合
                var itemStrs = new Collection<string>();                /////  要素文字列コレクションを生成する

                foreach (var item in obj as IEnumerable)
                {                                                       /////  コレクションの全要素を繰り返す
                    itemStrs.Add(ToDisplayString(item));                //////   コレクション要素に対する表示用文字列を取得する
                }

                return "{" + string.Join(", ", itemStrs) + "}:" +
                       itemStrs.Count;                                  /////  コレクション用に表示用文字列を作成して戻り値とし、関数終了
            }

            return obj.ToString();                                      //// 上記以外の場合、オブジェクトを文字列化して戻り値とし、関数終了
        }

    }


    //====================================================================================================
    /// <summary>
    /// 【自動テスト結果種別】自動テストの結果を示します。
    /// </summary>
    /// <remarks>
    /// 補足<br></br>
    /// ・成功 or 失敗 以外の結果を返すパターンに備えて列挙値にしています。<br></br>
    /// </remarks>
    //====================================================================================================
    public enum AutoTestResultKind
    {
        /// <summary>
        /// 【テスト結果：成功】
        /// </summary>
        Succeeded = 0,

        /// <summary>
        /// 【テスト結果：失敗】
        /// </summary>
        Failed = -1,
    }


    //====================================================================================================
    /// <summary>
    /// 【テスト結果情報】テスト用メソッドの実行結果(TResult型の戻り値 または 例外の型情報)を管理します。
    /// </summary>
    /// <typeparam name="TResult">戻り値の型</typeparam>
    /// <remarks>
    /// 補足<br></br>
    /// ・戻り値またはスローされた例外が予想結果と合致するかどうかをチェックするために使用します。<br></br>
    /// </remarks>
    //====================================================================================================
    public class AutoTestResultInfo<TResult>
    {
        //====================================================================================================
        // 公開フィールド
        //====================================================================================================

        /// <summary>
        /// 【結果情報値(読み取り専用)】TResult型の戻り値 または 例外の型情報。
        /// </summary>
        public readonly object value;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列化】このインスタンスの内容を文字列形式に変換します。
        /// </summary>
        /// <returns>文字列形式</returns>
        //--------------------------------------------------------------------------------
        public override string ToString() => value.ToString();


        //====================================================================================================
        // コンストラクター・変換演算子
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【戻り値指定コンストラクター】TResult型の戻り値から結果情報を生成します。
        /// </summary>
        /// <param name="result">[in ]：戻り値</param>
        //--------------------------------------------------------------------------------
        public AutoTestResultInfo(TResult result) => value = result;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【例外型情報指定コンストラクター】例外の型情報から結果情報を生成します。
        /// </summary>
        /// <param name="exceptionTypeInfo">[in ]：例外の型情報</param>
        //--------------------------------------------------------------------------------
        public AutoTestResultInfo(Type exceptionTypeInfo) => value = exceptionTypeInfo;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【変換演算子(戻り値 -> 結果情報)】TResult型の戻り値を、結果情報に変換します。
        /// </summary>
        /// <param name="result">[in ]：戻り値</param>
        //--------------------------------------------------------------------------------
        public static implicit operator AutoTestResultInfo<TResult>(TResult result) => new AutoTestResultInfo<TResult>(result);


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【変換演算子(例外の型情報 -> 結果情報)】例外の型情報を、結果情報に変換します。
        /// </summary>
        /// <param name="exceptionTypeInfo">[in ]：例外の型情報</param>
        //--------------------------------------------------------------------------------
        public static implicit operator AutoTestResultInfo<TResult>(Type exceptionTypeInfo) => new AutoTestResultInfo<TResult>(exceptionTypeInfo);


        //====================================================================================================
        // 公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【一致チェック】２つの結果情報が一致するかどうかをチェックします。
        /// </summary>
        /// <param name="other">[in ]：比較相手</param>
        /// <returns>
        /// チェック結果[true = 一致 / false = 不一致]
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・Object.Equals(Object, Object) と混同しないようにあえて名前を変えています。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public bool IsEqual(AutoTestResultInfo<TResult> other)
        {
            //------------------------------------------------------------
            /// ２つの結果情報が一致するかどうかをチェックする
            //------------------------------------------------------------
            if (value is ICollection && other.value is ICollection)
            {                                                           //// 双方ともコレクションI/Fを持つ場合
                return XICollection.IsEqual(value as ICollection,       /////  コレクション一致チェックを行い、その結果を戻り値として関数終了
                                            other.value as ICollection);
            }

            return Equals(value, other.value);                          //// オブジェクト一致チェックを行い、その結果を戻り値として関数終了
        }

    }
#endif

}
