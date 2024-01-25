// @(h)TestTaskException.cs ver 0.00 ( '24.01.25 Nov-Lab ) 作成開始
// @(h)TestTaskException.cs ver 0.51 ( '24.01.26 Nov-Lab ) ベータ版完成

// @(s)
// 　【Task クラスの例外テスト】非同期タスク処理内で未ハンドル例外が発生した場合の挙動を検証するためのテストクラスです。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using NovLab;
using NovLab.Threading;


namespace Test_NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【Task クラスの例外テスト】非同期タスク処理内で未ハンドル例外が発生した場合の挙動を検証するためのテストクラスです。
    /// </summary>
    //====================================================================================================
    public class TestTaskException
    {
        //====================================================================================================
        /// <summary>
        /// 【検証結果】例外処理方法と待機方法により、実装方法を以下のようにします。
        /// <code>
        /// タスク内での未ハンドル例外時：呼び出し元での待機：実装方法概要
        /// ----------------------------：------------------：------------------------------------------------
        /// ①（中断せずに続行となる）  ：待機しない        ：タスクを開始した後は、待機せずに呼び出し元メソッドを終了する。
        /// ②中断せずに続行したい      ：同期的に待機したい：mytask.RunSynchronously で実行する。
        /// ③中断せずに続行したい      ：非同期で待機したい：mytask.IsCompleted をチェックしながら Task.Delay で非同期的に待機する。
        /// ④呼び出し元でキャッチしたい：同期的に待機したい：mytask.Wait で待機する。 例外を呼び出し元でキャッチしない場合、mytask.Wait の場所で中断される。
        /// ⑤呼び出し元でキャッチしたい：非同期で待機したい：await mytask で待機する。例外を呼び出し元でキャッチしない場合、Program.cs の Applicaton.Run の場所で中断される。
        /// </code>
        /// 「非同期タスク処理側のデバッグがしやすいように、非同期タスク処理内で未ハンドル例外が発生したときは即座にその場で中断したい」
        /// という場合のために、<see cref="AsyncMethod"/> クラスを作りました。
        /// </summary>
        //====================================================================================================
        public void VerificationResult() { }


        //====================================================================================================
        // テスト用フォームとのやり取り用
        //====================================================================================================

        //================================================================================
        /// <summary>
        /// 【メッセージリスナーI/F】メッセージを受け取るリスナーに必要な機能を定義します。
        /// </summary>
        //================================================================================
        public interface IMessageListener
        {
            //----------------------------------------------------------------------
            /// <summary>
            /// 【メッセージ書き込み】メッセージリスナーにメッセージを書き込みます。
            /// </summary>
            /// <param name="message">[in ]：メッセージ文字列</param>
            //----------------------------------------------------------------------
            void WriteLine(string message);
        }


        /// <summary>
        /// 【メッセージリスナー】
        /// </summary>
        public static IMessageListener MessageListener { get; set; } = null;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メッセージ出力】
        /// </summary>
        /// <param name="message">[in ]：メッセージ文字列</param>
        //--------------------------------------------------------------------------------
        protected static void M_Message(string message)
        {
            //------------------------------------------------------------
            MessageListener?.WriteLine(message);                        //// メッセージリスナーへメッセージを書き込む(登録されている場合)
            Debug.Print(message);                                       //// デバッグ出力(メッセージ)
        }


        //====================================================================================================
        // バッググラウンド動作タスク(待機なし)の監視用
        //====================================================================================================

        //================================================================================
        /// <summary>
        /// 【監視タスク情報】メソッド名とTask オブジェクトをペアで管理します。
        /// </summary>
        //================================================================================
        protected class M_TaskInfo
        {
            public string methodName;
            public Task task;

            public M_TaskInfo(string methodName, Task task)
            {
                this.methodName = methodName;
                this.task = task;
            }
        }


        /// <summary>
        /// 【タスク監視リスト】バッググラウンド動作タスクを監視するためのリストです。
        /// </summary>
        protected static List<M_TaskInfo> m_watchingTasks = new List<M_TaskInfo>();


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【タスク監視リスト追加】タスク監視リストにタスクを追加します。
        /// </summary>
        /// <param name="methodName">[in ]：メソッド名</param>
        /// <param name="task">      [in ]：Task オブジェクト</param>
        //--------------------------------------------------------------------------------
        protected static void M_WatchingTaskAdd(string methodName, Task task)
        {
            m_watchingTasks.Add(new M_TaskInfo(methodName, task));
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【タスクチェック】バッググラウンド動作タスクの状態を監視します。<br></br>
        /// 終了したタスクを監視リストから削除し、完了状況をメッセージ出力します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public static void CheckTask()
        {
            //------------------------------------------------------------
            /// バッググラウンド動作タスクの状態を監視する
            //------------------------------------------------------------
            for (var tmpIndex = m_watchingTasks.Count - 1; tmpIndex >= 0; tmpIndex--)
            {                                                           //// タスク監視リストを後ろから繰り返す
                var tmpItem = m_watchingTasks[tmpIndex];                /////  タスク情報を取得する
                if (tmpItem.task.IsCompleted)
                {                                                       /////  タスクが完了している場合
                    m_watchingTasks.RemoveAt(tmpIndex);                 //////   タスク監視リストから削除する

                    if (tmpItem.task.Exception == null)
                    {                                                   //////   例外情報が設定されていない場合
                        M_Message($"{tmpItem.methodName}:Complete");    ///////    処理完了メッセージを出力する

                    }
                    else
                    {                                                   //////   例外情報が設定されている場合
                        M_Message($"{tmpItem.methodName}:"              ///////    例外メッセージを出力する
                            + tmpItem.task.Exception.ToString().XRemoveNewLineChars());
                    }
                }
            }
        }


        //====================================================================================================
        // テスト用メソッド集
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【①待機なし】<br></br>
        /// 待機方法：待機せずにメソッドを終了します。<br></br>
        /// 例外処理：デバッグは中断されず、Exception プロパティーに例外情報が設定されます。<br></br>
        /// </summary>
        /// <remarks>
        /// ・検証結果一覧はこちら：<see cref="VerificationResult"/><br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static void Pattern1()
        {
            //------------------------------------------------------------
            var methodName = MethodBase.GetCurrentMethod().Name;        //// メソッド名を取得する
            M_Message($"{methodName}:Start");                           //// 処理開始メッセージを出力する

            var mytask = Task.Run(M_MyTask);                            //// タスクを作成して開始する
            M_WatchingTaskAdd(methodName, mytask);                      //// タスク監視リストに追加する

            M_Message($"{methodName}:End(No Wait)");                    //// 処理終了メッセージを出力する
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【②Task.RunSynchronously で同期的に実行】<br></br>
        /// 待機方法：呼び出し元スレッドをブロックして同期的に待機します。<br></br>
        /// 例外処理：デバッグは中断されず、Exception プロパティーに例外情報が設定されます。<br></br>
        /// </summary>
        /// <remarks>
        /// ・検証結果一覧はこちら：<see cref="VerificationResult"/><br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static void Pattern2()
        {
            //------------------------------------------------------------
            var methodName = MethodBase.GetCurrentMethod().Name;        //// メソッド名を取得する
            M_Message($"{methodName}:Start");                           //// 処理開始メッセージを出力する

            var mytask = new Task(M_MyTask);                            //// タスクを生成する
            mytask.RunSynchronously();                                  //// タスクを同期的に実行する

            if (mytask.Exception == null)
            {                                                           //// 例外情報が設定されていない場合
                M_Message($"{methodName}:End");                         /////  処理終了メッセージを出力する
            }
            else
            {                                                           //// 例外情報が設定されている場合
                M_Message($"{methodName}:"                              /////  例外メッセージを出力する
                    + mytask.Exception.ToString().XRemoveNewLineChars());
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【③Task.IsCompleted をチェックしながら Task.Delay で非同期的に待機】<br></br>
        /// 待機方法：呼び出し元スレッドを動作させながら非同期で待機します。<br></br>
        /// 例外処理：デバッグは中断されず、Exception プロパティーに例外情報が設定されます。<br></br>
        /// </summary>
        /// <remarks>
        /// ・検証結果一覧はこちら：<see cref="VerificationResult"/><br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public async static void Pattern3Async()
        {
            //------------------------------------------------------------
            var methodName = nameof(Pattern3Async);                     //// メソッド名を取得する(async メソッド内で GetCurrentMethod を使うと"MoveNext"になってしまう)
            M_Message($"{methodName}:Start");                           //// 処理開始メッセージを出力する

            var mytask = Task.Run(M_MyTask);                            //// タスクを作成して開始する
            while (mytask.IsCompleted == false)
            {                                                           //// タスクが完了するまで、繰り返す
                await Task.Delay(10);                                   /////  非同期で10ミリ秒待機する
            }

            if (mytask.Exception == null)
            {                                                           //// 例外情報が設定されていない場合
                M_Message($"{methodName}:End");                         /////  処理終了メッセージを出力する
            }
            else
            {                                                           //// 例外情報が設定されている場合
                M_Message($"{methodName}:"                              /////  例外メッセージを出力する
                    + mytask.Exception.ToString().XRemoveNewLineChars());
            }
        }


        // ＜メモ＞
        // ・中断されても「次のステートメントの設定」でスキップすることはできるが、呼び出し元スレッドで中断するのでデバッグはしにくい
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【④Task.Wait で同期的に待機】<br></br>
        /// 待機方法：呼び出し元スレッドをブロックして同期的に待機します。<br></br>
        /// 例外処理：デバッグは Task.Wait の場所で中断されます(AggregateException -> DivideByZeroException)<br></br>
        /// </summary>
        /// <remarks>
        /// ・検証結果一覧はこちら：<see cref="VerificationResult"/><br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static void Pattern4()
        {
            //------------------------------------------------------------
            var methodName = MethodBase.GetCurrentMethod().Name;        //// メソッド名を取得する
            M_Message($"{methodName}:Start");                           //// 処理開始メッセージを出力する

            var mytask = Task.Run(M_MyTask);                            //// タスクを作成して開始する
            mytask.Wait();                                              //// タスクの終了を待機する -> 未ハンドル例外発生時はここで中断される

            M_Message($"{methodName}:End");                             //// 処理終了メッセージを出力する
        }


        // ＜メモ＞
        // ・中断されると「次のステートメントの設定」でスキップすることもできないので厄介
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【⑤await で非同期的に待機】<br></br>
        /// 待機方法：呼び出し元スレッドを動作させながら非同期で待機します。<br></br>
        /// 例外処理：デバッグは Program.cs の Applicaton.Run の場所で中断されます(TargetInvocationException -> AggregateException -> DivideByZeroException)<br></br>
        /// </summary>
        /// <remarks>
        /// ・検証結果一覧はこちら：<see cref="VerificationResult"/><br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public async static void Pattern5Async()
        {
            //------------------------------------------------------------
            var methodName = nameof(Pattern5Async);                     //// メソッド名を取得する(async メソッド内で GetCurrentMethod を使うと"MoveNext"になってしまう)
            M_Message($"{methodName}:Start");                           //// 処理開始メッセージを出力する

            var mytask = Task.Run(M_MyTask);                            //// タスクを作成して開始する
            await mytask;                                               //// タスクの終了を非同期で待機する -> 未ハンドル例外発生時は Program.cs の Applicaton.Run で中断される

            M_Message($"{methodName}:End");                             //// 処理終了メッセージを出力する
        }


        //====================================================================================================
        // テスト用タスク本体
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト用タスク】３秒後に未ハンドル例外を発生させます。
        /// </summary>
        //--------------------------------------------------------------------------------
        protected static void M_MyTask()
        {
            Thread.Sleep(3000);
            int abc = 0;
            abc = 100 / abc;        // ここで例外を発生させる。例外なしで正常終了するパターンを試したいときはここをコメントアウト
        }

    } // class

} // namespace
