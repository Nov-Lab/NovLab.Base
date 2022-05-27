// 下書き

using System;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using NovLab;
using NovLab.DebugSupport;


namespace Test_NovLab
{
    class ZZZDraft_Test_NovLab
    {

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

    }
}
