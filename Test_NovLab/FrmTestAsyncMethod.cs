// @(h)FrmTestAsyncMethod.cs ver 0.00 ( '24.01.25 Nov-Lab ) 作成開始
// @(h)FrmTestAsyncMethod.cs ver 0.51 ( '24.01.27 Nov-Lab ) ベータ版完成

// @(s)
// 　【AsyncMethod クラスのテスト画面】

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

using NovLab.Threading;


namespace Test_NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【AsyncMethod クラスのテスト画面】
    /// </summary>
    //====================================================================================================
    public partial class FrmTestAsyncMethod : Form, TestTaskException.IMessageListener
    {
        //====================================================================================================
        // テスト用フォーム表示関連
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト用フォーム表示】本フォームを表示します。
        /// </summary>
        //--------------------------------------------------------------------------------
        [TestFormShowMethod("AsyncMethod クラスのテスト")]
        public static void TestFormOpen()
        {
            //------------------------------------------------------------
            /// 本フォームを表示する
            //------------------------------------------------------------
            if (m_singleton != null && m_singleton.IsDisposed == false)
            {                                                           //// シングルトンインスタンスが生成されていて、かつ破棄されていない場合
                if (m_singleton.WindowState == FormWindowState.Minimized)
                {                                                       /////  フォームが最小化されている場合
                    m_singleton.WindowState = FormWindowState.Normal;   //////   通常表示にする
                }

                m_singleton.Activate();                                 /////  フォームをアクティブにする
            }
            else
            {                                                           //// シングルトンインスタンスが生成されていないか、破棄されている場合
                m_singleton = new FrmTestAsyncMethod();                 /////  シングルトンインスタンスを生成する(破棄されていた場合は再生成する)
                m_singleton.Show();                                     /////  フォームを表示する
            }
        }

        /// <summary>
        /// 【シングルトンインスタンス】本フォームのシングルトンインスタンスです。
        /// </summary>
        protected static FrmTestAsyncMethod m_singleton = null;


        //====================================================================================================
        // 非同期メソッド管理テーブル関連
        //====================================================================================================
        #region 非同期メソッド管理テーブル関連

        /// <summary>
        /// 【非同期メソッド管理テーブル】動作中の非同期メソッドを管理するために必要な情報を管理します。
        /// </summary>
        protected M_ManagementTable m_managementTable = new M_ManagementTable();


        //================================================================================
        /// <summary>
        /// 【非同期メソッド管理テーブル】動作中の非同期メソッドを管理するために必要な情報を管理するテーブルです。
        /// </summary>
        /// <remarks>
        /// ・Key は <see cref="TestAsyncArgs"/> で、動作指示や情報の受け渡しに使います。<br></br>
        /// ・Value は <see cref="Label"/> で、メッセージ文字列の出力先です。<br></br>
        /// <br></br>
        /// 補足<br></br>
        /// ・ラベルなどのコントロールは別スレッドで実行される非同期メソッドからは直接操作できないため、
        ///   引数セットを介して情報を受け渡しして、メインスレッド側で操作します。<br></br>
        /// </remarks>
        //================================================================================
        protected class M_ManagementTable : IEnumerable<KeyValuePair<TestAsyncArgs, Label>>
        {
            //================================================================================
            // 内部フィールド
            //================================================================================

            /// <summary>
            /// 【テーブル本体】
            /// </summary>
            protected Dictionary<TestAsyncArgs, Label> m_table = new Dictionary<TestAsyncArgs, Label>();


            //================================================================================
            // IEnumerable<KeyValuePair<TestAsyncArgs, Label>> I/F の実装
            //================================================================================
            /// <summary>
            /// 【列挙子取得】
            /// </summary>
            /// <returns></returns>
            public IEnumerator<KeyValuePair<TestAsyncArgs, Label>> GetEnumerator() => m_table.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => m_table.GetEnumerator();


            //================================================================================
            // 公開メソッド
            //================================================================================

            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【非同期メソッド追加】非同期メソッドをテーブルに追加します。
            /// </summary>
            /// <param name="args">     [in ]：非同期メソッド用引数セット</param>
            /// <param name="lblTarget">[in ]：メッセージ出力用ラベル</param>
            //--------------------------------------------------------------------------------
            public void Add(TestAsyncArgs args, Label lblTarget) => m_table.Add(args, lblTarget);

            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【非同期メソッド削除】非同期メソッドをテーブルから削除します。
            /// </summary>
            /// <param name="args">[in ]：非同期メソッド用引数セット</param>
            //--------------------------------------------------------------------------------
            public void Remove(TestAsyncArgs args) => m_table.Remove(args);

        } // class

#endregion


        //====================================================================================================
        // フォームイベント
        //====================================================================================================
        public FrmTestAsyncMethod()
        {
            InitializeComponent();
        }

        private void FrmTestAsyncMethod_Load(object sender, EventArgs e)
        {
            /// Task クラスの例外テストにリスナーを設定する
            TestTaskException.MessageListener = this;
        }


        //====================================================================================================
        // TaskExceptionTest.IMessageListener I/F の実装
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メッセージ書き込み】メッセージ文字列をリストボックスに追加します。
        /// </summary>
        /// <param name="message">[in ]：メッセージ文字列</param>
        //--------------------------------------------------------------------------------
        public void WriteLine(string message)
        {
            var moveSelection = false;
            if (LstMessage.SelectedIndex == LstMessage.Items.Count - 1)
            {
                moveSelection = true;
            }

            LstMessage.Items.Add(message);
            if (moveSelection == true)
            {
                LstMessage.SelectedIndex = LstMessage.Items.Count - 1;
            }
        }


        //====================================================================================================
        // Task クラスの例外テスト関連
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【タスクチェックタイマー_Tick】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void TmrCheckTask_Tick(object sender, EventArgs e)
        {
            TestTaskException.CheckTask();
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【Task クラスの例外テスト①】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnTaskExceptionTest1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            ((Button)sender).Enabled = false;
            TestTaskException.Pattern1();
            ((Button)sender).Enabled = true;
            this.Enabled = true;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【Task クラスの例外テスト②】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnTaskExceptionTest2_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            ((Button) sender).Enabled = false;
            TestTaskException.Pattern2();
            ((Button) sender).Enabled = true;
            this.Enabled = true;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【Task クラスの例外テスト③】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnTaskExceptionTest3_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            ((Button) sender).Enabled = false;
            TestTaskException.Pattern3Async();
            ((Button) sender).Enabled = true;
            this.Enabled = true;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【Task クラスの例外テスト④】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnTaskExceptionTest4_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            ((Button) sender).Enabled = false;
            TestTaskException.Pattern4();
            ((Button) sender).Enabled = true;
            this.Enabled = true;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【Task クラスの例外テスト⑤】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnTaskExceptionTest5_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            ((Button)sender).Enabled = false;
            TestTaskException.Pattern5Async();
            ((Button)sender).Enabled = true;
            this.Enabled = true;
        }


        //====================================================================================================
        // AsyncMethod クラスのテスト
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【更新タイマー_Tick】メッセージチェック処理を行います。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void TmrRefresh_Tick(object sender, EventArgs e) => M_MessageCheck();


        // ＜メモ＞
        // ・コールバックやイベント機構を使用しても、別スレッドで実行する非同期メソッドから
        //   ラベルを直接操作することはできないため、引数セットを介して情報の受け渡しを行い、
        //   メインスレッド側でラベルに表示する。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メッセージチェック】非同期メソッドからのメッセージがある場合はラベルに表示します。
        /// </summary>
        //--------------------------------------------------------------------------------
        protected void M_MessageCheck()
        {
            //------------------------------------------------------------
            /// 非同期メソッドからのメッセージがある場合はラベルに表示する
            //------------------------------------------------------------
            foreach (var tmpItem in m_managementTable)
            {                                                           //// 非同期メソッド管理テーブルを繰り返す
                ReceiveMessage(tmpItem.Key, tmpItem.Value);             /////  非同期メソッド用メッセージ受信処理を行う
            }


            //------------------------------------------------------------
            /// 【ローカル関数】非同期メソッド用メッセージ受信
            //------------------------------------------------------------
            void ReceiveMessage(TestAsyncArgs args,         // [in ]：非同期メソッド用引数セット
                                Label lblTarget)            // [in ]：メッセージ出力用ラベル
            {
                var message = args.PullMessage();                       //// 引数セットからメッセージ文字列を取り出す
                if (string.IsNullOrEmpty(message) == false)
                {                                                       //// メッセージ文字列に内容が設定されている場合
                    lblTarget.Text = message;                           /////  メッセージ出力用ラベルに表示する
                }
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【終了要求ボタン_Click】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnRequestExit_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// すべての非同期メソッドに終了要求を送る
            //------------------------------------------------------------
            foreach (var tmpItem in m_managementTable)
            {                                                           //// 非同期メソッド管理テーブルを繰り返す
                tmpItem.Key.exitRequestSignal.Set();                    /////  終了要求シグナルをセットする
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【キャンセル要求ボタン_Click】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnRequestCancel_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// すべての非同期メソッドにキャンセル要求を送る
            //------------------------------------------------------------
            foreach (var tmpItem in m_managementTable)
            {                                                           //// 非同期メソッド管理テーブルを繰り返す
                tmpItem.Key.ctsCancelRequest.Cancel();                  /////  取り消しトークンに取り消し要求を伝える
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【例外要求ボタン_Click】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnRequestException_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// すべての非同期メソッドに例外要求を送る
            //------------------------------------------------------------
            foreach (var tmpItem in m_managementTable)
            {                                                           //// 非同期メソッド管理テーブルを繰り返す
                tmpItem.Key.requestUnhandledException = true;           /////  未ハンドル例外発生要求フラグ = true にセットする
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【一斉に実行ボタン_Click】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnRunAll_Click(object sender, EventArgs e)
        {
            if (BtnNoResultNoArg.Enabled == true)
            {
                BtnNoResultNoArg.PerformClick();
            }
            if (BtnNoResultWithArg.Enabled == true)
            {
                BtnNoResultWithArg.PerformClick();
            }
            if (BtnWithResultNoArg.Enabled == true)
            {
                BtnWithResultNoArg.PerformClick();
            }
            if (BtnWithResultWithArg.Enabled == true)
            {
                BtnWithResultWithArg.PerformClick();
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【クリアボタン_Click】
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            LblNoResultNoArg.Text = "";
            LblNoResultWithArg.Text = "";
            LblWithResultNoArg.Text = "";
            LblWithResultWithArg.Text = "";
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【AsyncMethod.RunAsync のテストボタン_Click(戻り値なし、引数なしパターン)】
        /// </summary>
        //--------------------------------------------------------------------------------
        private async void BtnNoResultNoArg_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// 初期処理
            //------------------------------------------------------------
            var args = new TestAsyncArgs();                             //// 引数セットを生成する
            var lblTarget = LblNoResultNoArg;                           //// メッセージ出力用ラベル = 戻り値なし、引数なしテスト用ラベル


            //------------------------------------------------------------
            /// 非同期メソッドを実行する(戻り値なし、引数なしパターン)
            //------------------------------------------------------------
            {                                                           //// 前準備(finally での後始末に対応する分)
                BtnNoResultNoArg.Enabled = false;                       /////  ボタンを使用不可能にする
                m_managementTable.Add(args, lblTarget);                 /////  非同期メソッド管理テーブルに情報を追加する
            }

            try
            {                                                           //// try開始
                lblTarget.Text = "Running...";                          /////  処理中メッセージをラベルに表示する
                await AsyncMethod.RunWait(
                    TestAsyncRoutine.AsyncActionNonParam);              /////  非同期メソッドを実行する
                lblTarget.Text = "End";                                 /////  処理終了メッセージをラベルに表示する
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外(非同期メソッドで発生したハンドルされていない例外もここでキャッチする)
                lblTarget.Text = "Catch:" + ex.Message;                 /////  例外メッセージをラベルに表示する
            }
            finally
            {                                                           //// finally：後始末
                m_managementTable.Remove(args);                         /////  非同期メソッド管理テーブルから情報を削除する
                BtnNoResultNoArg.Enabled = true;                        /////  ボタンを使用可能に戻す
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【AsyncMethod.RunAsync のテストボタン_Click(戻り値なし、引数ありパターン)】
        /// </summary>
        //--------------------------------------------------------------------------------
        private async void BtnNoResultWithArg_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// 初期処理
            //------------------------------------------------------------
            var args = new TestAsyncArgs();                             //// 引数セットを生成する
            var lblTarget = LblNoResultWithArg;                         //// メッセージ出力用ラベル = 戻り値なし、引数ありテスト用ラベル


            //------------------------------------------------------------
            /// 非同期メソッドを実行する(戻り値なし、引数ありパターン)
            //------------------------------------------------------------
            {                                                           //// 前準備(finally での後始末に対応する分)
                BtnNoResultWithArg.Enabled = false;                     /////  ボタンを使用不可能にする
                m_managementTable.Add(args, lblTarget);                 /////  非同期メソッド管理テーブルに情報を追加する
            }

            try
            {                                                           //// try開始
                lblTarget.Text = "Running...";                          /////  処理中メッセージをラベルに表示する
                await AsyncMethod.RunWait(
                    TestAsyncRoutine.AsyncActionWithParam, args);       /////  非同期メソッドを実行する
                lblTarget.Text = "End";                                 /////  処理終了メッセージをラベルに表示する
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外(非同期メソッドで発生したハンドルされていない例外もここでキャッチする)
                lblTarget.Text = "Catch:" + ex.Message;                 /////  例外メッセージをラベルに表示する
            }
            finally
            {                                                           //// finally：後始末
                m_managementTable.Remove(args);                         /////  非同期メソッド管理テーブルから情報を削除する
                BtnNoResultWithArg.Enabled = true;                      /////  ボタンを使用可能に戻す
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【AsyncMethod.RunAsync のテストボタン_Click(戻り値あり、引数なしパターン)】
        /// </summary>
        //--------------------------------------------------------------------------------
        private async void BtnWithResultNoArg_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// 初期処理
            //------------------------------------------------------------
            var args = new TestAsyncArgs();                             //// 引数セットを生成する
            var lblTarget = LblWithResultNoArg;                         //// メッセージ出力用ラベル = 戻り値あり、引数なしテスト用ラベル


            //------------------------------------------------------------
            /// 非同期メソッドを実行する(戻り値あり、引数なしパターン)
            //------------------------------------------------------------
            {                                                           //// 前準備(finally での後始末に対応する分)
                BtnWithResultNoArg.Enabled = false;                     /////  ボタンを使用不可能にする
                m_managementTable.Add(args, lblTarget);                 /////  非同期メソッド管理テーブルに情報を追加する
            }

            try
            {                                                           //// try開始
                lblTarget.Text = "Running...";                          /////  処理中メッセージをラベルに表示する
                var result = await AsyncMethod.RunWait(
                    TestAsyncRoutine.AsyncFuncNonParam);                /////  非同期メソッドを実行する
                lblTarget.Text = result;                                /////  非同期メソッドからの戻り値をラベルに表示する
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外(非同期メソッドで発生したハンドルされていない例外もここでキャッチする)
                lblTarget.Text = "Catch:" + ex.Message;                 /////  例外メッセージをラベルに表示する
            }
            finally
            {                                                           //// finally：後始末
                m_managementTable.Remove(args);                         /////  非同期メソッド管理テーブルから情報を削除する
                BtnWithResultNoArg.Enabled = true;                      /////  ボタンを使用可能に戻す
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【AsyncMethod.RunAsync のテストボタン_Click(戻り値あり、引数ありパターン)】
        /// </summary>
        //--------------------------------------------------------------------------------
        private async void BtnWithResultWithArg_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// 初期処理
            //------------------------------------------------------------
            var args = new TestAsyncArgs();                             //// 引数セットを生成する
            var lblTarget = LblWithResultWithArg;                       //// メッセージ出力用ラベル = 戻り値あり、引数ありテスト用ラベル


            //------------------------------------------------------------
            /// 非同期メソッドを実行する(戻り値あり、引数ありパターン)
            //------------------------------------------------------------
            {                                                           //// 前準備(finally での後始末に対応する分)
                BtnWithResultWithArg.Enabled = false;                   /////  ボタンを使用不可能にする
                m_managementTable.Add(args, lblTarget);                 /////  非同期メソッド管理テーブルに情報を追加する
            }

            try
            {                                                           //// try開始
                lblTarget.Text = "Running...";                          /////  処理中メッセージをラベルに表示する
                var result = await AsyncMethod.RunWait(
                   TestAsyncRoutine.AsyncFuncWithParam, args);          /////  非同期メソッドを実行する
                lblTarget.Text = result;                                /////  非同期メソッドからの戻り値をラベルに表示する
            }
            catch (Exception ex)
            {                                                           //// catch：すべての例外(非同期メソッドで発生したハンドルされていない例外もここでキャッチする)
                lblTarget.Text = "Catch:" + ex.Message;                 /////  例外メッセージをラベルに表示する
            }
            finally
            {                                                           //// finally：後始末
                m_managementTable.Remove(args);                         /////  非同期メソッド管理テーブルから情報を削除する
                BtnWithResultWithArg.Enabled = true;                    /////  ボタンを使用可能に戻す
            }
        }

    } // class

} // namespace
