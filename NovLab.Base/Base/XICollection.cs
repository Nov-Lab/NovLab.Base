// @(h)XICollection.cs ver 0.00 ( '22.05.18 Nov-Lab ) 作成開始
// @(h)XICollection.cs ver 0.21 ( '22.05.19 Nov-Lab ) アルファ版完成

// @(s)
// 　【ICollection 拡張メソッド】コレクションI/Fに拡張メソッドを追加します。

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using NovLab;
using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【ICollection 拡張メソッド】コレクションI/Fに拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static class XICollection
    {
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コピー配列作成】コレクションの全要素を新しい配列にコピーします。
        /// </summary>
        /// <typeparam name="TItem">コレクション内の要素の型</typeparam>
        /// <param name="target">[in ]：対象コレクション</param>
        /// <returns>
        /// コピーした配列
        /// </returns>
        //--------------------------------------------------------------------------------
        public static TItem[] XToArray<TItem>(this ICollection<TItem> target)
        {
            //------------------------------------------------------------
            /// コレクションの全要素を新しい配列にコピーする
            //------------------------------------------------------------
            var array = new TItem[target.Count];                        //// 対象コレクションの要素数で配列を生成する
            target.CopyTo(array, 0);                                    //// 対象コレクションの内容を配列にコピーする
            return array;                                               //// 戻り値 = コピーした配列 で関数終了
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod]
        public static void ZZZ_XToArray(IAutoTestExecuter ifExecuter)
        {
            var floatResult = new float[] { 1.23f, 2.34f, 3.45f };      // floatコレクションの予想結果
            var strResult = new string[] { "ABC", "あいう", "漢字" };   // stringコレクションの予想結果

            //------------------------------------------------------------
            /// floatコレクション
            //------------------------------------------------------------
            // Collection<float>
            {
                var testCol = new Collection<float>();
                testCol.Add(1.23f); testCol.Add(2.34f); testCol.Add(3.45f);
                AutoTest.Test(XToArray, testCol, floatResult, ifExecuter, "Collection<float> から配列を作成");
            }

            // List<float>
            {
                var testList = new List<float>();
                testList.Add(1.23f); testList.Add(2.34f); testList.Add(3.45f);
                AutoTest.Test(XToArray, testList, floatResult, ifExecuter, "List<float> から配列を作成");
            }

            // float[]
            {
                var testArray = new float[3] { 1.23f, 2.34f, 3.45f };
                AutoTest.Test(XToArray, testArray, floatResult, ifExecuter, "float[] から配列を作成");
            }


            //------------------------------------------------------------
            /// 文字列コレクション
            //------------------------------------------------------------
            // Collection<string>
            {
                var testCol = new Collection<string>();
                testCol.Add("ABC"); testCol.Add("あいう"); testCol.Add("漢字");
                AutoTest.Test(XToArray, testCol, strResult, ifExecuter, "Collection<string> から配列を作成");
            }

            // List<string>
            {
                var testList = new List<string>();
                testList.Add("ABC"); testList.Add("あいう"); testList.Add("漢字");
                AutoTest.Test(XToArray, testList, strResult, ifExecuter, "List<string> から配列を作成");
            }

            // string[]
            {
                var testArray = new string[3] { "ABC", "あいう", "漢字" };
                AutoTest.Test(XToArray, testArray, strResult, ifExecuter, "string[] から配列を作成");
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コレクション追加】指定したコレクションの全要素を末尾に追加します。
        /// </summary>
        /// <typeparam name="TItem">コレクション内の要素の型</typeparam>
        /// <param name="target">[in ]：対象コレクション</param>
        /// <param name="source">[in ]：追加元コレクション(IEnumerable)</param>
        /// <remarks>
        /// 補足<br></br>
        /// ・対象コレクションが固定サイズや読み取り専用の場合は例外をスローします。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static void XAppend<TItem>(this ICollection<TItem> target, IEnumerable<TItem> source)
        {
            //------------------------------------------------------------
            /// 自分自身を追加しようとしている場合はバッファを経由する
            //------------------------------------------------------------
            if (ReferenceEquals(target, source))
            {                                                           //// 対称コレクションと追加元コレクションが同じインスタンスの場合
                var buffer = new Collection<TItem>();                   /////  バッファを生成する
                buffer.XAppend(source);                                 /////  バッファに追加元コレクションの全要素を追加する
                target.XAppend(buffer);                                 /////  バッファの全要素を対象インスタンスに追加する
                return;                                                 /////  関数終了
            }


            //------------------------------------------------------------
            /// 指定したコレクションの全要素を末尾に追加する
            //------------------------------------------------------------
            foreach (var item in source)
            {                                                           //// 追加元コレクションの全要素を繰り返す
                target.Add(item);                                       /////  対象コレクションに要素を追加する
            }
        }

        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod]
        public static void ZZZ_XAppend(IAutoTestExecuter ifExecuter)
        {
            var floatAppend = new float[] { 4.56f, 5.67f };     // floatコレクションの追加内容
            var strAppend = new string[] { "追加", "文字列" };  // stringコレクションの追加内容

            var floatResult = new float[] { 1.23f, 2.34f, 3.45f, 4.56f, 5.67f };        // floatコレクションの予想結果
            var strResult = new string[] { "ABC", "あいう", "漢字", "追加", "文字列" }; // stringコレクションの予想結果

            var floatTwice = new float[] { 1.23f, 2.34f, 3.45f, 1.23f, 2.34f, 3.45f };          // floatコレクションの倍化処理予想結果
            var strTwice = new string[] { "ABC", "あいう", "漢字", "ABC", "あいう", "漢字" };   // stringコレクションの倍化処理予想結果


            //------------------------------------------------------------
            /// floatコレクション
            //------------------------------------------------------------
            // Collection<float>
            {
                var testCol = new Collection<float>();
                testCol.Add(1.23f); testCol.Add(2.34f); testCol.Add(3.45f);
                AutoTest.TestX(XAppend, testCol, floatAppend, new Collection<float>(floatResult), ifExecuter, "Collection<float> に配列を追加");
            }

            // List<float>
            {
                var testList = new List<float>();
                testList.Add(1.23f); testList.Add(2.34f); testList.Add(3.45f);
                AutoTest.TestX(XAppend, testList, floatAppend, new List<float>(floatResult), ifExecuter, "List<float> に配列を追加");
            }

            // float[]
            {
                var testArray = new float[3] { 1.23f, 2.34f, 3.45f };
                AutoTest.TestX(XAppend, testArray, floatAppend, typeof(System.NotSupportedException), ifExecuter, "float[] に配列を追加：固定サイズなので例外");
            }


            //------------------------------------------------------------
            /// 文字列コレクション
            //------------------------------------------------------------
            // Collection<string>
            {
                var testCol = new Collection<string>();
                testCol.Add("ABC"); testCol.Add("あいう"); testCol.Add("漢字");
                AutoTest.TestX(XAppend, testCol, strAppend, new Collection<string>(strResult), ifExecuter, "Collection<string> に配列を追加");
            }

            // List<string>
            {
                var testList = new List<string>();
                testList.Add("ABC"); testList.Add("あいう"); testList.Add("漢字");
                AutoTest.TestX(XAppend, testList, strAppend, new List<string>(strResult), ifExecuter, "List<string> に配列を追加");
            }

            // string[]
            {
                var testArray = new string[3] { "ABC", "あいう", "漢字" };
                AutoTest.TestX(XAppend, testArray, strAppend, typeof(System.NotSupportedException), ifExecuter, "string[] に配列を追加：固定サイズなので例外");
            }


            //------------------------------------------------------------
            /// 自分自身を追加パターン
            //------------------------------------------------------------
            // Collection<float>
            {
                var testCol = new Collection<float>();
                testCol.Add(1.23f); testCol.Add(2.34f); testCol.Add(3.45f);
                AutoTest.TestX(XAppend, testCol, testCol, new Collection<float>(floatTwice), ifExecuter, "Collection<float> に自分自身を追加");
            }

            // Collection<string>
            {
                var testCol = new Collection<string>();
                testCol.Add("ABC"); testCol.Add("あいう"); testCol.Add("漢字");
                AutoTest.TestX(XAppend, testCol, testCol, new Collection<string>(strTwice), ifExecuter, "Collection<string> に自分自身を追加");
            }
        }
#endif


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コレクション一致チェック】２つのコレクションの内容が一致するかどうかをチェックします。
        /// </summary>
        /// <param name="colA">[in ]：コレクションA</param>
        /// <param name="colB">[in ]：コレクションB</param>
        /// <returns>
        /// チェック結果[true = 一致 / false = 不一致]
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・Object.Equals(Object, Object) と混同しないようにあえて名前を変えています。<br></br>
        /// ・Object.Equals(Object, Object) だと、同じ内容のコレクションでも不一致と判定します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static bool IsEqual(ICollection colA, ICollection colB)
        {
            //------------------------------------------------------------
            /// ２つのコレクションの内容が一致するかどうかをチェックする
            //------------------------------------------------------------
            if (ReferenceEquals(colA, colB))
            {                                                           //// コレクションAとBが同じインスタンスを参照している場合
                return true;                                            /////  戻り値 = true(一致) で関数終了
            }

            if (colA.Count != colB.Count)
            {                                                           //// コレクションAとBとで件数が異なる場合
                return false;                                           /////  戻り値 = false(不一致) で関数終了
            }

            bool success;
            var enmA = colA.GetEnumerator();                            //// コレクションAの列挙子を取得する
            var enmB = colB.GetEnumerator();                            //// コレクションBの列挙子を取得する

            while (true)
            {                                                           //// チェック終了まで繰り返す
                success = enmA.MoveNext(); enmB.MoveNext();             /////  列挙子AとBを次に進める
                if (success == false)
                {                                                       /////  最後の要素を通り過ぎた場合(つまり、すべての要素が一致した場合)
                    return true;                                        //////   戻り値 = true(一致) で関数終了
                }

                if (Equals(enmA.Current, enmB.Current) == false)
                {                                                       /////  AとBとで要素の内容が異なる場合
                    return false;                                       //////  戻り値 = false(不一致) で関数終了
                }
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【コレクション一致チェック】２つのコレクションの内容が一致するかどうかをチェックします。
        /// </summary>
        /// <param name="target">[in ]：対象コレクション</param>
        /// <param name="other"> [in ]：比較相手コレクション</param>
        /// <returns>
        /// チェック結果[true = 一致 / false = 不一致]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static bool XEquals(this ICollection target, ICollection other) => IsEqual(target, other);

        //--------------------------------------------------------------------------------
        // 手動テスト用メソッド(構文の異なるパターンもテストするため自動化はできない)
        //--------------------------------------------------------------------------------
#if DEBUG
        [ManualTestMethod]
        public static void ZZZ_XEquals()
        {
            //------------------------------------------------------------
            /// floatコレクション
            //------------------------------------------------------------
            {
                var testAry = new float[] { 1.23f, 2.34f, 3.45f };  // float配列
                var testCol = new Collection<float>();              // floatコレクション
                var testLst = new List<float>();                    // floatリスト

                SetTestData();
                Debug.Print("内容が一致するパターン…戻り値は true");
                Test();

                SetTestData(); testCol[1] = 0f; testLst[1] = 9f;
                Debug.Print("インデックス１の内容が一致しないパターン…戻り値は false");
                Test();

                SetTestData(); testCol.Add(0f); testLst.RemoveAt(0);
                Debug.Print("要素数が一致しないパターン…戻り値は false");
                Test();

                Debug.Print("同一インスタンスを比較するパターン…戻り値は true");
                Debug.Indent();
                Debug.Print("IsEqual(配列        , 配列        ):" + IsEqual(testAry, testAry));
                Debug.Print("IsEqual(コレクション, コレクション):" + IsEqual(testCol, testCol));
                Debug.Print("IsEqual(リスト      , リスト      ):" + IsEqual(testLst, testLst));
                Debug.Unindent();
                Debug.Print("");

                void SetTestData()
                {
                    testCol.Clear(); testCol.Add(1.23f); testCol.Add(2.34f); testCol.Add(3.45f);
                    testLst.Clear(); testLst.Add(1.23f); testLst.Add(2.34f); testLst.Add(3.45f);
                }

                void Test()
                {
                    Debug.Indent();
                    Debug.Print("配列の内容        :" + AutoTest.ToDisplayString(testAry));
                    Debug.Print("コレクションの内容:" + AutoTest.ToDisplayString(testCol));
                    Debug.Print("リストの内容      :" + AutoTest.ToDisplayString(testLst));
                    Debug.Print("IsEqual(配列, コレクション  ):" + IsEqual(testAry, testCol));
                    Debug.Print("IsEqual(配列, リスト        ):" + IsEqual(testAry, testLst));
                    Debug.Print("IsEqual(コレクション, リスト):" + IsEqual(testCol, testLst));
                    Debug.Print("配列.Equals(コレクション)    :" + testAry.XEquals(testCol));
                    Debug.Print("配列.Equals(リスト)          :" + testAry.XEquals(testLst));
                    Debug.Print("コレクション.Equals(リスト)  :" + testCol.XEquals(testLst));
                    Debug.Unindent();
                    Debug.Print("");
                }
            }

            //------------------------------------------------------------
            /// 文字列コレクション
            //------------------------------------------------------------
            {

                var testAry = new string[] { "ABC", "あいう", "漢字" }; // 文字列配列
                var testCol = new Collection<string>();                 // 文字列コレクション
                var testLst = new List<string>();                       // 文字列リスト

                SetTestData();
                Debug.Print("内容が一致するパターン…戻り値は true");
                Test();

                SetTestData(); testCol[1] = "変更"; testLst[1] = "差し替え";
                Debug.Print("インデックス１の内容が一致しないパターン…戻り値は false");
                Test();

                SetTestData(); testCol.Add("追加"); testLst.RemoveAt(0);
                Debug.Print("要素数が一致しないパターン…戻り値は false");
                Test();

                Debug.Print("同一インスタンスを比較するパターン…戻り値は true");
                Debug.Indent();
                Debug.Print("IsEqual(配列        , 配列        ):" + IsEqual(testAry, testAry));
                Debug.Print("IsEqual(コレクション, コレクション):" + IsEqual(testCol, testCol));
                Debug.Print("IsEqual(リスト      , リスト      ):" + IsEqual(testLst, testLst));
                Debug.Unindent();
                Debug.Print("");

                void SetTestData()
                {
                    testCol.Clear(); testCol.Add("ABC"); testCol.Add("あいう"); testCol.Add("漢字");
                    testLst.Clear(); testLst.Add("ABC"); testLst.Add("あいう"); testLst.Add("漢字");
                }

                void Test()
                {
                    Debug.Indent();
                    Debug.Print("配列の内容        :" + AutoTest.ToDisplayString(testAry));
                    Debug.Print("コレクションの内容:" + AutoTest.ToDisplayString(testCol));
                    Debug.Print("リストの内容      :" + AutoTest.ToDisplayString(testLst));
                    Debug.Print("IsEqual(配列, コレクション  ):" + IsEqual(testAry, testCol));
                    Debug.Print("IsEqual(配列, リスト        ):" + IsEqual(testAry, testLst));
                    Debug.Print("IsEqual(コレクション, リスト):" + IsEqual(testCol, testLst));
                    Debug.Print("配列.Equals(コレクション)    :" + testAry.XEquals(testCol));
                    Debug.Print("配列.Equals(リスト)          :" + testAry.XEquals(testLst));
                    Debug.Print("コレクション.Equals(リスト)  :" + testCol.XEquals(testLst));
                    Debug.Unindent();
                    Debug.Print("");
                }
            }

            //------------------------------------------------------------
            /// float配列とint配列の比較
            //------------------------------------------------------------
            {
                var floatAry = new float[] { 123f, 234f, 345f };    // float配列
                var intAry = new int[] { 123, 234, 345 };           // int配列

                Debug.Print("要素の型が一致しないパターン…戻り値は false");
                Debug.Indent();
                Debug.Print("IsEqual(float配列, int配列):" + IsEqual(floatAry, intAry));
                Debug.Print("float配列.Equals(int配列)  :" + floatAry.XEquals(intAry));
                Debug.Print("int配列.Equals(float配列)  :" + intAry.XEquals(floatAry));
                Debug.Unindent();
            }
        }
#endif

    }
}
