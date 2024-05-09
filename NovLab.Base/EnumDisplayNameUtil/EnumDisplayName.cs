// @(h)EnumDisplayName.cs ver 0.00 ( '24.05.06 Nov-Lab ) 作成開始
// @(h)EnumDisplayName.cs ver 0.51 ( '24.05.09 Nov-Lab ) ベータ版完成
// @(h)EnumDisplayName.cs ver 0.52 ( '24.05.13 Nov-Lab ) 機能修正：CreateTable メソッドで、デリゲートが適合しない場合は属性不正例外をスローするようにした

// @(s)
// 　【列挙値表示名】列挙値に対応する表示名を取得するための機能を提供します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;


namespace NovLab.EnumDisplayNameUtil
{
    //====================================================================================================
    /// <summary>
    /// 【列挙値表示名】列挙値から対応する表示名を取得する機能を提供します。<br/>
    /// <br/>
    /// 列挙値に対応する表示名は、下記の方法で取得手段を用意しておきます。<br/>
    /// ・編集のできる独自の列挙体に表示名を付加する場合は、
    ///   <see cref="About_EnumDisplayNameInfoAttribute"/> を参照してください。<br/>
    /// ・編集できない既存の列挙体に表示名を後付けする場合や、属性で付加した表示名を上書きしたい場合は、
    ///   <see cref="About_EnumDisplayNameProviderAttribute"/> を参照してください。<br/>
    /// ・なお、NovLab.Windows.Forms クラスライブラリには、リストボックスやコンボボックスに列挙体表示名を表示する機能などが用意されています。<br/>
    /// </summary>
    /// <remarks>
    /// 補足<br/>
    /// ・フォールバック機構により、下記の優先度で表示名を取得します。このフォールバック処理は個別の列挙値に対して行われますので、より高い優先度の方法で差分だけを供給することもできます。<br/>
    /// ①列挙値表示名供給関数が供給する列挙値表示名のうち、<see cref="EnumDisplayNameProviderAttribute"/> 属性で指定した優先度が最も高いもの(*1)<br/>
    /// ②列挙値にメタデータで直接付加した列挙値表示名<br/>
    /// ③列挙値.ToString() の結果<br/>
    /// (*1)同じ列挙体に対して同じ優先度の列挙値表示名供給関数が複数定義されている場合、どの供給関数が使われるかは未定義であり不定です。<br/>
    /// </remarks>
    //====================================================================================================
    public static partial class EnumDisplayName
    {
        //====================================================================================================
        // 内部フィールド
        //====================================================================================================

        /// <summary>
        /// 【初期化済みフラグ】列挙値表示名供給関数情報テーブルを作成してあるかどうかを示します。
        /// </summary>
        private static bool m_initialized = false;


        //====================================================================================================
        // 列挙値表示名取得関連
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// <inheritdoc cref="GetFrom(Enum)"/>
        /// </summary>
        /// <param name="target">[in ]：対象列挙値</param>
        /// <returns><inheritdoc cref="GetFrom(Enum)"/></returns>
        /// <remarks>
        /// 補足<br/>
        /// ・<c>displayString = enumValue.XToDisplayName();</c> のように直感的に記述するためのユーティリティーメソッドです。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  // 積極的にインライン化する
        public static string XToDisplayName(this Enum target) => GetFrom(target);


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【列挙値表示名取得】列挙値表示名供給関数や属性を参照して、列挙値に対応する表示名を取得します。<br/>
        /// Enum クラスの仕様により、列挙体の中に同じ内部整数値を持つ列挙値が複数定義されている場合は、
        /// どの列挙値に対応する表示名を返すかは未定義であり不定です。<br/>
        ///  (参考メモ：<see cref="Memo_DuplicateEnumValue"/>)<br/>
        /// <br/>
        /// ヒント<br/>
        /// ・特定の列挙値表示名供給関数を使いたい場合は <c>displayName = MyProvider(enumValue);</c> のように、供給関数を直接呼び出します。<br/>
        /// </summary>
        /// <param name="enumValue">[in ]：列挙値</param>
        /// <returns>
        /// 列挙値表示名
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string GetFrom(Enum enumValue)
        {
            //[-] 保留：必要になったら(EnumDisplayName)。AppDomain.AssemblyLoad イベントを使えば、後からアセンブリが読み込まれたときに自動でテーブルを再作成できるかも
            //------------------------------------------------------------
            /// 前準備
            //------------------------------------------------------------
            if (m_initialized == false)
            {                                                           //// 初期化済みでない場合(列挙値表示名を初めて取得する場合)
                CreateTable();                                          /////  列挙値表示名供給関数情報テーブル作成処理を行う
            }


            //------------------------------------------------------------
            /// ①列挙値表示名供給関数が供給する列挙値表示名のうち最も優先度の高いものを取得する
            //------------------------------------------------------------
            var providerInfos =
                ProviderTable.GetProviderInfos(enumValue.GetType());    //// 列挙値の型に対応する列挙値表示名供給関数情報配列を取得する
            foreach (var providerInfo in providerInfos)
            {                                                           //// 列挙値表示名供給関数情報配列を繰り返す
                var result = providerInfo.fncProvider(enumValue);       /////  供給関数を呼び出して列挙値表示名を取得する
                if (result != null)
                {                                                       /////  取得できた場合
                    return result;                                      //////   戻り値 = 供給関数で取得した列挙値表示名 で関数終了
                }
            }


            //------------------------------------------------------------
            /// ②列挙値に対してメタデータで直接付加された列挙値表示名を取得する
            //------------------------------------------------------------
            var attr =                                                  //// 列挙値に付加された列挙値表示名情報属性を取得する
                enumValue.XGetCustomAttribute<EnumDisplayNameInfoAttribute>(false);
            if (attr != null)
            {                                                           //// 取得できた場合
                return attr.DisplayName;                                /////  戻り値 = 属性で付加された列挙値表示名 で関数終了
            }


            //------------------------------------------------------------
            /// ③上記いずれの方法でも取得できなかった場合は、ToString()の結果を返す
            //------------------------------------------------------------
            return enumValue.ToString();
        }


        //====================================================================================================
        // テーブル管理関連
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【列挙値表示名供給関数情報テーブル作成】
        /// 読み込み済みアセンブリから、列挙値表示名供給関数を収集してテーブルを作成します。
        /// </summary>
        /// <remarks>
        /// 補足<br/>
        /// ・初めて列挙値表示名を取得しようとしたときに呼び出されます。<br/>
        /// ・後から読み込んだアセンブリがあるなどしてもう一度収集しなおしたい場合は、手動でこのメソッドを呼び出します。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static void CreateTable()
        {
            //------------------------------------------------------------
            /// 列挙値表示名供給関数を収集してテーブルを作成する
            //------------------------------------------------------------
            ProviderTable.Clear();                                      //// 列挙値表示名供給関数情報テーブルをクリアする

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {                                                           //// 読み込み済みアセンブリを繰り返す
                foreach (var typeInfo in assembly.GetTypes())
                {                                                       /////  アセンブリ内の型情報を繰り返す
                    foreach (var methodInfo in typeInfo.GetMethods())
                    {                                                   //////   メソッド情報を繰り返す
                        var attributes =                                ///////    メソッドに付加されている列挙値表示名供給関数属性の配列を取得する
                            methodInfo.GetCustomAttributes(typeof(EnumDisplayNameProviderAttribute), false);
                        foreach (EnumDisplayNameProviderAttribute tmpAttr in attributes) // (キャストは必ず成功する)
                        {                                               ///////    列挙値表示名供給関数属性配列を繰り返す(シングルユース属性なので実際は１つだけ取得可能なはず)
                            try
                            {                                           ////////     try開始
                                var fncProvider =                       /////////      メソッド情報から列挙値表示名供給関数デリゲートインスタンスを生成する
                                    (EnumDisplayNameProvider)Delegate.CreateDelegate(
                                            typeof(EnumDisplayNameProvider), methodInfo);
                                ProviderTable.Register(
                                    fncProvider, tmpAttr);              /////////      列挙値表示名供給関数情報テーブルに登録する
                            }
                            catch (ArgumentException ex)
                            {                                           ////////     catch：引数不正例外(デリゲート不適合など)
                                var message = AttributeException.MakeDelegateMismatchMessage<EnumDisplayNameProvider>
                                                    (methodInfo);       /////////      デリゲート不適合エラーメッセージを作成する
                                throw                                   /////////      属性不正例外をスローする
                                    new AttributeException(message, nameof(EnumDisplayNameProviderAttribute), ex);
                            }
                        }
                    }
                }
            }

            m_initialized = true;                                       //// 初期化済みフラグ = true にセットする

#if DEBUG                                                               //// DEBUGビルドの場合
            ProviderTable.ZZZ_DuplicateCheck();                         /////  列挙値表示名供給関数情報テーブルの重複チェック処理を行う
#endif
        }


        //====================================================================================================
        // 内部クラス
        //====================================================================================================

        //================================================================================
        /// <summary>
        /// 【列挙値表示名供給関数情報】列挙値表示名供給関数インスタンスとその優先度をペアで管理します。<br/>
        /// <see cref="List{T}.Sort"/> で優先度の高い順に並べ替えることができます。<br/>
        /// </summary>
        //================================================================================
        protected partial class ProviderInfo : IComparable<ProviderInfo>
        {
            //================================================================================
            // 公開フィールドとコンストラクター
            //================================================================================

            /// <summary>
            /// 【列挙値表示名供給関数のインスタンス】
            /// </summary>
            public readonly EnumDisplayNameProvider fncProvider;

            /// <summary>
            /// 【列挙値表示名供給関数優先度】
            /// </summary>
            public readonly EnumDisplayNameProviderPriority priority;


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【完全コンストラクター】すべての内容を指定して列挙値表示名供給関数情報の新しいインスタンスを生成します。
            /// </summary>
            /// <param name="fncProvider"><inheritdoc cref="fncProvider" path="/summary"/></param>
            /// <param name="priority">   <inheritdoc cref="priority" path="/summary"/></param>
            //--------------------------------------------------------------------------------
            public ProviderInfo(EnumDisplayNameProvider fncProvider, EnumDisplayNameProviderPriority priority)
            {
                this.fncProvider = fncProvider;
                this.priority = priority;
            }


            //================================================================================
            // IComparable I/F の実装
            //================================================================================

            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【比較】他のインスタンスと内容を比較します。
            /// 列挙値表示名供給関数優先度の高い順(内部整数値の大きい順)で並ぶように比較します。
            /// </summary>
            /// <param name="other">[in ]：比較相手</param>
            /// <returns>
            /// 比較結果値[0より小さい = 比較相手よりも小さい、0 = 比較相手と等しい、0より大きい = 比較相手よりも大きい]
            /// </returns>
            /// <remarks>
            /// 補足<br/>
            /// ・<see cref="IComparable{T}.CompareTo(T)"/> の実装です。<br/>
            /// ・<see cref="System.Collections.Generic.List{T}.Sort"/> などのソート処理で使用します。<br/>
            /// ・列挙値表示名供給関数優先度が同じで、デリゲートインスタンスのみが異なる場合の順序は、未定義であり不定です。<br/>
            /// </remarks>
            //--------------------------------------------------------------------------------
            public int CompareTo(ProviderInfo other)
            {
                //------------------------------------------------------------
                /// 他のインスタンスと内容を比較する
                //------------------------------------------------------------
                int result = priority.CompareTo(other.priority);            //// 列挙値表示名供給関数優先度で比較する
                if (result != 0)
                {                                                           //// 同一でない場合
                    return result * -1;                                     /////  戻り値 = 比較結果値を反転した値(降順) で関数終了
                }

                return 0;                                                   //// 上記すべてが同一だった場合、戻り値 = 0(等しい) で関数終了
            }

        } // class


        //================================================================================
        /// <summary>
        /// 【列挙値表示名供給関数情報テーブル】
        /// 列挙体の型と、それに対応する複数の列挙値表示名供給関数情報を管理します。
        /// </summary>
        //================================================================================
        private static partial class ProviderTable
        {
            /// <summary>
            /// 【対応テーブル】列挙体の型と、それに対応する複数の列挙値表示名供給関数情報をリストで管理します。<br/>
            /// ・key = 列挙体の型<br/>
            /// ・value = 列挙値表示名供給関数情報リスト(常に優先度の高い順でソートされています)<br/>
            /// </summary>
            private readonly static Dictionary<Type, List<ProviderInfo>> m_table = new Dictionary<Type, List<ProviderInfo>>();


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【列挙値表示名供給関数情報テーブルクリア】テーブルをクリアします。
            /// </summary>
            //--------------------------------------------------------------------------------
            public static void Clear() => m_table.Clear();


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【列挙値表示名供給関数情報テーブル登録】列挙値表示名供給関数の情報をテーブルに追加します。
            /// </summary>
            /// <param name="fncProvider">[in ]：列挙値表示名供給関数のインスタンス</param>
            /// <param name="attr">       [in ]：列挙値表示名供給関数属性</param>
            //--------------------------------------------------------------------------------
            public static void Register(EnumDisplayNameProvider fncProvider, EnumDisplayNameProviderAttribute attr)
            {
                //------------------------------------------------------------
                /// 列挙値表示名供給関数の情報をテーブルに追加する
                //------------------------------------------------------------
                if (m_table.ContainsKey(attr.enumType) == false)
                {                                                           //// 列挙体の型に対応する要素がテーブルに存在しない場合
                    m_table.Add(attr.enumType, new List<ProviderInfo>());   /////  列挙体の型に対応する要素をテーブルに追加する
                }

                m_table[attr.enumType].Add(
                    new ProviderInfo(fncProvider, attr.priority));          //// 列挙値表示名供給関数情報を生成してリストに追加する
                m_table[attr.enumType].Sort();                              //// 列挙値表示名供給関数情報リストをソートする
            }


            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【列挙値表示名供給関数情報配列取得】
            /// 指定した列挙体の型に対応する列挙値表示名供給関数情報の配列を取得します。
            /// </summary>
            /// <param name="enumType">[in ]：列挙体の型</param>
            /// <returns>
            /// 列挙値表示名供給関数情報配列(列挙体の型に対応する情報がない場合は空の配列)
            /// </returns>
            //--------------------------------------------------------------------------------
            public static ProviderInfo[] GetProviderInfos(Type enumType)
            {
                //------------------------------------------------------------
                /// 列挙体の型に対応する列挙値表示名供給関数情報の配列を取得する
                //------------------------------------------------------------
                if (m_table.ContainsKey(enumType))
                {                                                           //// 列挙体の型に対応する情報が対応テーブルに存在する場合
                    return m_table[enumType].ToArray();                     /////  戻り値 = 列挙値表示名供給関数情報の配列 で関数終了
                }
                else
                {                                                           //// 列挙体の型に対応する情報が対応テーブルに存在しない場合
                    return new ProviderInfo[0];                             /////  戻り値 = 空の配列 で関数終了
                }
            }


#if DEBUG
            // ＜メモ＞
            // ・このメソッドをテストするときは ZZZ_Test_DuplicateEnumDisplayNameProvider クラスを有効化する。
            //--------------------------------------------------------------------------------
            /// <summary>
            /// 【列挙値表示名供給関数情報テーブル重複チェック】
            /// 同じ列挙体に対して同じ優先度の列挙値表示名供給関数が複数定義されているかどうかをチェックして、
            /// 重複している場合はワーニングトレース出力します。<br/>
            /// </summary>
            /// <remarks>
            /// ・DEBUG ビルドでのみ有効です。
            /// </remarks>
            //--------------------------------------------------------------------------------
            [Conditional("DEBUG")]  // DEBUG ビルドでのみ有効
            public static void ZZZ_DuplicateCheck()
            {
                //------------------------------------------------------------
                /// 同じ列挙体に対して同じ優先度の列挙値表示名供給関数が複数定義されているかどうかをチェックして、
                ///-重複している場合はワーニングトレース出力する
                //------------------------------------------------------------
                var checkTable =                                            //// チェックテーブルを生成する(key = 優先度, value = 供給関数リスト)
                    new Dictionary<EnumDisplayNameProviderPriority, List<EnumDisplayNameProvider>>();

                foreach (var tmpPair in m_table)
                {                                                           //// 対応テーブルを繰り返す
                    //----------------------------------------
                    // 優先度ごとのチェックテーブルを作成
                    //----------------------------------------
                    checkTable.Clear();                                         /////  チェックテーブルをクリアする

                    foreach (var tmpItem in tmpPair.Value)
                    {                                                           /////  列挙値表示名供給関数情報リストを繰り返す
                        if (checkTable.ContainsKey(tmpItem.priority) == false)
                        {                                                       //////   チェックテーブルに優先度に対応する要素がない場合は追加する
                            checkTable.Add(tmpItem.priority, new List<EnumDisplayNameProvider>());
                        }

                        checkTable[tmpItem.priority].Add(tmpItem.fncProvider);  //////   チェックテーブルに供給関数を追加する
                    }


                    //----------------------------------------
                    // 同じ優先度に複数登録されている場合はワーニング
                    //----------------------------------------
                    foreach (var tmpItem in checkTable)
                    {                                                       /////  チェックテーブルを繰り返す
                        if (tmpItem.Value.Count == 1)
                        {                                                   //////   重複登録されていない場合(供給関数が１つだけの場合)
                            continue;                                       ///////    スキップして次の要素へ
                        }

                                                                            //////   重複登録されている場合はワーニング出力する
                        Trace.TraceWarning($"{tmpPair.Key} 列挙体の優先度={tmpItem.Key} に対する供給関数が複数定義されています。");
                        foreach (var tmpProvider in tmpItem.Value)
                        {
                            Trace.TraceWarning($"・{tmpProvider.Method.DeclaringType}.{tmpProvider.Method.Name}");
                        }
                    }
                }
            }
#endif

        } //  class ProviderTable


        // 【参考用覚え書き】特定列挙型の列挙値を受け取る関数を、Enum 列挙値を受け取るデリゲートに変換することはできない(コンパイルエラーになる)
#if false
        private static void Test()
        {
            var test0 = (EnumDisplayNameProvider)Test0; test0(DayOfWeek.Monday);
            var test1 = (EnumDisplayNameProvider)Test1; test1(DayOfWeek.Monday);
        }

        private static string Test0(Enum enumValue) => enumValue.ToString();

        private static string Test1(DayOfWeek enumValue) => enumValue.ToString();
#endif

    } // class


#if false// テストするときだけ有効化すること
#if DEBUG
    //====================================================================================================
    /// <summary>
    /// <see cref="EnumDisplayName.ProviderTable.ZZZ_DuplicateCheck"/> のテスト
    /// </summary>
    //====================================================================================================
    public static partial class ZZZ_Test_DuplicateEnumDisplayNameProvider
    {
        //--------------------------------------------------------------------------------
        // 手動テストメソッド
        //--------------------------------------------------------------------------------
        [ManualTestMethod("-- 同じ列挙体に対して同じ優先度の列挙値表示名供給関数が複数定義されている場合のテスト --")]
        public static void Test()
        {
            // どの供給関数が使われるかは未定義であり不定
            Debug.Print("EventLogEntryType.Error=" + EventLogEntryType.Error.XToDisplayName());
        }


        //--------------------------------------------------------------------------------
        // 重複する列挙値表示名供給関数その１
        //--------------------------------------------------------------------------------
        public static partial class Dupl1
        {
            [EnumDisplayNameProvider(typeof(EventLogEntryType), EnumDisplayNameProviderPriority.NovLab)]
            public static string DuplFunc1(Enum enumValue)
            {
                if (enumValue is EventLogEntryType specificEnum)
                {
                    if (specificEnum == EventLogEntryType.Error)
                    {
                        return "エラーのイベント";
                    }
                }
                return null;
            }

        } // class


        //--------------------------------------------------------------------------------
        // 重複する列挙値表示名供給関数その２
        //--------------------------------------------------------------------------------
        public static partial class Dupl2
        {
            [EnumDisplayNameProvider(typeof(EventLogEntryType), EnumDisplayNameProviderPriority.NovLab)]
            public static string DuplFunc2(Enum enumValue)
            {
                if (enumValue is EventLogEntryType specificEnum)
                {
                    if (specificEnum == EventLogEntryType.Error)
                    {
                        return "Error Event";
                    }
                }
                return null;
            }

        } // class

    } // class
#endif
#endif

} // namespace
