// @(h)TestMethodInfo.cs ver 0.00 ( '22.05.18 Nov-Lab ) 下書きを元に作成開始
// @(h)TestMethodInfo.cs ver 0.21 ( '22.05.19 Nov-Lab ) アルファ版完成
// @(h)TestMethodInfo.cs ver 0.22 ( '24.01.21 Nov-Lab ) 機能変更：テスト用メソッド呼び出しは MethodInfo.Invoke ではなくデリゲートを使用するようにした。前者だと、例外発生時に発生個所で中断されずに、呼び出し元(Invoke の場所)まで戻されてしまうため、デバッグがやりにくい
// @(h)TestMethodInfo.cs ver 0.33 ( '24.04.21 Nov-Lab ) 機能変更：非同期メソッドのテストに対応した。

// @(s)
// 　【テスト用メソッド情報】テスト用メソッドを扱うために必要な情報を管理します。

using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.ObjectModel;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace NovLab.DebugSupport
{

#if DEBUG   // DEBUGビルドのみ有効

    //====================================================================================================
    /// <summary>
    /// 【テスト用メソッド情報】テスト用メソッドを扱うために必要な情報を管理します。
    /// </summary>
    //====================================================================================================
    public partial class TestMethodInfo : IComparable<TestMethodInfo>
    {
        //====================================================================================================
        // 内部フィールド
        //====================================================================================================
        /// <summary>
        /// 【メソッド情報(読み取り専用)】テスト用メソッドの MethodInfo です。
        /// </summary>
        protected readonly MethodInfo m_methodInfo;

        /// <summary>
        /// 【テスト用メソッド属性(読み取り専用)】テスト用メソッドの属性情報です。表示名の取得などに使用します。
        /// </summary>
        /// <remarks>
        /// 補足<br/>
        /// ・実体は派生クラス(<see cref="ManualTestMethodAttribute"/>, <see cref="AutoTestMethodAttribute"/>)のいずれかです。<br/>
        /// </remarks>
        protected readonly BaseTestMethodAttribute m_attributeInfo;

        /// <summary>
        /// 【テスト用メソッド実行情報】
        /// </summary>
        protected readonly Invoker m_invoker;


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【文字列化】テスト用メソッドの表示名を取得します。リストボックスでの一行表示に使用します。
        /// </summary>
        /// <returns>文字列形式(表示名)</returns>
        /// <remarks>
        /// 補足<br/>
        /// ・自動テストの場合はプリフィックス「自動テスト：」を付加します。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public override string ToString()
        {
            //------------------------------------------------------------
            /// テスト用メソッドの表示名を取得する
            //------------------------------------------------------------
            if (TestMethodKind == TestMethodKind.Auto)
            {                                                           //// 自動テスト用メソッドの場合
                return "自動テスト：" + DisplayText;                    /////  戻り値 = "自動テスト：<表示名>" で関数終了
            }
            else
            {                                                           //// 自動テスト用メソッドでない場合
                return DisplayText;                                     /////  戻り値 = 表示名 で関数終了
            }
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【完全コンストラクター】すべての内容を指定してテスト用メソッド情報を生成します。
        /// </summary>
        /// <param name="methodInfo">   [in ]：メソッド情報(テスト用メソッドの MethodInfo)</param>
        /// <param name="attributeInfo">[in ]：テスト用メソッド属性</param>
        /// <param name="invoker">      [in ]：テスト用メソッド実行情報</param>
        //--------------------------------------------------------------------------------
        protected TestMethodInfo(MethodInfo methodInfo, BaseTestMethodAttribute attributeInfo, Invoker invoker)
        {
            m_methodInfo = methodInfo;
            m_attributeInfo = attributeInfo;
            m_invoker = invoker;
        }


        //====================================================================================================
        // 公開プロパティー
        //====================================================================================================

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
                if (m_attributeInfo.displayText.XIsValid())
                {                                                           //// テスト用メソッド属性で表示名が指定されている場合
                    return m_attributeInfo.displayText;                     /////  戻り値 = 表示名 で関数終了
                }
                else
                {                                                           //// テスト用メソッド属性で表示名が指定されていない場合
                    var methodName = m_methodInfo.Name;                     /////  メソッド名を取得する
                    methodName =                                            /////  メソッド名のプリフィックスに目印文字列"ZZZ_"がついている場合は削除する
                        methodName.XRemoveStart("ZZZ_", true, null);
                    return m_methodInfo.DeclaringType.Name + "." +          /////  戻り値 = <クラス名>.<メソッド名> で関数終了
                           methodName;
                }
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト用メソッド種別(読み取り専用)】テスト用メソッドの種類を示します。
        /// </summary>
        /// <returns>
        /// テスト用メソッド種別
        /// </returns>
        //--------------------------------------------------------------------------------
        public TestMethodKind TestMethodKind => m_attributeInfo.Kind;


        //====================================================================================================
        // 公開メソッド
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト用メソッド実行】非同期で、テスト用メソッドを実行します。
        /// </summary>
        /// <returns>
        /// 戻り値を返さない非同期操作タスク
        /// </returns>
        //--------------------------------------------------------------------------------
        public async Task InvokeAsync() => await m_invoker.InvokeAsync();


        //====================================================================================================
        // IComparable I/F の実装
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【比較】他のインスタンスと内容を比較します。
        /// テスト用メソッド種別->表示名の２段階で比較します。
        /// </summary>
        /// <param name="other">[in ]：比較相手</param>
        /// <returns>
        /// 比較結果値[0より小さい = 比較相手よりも小さい、0 = 比較相手と等しい、0より大きい = 比較相手よりも大きい]
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・<see cref="IComparable{T}.CompareTo(T)"/> の実装です。<br/>
        /// ・<see cref="System.Collections.Generic.List{T}.Sort"/> などのソート処理で使用します。<br/>
        /// ・テスト用メソッド種別と表示名が同じで、デリゲートインスタンスのみが異なる場合の順序は、未定義であり不定です。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public int CompareTo(TestMethodInfo other)
        {
            //------------------------------------------------------------
            /// 他のインスタンスと内容を比較する
            //------------------------------------------------------------
            int result =
                TestMethodKind.CompareTo(other.TestMethodKind);         //// テスト用メソッド種別で比較する
            if (result != 0)
            {                                                           //// 同一でない場合
                return result;                                          /////  戻り値 = 比較結果値 で関数終了
            }

            result = DisplayText.CompareTo(other.DisplayText);          //// 表示名で比較する
            return result;                                              //// 戻り値 = 比較結果値 で関数終了
        }


        //====================================================================================================
        // テスト用メソッド列挙関連
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【テスト用メソッド列挙】指定した型情報に含まれるテスト用メソッドを列挙して取得します。
        /// </summary>
        /// <param name="typeInfo">[in ]：型情報</param>
        /// <returns>
        /// テスト用メソッド情報配列
        /// </returns>
        //--------------------------------------------------------------------------------
        public static TestMethodInfo[] EnumTestMethod(Type typeInfo)
        {
            //------------------------------------------------------------
            /// 指定した型情報に含まれるテスト用メソッドを列挙する
            //------------------------------------------------------------
            var infos = new Collection<TestMethodInfo>();               //// テスト用メソッド情報コレクションを生成する

            foreach (var methodInfo in typeInfo.GetMethods())
            {                                                           //// 型情報に含まれるメソッド情報を繰り返す
                var attributes =                                        /////  テスト用メソッド属性(BaseTestMethodAttribute の派生型属性)の配列を取得する
                    methodInfo.GetCustomAttributes(typeof(BaseTestMethodAttribute), false);
                foreach (BaseTestMethodAttribute attr in attributes)    // (キャストは必ず成功する)
                {                                                       /////  テスト用メソッド属性配列を繰り返す(個々はシングルユース属性だが、手動テストと自動テストの重ね掛けはできる)
                    var invoker = new Invoker(methodInfo);              //////   メソッド情報からテスト用メソッド実行情報を生成する(デリゲートに適合しない場合は例外)
                    var info =                                          //////   テスト用メソッド情報を生成する
                        new TestMethodInfo(methodInfo, attr, invoker);
                    infos.Add(info);                                    //////   テスト用メソッド情報をコレクションに追加する
                }
            }

            return infos.XToArray();                                    //// 戻り値 = テスト用メソッド情報配列 で関数終了
        }


        //====================================================================================================
        /// <summary>
        /// 【テスト用メソッド実行情報】テスト用メソッドの実行機能を提供します。
        /// </summary>
        /// <remarks>
        /// 補足<br/>
        /// ・テスト用メソッド(同期的)とテスト用メソッド(非同期)を同じように扱うためのクラスです。<br/>
        /// </remarks>
        //====================================================================================================
        protected partial class Invoker
        {
            // ＜メモ＞
            // ・どちらか１つだけが設定されます。
            //====================================================================================================
            // 内部フィールド
            //====================================================================================================
            /// <summary>
            /// 【テスト用メソッドデリゲート(同期的)(読み取り専用)】テスト用メソッド(同期的)のデリゲートインスタンスです。
            /// </summary>
            protected readonly Action m_actTestMethodSync = null;

            /// <summary>
            /// 【テスト用メソッドデリゲート(非同期)(読み取り専用)】テスト用メソッド(非同期)のデリゲートインスタンスです。
            /// </summary>
            protected readonly Func<Task> m_actTestMethodAsync = null;  // テスト用メソッド自体の機能としては戻り値を返さないので、プリフィックスは act にしている


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【完全コンストラクター】すべての内容を指定してテスト用メソッド実行情報を生成します。
            /// </summary>
            /// <param name="methodInfo">[in ]：メソッド情報(テスト用メソッドの MethodInfo)</param>
            /// <exception cref="ArgumentException">引数不正例外。テスト用メソッドデリゲートのいずれにも適合しません。</exception>
            //--------------------------------------------------------------------------------
            public Invoker(MethodInfo methodInfo)
            {
                //------------------------------------------------------------
                /// テスト用メソッド実行情報を生成する
                //------------------------------------------------------------
                var mismatchDelegates = new List<string>();                 //// 不適合デリゲート名リストを生成する

                //----------------------------------------
                // テスト用メソッド(同期的)として試行
                //----------------------------------------
                try
                {                                                           //// try開始
                    m_actTestMethodSync =                                   /////  メソッド情報からテスト用メソッドデリゲート(同期的)を作成してフィールドに設定する
                        (Action)Delegate.CreateDelegate(typeof(Action), methodInfo);
                    return;                                                 /////  成功した場合(例外が発生しなかった場合)、関数終了
                }
                catch (ArgumentException)
                {                                                           //// catch：引数不正例外(デリゲート不適合)
                    mismatchDelegates.Add("static Action");                 /////  不適合デリゲート名リストに"Action"を追加する
                }                                                           //// 上記以外の例外は処理せずに呼び出し元に受け取らせる

                //----------------------------------------
                // テスト用メソッド(非同期)として試行
                //----------------------------------------
                try
                {                                                           //// try開始
                    m_actTestMethodAsync =                                  /////  メソッド情報からテスト用メソッドデリゲート(非同期)を作成してフィールドに設定する
                        (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), methodInfo);
                    return;                                                 /////  成功した場合(例外が発生しなかった場合)、関数終了
                }
                catch (ArgumentException)
                {                                                           //// catch：引数不正例外(デリゲート不適合)
                    mismatchDelegates.Add("static Func<Task>");             /////  不適合デリゲート名リストに"Func<Task>"を追加する
                }                                                           //// 上記以外の例外は処理せずに呼び出し元に受け取らせる


                //------------------------------------------------------------
                /// 生成に失敗した場合は、引数不正例外(デリゲート不適合)をスローする
                //------------------------------------------------------------
                var msg = string.Join(", ", mismatchDelegates);
                throw new ArgumentException($"{methodInfo.ToString()} はテスト用メソッドデリゲート({msg})のいずれにも適合しません。"
                                          , nameof(methodInfo));
            }


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【テスト用メソッド実行】非同期で、テスト用メソッドを実行します。
            /// </summary>
            /// <returns>
            /// 戻り値を返さない非同期操作タスク
            /// </returns>
            //--------------------------------------------------------------------------------
            public async Task InvokeAsync()
            {
                //------------------------------------------------------------
                /// テスト用メソッドを実行する
                //------------------------------------------------------------
                if (m_actTestMethodSync != null)
                {                                                           //// テスト用メソッドデリゲート(同期的)が設定されている場合
                    m_actTestMethodSync();                                  /////  テスト用メソッド(同期的)を同期的に実行する
                    return;                                                 /////  関数終了
                }

                if (m_actTestMethodAsync != null)
                {                                                           //// テスト用メソッドデリゲート(非同期)が設定されている場合
                    await m_actTestMethodAsync();                           /////  テスト用メソッド(非同期)を非同期で実行する
                    return;                                                 /////  関数終了
                }
            }

        } // class

    } // class

#endif


    // ＜メモ＞
    // ・デリゲート規約不適合パターンをテストしたい部分を true にする
    //====================================================================================================
    // デリゲート規約不適合パターンのテスト用
    //====================================================================================================
#if DEBUG   // DEBUGビルドのみ有効
    public class ZZZ_DelegateRegulationTest
    {

#if false   // 重ね掛けはできてしまうが、実害はないので良しとする
        [AutoTestMethod("重ね掛け：自動テスト")]
        [ManualTestMethod("重ね掛け：手動テスト")]
        public static void ZZZ_Duplication() { }
#endif


#if false
        [AutoTestMethod("自動テスト用メソッドデリゲートの規約に適合しない(戻り値が不一致)")]
        public static int ZZZ_RegulationViolationA1() { return 1; }
#endif

#if false
        [AutoTestMethod("自動テスト用メソッドデリゲートの規約に適合しない(引数が不一致)")]
        public static void ZZZ_RegulationViolationA2(int param) { }
#endif

#if false
        [AutoTestMethod("自動テスト用メソッドデリゲートの規約に適合しない(戻り値および引数が不一致)")]
        public static int ZZZ_RegulationViolationA3(int param1, int param2) { return 1; }
#endif


#if false
        [ManualTestMethod("手動テスト用メソッドデリゲートの規約に適合しない(戻り値が不一致)")]
        public static int ZZZ_RegulationViolationM1() { return 1; }
#endif

#if false
        [ManualTestMethod("手動テスト用メソッドデリゲートの規約に適合しない(引数が不一致)")]
        public static void ZZZ_RegulationViolationM2(int param) { }
#endif

#if false
        [ManualTestMethod("手動テスト用メソッドデリゲートの規約に適合しない(戻り値および引数が不一致)")]
        public static int ZZZ_RegulationViolationM3(int param1, int param2) { return 1; }
#endif

    } // class

#endif

} // namespace
