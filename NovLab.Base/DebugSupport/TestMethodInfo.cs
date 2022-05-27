// @(h)TestMethodInfo.cs ver 0.00 ( '22.05.18 Nov-Lab ) 下書きを元に作成開始
// @(h)TestMethodInfo.cs ver 0.21 ( '22.05.19 Nov-Lab ) アルファ版完成
// @(h)TestMethodInfo.cs ver 0.21a( '22.05.25 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【テスト用メソッド情報】テスト用メソッドを扱うために必要な情報を管理します。

using System;
using System.Reflection;
using System.Collections.ObjectModel;


namespace NovLab.DebugSupport
{

#if DEBUG   // DEBUGビルドのみ有効
    //====================================================================================================
    /// <summary>
    /// 【テスト用メソッド情報】テスト用メソッドを扱うために必要な情報を管理します。
    /// </summary>
    //====================================================================================================
    public class TestMethodInfo
    {
        //====================================================================================================
        // 公開フィールド
        //====================================================================================================
        /// <summary>
        /// 【メソッド情報(読み取り専用)】
        /// </summary>
        public readonly MethodInfo methodInfo;

        /// <summary>
        /// 【テスト用メソッド基底属性(読み取り専用)】実体は <see cref="ManualTestMethodAttribute"/> などの派生クラスです。
        /// </summary>
        public readonly BaseTestMethodAttribute attributeInfo;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【完全コンストラクター】すべての情報を指定してテスト用メソッド情報を生成します。
        /// </summary>
        /// <param name="methodInfo">   [in ]：メソッド情報</param>
        /// <param name="attributeInfo">[in ]：テスト用メソッド基底属性</param>
        //--------------------------------------------------------------------------------
        public TestMethodInfo(MethodInfo methodInfo, BaseTestMethodAttribute attributeInfo)
        {
            this.methodInfo = methodInfo;
            this.attributeInfo = attributeInfo;
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【表示名(読み取り専用)】テスト用メソッドの表示名です。
        /// </summary>
        //--------------------------------------------------------------------------------
        public string DisplayText
        {
            get
            {
                //------------------------------------------------------------
                /// テスト用メソッドの表示名を取得する
                //------------------------------------------------------------
                if (attributeInfo.displayText.XIsValid())
                {                                                           //// テスト用メソッド基底属性で表示名が指定されている場合
                    return attributeInfo.displayText;                       /////  戻り値 = 表示名 で関数終了
                }
                else
                {                                                           //// テスト用メソッド基底属性で表示名が指定されていない場合
                    var methodName = methodInfo.Name;                       /////  メソッド名を取得する
                    methodName =                                            /////  メソッド名のプリフィックスに目印文字列"ZZZ_"がついている場合は削除する
                        methodName.XRemoveStart("ZZZ_", true, null);
                    return methodInfo.DeclaringType.Name + "." +            /////  戻り値 = <クラス名>.<メソッド名> で関数終了
                           methodName;
                }
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列化】テスト用メソッドの表示名を取得します。リストボックスでの一行表示に使用します。
        /// </summary>
        /// <returns>文字列形式(表示名)</returns>
        /// <remarks>
        /// 補足<br></br>
        /// ・自動テストの場合はプリフィックス「自動テスト：」を付加します。<br></br>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public override string ToString()
        {
            //------------------------------------------------------------
            /// テスト用メソッドの表示名を取得する
            //------------------------------------------------------------
            if (attributeInfo is AutoTestMethodAttribute)
            {                                                           //// 自動テスト用メソッドの場合
                return "自動テスト：" + DisplayText;                    /////  戻り値 = "自動テスト：<表示名>" で関数終了
            }
            else
            {                                                           //// 自動テスト用メソッドでない場合
                return DisplayText;                                     /////  戻り値 = 表示名 で関数終了
            }
        }


        //====================================================================================================
        // テスト用メソッド列挙関連
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【手動テスト用メソッド列挙】指定した型情報に含まれる手動テスト用メソッドを列挙します
        /// </summary>
        /// <param name="typeInfo">[in ]：型情報</param>
        /// <returns>
        /// テスト用メソッド情報配列
        /// </returns>
        //--------------------------------------------------------------------------------
        public static TestMethodInfo[] EnumManualTest(Type typeInfo) => M_EnumTestMethod<ManualTestMethodAttribute>(typeInfo);


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【自動テスト用メソッド列挙】指定した型情報に含まれる自動テスト用メソッドを列挙します
        /// </summary>
        /// <param name="typeInfo">[in ]：型情報</param>
        /// <returns>
        /// テスト用メソッド情報配列
        /// </returns>
        //--------------------------------------------------------------------------------
        public static TestMethodInfo[] EnumAutoTest(Type typeInfo) => M_EnumTestMethod<AutoTestMethodAttribute>(typeInfo);


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト用メソッド列挙】指定した型情報に含まれるテスト用メソッドを列挙します。
        /// </summary>
        /// <typeparam name="TAttr">テスト用メソッド属性の派生型</typeparam>
        /// <param name="typeInfo">[in ]：型情報</param>
        /// <returns>
        /// テスト用メソッド情報配列
        /// </returns>
        //--------------------------------------------------------------------------------
        protected static TestMethodInfo[] M_EnumTestMethod<TAttr>(Type typeInfo) where TAttr : BaseTestMethodAttribute
        {
            //------------------------------------------------------------
            /// 指定した型情報に含まれるテスト用メソッドを列挙する
            //------------------------------------------------------------
            var infos = new Collection<TestMethodInfo>();               //// テスト用メソッド情報コレクションを生成する

            foreach (var methodInfo in typeInfo.GetMethods())
            {                                                           //// メソッド情報を繰り返す
                var attributes =                                        /////  テスト用メソッド基底属性の配列を取得する
                    methodInfo.GetCustomAttributes(typeof(TAttr), false);
                foreach (TAttr attr in attributes)                      // (キャストは必ず成功する)
                {                                                       /////  テスト用メソッド基底属性配列を繰り返す
                    infos.Add(new TestMethodInfo(methodInfo, attr));    //////   テスト用メソッド情報を生成してコレクションに追加する
                }
            }

            return infos.XToArray();                                    //// 戻り値 = テスト用メソッド情報配列 で関数終了
        }

    }
#endif

}
