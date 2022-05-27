// @(h)XmlUtil.cs ver 0.00 ( '22.05.08 Nov-Lab ) 作成開始
// @(h)XmlUtil.cs ver 0.51 ( '22.05.08 Nov-Lab ) ベータ版完成
// @(h)XmlUtil.cs ver 0.51a( '22.05.24 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【XMLユーティリティー】定型的なXML操作機能を提供します。

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【XMLユーティリティー】定型的なXML操作機能を提供します。
    /// </summary>
    //====================================================================================================
    public class XmlUtil
    {
        //[-] 保留：必要になったらファイル入出力操作系メソッドも作成する。


        //====================================================================================================
        // XML文字列操作
        //====================================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【XML文字列解析】XML文字列を解析してオブジェクトに変換します。
        /// </summary>
        /// <typeparam name="TObject">オブジェクトの型</typeparam>
        /// <param name="xmlString">[in ]：XML文字列</param>
        /// <returns>
        /// 変換結果[null = 解析失敗]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static TObject ParseXmlString<TObject>(string xmlString) where TObject : class
        {
            //------------------------------------------------------------
            /// XML文字列を解析してオブジェクトに変換する
            //------------------------------------------------------------
            var strReader = new StringReader(xmlString);                //// XML文字列から読み取る StringReader を生成する
            var xmlReader = new XmlSerializer(typeof(TObject));         //// XMLシリアライザーを生成する

            try
            {                                                           //// try開始
                var result = xmlReader.Deserialize(strReader);          /////  XML文字列を逆シリアル化して変換結果を取得する
                return (TObject)result;                                 /////  戻り値 = 変換結果 で関数終了
            }
            catch
            {                                                           //// catch:すべての例外
                return null;                                            /////  戻り値 = null(解析失敗) で関数終了
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
        //--------------------------------------------------------------------------------
        public static string ToXmlString<TObject>(TObject source) where TObject : class
        {
            //------------------------------------------------------------
            /// オブジェクトの内容からXML文字列を作成する
            //------------------------------------------------------------
            var strWriter = new StringWriter();                         //// StringWriter を生成する
            var xmlWriter = new XmlSerializer(typeof(TObject));         //// XMLシリアライザーを生成する
            xmlWriter.Serialize(strWriter, source);                     //// 変換元オブジェクトの内容をシリアル化して StringWriter へ書き出す
            return strWriter.ToString();                                //// 戻り値 = XML文字列 で関数終了
        }

    }
}
