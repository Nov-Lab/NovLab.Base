﻿// @(h)KeyedList.cs ver 0.00 ( '22.06.30 Nov-Lab ) 作成開始
// @(h)KeyedList.cs ver 0.51 ( '22.07.03 Nov-Lab ) ベータ版完成
// @(h)KeyedList.cs ver 0.51a( '22.10.18 Nov-Lab ) その他  ：デバッグ関数を微修正

// @(s)
// 　【キー付きリスト】キーとインデックスによってアクセスでき、順序管理や並べ替えもできる、キー付き要素のリストです。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using NovLab.DebugSupport;
using NovLab.Globalization;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【キー付き要素I/F】キー値が埋め込まれているオブジェクトにおいて、キー値へのアクセス機能を提供します。
    /// </summary>
    /// <typeparam name="TKey">キー値の型</typeparam>
    //====================================================================================================
    public interface IKeyedItem<TKey>
    {
        //====================================================================================================
        // プロパティアクセサー
        //====================================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【キー値(読み取り専用)】このオブジェクトが持つキー値を取得します。
        /// </summary>
        //--------------------------------------------------------------------------------
        TKey KeyValue { get; }


        // ＜メモ＞
        // ・C#8.0以上であれば、インターフェイスでも static メソッドを定義できるらしい。
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【キー値の等値比較子(読み取り専用)】キー値の等値比較子を取得します。
        /// </summary>
        /// <remarks>
        /// 補足<br></br>
        /// ・キーの型の既定の等値比較子を使用する場合は null を返すようにします。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        IEqualityComparer<TKey> KeyEqualityComparer { get; }
    }


    // ＜メモ＞
    // ・コレクションクラスの比較
    //   キー付きリスト              ：キーとインデックスの両方でアクセスでき、キー値の扱い方は要素クラス側で管理し、ソートやXMLシリアル化にも対応
    //   KeyedCollection             ：キーとインデックスの両方でアクセスできるが、キー値の扱い方はコレクション側で管理し、ソート機能はない
    //   List<T>                     ：キーでのアクセスができない
    //   Dictionary<TKey, TValue>    ：インデックスでのアクセスができない。XMLシリアル化にも対応していない？
    //   KeyedByTypeCollection<TItem>：要素そのものがキーを兼ねる
    //   OrderedDictionary           ：キーと要素が別管理なので不整合に気を付けなくてはならない上に、キーも要素もObject型なので型変換にも注意が必要
    //====================================================================================================
    /// <summary>
    /// 【キー付きリスト】キーとインデックスによってアクセスでき、順序管理や並べ替えもできる、キー付き要素のリストです。
    /// </summary>
    /// <typeparam name="TKey"> キー値の型</typeparam>
    /// <typeparam name="TItem">リスト要素の型</typeparam>
    /// <remarks>
    /// 補足<br></br>
    /// ・KeyedCollection をベースに、要素クラス自体がキー値の扱い方を管理できるようにし、ソート機能などを追加したものです。<br></br>
    /// </remarks>
    //====================================================================================================
    [Serializable]
    public class KeyedList<TKey, TItem> : KeyedCollection<TKey, TItem> where TItem : IKeyedItem<TKey>, new()
    {
        //====================================================================================================
        // コンストラクター
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【既定のコンストラクター】キー付きリストを生成します。
        /// </summary>
        //--------------------------------------------------------------------------------
        public KeyedList() : base((new TItem()).KeyEqualityComparer) { }


        //====================================================================================================
        // KeyedCollection 抽象メソッドの実装
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【キー値抽出】リスト要素からキー値を抽出します。
        /// </summary>
        /// <param name="item">[in ]：リスト要素</param>
        /// <returns>
        /// キー値
        /// </returns>
        //--------------------------------------------------------------------------------
        protected override TKey GetKeyForItem(TItem item)
        {
            return item.KeyValue;
        }


        //====================================================================================================
        // 公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【キー値からインデックスを取得】キー値に対応する要素のインデックスを取得します。
        /// </summary>
        /// <param name="key">[in ]：キー値</param>
        /// <returns>
        /// インデックス[-1 = 見つからなかった]
        /// </returns>
        //--------------------------------------------------------------------------------
        public int IndexOfKey(TKey key)
        {
            //------------------------------------------------------------
            /// キー値に対応する要素のインデックスを取得する
            //------------------------------------------------------------
            try
            {                                                           //// try開始
                var item = this[key];                                   /////  キー値に対応する要素を取得する(見つからない場合は例外)
                return IndexOf(item);                                   /////  戻り値 = 要素に対応するインデックス で関数終了

            }
            catch (KeyNotFoundException)
            {                                                           //// catch：キー値に対応する要素が見つからなかった場合
                return -1;                                              /////  戻り値 = -1(見つからなかった) で関数終了
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ソート】TItem の既定の比較子を使用して全要素を並べ替えます。
        /// </summary>
        /// <remarks>
        /// 補足<br></br>
        /// ・このメソッドを使用する場合、TItem は IComparable インターフェイスを実装している必要があります。
        /// </remarks>
        //--------------------------------------------------------------------------------
        public void Sort()
        {
            //------------------------------------------------------------
            /// TItem の既定の比較子を使用して全要素を並べ替える
            //------------------------------------------------------------
            var tempList = new List<TItem>(this);                       //// このインスタンスの内容をコピーして作業用リストを生成する
            tempList.Sort();                                            //// 既定の比較子を使用して作業用リストをソートする
            this.Clear(); this.XAppend(tempList);                       //// ソート結果をこのインスタンスに反映する
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【ソート(比較メソッド指定)】比較メソッドを指定して全要素を並べ替えます。
        /// </summary>
        /// <param name="comparison">[in ]：比較メソッド</param>
        //--------------------------------------------------------------------------------
        public void Sort(Comparison<TItem> comparison)
        {
            //------------------------------------------------------------
            /// 比較メソッドを指定して全要素を並べ替える
            //------------------------------------------------------------
            var tempList = new List<TItem>(this);                       //// このインスタンスの内容をコピーして作業用リストを生成する
            tempList.Sort(comparison);                                  //// 比較用メソッドを指定して作業用リストをソートする
            this.Clear(); this.XAppend(tempList);                       //// ソート結果をこのインスタンスに反映する
        }

    }




    // 手動テスト用のクラス
#if DEBUG
    //====================================================================================================
    // KeyedList の手動テスト用クラス
    //====================================================================================================
    public class ZZZTest_KeyedList
    {
        [ManualTestMethod("KeyedList の総合的テスト")]
        public static void ZZZ_OverallTest()
        {
            // 在庫フルーツリストを作成
            var stockFruits = new KeyedList<string, ZZZFruits>();
            stockFruits.Add(new ZZZFruits("Apple", 150));
            stockFruits.Add(new ZZZFruits("Peach", 300));
            stockFruits.Add(new ZZZFruits("Orange", 100));
            stockFruits.Add(new ZZZFruits("みかん", 130));
            try { stockFruits.Add(new ZZZFruits("ミカン", 140)); } catch { }        // テキスト比較の場合は"みかん"と重複する
            try { stockFruits.Add(new ZZZFruits("ＯＲＡＮＧＥ", 90)); } catch { }   // テキスト比較の場合は"Orange"と重複する
            try { stockFruits.Add(new ZZZFruits("orange", 110)); } catch { }        // 大文字と小文字を区別しない場合は"Orange"と重複する
            stockFruits.Insert(0, new ZZZFruits("Muscat", 800));                    // 先頭位置へ挿入


            // リスト内要素に対するキー値変更のテスト
            //stockFruits["Orange"].Name = "みかん"; // リスト内のある要素のキー値を、他の要素と重複する内容に変更することはできない
            //stockFruits["Apple"].Name = "りんご";  // 他の要素と重複しない内容ならば変更できる


            // 入荷フルーツリストを作成
            var arrivalFruits = new KeyedList<string, ZZZFruits>();
            arrivalFruits.Add(new ZZZFruits("Melon", 1200));
            arrivalFruits.Add(new ZZZFruits("Mango", 900));


            // XICollection.XAppend のテスト：在庫フルーツリストに入荷フルーツリストを追加
            stockFruits.XAppend(arrivalFruits);
            M_PrintList(stockFruits);
            M_Test_KeySearch(stockFruits);


            Debug.Print("●既定の比較子(名前)でソート");
            stockFruits.Sort();
            M_PrintList(stockFruits);
            M_Test_KeySearch(stockFruits);


            Debug.Print("●価格でソート");
            stockFruits.Sort(ZZZFruits.CompareByPrice);
            M_PrintList(stockFruits);
            M_Test_KeySearch(stockFruits);


            Debug.Print("●XML文字列経由で完全コピー");
            var fullcloneFruits = XmlUtil.FullClone(stockFruits);   // 完全コピーを生成
            fullcloneFruits["Orange"].Price = 12345;                // Orangeの価格を変更
            fullcloneFruits.Remove("ミカン");                       // ミカンを削除
            fullcloneFruits.Add(new ZZZFruits("Grape", 500));       // Grapeを追加
            fullcloneFruits.Sort(ZZZFruits.CompareByPrice);         // 価格でソート

            Debug.Print("＜完全コピー後に内容を変更：Orangeの価格を変更, ミカンを削除, Grapeを追加＞");
            M_PrintList(fullcloneFruits);
            M_Test_KeySearch(fullcloneFruits);

            Debug.Print("＜コピー元＞完全コピーに対する変更は、コピー元には影響しない");
            M_PrintList(stockFruits);
            M_Test_KeySearch(stockFruits);


            // キー検索のテスト
            void M_Test_KeySearch(KeyedList<string, ZZZFruits> list)
            {
                M_PrintItem(list, "orange");
                M_PrintItem(list, "Orange");
                M_PrintItem(list, "ＯＲＡＮＧＥ");
                M_PrintItem(list, "ミカン");
                M_PrintItem(list, "kiwi");
                Debug.Print("");
            }


            // フルーツリストをデバッグ出力
            void M_PrintList(KeyedList<string, ZZZFruits> list)
            {
                foreach (var item in list)
                {
                    Debug.Print(item.ToString());
                }
                Debug.Print("");
            }


            // 名前に対応するフルーツ情報をデバッグ出力
            void M_PrintItem(KeyedList<string, ZZZFruits> list, string name)
            {
                string findItemDesc;

                try
                {
                    findItemDesc = " [" + list[name].ToString() + "]";
                }
                catch (KeyNotFoundException)
                {
                    findItemDesc = "";
                }

                Debug.Print("Index of " + name + " = " + list.IndexOfKey(name) + findItemDesc);
            }
        }

    }
#endif


    // 手動テスト用のクラス
#if DEBUG
    //====================================================================================================
    /// <summary>
    /// 【フルーツ情報】果物の名前と価格をペアで管理します。
    /// </summary>
    //====================================================================================================
    public class ZZZFruits : IKeyedItem<string>, IComparable<ZZZFruits>
    {
        //====================================================================================================
        // 公開フィールド
        //====================================================================================================
        /// <summary>
        /// 【名前】
        /// </summary>
        public string Name;

        /// <summary>
        /// 【価格】
        /// </summary>
        public int Price;

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列形式作成】このインスタンスの内容を表す文字列を取得します。
        /// </summary>
        /// <returns>文字列形式</returns>
        //--------------------------------------------------------------------------------
        public override string ToString() => Name + " \\" + Price;


        //====================================================================================================
        // IKeyedItem I/F の実装とキー値操作
        //====================================================================================================
        /// <summary>
        /// 【キー値(読み取り専用)】キー値(名前)を取得します。
        /// </summary>
        string IKeyedItem<string>.KeyValue => Name;


        /// <summary>
        /// 【キー値の等値比較子(読み取り専用)】キー値の等値比較子を取得します。
        /// </summary>
        /// <remarks>
        /// 参考メモ：<see cref="ZZZ_Memo_EqualityComparer"/>
        /// </remarks>
        IEqualityComparer<string> IKeyedItem<string>.KeyEqualityComparer
        {
            get
            {
                // キー値の扱い方はここで変更できる
                return EqualityComparer<string>.Default;    // 完全序数比較(大文字と小文字・ひらがなとカタカナ・半角と全角を区別し、カルチャに依存しない)の場合はこちら
                //return StringComparer.OrdinalIgnoreCase;    // 大文字と小文字を区別しない序数比較(ファイル名・アカウント名など)
                //return new TextComparer();                  // テキスト比較(大文字と小文字・ひらがなとカタカナ・半角と全角を区別せず、カルチャに依存する)場合はこちら
            }
        }


        // ＜メモ＞
        // ・フルーツ情報全体での等値チェックは省略しているが、必要な場合は IEqualityComparer<ZZZFruits> も実装すること。
        //   その際、名前の等値チェックは KeyEqualityComparer を用いて整合性を保つこと。


        //====================================================================================================
        // コンストラクター
        //====================================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【既定のコンストラクター】既定の内容でフルーツ情報を生成します。
        /// </summary>
        /// <remarks>
        /// 補足<br></br>
        /// ・<see cref="KeyedList{TKey, TItem}"/> におけるリスト要素の型制約を満たすために必要です。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public ZZZFruits() { }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【完全コンストラクター】すべての内容を指定してフルーツ情報を生成します。
        /// </summary>
        /// <param name="Name"> [in ]：名前</param>
        /// <param name="Price">[in ]：価格</param>
        //--------------------------------------------------------------------------------
        public ZZZFruits(string Name, int Price)
        {
            this.Name = Name;
            this.Price = Price;
        }


        //====================================================================================================
        // IComparable I/F の実装
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【比較】他のインスタンスと内容を比較します。
        /// 名前->価格の２段階で比較します。
        /// </summary>
        /// <param name="other">[in ]：比較相手</param>
        /// <returns>
        /// 比較結果値[0より小さい = 比較相手よりも小さい、0 = 比較相手と等しい、0より大きい = 比較相手よりも大きい]
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・IComparable&lt;T&gt;.CompareTo(T) の実装です。ソート処理に使用します。
        /// </remarks>
        //--------------------------------------------------------------------------------
        int IComparable<ZZZFruits>.CompareTo(ZZZFruits other)
        {
            //------------------------------------------------------------
            /// 他のインスタンスと内容を比較する
            //------------------------------------------------------------
            var result = CompareByName(this, other);                    //// 名前で比較する
            if (result != 0)
            {                                                           //// 名前が同一でない場合
                return result;                                          /////  戻り値 = 比較結果値 で関数終了
            }
            else
            {                                                           //// 名前が同一な場合
                return CompareByPrice(this, other);                     /////  価格での比較結果を戻り値とし、関数終了
            }
        }


        //====================================================================================================
        // 比較メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【名前で比較】２つのフルーツ情報を名前で比較します。
        /// String.CompareTo により、単語 (大文字/小文字を区別し、カルチャに依存した) 比較を実行します。
        /// </summary>
        /// <param name="info1">[in ]：フルーツ情報１</param>
        /// <param name="info2">[in ]：フルーツ情報２</param>
        /// <returns>
        /// 比較結果値[0より小さい = 情報１＜情報２、0 = 情報１＝情報２、0より大きい = 情報１＞情報２]
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・System.Comparison&lt;T&gt; デリゲートに合致するメソッドです。ソート処理に使用します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static int CompareByName(ZZZFruits info1, ZZZFruits info2) => info1.Name.CompareTo(info2.Name);


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【価格で比較】２つのフルーツ情報を価格で比較します。
        /// </summary>
        /// <param name="info1">[in ]：フルーツ情報１</param>
        /// <param name="info2">[in ]：フルーツ情報２</param>
        /// <returns>
        /// 比較結果値[0より小さい = 情報１＜情報２、0 = 情報１＝情報２、0より大きい = 情報１＞情報２]
        /// </returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・System.Comparison&lt;T&gt; デリゲートに合致するメソッドです。ソート処理に使用します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static int CompareByPrice(ZZZFruits info1, ZZZFruits info2) => info1.Price.CompareTo(info2.Price);

    }
#endif




#if false   //[-] 保留：下書き中のままなので、有効化する場合は要調整
    //====================================================================================================
    /// <summary>
    /// 【キー付きリスト・staticメソッド】キー付きリストの各種操作メソッドを提供します。
    /// </summary>
    //====================================================================================================
    public static partial class KeyedList
    {
        // ＜メモ＞
        // ・必要になったら KeyedList.NullSafeRemove なども作る。
        //====================================================================================================
        // null対応キー付きリスト操作メソッド
        // ・件数０のリストは xml シリアル化を省略したい
        //   → 件数０のリストは null インスタンスにしたい
        //   → 追加・件数取得・foreach ループを行う際に、null チェックを省略したい
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【null対応キー付きリスト要素追加】
        /// キー付きリストに要素を追加します。リストを生成していない場合(インスタンスがnullの場合)は生成してから追加します。
        /// </summary>
        /// <param name="list">[ref]：キー付きリスト(null の場合は生成する)</param>
        /// <param name="item">[in ]：キー付き要素</param>
        //--------------------------------------------------------------------------------
        public static void NullSafeAdd<TKey, TItem>(ref KeyedList<TKey, TItem> list, TItem item) where TItem : IKeyedItem<TKey>
        {
            //------------------------------------------------------------
            /// キー付きリストに要素を追加する
            //------------------------------------------------------------
            if (list == null)
            {                                                           //// キー付きリスト = null の場合
                list = new KeyedList<TKey, TItem>();                    /////  キー付きリストを生成する
            }

            list.Add(item);                                             //// リストに要素を追加する
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【null対応キー付きリスト全要素配列取得】
        /// キー付きリストの全要素をコピーした新しい配列を取得します。リストが null の場合は要素数0の配列を返します。
        /// </summary>
        /// <param name="list">[in ]：キー付きリスト</param>
        /// <returns>
        /// キー付きリストの全要素をコピーした新しい配列
        /// </returns>
        /// <remarks>
        /// ・nullチェックを省略して件数取得や foreach ループを行うことができます。
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static TItem[] NullSafeGetArray<TKey, TItem>(KeyedList<TKey, TItem> list) where TItem : IKeyedItem<TKey>
        {
            //------------------------------------------------------------
            /// キー付きリストの全要素をコピーした新しい配列を取得する
            //------------------------------------------------------------
            if (list == null)
            {                                                           //// キー付きリスト = null の場合
                return new TItem[0];                                    /////  戻り値 = 要素数0の配列 で関数終了
            }
            else
            {                                                           //// キー付きリスト = null でない場合
                return list.ToArray();                                  /////  戻り値 = キー付きリストの全要素をコピーした新しい配列 で関数終了
            }
        }
    }
#endif

}
