// @(h)FrmAppTestNovLab.cs ver 0.00 ( '22.04.09 Nov-Lab ) 作成開始
// @(h)FrmAppTestNovLab.cs ver 0.51 ( '22.05.08 Nov-Lab ) ベータ版完成
// @(h)FrmAppTestNovLab.cs ver 0.51a( '22.05.25 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【メイン画面】Test for NovLab のメイン画面です。

using System;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
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
                                          , IAutoTestExecuter   // 自動テスト実行者I/F
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
        /// 【テスト用メソッド情報コレクション】
        /// </summary>
        protected static Collection<TestMethodInfo> m_testMethodInfos = new Collection<TestMethodInfo>();
#endif


#if DEBUG   // DEBUGビルドのみ有効
        //====================================================================================================
        // IAutoTestExecuter I/Fの実装
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果通知】テスト結果をリストボックスに追加します。
        /// </summary>
        /// <param name="autoTestResult">  [in ]：自動テスト結果種別</param>
        /// <param name="testDescription"> [in ]：テスト内容文字列</param>
        /// <param name="testPattern">     [in ]：テストパターン名[null = 省略]</param>
        /// <param name="execResult">      [in ]：実行結果(戻り値 または 例外の型情報)</param>
        /// <param name="expectResult">    [in ]：予想結果(戻り値 または 例外の型情報)</param>
        /// <param name="exceptionMessage">[in ]：例外メッセージ</param>
        //--------------------------------------------------------------------------------
        public void NoticeTestResult(AutoTestResultKind autoTestResult, string testDescription, string testPattern,
                                     object execResult, object expectResult, string exceptionMessage)
        {
            //------------------------------------------------------------
            /// テスト結果をリストボックスに追加する
            //------------------------------------------------------------
            AppendTestResult(new TestResult(autoTestResult, testDescription, testPattern, execResult, expectResult, exceptionMessage));

            if (autoTestResult == AutoTestResultKind.Succeeded)
            {                                                           //// テスト成功の場合
                m_succeededCount++;                                     /////  テスト成功件数に１加算する
            }
            else
            {                                                           //// テスト成功でない場合
                m_failedCount++;                                        /////  テスト失敗件数に１加算する
            }
        }
#endif


        //====================================================================================================
        // 公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト結果追加】テスト結果(TestResult または 文字列)をリストボックスに追加します。
        /// </summary>
        /// <param name="testResult">[in ]：テスト結果</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・AppFormTraceListener からも使用するため public メソッドにしています。<br></br>
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
#if false   //[-] 保留：NovLab.Base ソリューションからだとアクセスできないので、仕組みを検討する必要がある。
            //------------------------------------------------------------
            /// 未使用のアセンブリを強制的に読み込ませ、テスト用メソッドを列挙可能にする
            //------------------------------------------------------------
            object dmy;
            dmy = typeof(NovLab.Windows.Forms.XMessageBox);             //// NovLab.Windows.Forms アセンブリ
#endif


#if DEBUG   // DEBUGビルドのみ有効
            //------------------------------------------------------------
            /// テスト用メソッド情報を収集する
            //------------------------------------------------------------
            //----------------------------------------
            // 手動テスト用メソッド
            //----------------------------------------
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {                                                           //// 読み込み済みアセンブリを繰り返す
                foreach (var typeInfo in assembly.GetTypes())
                {                                                       /////  アセンブリ内の型情報を繰り返す
                    var infos = TestMethodInfo.EnumManualTest(typeInfo);//////   手動テスト用メソッドを列挙する
                    m_testMethodInfos.XAppend(infos);                   //////   テスト用メソッド情報コレクションに追加する
                }
            }

            //----------------------------------------
            // 自動テスト用メソッド
            //----------------------------------------
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {                                                           //// 読み込み済みアセンブリを繰り返す
                foreach (var typeInfo in assembly.GetTypes())
                {                                                       /////  アセンブリ内の型情報を繰り返す
                    var infos = TestMethodInfo.EnumAutoTest(typeInfo);  //////   自動テスト用メソッドを列挙する
                    m_testMethodInfos.XAppend(infos);                   //////   テスト用メソッド情報コレクションに追加する
                }
            }


            //------------------------------------------------------------
            /// テスト用メソッドの呼び出し規約をチェックする
            //------------------------------------------------------------
            foreach (var info in m_testMethodInfos)
            {                                                           //// テスト用メソッド情報コレクションを繰り返す
                info.attributeInfo.CheckRegulation(info.methodInfo);    /////  付加されているテスト用メソッド属性の種類に応じて呼び出し規約をチェックする
                LstTestMethod.Items.Add(info);                          /////  テスト用メソッドリストボックスに追加する
            }

#else       // リリースビルドの場合
            LstTestMethod.Items.Add("DEBUGビルドでのみ動作します");
#endif


            //------------------------------------------------------------
            /// メイン画面出力トレースリスナーを設定する
            //------------------------------------------------------------
            Debug.Listeners.Add(new MainFormTraceListener(this));
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

            foreach (var info in m_testMethodInfos)
            {                                                           //// テスト用メソッド情報コレクションを繰り返す
                if (info.attributeInfo is AutoTestMethodAttribute)
                {                                                       /////  自動テスト用メソッドの場合
                    M_InvokeTest(info);                                 //////   テスト用メソッド実行処理を行う
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
#endif
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
            AppendTestResult("■" + testMethodInfo.ToString());                 //// テスト結果にテスト用メソッドの表示名を追加する

            // ＜メモ＞
            // ・派生クラス -> 基本クラス の順に判定・処理すること
            if (testMethodInfo.attributeInfo is AutoTestMethodAttribute)
            {                                                                   //// 自動テスト用メソッドの場合
                AutoTestMethodAttribute.Invoke(testMethodInfo.methodInfo, this);/////  自動テスト用メソッドを実行する
                AppendTestResult("");                                           /////  テスト結果に空行を追加する
                return;                                                         /////  関数終了
            }

            if (testMethodInfo.attributeInfo is ManualTestMethodAttribute)
            {                                                                   //// 手動テスト用メソッドの場合
                ManualTestMethodAttribute.Invoke(testMethodInfo.methodInfo);    /////  テスト用メソッドを実行する
                AppendTestResult("");                                           /////  テスト結果に空行を追加する
                return;                                                         /////  関数終了
            }
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

    }
}
