//
// 【補足メモ】XmlUtil.ToXmlString メソッドの改良内容について
//

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【補足メモ】<see cref="XmlUtil.ToXmlString"/> メソッドの改良内容について<br/>
    /// ・以前は引数にジェネリック型を使っていたが、シンプルに object 型を使うようにした。<br/>
    /// ・ジェネリック型を使った場合、<see cref="ZZZNameOnly.ToXmlString"/> のようなショートカットを
    ///   基本クラスでのみ定義し、それを派生クラスのインスタンスから呼び出した場合に、例外が発生する。<br/>
    /// ・派生クラス側でも同様のショートカットメソッドを定義することで回避することもできるが、
    ///   <see cref="XmlUtil.ToXmlString"/> を改良したほうが手っ取り早いので改良した。<br/>
    /// </summary>
    /// <remarks>
    /// 補足<br/>
    /// ・改良前のロジックは <see cref="OldToXmlString"/> を参照。<br/>
    /// </remarks>
    //====================================================================================================
#if DEBUG
    public partial class ZZZ_Remarks_ToXmlStringImprovement
    {
        //====================================================================================================
        // 手動テストで実際の動作を確認可能
        //====================================================================================================
        [ManualTestMethod("補足：XmlUtil.ToXmlString メソッドの改良内容について")]
        public static void Sample()
        {
            var nameOnly = new ZZZNameOnly("Taro Yamada");
            var nameAndAge = new ZZZNameAndAge("Jiro Yamada", 23);


            //------------------------------------------------------------
            // 改善後
            //------------------------------------------------------------
            // 改善後：XmlUtilクラスのメソッドを使って基本クラスのインスタンスからXML文字列作成
            {
                var xmlString = XmlUtil.ToXmlString(nameOnly);
                Debug.Print(xmlString);
                var parse = XmlUtil.ParseXmlString<ZZZNameOnly>(xmlString);
                EqualCheck(nameOnly, parse);
            }

            // 改善後：XmlUtilクラスのメソッドを使って派生クラスのインスタンスからXML文字列作成
            {
                var xmlString = XmlUtil.ToXmlString(nameAndAge);
                Debug.Print(xmlString);
                var parse = XmlUtil.ParseXmlString<ZZZNameAndAge>(xmlString);
                EqualCheck(nameAndAge, parse);
            }

            // 改善後：ショートカットメソッドを使って基本クラスのインスタンスからXML文字列作成
            {
                var xmlString = nameOnly.ToXmlString();
                Debug.Print(xmlString);
                var parse = XmlUtil.ParseXmlString<ZZZNameOnly>(xmlString);
                EqualCheck(nameOnly, parse);
            }

            // 改善後：ショートカットメソッドを使って派生クラスのインスタンスからXML文字列作成
            {
                var xmlString = nameAndAge.ToXmlString();
                Debug.Print(xmlString);
                var parse = XmlUtil.ParseXmlString<ZZZNameAndAge>(xmlString);
                EqualCheck(nameAndAge, parse);
            }


            //------------------------------------------------------------
            // 改善前
            //------------------------------------------------------------
            // 改善前：XmlUtilクラスのメソッドを使って基本クラスのインスタンスからXML文字列作成
            {
                var xmlString = OldToXmlString(nameOnly);
                Debug.Print(xmlString);
                var parse = XmlUtil.ParseXmlString<ZZZNameOnly>(xmlString);
                EqualCheck(nameOnly, parse);
            }

            // 改善前：XmlUtilクラスのメソッドを使って派生クラスのインスタンスからXML文字列作成
            {
                var xmlString = OldToXmlString(nameAndAge);
                Debug.Print(xmlString);
                var parse = XmlUtil.ParseXmlString<ZZZNameAndAge>(xmlString);
                EqualCheck(nameAndAge, parse);
            }

            // 改善前：ショートカットメソッドを使って基本クラスのインスタンスからXML文字列作成
            {
                var xmlString = nameOnly.OldToXmlString();
                Debug.Print(xmlString);
                var parse = XmlUtil.ParseXmlString<ZZZNameOnly>(xmlString);
                EqualCheck(nameOnly, parse);
            }

            // 改善前：ショートカットメソッドを使って派生クラスのインスタンスからXML文字列作成 -> 基本クラスとしてXML文字列を作成しようとするため例外
            try
            {
                var xmlString = nameAndAge.OldToXmlString();
                Debug.Print(xmlString);
                var parse = XmlUtil.ParseXmlString<ZZZNameAndAge>(xmlString);
                EqualCheck(nameAndAge, parse);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.GetType().ToString() + "：" + ex.Message);
            }


            //------------------------------------------------------------
            // 【ローカル関数】同一内容チェック
            //------------------------------------------------------------
            void EqualCheck<TObject>(TObject obj1,  // [in ]：オブジェクト１
                                     TObject obj2)  // [in ]：オブジェクト２
            {
                if (obj1.Equals(obj2))
                {
                    Debug.Print($"OK {obj1}:{obj2}");
                }
                else
                {
                    Debug.Print($"不一致 {obj1}:{obj2}");
                }

                Debug.Print("");
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【XML文字列作成】オブジェクトの内容からXML文字列を作成します。
        /// </summary>
        /// <typeparam name="TObject">オブジェクトの型</typeparam>
        /// <param name="source">[in ]：変換元オブジェクト</param>
        /// <returns>
        /// XML文字列
        /// </returns>
        /// <remarks>
        /// ・ジェネリック型を使っていた頃の <see cref="XmlUtil.ToXmlString"/><br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string OldToXmlString<TObject>(TObject source) where TObject : class
        {
            //------------------------------------------------------------
            /// オブジェクトの内容からXML文字列を作成する
            //------------------------------------------------------------
            var strWriter = new StringWriter();                         //// StringWriter を生成する
            var xmlWriter = new XmlSerializer(typeof(TObject));         //// XMLシリアライザーを生成する
            xmlWriter.Serialize(strWriter, source);                     //// 変換元オブジェクトの内容をシリアル化して StringWriter へ書き出す
            return strWriter.ToString();                                //// 戻り値 = XML文字列 で関数終了
        }


        //================================================================================
        /// <summary>
        /// 【基本クラスのサンプル】
        /// </summary>
        //================================================================================
        public partial class ZZZNameOnly
        {
            public string name;

            public override string ToString() => $"[{name}]";

            // 改善後の ToXmlString へのショートカット
            public string ToXmlString() => XmlUtil.ToXmlString(this);

            // 改善前の ToXmlString へのショートカット
            public string OldToXmlString() => ZZZ_Remarks_ToXmlStringImprovement.OldToXmlString(this);

            public override bool Equals(object obj)
            {
                if (obj is ZZZNameOnly other)
                {
                    return (this.name == other.name);
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return name.GetHashCode();
            }

            public ZZZNameOnly(string name)
            {
                this.name = name;
            }

            protected ZZZNameOnly() { }

        } // class ZZZNameOnly


        //================================================================================
        /// <summary>
        /// 【派生クラスのサンプル】
        /// </summary>
        //================================================================================
        public partial class ZZZNameAndAge : ZZZNameOnly
        {
            public int age;

            public override string ToString() => $"[{name}({age})]";

            public override bool Equals(object obj)
            {
                if (base.Equals(obj) == false)
                {
                    return false;
                }

                if (obj is ZZZNameAndAge other)
                {
                    return (this.age == other.age);
                }
                else
                {
                    return false;
                }
            }

            public override int GetHashCode()
            {
                return base.GetHashCode() ^ age.GetHashCode();
            }

            public ZZZNameAndAge(string name, int age) : base(name)
            {
                this.age = age;
            }

            protected ZZZNameAndAge() { }

        } // class ZZZNameAndAge

    } // class ZZZToXmlStringImprovement
#endif

} // namespace
