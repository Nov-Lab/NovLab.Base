// 下書き

using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using NovLab;
using NovLab.DebugSupport;
using System.Windows.Forms;


namespace Test_NovLab
{
    public partial class ZZZDraft_Test_NovLab
    {
        [ManualTestMethod("メイン画面のテスト用：他のテストを実行中は次のテストを行えないようにするブロック処理のテスト")]
        public async static System.Threading.Tasks.Task SleepAsync()
        {
            Debug.Print("非同期メソッドのテスト中");
            await System.Threading.Tasks.Task.Delay(5000);
            Debug.Print("非同期メソッドのテスト完了");
        }


        [ManualTestMethod("一時的テスト")]
        public static void ZZZ()
        {
            var strValue = "ABC\r\nDEF\rGHI\nJKL";
            Debug.Print(strValue.XEscape(EscapeConverter.CcVisualization));
            Debug.Print(strValue.XReplaceNewLineChars(" "));
            Debug.Print(strValue.XReplaceNewLineChars("\r\n"));
        }


        [ManualTestMethod("実験中：サロゲートペア、絵文字、異体字セレクタ")]
        public static void ZZZ_SurrogatePair()
        {
            var testStr = "Hello, 🎁 for you.";

            Debug.Print("Index of 🎁 in 「" + testStr + "」:" + testStr.IndexOf("🎁"));
            Debug.Print("Length of 🎁:" + "🎁".Length);
        }


#if false   // 確認済み
        [ManualTestMethod("System.Environment.NewLine = Windows環境では CR+LF")]
        public static void ZZZ_NewLine()
        {
            Debug.Print(System.Environment.NewLine.XEscape(EscapeConverter.CcVisualization));
        }
#endif


#if DEBUG
        /// <summary>
        /// 【起動時自動テスト】現在作成中のものなど、起動時に自動で行いたいテストをここで呼び出します。
        /// </summary>
        public static void ZZZ_StartUpTest()
        {
            //ZZZTest_FILEandLINE.ZZZ_OverallTest();  // テスト済み

        }
#endif

        } // class

} // namespace
