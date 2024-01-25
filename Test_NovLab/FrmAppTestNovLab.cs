// @(h)FrmAppTestNovLab.cs ver 0.00 ( '22.04.09 Nov-Lab ) 作成開始
// @(h)FrmAppTestNovLab.cs ver 0.51 ( '22.05.08 Nov-Lab ) ベータ版完成
// @(h)FrmAppTestNovLab.cs ver 0.52 ( '24.01.16 Nov-Lab ) 機能修正：テスト結果の詳細文字列に、実行前のインスタンス内容と実行後のインスタンス内容を追加した。
// @(h)FrmAppTestNovLab.cs ver 0.53 ( '24.01.17 Nov-Lab ) 機能修正：実行結果と予想結果は文字列で扱うようにした。
// @(h)FrmAppTestNovLab.cs ver 0.53a( '24.01.21 Nov-Lab ) 仕変対応：AutoTest, ManualTestMethodInfo, AutoTestMethodInfo クラスの仕様変更に対応した。機能変更なし。
// @(h)FrmAppTestNovLab.cs ver 0.54 ( '24.01.26 Nov-Lab ) 機能追加：テスト用フォームを自動的に検索・収集し、メニュー操作で表示できるようにした。

// @(s)
// 　【メイン画面】Test for NovLab のメイン画面です。

using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using NovLab;
using NovLab.DebugSupport;


namespace Test_NovLab
{
    // ＜メモ＞
    // ・DEBUGビルドでのみ動作しますが、リリースビルドでもビルドだけはできます。

    //====================================================================================================
    /// <summary>
    /// 【メイン画面】Test for NovLab のメイン画面です。
    /// </summary>
    //====================================================================================================
    public partial class FrmAppTestNovLab : Form
#if DEBUG   // DEBUGビルドのみ有効
                                          , IAutoTestResultListener // 自動テスト結果リスナーI/F
#endif
    {
        //====================================================================================================
        // 内部フィールド
        //====================================================================================================
        /// <summary>
        /// 【テスト成功件数】
        /// </summary>
        protected int m_succeededCount;

        /// <summary>
        /// 【テスト失敗件数】
        /// </summary>
        protected int m_failedCount;

#if DEBUG   // DEBUGビルドのみ有効

        /// <summary>
        /// 【テスト用メソッド情報リスト】
        /// </summary>
        protected static List<TestMethodInfo> m_testMethodInfos = new List<TestMethodInfo>();

#endif  // DEBUG


#if DEBUG   // DEBUGビルドのみ有効
        //====================================================================================================
        // IAutoTestResultListener I/Fの実装
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果通知】テスト結果をリストボックスに追加します。
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
        public void NoticeTestResult(AutoTestResultKind autoTestResult, string testDescription, string testPattern,
                                     string execResult, string expectResult, string exceptionMessage,
                                     string befContent, string aftContent)
        {
            //------------------------------------------------------------
            /// テスト結果をリストボックスに追加する
            //------------------------------------------------------------
            AppendTestResult(new TestResult(autoTestResult, testDescription, testPattern,
                execResult, expectResult, exceptionMessage,
                befContent, aftContent));

            if (autoTestResult == AutoTestResultKind.Succeeded)
            {                                                           //// テスト成功の場合
                m_succeededCount++;                                     /////  テスト成功件数に１加算する
            }
            else
            {                                                           //// テスト成功でない場合
                m_failedCount++;                                        /////  テスト失敗件数に１加算する
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メッセージ書き込み】メッセージをテスト結果リストボックスに追加します。
        /// </summary>
        /// <param name="message">[in ]：メッセージ文字列</param>
        //--------------------------------------------------------------------------------
        public void Print(string message) => AppendTestResult(message);

#endif  // DEBUG


        //====================================================================================================
        // 公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果追加】テスト結果(TestResult または 文字列)をリストボックスに追加します。
        /// </summary>
        /// <param name="testResult">[in ]：テスト結果(TestResult または 文字列)</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・<see cref="MainFormTraceListener"/> などの外部クラスからも使用するため public メソッドにしています。<br></br>
        /// </remarks>
        /// 関連リンク： <see cref="TestResult"/>
        //--------------------------------------------------------------------------------
        public void AppendTestResult(object testResult)
        {
            bool postSelect;    // 追加処理後選択フラグ


            // ＜メモ＞この処理は単一選択リストボックス用です。
            //------------------------------------------------------------
            /// 追加処理後に選択状態にすべきかどうかを決定する
            //------------------------------------------------------------
            if (LstTestResult.SelectedIndex == -1)
            {                                                           //// 選択中の項目がない場合
                postSelect = true;                                      /////  追加処理後選択フラグ = true
            }
            else if (LstTestResult.SelectedIndex == LstTestResult.Items.Count - 1)
            {                                                           //// 最後の項目を選択している場合
                postSelect = true;                                      /////  追加処理後選択フラグ = true
            }
            else
            {                                                           //// 最後以外の項目を選択している場合
                postSelect = false;                                     /////  追加処理後選択フラグ = false
            }


            //------------------------------------------------------------
            /// テスト結果を追加する
            //------------------------------------------------------------
            var addedIndex = LstTestResult.Items.Add(testResult);       //// リストボックスにテスト結果を追加する
            if (postSelect)
            {                                                           //// 追加処理後選択フラグ = true の場合
                LstTestResult.SetSelected(addedIndex, true);            /////  追加した項目を選択状態にする
            }
        }


        //====================================================================================================
        // フォームイベント
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メインフォーム_コンストラクター】新しいインスタンスを初期化します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public FrmAppTestNovLab()
        {
            //------------------------------------------------------------
            // 自動生成された部分
            //------------------------------------------------------------
            InitializeComponent();
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メインフォーム_Load】本フォームを初期化します。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void FrmAppTestNovLabBase_Load(object sender, EventArgs e)
        {
            // アセンブリを強制的に読み込ませるためのメソッドを各プロジェクトに用意しておき、
            // 目印属性によって検索・収集してそれを呼び出す仕組みではどうか？
#if false   //[-] 保留：NovLab.Base ソリューションからだとアクセスできないので、仕組みを検討する必要がある。
            //------------------------------------------------------------
            /// 未使用のアセンブリを強制的に読み込ませ、テスト用メソッドを列挙可能にする
            //------------------------------------------------------------
            object dmy;
            dmy = typeof(NovLab.Windows.Forms.XMessageBox);             //// NovLab.Windows.Forms アセンブリ
#endif


            //------------------------------------------------------------
            // 起動時自動テスト
            //------------------------------------------------------------
#if DEBUG   // DEBUGビルドのみ有効
            ZZZDraft_Test_NovLab.ZZZ_StartUpTest();
#endif


            //------------------------------------------------------------
            /// テスト用フォーム表示メソッドを収集し、メニュー項目を生成・追加する
            //------------------------------------------------------------
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {                                                           //// 読み込み済みアセンブリを繰り返す
                foreach (var typeInfo in assembly.GetTypes())
                {                                                       /////  アセンブリ内の型情報を繰り返す
                    foreach (var methodInfo in typeInfo.GetMethods())
                    {                                                   //////   メソッド情報を繰り返す
                        var attributes =                                ///////    テスト用フォーム表示メソッド属性の配列を取得する
                            methodInfo.GetCustomAttributes(typeof(TestFormShowMethodAttribute), false);
                        foreach (TestFormShowMethodAttribute tmpAttr in attributes) // (キャストは必ず成功する)
                        {                                               ///////    テスト用フォーム表示メソッド属性配列を繰り返す(シングルユース属性なので実際は１つだけ取得可能なはず)
                            var actShowTestForm =                       ////////     メソッド情報からActionデリゲートインスタンスを生成する
                                (Action)Delegate.CreateDelegate(
                                        typeof(Action), methodInfo);

                            var tmpMenuItem = new ToolStripMenuItem(tmpAttr.displayText, null, M_MnuWindow_ShowTestForm_Click)
                            {                                           ////////     メニュー項目を生成する
                                Tag = actShowTestForm,                  /////////      タグにActionデリゲートインスタンスを設定する
                            };

                            MnuWindow.DropDownItems.Add(tmpMenuItem);   ////////     メニュー項目を「ウィンドウ」メニューに追加する
                        }
                    }
                }
            }


#if DEBUG   // DEBUGビルドのみ有効
            //------------------------------------------------------------
            /// テスト用メソッド情報を収集する
            //------------------------------------------------------------
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {                                                           //// 読み込み済みアセンブリを繰り返す
                foreach (var typeInfo in assembly.GetTypes())
                {                                                       /////  アセンブリ内の型情報を繰り返す
                    var infos =
                        TestMethodInfo.EnumTestMethod(typeInfo);        //////   型情報に含まれるテスト用メソッドを列挙する
                    m_testMethodInfos.XAppend(infos);                   //////   テスト用メソッド情報リストに追加する
                }
            }

            m_testMethodInfos.Sort();                                   //// テスト用メソッド情報リストをソートする


            //------------------------------------------------------------
            /// 収集したテスト用メソッド情報をリストボックスに追加する
            //------------------------------------------------------------
            foreach (var tmpInfo in m_testMethodInfos)
            {                                                           //// テスト用メソッド情報コレクションを繰り返す
                LstTestMethod.Items.Add(tmpInfo);                       /////  テスト用メソッドリストボックスに追加する
            }

#endif  // DEBUG


            //------------------------------------------------------------
            /// リリースビルドの場合は注意文をリストボックスに追加する
            //------------------------------------------------------------
#if !DEBUG
            LstTestMethod.Items.Add("DEBUGビルドでのみ動作します");
#endif


            //------------------------------------------------------------
            /// デバッグ出力監視リスナーにメイン画面出力トレースリスナーを登録する
            //------------------------------------------------------------
            Debug.Listeners.Add(new MainFormTraceListener(this));


            //------------------------------------------------------------
            /// 自動テスト結果リスナーに登録する
            //------------------------------------------------------------
#if DEBUG   // DEBUGビルドのみ有効
            AutoTest.AddListener(this);
#endif
        }


        //====================================================================================================
        // メニューイベント
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メニュー項目：ウィンドウ - テスト用ウィンドウを開く_Click】
        /// メニュー項目の Tag に設定されている Action デリゲートを実行し、テスト用フォームを開きます。
        /// </summary>
        //--------------------------------------------------------------------------------
        protected void M_MnuWindow_ShowTestForm_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// メニュー項目のTag に設定されている Action デリゲートを実行し、
            ///-テスト用フォームを開く
            //------------------------------------------------------------
            ToolStripItem item = (ToolStripItem)sender;

            if (item.Tag is Action actShowTestForm)
            {                                                           //// Tag に設定されているオブジェクトが Action デリゲートの場合
                actShowTestForm();                                      /////  Actionデリゲートを実行する
            }
            else
            {                                                           //// Tag に設定されているオブジェクトが Action デリゲートでない場合(バグチェック)
                Debug.Print("Tag プロパティーの内容が不正です：" +      /////  デバッグ出力(Tag内容不正)
                            XObject.XNullSafeToString(item.Tag));
            }
        }


        //====================================================================================================
        // コントロールイベント
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト全実行ボタン_Click】すべての自動テストを実行します。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void BtnExecAllAutoTest_Click(object sender, EventArgs e)
        {
#if DEBUG   // DEBUGビルドのみ有効
            //------------------------------------------------------------
            /// すべての自動テストを実行する
            //------------------------------------------------------------
            m_succeededCount = 0;                                       //// テスト成功件数 = 0 にクリアする
            m_failedCount = 0;                                          //// テスト失敗件数 = 0 にクリアする

            foreach (var tmpInfo in m_testMethodInfos)
            {                                                           //// テスト用メソッド情報コレクションを繰り返す
                if (tmpInfo.TestMethodKind == TestMethodKind.Auto)
                {                                                       /////  自動テスト用メソッドの場合
                    M_InvokeTest(tmpInfo);                              //////   テスト用メソッド実行処理を行う
                }
            }

            if (m_failedCount == 0)
            {                                                           //// テスト失敗件数 = 0 の場合
                AppendTestResult(                                       /////  クイックテスト結果(全成功)を追加する
                    "すべてのテストに成功しました [成功:" + m_succeededCount + "]");
            }
            else
            {                                                           //// テスト失敗件数 = 0 でない場合
                AppendTestResult(                                       /////  クイックテスト結果(失敗あり)を追加する
                    "失敗したテストがあります [成功:" + m_succeededCount + " / 失敗:" + m_failedCount + "]");
            }

            LstTestResult.Focus();                                      //// テスト結果リストボックスへフォーカスを移動する
#endif  // DEBUG
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト用メソッドリストボックス_DoubleClick】
        /// リストボックスで選択されているテスト用メソッドを実行します。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void LstTestMethod_DoubleClick(object sender, EventArgs e)
        {
#if DEBUG   // DEBUGビルドのみ有効
            //------------------------------------------------------------
            /// リストボックスで選択されているテスト用メソッドを実行する
            //------------------------------------------------------------
            var selItem = LstTestMethod.SelectedItem;                   //// 選択中リスト項目を取得する
            if (selItem == null)
            {                                                           //// 選択中リスト項目がない場合
                Debug.Print("項目が選択されていません。");
                return;
            }

            var testMethodInfo = selItem as TestMethodInfo;             //// 選択中リスト項目をテスト用メソッド情報にキャスト試行する
            if (testMethodInfo == null)
            {                                                           //// キャスト失敗の場合(バグチェック)
                Debug.Print(nameof(TestMethodInfo) + "に変換できません。");
                return;
            }

            M_InvokeTest(testMethodInfo);                               //// テスト用メソッド実行処理を行う
            LstTestResult.Focus();                                      //// テスト結果リストボックスへフォーカスを移動する
#endif
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果リストボックス_SelectedIndexChanged】リストボックスで選択中のテスト結果をテキストボックスに表示します。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void LstTestResult_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG   // DEBUGビルドのみ有効
            //------------------------------------------------------------
            /// リストボックスで選択中のテスト結果をテキストボックスに表示する
            //------------------------------------------------------------
            if (LstTestResult.SelectedIndex == -1)
            {                                                           //// テスト結果リストボックスで選択項目がない場合
                TxtTestResult.Text = "";                                /////  テスト結果テキストボックスをクリアする
                return;                                                 /////  関数終了
            }

            var testResult = LstTestResult.SelectedItem as TestResult;  //// 選択中項目をテスト結果情報にキャスト試行する
            if (testResult == null)
            {                                                           //// キャスト失敗の場合
                TxtTestResult.Text =                                    /////  選択中項目を文字列化してテスト結果テキストボックスに設定する
                    LstTestResult.SelectedItem.ToString();
            }
            else
            {                                                           //// キャスト成功の場合
                TxtTestResult.Text = testResult.ToDetailString();       /////  選択中項目を詳細文字列化してテスト結果テキストボックスに設定する
            }
#endif
        }


        //====================================================================================================
        // 内部メソッド
        //====================================================================================================

#if DEBUG   // DEBUGビルドのみ有効
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト用メソッド実行】テスト用メソッド情報に従ってテストを実行します。
        /// </summary>
        /// <param name="testMethodInfo">[in ]：テスト用メソッド情報</param>
        //--------------------------------------------------------------------------------
        protected void M_InvokeTest(TestMethodInfo testMethodInfo)
        {
            //------------------------------------------------------------
            /// テスト用メソッド情報に従ってテストを実行する
            //------------------------------------------------------------
            AppendTestResult("■" + testMethodInfo.ToString());         //// テスト結果にテスト用メソッドの表示名を追加する
            testMethodInfo.Invoke();                                    //// テスト用メソッドを実行する
            AppendTestResult("");                                       //// テスト結果に空行を追加する
        }
#endif


        //====================================================================================================
        // テスト結果リストボックス用コンテキストメニュー
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果リストボックス用コンテキストメニュー_Opening】
        /// テスト結果リストボックスの状態に合わせてコンテキストメニューを初期化します。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void CMnuTestResult_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //------------------------------------------------------------
            /// テスト結果リストボックスの状態に合わせてコンテキストメニューを初期化する
            //------------------------------------------------------------
            if (LstTestResult.Items.Count == 0)
            {                                                           //// テスト結果リストボックスに項目がない場合
                CMnuTestResult_MoveFirst.Enabled = false;
                CMnuTestResult_MoveLast.Enabled = false;
                CMnuTestResult_Clear.Enabled = false;
                CMnuTestResult_CopyAll.Enabled = false;
            }
            else
            {                                                           //// テスト結果リストボックスに項目がある場合
                CMnuTestResult_MoveFirst.Enabled = true;
                CMnuTestResult_MoveLast.Enabled = true;
                CMnuTestResult_Clear.Enabled = true;
                CMnuTestResult_CopyAll.Enabled = true;
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メニュー：テスト結果リストボックス用コンテキストメニュー - 最上部へ移動_Click】
        /// テスト結果リストボックスの最初の項目へ移動します。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void CMnuTestResult_MoveFirst_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// 最初の項目へ移動する(選択状態にする)
            //------------------------------------------------------------
            if (LstTestResult.Items.Count == 0) return;
            LstTestResult.SelectedIndex = 0;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メニュー：テスト結果リストボックス用コンテキストメニュー - 最下部へ移動_Click】
        /// テスト結果リストボックスの最後の項目へ移動します。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void CMnuTestResult_MoveLast_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// 最後の項目へ移動する(選択状態にする)
            //------------------------------------------------------------
            if (LstTestResult.Items.Count == 0) return;
            LstTestResult.SelectedIndex = LstTestResult.Items.Count - 1;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メニュー：テスト結果リストボックス用コンテキストメニュー - クリア_Click】
        /// テスト結果リストボックスをクリアします。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void CMnuTestResult_Clear_Click(object sender, EventArgs e)
        {
            //------------------------------------------------------------
            /// テスト結果リストボックスをクリアする
            //------------------------------------------------------------
            LstTestResult.SelectedIndex = -1;                           //// 選択中インデックス = -1(なし) に設定し、SelectedIndexChanged イベントを発生させる
            LstTestResult.Items.Clear();                                //// テスト結果リストボックスをクリアする
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【メニュー：テスト結果リストボックス用コンテキストメニュー - すべてコピー_Click】
        /// すべてのテスト結果をクリップボードへコピーします。
        /// </summary>
        //--------------------------------------------------------------------------------
        private void CMnuTestResult_CopyAll_Click(object sender, EventArgs e)
        {
#if DEBUG   // DEBUGビルドのみ有効
            // ＜メモ＞
            // ・間に空行を入れるタイミング
            //   テスト結果情報 -> テスト結果情報：空行を入れる
            //   テスト結果情報 -> 単純文字列    ：空行を入れる
            //   単純文字列     -> テスト結果情報：空行を入れる
            //   単純文字列     -> 単純文字列    ：空行を入れない
            //------------------------------------------------------------
            /// すべてのテスト結果をクリップボードへコピーする
            //------------------------------------------------------------
            var workStr = new StringBuilder();                          //// 文字列編集領域を生成する

            var postSimpleString = true;                                //// 単純文字列直後フラグ = true に初期化する
            foreach (var item in LstTestResult.Items)
            {                                                           //// テスト結果リストボックスの全項目を繰り返す
                if(item is TestResult testResult)
                {                                                       /////  テスト結果情報の場合
                    workStr.Append("\r\n");                             //////   文字列編集領域に空行を追加する
                    workStr.Append(                                     //////   テスト結果情報を詳細文字列化して文字列編集領域に追加する
                        testResult.ToDetailString() + "\r\n");
                    postSimpleString = false;                           //////   単純文字列直後フラグ = false
                }
                else
                {                                                       /////  テスト結果情報でない場合(単純文字列の場合)
                    if (postSimpleString == false)
                    {                                                   //////   単純文字列直後でない場合
                        workStr.Append("\r\n");                         ///////    文字列編集領域に空行を追加する
                    }

                    workStr.Append(item.ToString() + "\r\n");           //////   リストボックス項目を文字列化して文字列編集領域に追加する
                    postSimpleString = true;                            //////   単純文字列直後フラグ = true
                }
            }

            Clipboard.SetText(workStr.ToString());                      //// 文字列編集領域の内容をクリップボードにコピーする
#endif
        }

    } // class

} // namespace
