// @(h)XEnum.cs ver 0.00 ( '22.04.19 Nov-Lab ) 既存のソースを元に作成開始
// @(h)XEnum.cs ver 0.21 ( '22.04.19 Nov-Lab ) アルファ版完成
// @(h)XEnum.cs ver 0.22 ( '22.05.03 Nov-Lab ) 機能追加：扱い保留としていた XStrMake と XStrParse を有効化した
// @(h)XEnum.cs ver 0.23 ( '24.05.09 Nov-Lab ) バグ修正：XEnum.XGetFieldInfo：列挙値から列挙定数名を取得できないパターンにも対応した
// @(h)XEnum.cs ver 0.24 ( '24.05.10 Nov-Lab ) 機能追加：XHasIgnore, GetValidValues, XGetValidValues を追加した
// @(h)XEnum.cs ver 0.24a( '24.05.14 Nov-Lab ) その他  ：コメント整理

// @(s)
// 　【Enum拡張メソッド】System.Enum クラスに拡張メソッドを追加します。

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using NovLab.DebugSupport;


namespace NovLab
{
    //====================================================================================================
    /// <summary>
    /// 【Enum拡張メソッド】System.Enum クラスに拡張メソッドを追加します。
    /// </summary>
    //====================================================================================================
    public static partial class XEnum
    {
        //====================================================================================================
        // メタデータ操作
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【フィールド情報取得】列挙値からフィールド情報を取得します。
        /// フィールドのメタデータにアクセスする際に使用します。
        /// </summary>
        /// <param name="target">[in ]：対象列挙値</param>
        /// <returns>
        /// フィールド情報[null = 取得失敗]
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・Enum クラスの仕様により、同じ内部整数値を持つ列挙値が複数定義されている場合、
        ///   どの列挙定数名に対するフィールド情報を取得するかは未定義であり不定です。
        ///  (参考メモ：<see cref="Memo_DuplicateEnumValue"/>)<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static System.Reflection.FieldInfo XGetFieldInfo(this Enum target)
        {
            //------------------------------------------------------------
            /// 列挙値からフィールド情報を取得する
            //------------------------------------------------------------
            //----------------------------------------
            // 列挙値から列挙定数名を取得
            //----------------------------------------
            var enumType = target.GetType();                            //// 列挙型の型情報を取得する

            var enumName = Enum.GetName(enumType, target);              //// 列挙値から列挙定数名を取得する
            if (enumName == null)
            {                                                           //// 取得失敗の場合((DayOfWeek)65535 のようにキャストした列挙値などであり得る)
                return null;                                            /////  戻り値 = null(取得失敗) で関数終了
            }

            //----------------------------------------
            // 列挙定数名からフィールド情報を取得
            //----------------------------------------
            var fieldInfo = enumType.GetField(enumName);                //// 列挙定数名からフィールド情報を取得する
            if (fieldInfo == null)
            {                                                           //// 取得失敗の場合(ありえないはずだが)
                Debug.Fail(enumName + " is not member of " +
                           enumType.Name);                              /////  エラーメッセージ出力
                return null;                                            /////  戻り値 = null(取得失敗) で関数終了
            }

            return fieldInfo;                                           //// 戻り値 = フィールド情報 で関数終了
        }


        //====================================================================================================
        // 属性操作
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【カスタム属性取得】列挙値からカスタム属性を取得します。
        /// 取得失敗時は、必須フラグ = false の場合は null を返し、必須フラグ = true の場合は例外をスローします。
        /// </summary>
        /// <typeparam name="TAttribute">カスタム属性の型</typeparam>
        /// <param name="target">   [in ]：対象列挙値</param>
        /// <param name="essential">[in ]：必須フラグ</param>
        /// <returns>
        /// カスタム属性[null = 取得失敗]
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・Enum クラスの仕様により、同じ内部整数値を持つ列挙値が複数定義されている場合、
        ///   どの列挙定数名に対するカスタム属性を取得するかは未定義であり不定です。
        ///  (参考メモ：<see cref="Memo_DuplicateEnumValue"/>)<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static TAttribute XGetCustomAttribute<TAttribute>(this Enum target,
                                                                 bool essential = false) where TAttribute : Attribute
        {
            //------------------------------------------------------------
            /// 列挙値からカスタム属性を取得する
            //------------------------------------------------------------
            var fieldInfo = target.XGetFieldInfo();                     //// 列挙値からフィールド情報を取得する
            if (fieldInfo == null)
            {                                                           //// 取得失敗の場合(ありえないはずだが)
                return null;                                            /////  戻り値 = null(取得失敗) で関数終了
            }

            var attr =                                                  //// 指定された型のカスタム属性を取得する
                (TAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TAttribute));
            if (essential == true && attr == null)
            {                                                           //// 必須フラグ = true かつ 取得失敗の場合
                throw new ArgumentException(                            /////  引数不正例外をスローする
                    typeof(TAttribute).Name + " not found", target.ToString());
            }

            return attr;                                                //// 戻り値 = 取得したカスタム属性 で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【カスタム属性付加チェック】
        /// 列挙値にカスタム属性が付加されているかどうかをチェックします。
        /// </summary>
        /// <typeparam name="TAttribute">カスタム属性の型</typeparam>
        /// <param name="target">[in ]：対象列挙値</param>
        /// <returns>
        /// チェック結果[true = 付加されている / false = 付加されていない]
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・Enum クラスの仕様により、同じ内部整数値を持つ列挙値が複数定義されている場合、
        ///   どの列挙定数名に対するカスタム属性がチェック対象になるかは未定義であり不定です。
        ///  (参考メモ：<see cref="Memo_DuplicateEnumValue"/>)<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static bool XHasAttribute<TAttribute>(this Enum target) where TAttribute : Attribute
        {
            //------------------------------------------------------------
            /// 列挙値にカスタム属性が付加されているかどうかをチェックする
            //------------------------------------------------------------
            var attr = XGetCustomAttribute<TAttribute>(target);         //// 列挙値からカスタム属性を取得する
            if (attr != null)
            {                                                           //// 取得成功の場合
                return true;                                            /////  戻り値 = true(付加されている) で関数終了
            }
            else
            {                                                           //// 取得失敗の場合
                return false;                                           /////  戻り値 = false(付加されていない) で関数終了
            }
        }


        //====================================================================================================
        // 列挙処理操作
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【列挙処理対象外チェック】列挙値が列挙処理対象外であるかどうか
        /// (<see cref="EnumerateIgnoreAttribute"/> 属性を付与されているかどうか)をチェックします。
        /// </summary>
        /// <param name="target">[in ]：対象列挙値</param>
        /// <returns>
        /// チェック結果[true = 列挙処理対象外 / false = 列挙処理対象外でない]
        /// </returns>
        //--------------------------------------------------------------------------------
        public static bool XHasIgnore(this Enum target)
        {
            //------------------------------------------------------------
            /// 列挙値が列挙処理対象外であるかどうかをチェックする
            //------------------------------------------------------------
            if (target.XHasAttribute<EnumerateIgnoreAttribute>())
            {                                                           //// 列挙値に列挙処理対象外マーク属性が付加されている場合
                return true;                                            /////  戻り値 = true(列挙処理対象外) で関数終了
            }

            return false;                                               //// 上記のチェックをパスした場合、戻り値 = false(列挙処理対象外でない) で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【有効列挙値列挙】対象の列挙体に含まれる有効列挙値を列挙します。<br/>
        /// このとき、以下の列挙値は除外します。<br/>
        /// ・列挙処理対象外属性(<see cref="EnumerateIgnoreAttribute"/>)が付加されている列挙値。<br/>
        /// ・引数で指定された除外対象列挙値配列に含まれる列挙値。<br/>
        /// </summary>
        /// <param name="enumType">[in ]：列挙体の型</param>
        /// <param name="excludes">[in ]：除外対象列挙値配列</param>
        /// <returns>
        /// 有効列挙値配列
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・非ジェネリック版の <see cref="GetValidValues"/> は、実行時に型を指定することができます。<br/>
        /// ・ジェネリック版の <see cref="XGetValidValues"/> は、型が固定の場合に便利です。<br/>
        /// </remarks>
        /// <exception cref="ArgumentNullException">引数不正例外(null値)。<paramref name="enumType"/> を null にすることはできません。</exception>
        /// <exception cref="ArgumentException">引数不正例外。<paramref name="enumType"/> が Enum 型ではありません。</exception>
        //--------------------------------------------------------------------------------
        public static Enum[] GetValidValues(Type enumType, params Enum[] excludes)
        {
            // ＜メモ＞
            // ・Enum.GetValues がスローする例外を参考にした
            //------------------------------------------------------------
            /// 引数をチェックする
            //------------------------------------------------------------
            if (enumType == null)
            {                                                           //// 引数で指定された型が null の場合
                throw new ArgumentNullException(nameof(enumType));      /////  引数不正例外(null)をスローする
            }

            if (enumType.IsEnum == false)
            {                                                           //// 引数で指定された型が列挙体の型でない場合
                throw                                                   /////  引数不正例外(Enum型でない)をスローする
                    new ArgumentException("指定された型は Enum でなければなりません。", nameof(enumType));
            }


            //------------------------------------------------------------
            /// 対象の列挙型に含まれる列挙値から、有効列挙値配列を生成して取得する
            //------------------------------------------------------------
            var items = new List<Enum>();                               //// 有効列挙値リストを生成する

            foreach (Enum enumValue in Enum.GetValues(enumType))
            {                                                           //// 対象の列挙値型に含まれる列挙値を繰り返す
                if (enumValue.XHasIgnore())
                {                                                       /////  列挙処理対象外の場合(列挙対象除外属性が付加されている場合)
                    continue;                                           //////   スキップして次の要素へ
                }

                if (IsExcludesContains(enumValue))
                {                                                       /////  除外対象列挙値配列に含まれる場合
                    continue;                                           //////   スキップして次の要素へ
                }

                items.Add(enumValue);                                   /////  列挙値を有効列挙値リストに追加する
            }

            return items.ToArray();                                     //// 有効列挙値リストから配列を生成して戻り値とし、関数終了


            //------------------------------------------------------------
            /// 【ローカル関数】除外対象列挙値配列に含まれるかをチェック
            //------------------------------------------------------------
            bool IsExcludesContains(Enum enumValue)     // [in ]：列挙値
            {
                if (excludes != null)
                {                                                       //// 除外対象列挙値配列 = null でない場合
                    foreach (var tmpEnum in excludes)
                    {                                                   /////  除外対象列挙値配列を繰り返す
                        if (enumValue.Equals(tmpEnum))
                        {                                               //////   列挙値が一致する場合(同じ列挙体で同じ内部整数値の場合)
                            return true;                                ///////    戻り値 = true(除外対象) で関数終了
                        }
                    }
                }

                return false;                                           //// 除外対象列挙値配列に含まれない場合、戻り値 = false(除外対象でない) で関数終了
            }

        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// <inheritdoc cref="GetValidValues"/>
        /// </summary>
        /// <typeparam name="TEnum">列挙体の型</typeparam>
        /// <param name="excludes"><inheritdoc cref="GetValidValues" path="/param[@name='excludes']"/></param>
        /// <returns><inheritdoc cref="GetValidValues"/></returns>
        /// <remarks><inheritdoc cref="GetValidValues"/></remarks>
        //--------------------------------------------------------------------------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]  // 積極的にインライン化する
        public static TEnum[] XGetValidValues<TEnum>(params TEnum[] excludes) where TEnum : struct, Enum
        {
            return M_CopyBaseToSpecific<TEnum>(GetValidValues(typeof(TEnum), M_CopySpecificToBase(excludes)));
        }


        //--------------------------------------------------------------------------------
        // 自動テスト用メソッド
        //--------------------------------------------------------------------------------
#if DEBUG
        [AutoTestMethod]
        public static void ZZZ_GetValidValues()
        {
            //--------------------------------------------------------
            SubRoutineNP(typeof(ZZZ_DiscKind), new Enum[] { ZZZ_DiscKind.CD, ZZZ_DiscKind.DVD, ZZZ_DiscKind.BD }, "除外指定なし");
            SubRoutine(typeof(ZZZ_DiscKind), new Enum[] { ZZZ_DiscKind.BD }, new Enum[] { ZZZ_DiscKind.CD, ZZZ_DiscKind.DVD }, "BDを除外指定");
            SubRoutine(typeof(ZZZ_DiscKind), new Enum[] { ZZZ_DiscKind.CD }, new Enum[] { ZZZ_DiscKind.DVD, ZZZ_DiscKind.BD }, "CDを除外指定");


            //--------------------------------------------------------
            AutoTest.Print("");
            AutoTest.Print("＜異常系のテスト＞");

            SubRoutineNP(null, typeof(ArgumentNullException), "enumType に null を指定");
            SubRoutineNP(typeof(int), typeof(ArgumentException), "enumType に int 型 を指定");


            //--------------------------------------------------------
            // params指定なし版
            void SubRoutineNP(Type enumType,                          // [in ]：列挙体の型
                              AutoTestResultInfo<Enum[]> expectResult,// [in ]：予想結果(Enum配列 または 例外の型情報)
                              string testPattern = null)              // [in ]：テストパターン名[null = 省略]

            {
                SubRoutine(enumType, new Enum[] { }, expectResult, testPattern);
            }


            //--------------------------------------------------------
            // params指定あり版
            void SubRoutine(Type enumType,                          // [in ]：列挙体の型
                            Enum[] excludes,
                            AutoTestResultInfo<Enum[]> expectResult,// [in ]：予想結果(Enum配列 または 例外の型情報)
                            string testPattern = null)              // [in ]：テストパターン名[null = 省略]

            {
                AutoTest.Test(GetValidValues, enumType, excludes, expectResult, testPattern);
            }
        }


        public enum ZZZ_DiscKind
        {
            [EnumerateIgnore]   // 列挙処理対象外
            Unknown = 0,

            CD,

            DVD,

            BD,
        }
#endif


        //====================================================================================================
        // 列挙値配列操作(必要になったら公開メソッド化を検討する)
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【列挙値配列コピー(特定型からEnum型へ)】特定型の列挙値配列を、Enum型の列挙値配列にコピーします。
        /// </summary>
        /// <typeparam name="TEnum">列挙体の型</typeparam>
        /// <param name="source">[in ]：特定型の列挙値配列</param>
        /// <returns>
        /// Enum型の列挙値配列
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・ジェネリックメソッドから非ジェネリックメソッドを呼び出す場合などに使用します。
        /// </remarks>
        //--------------------------------------------------------------------------------
        private static Enum[] M_CopySpecificToBase<TEnum>(TEnum[] source) where TEnum : struct, Enum
        {
            //------------------------------------------------------------
            /// 特定型の列挙値配列を、Enum型の列挙値配列にコピーする
            //------------------------------------------------------------
            Enum[] result = new Enum[source.Length];                    //// Enum型の列挙値配列を生成する
            for (int tmpIndex = 0; tmpIndex < source.Length; tmpIndex++)
            {                                                           //// 特定型の列挙値配列を繰り返す
                result[tmpIndex] = (Enum)source[tmpIndex];              /////  要素をコピーする
            }
            return result;                                              //// 戻り値 = Enum型の列挙値配列 で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【列挙値配列コピー(Enum型から特定型へ)】Enum型の列挙値配列を、特定型の列挙値配列にコピーします。
        /// </summary>
        /// <typeparam name="TEnum">列挙体の型</typeparam>
        /// <param name="source">[in ]：Enum型の列挙値配列</param>
        /// <returns>
        /// 特定型の列挙値配列
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・ジェネリックメソッドから非ジェネリックメソッドを呼び出す場合などに使用します。
        /// </remarks>
        //--------------------------------------------------------------------------------
        private static TEnum[] M_CopyBaseToSpecific<TEnum>(Enum[] source) where TEnum : struct, Enum
        {
            //------------------------------------------------------------
            /// Enum型の列挙値配列を、特定型の列挙値配列にコピーする
            //------------------------------------------------------------
            TEnum[] result = new TEnum[source.Length];                  //// 特定型の列挙値配列を生成する
            for (int tmpIndex = 0; tmpIndex < source.Length; tmpIndex++)
            {                                                           //// Enum型の列挙値配列を繰り返す
                result[tmpIndex] = (TEnum)source[tmpIndex];             /////  要素をコピーする
            }
            return result;                                              //// 戻り値 = 特定型の列挙値配列 で関数終了
        }



#if false   // 扱いを保留中
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【フルネーム取得】列挙値のフルネーム(＜フルパス列挙体名＞.＜列挙定数名＞)を取得します。
        /// </summary>
        /// <param name="target">[in ]：列挙値</param>
        /// <returns>
        /// フルネーム
        /// </returns>
        //--------------------------------------------------------------------------------
        public static string XGetFullName(this Enum target)
        {
            return target.GetType().ToString() + "." +
                   target.ToString();
        }


        // 「(DayOfWeek)6」のように、普通にキャストで良さそうな？
        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【列挙値取得】内部整数値に対応する列挙値を取得します。
        /// 列挙体に同じ内部整数値を持つ列挙値が複数定義されている場合は、どの列挙値を返すかは不定です。
        /// </summary>
        /// <param name="enumType">[in ]：列挙体の型情報</param>
        /// <param name="binValue">[in ]：内部整数値</param>
        /// <returns>
        /// 列挙値(null = 取得失敗)
        /// </returns>
        //--------------------------------------------------------------------------------
        public static object XGetEnumValue(Type enumType, object binValue)
        {
            //------------------------------------------------------------
            /// 内部整数値に対応する列挙値を取得する
            //------------------------------------------------------------
            var enumName = Enum.GetName(enumType, binValue);            //// 内部整数値に対応する列挙定数名を取得する
            if (enumName == null)
            {                                                           //// 取得失敗の場合(対応する列挙値が定義されていない場合)
                return null;                                            /////  戻り値 = null(取得失敗) で関数終了
            }

            return Enum.Parse(enumType, enumName);                      //// 戻り値 = 列挙定数名に対応する列挙値 で関数終了(必ず成功する)
        }
#endif


        //====================================================================================================
        // 列挙値を文字列形式でシリアル化するためのメソッド群
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【XMLシリアル化用列挙値文字列作成】
        /// 列挙値から列挙値文字列を作成します。<br/>
        /// ・列挙値 = デフォルト値 の場合、列挙値文字列は null になります。<br/>
        /// ・デフォルト値を null とする処理が不要な場合は、ToString() で作成します。<br/>
        /// </summary>
        /// <typeparam name="TEnum">列挙体の型</typeparam>
        /// <param name="target">      [in ]：列挙値</param>
        /// <param name="defaultValue">[in ]：デフォルト値</param>
        /// <returns>
        /// 列挙値文字列(列挙値がデフォルト値の場合は null)
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・XStrMake と XStrParse は対称的です。<br/>
        /// ・追加・変更・削除される可能性のある列挙値や、内部整数値0を持たない列挙値(TraceEventTypeなど)を
        ///   XMLファイルへシリアル化するために使用します。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static string XStrMake<TEnum>(this TEnum target, TEnum defaultValue) where TEnum : struct, Enum
        {
            //------------------------------------------------------------
            /// 列挙値から列挙値文字列を作成する
            //------------------------------------------------------------
            if (target.Equals(defaultValue))
            {                                                           //// 列挙値 = デフォルト値 の場合
                return null;                                            /////  戻り値 = null で関数終了
            }
            else
            {                                                           //// 列挙値 = デフォルト値 でない場合
                return target.ToString();                               /////  戻り値 = 列挙定数名 で関数終了
            }
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【XMLシリアル化用列挙値文字列解析】
        /// 列挙値文字列から対応する列挙値を取得します。<br/>
        /// 列挙値文字列が無効な場合(null、空文字列、列挙体に含まれない文字列)の場合はデフォルト値を返します。
        /// </summary>
        /// <typeparam name="TEnum">列挙体の型</typeparam>
        /// <param name="enumName">    [in ]：列挙値文字列</param>
        /// <param name="defaultValue">[in ]：デフォルト値(解析失敗時に返す値)</param>
        /// <returns>
        /// 列挙値
        /// </returns>
        /// <remarks>
        /// 補足<br/>
        /// ・XStrMake と XStrParse は対称的です。<br/>
        /// ・追加・変更・削除される可能性のある列挙値や、内部整数値0を持たない列挙値(TraceEventTypeなど)を
        ///   XMLファイルへシリアル化するために使用します。<br/>
        /// </remarks>
        //--------------------------------------------------------------------------------
        public static TEnum XStrParse<TEnum>(string enumStr, TEnum defaultValue) where TEnum : struct, Enum
        {
            //------------------------------------------------------------
            /// 列挙値文字列に対応する列挙値を取得する
            //------------------------------------------------------------
            if (string.IsNullOrEmpty(enumStr))
            {                                                           //// 列挙値文字列 = null または 空文字列 の場合
                return defaultValue;                                    /////  戻り値 = デフォルト値 で関数終了
            }

            var success = Enum.TryParse(enumStr, out TEnum enumValue);  //// 列挙値文字列の解析を試行して列挙値を取得する
            if (success)
            {                                                           //// 解析成功の場合
                return enumValue;                                       /////  戻り値 = 列挙値 で関数終了
            }
            else
            {                                                           //// 解析失敗の場合
                return defaultValue;                                    /////  戻り値 = デフォルト値 で関数終了
            }
        }


#if false   // 扱いを保留中
        //====================================================================================================
        // 列挙値リストを文字列形式でシリアル化するためのメソッド群
        //====================================================================================================

        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【XMLシリアル化用列挙値文字列作成】
        /// 列挙値リストから列挙値文字列リストを作成します。
        /// 列挙値リストが無効(null または 空のリスト)の場合、列挙値文字列リストは null になります。
        /// </summary>
        /// <typeparam name="TEnum">列挙体の型</typeparam>
        /// <param name="enumValues">[in ]：列挙値リスト</param>
        /// <returns>
        /// 列挙値文字列リスト
        /// </returns>
        //--------------------------------------------------------------------------------
        public static List<string> XStrListMake<TEnum>(this List<TEnum> enumValues) where TEnum : struct, Enum
        {
            //------------------------------------------------------------
            /// 列挙値リストから列挙値文字列リストを作成する
            //------------------------------------------------------------
            if (enumValues == null)
            {                                                           //// 列挙値リスト = null の場合
                return null;                                            /////  戻り値 = null で関数終了
            }
            if (enumValues.Count == 0)
            {                                                           //// 列挙値リストの件数 = 0 の場合
                return null;                                            /////  戻り値 = null で関数終了
            }

            var enumStrs = new List<string>();                          //// 列挙値文字列リストを生成する
            foreach (var enumValue in enumValues)
            {                                                           //// 列挙値リストを繰り返す
                enumStrs.Add(enumValue.ToString());                     /////  列挙定数名を列挙値文字列リストに追加する
            }

            return enumStrs;                                            //// 戻り値 = 列挙値文字列リスト で関数終了
        }


        //--------------------------------------------------------------------------------
        /// <summary>
        /// 【XMLシリアル化用列挙値文字列リスト解析】
        /// 列挙値文字列リストから列挙値リストを生成します。
        /// 列挙値文字列が無効な要素(null、空文字列、列挙体に含まれない文字列)は無視します。
        /// 列挙値文字列リストが無効(null または 空のリスト)の場合、列挙値リストは null になります。
        /// </summary>
        /// <typeparam name="TEnum">列挙体の型</typeparam>
        /// <param name="enumStrs">[in ]：列挙値文字列リスト</param>
        /// <returns>
        /// 列挙値リスト
        /// </returns>
        //--------------------------------------------------------------------------------
        public static List<TEnum> XStrListParse<TEnum>(List<string> enumStrs) where TEnum : struct, Enum
        {
            //------------------------------------------------------------
            /// 列挙値文字列リストから列挙値リストを生成する
            //------------------------------------------------------------
            if (enumStrs == null)
            {                                                           //// 列挙値文字列リスト = null の場合
                return null;                                            /////  戻り値 = null で関数終了
            }

            if (enumStrs.Count == 0)
            {                                                           //// 列挙値文字列リストの件数 = 0 の場合
                return null;                                            /////  戻り値 = null で関数終了
            }

            var enumValues = new List<TEnum>();                         //// 列挙値リストを生成する
            foreach (var enumStr in enumStrs)
            {                                                           //// 列挙値文字列リストを繰り返す
                var success =
                    Enum.TryParse(enumStr, out TEnum enumValue);        //// 列挙値文字列の解析を試行して列挙値を取得する
                if (success)
                {                                                       //// 解析成功の場合
                    enumValues.Add(enumValue);                          //////   列挙値をリストに追加する
                }
            }

            if (enumValues.Count == 0)
            {                                                           //// 列挙値リストの件数 = 0 の場合(全要素とも解析失敗した場合)
                return null;                                            /////  戻り値 = null で関数終了
            }

            return enumValues;                                          //// 戻り値 = 列挙値リスト で関数終了
        }
#endif


        //====================================================================================================
        //
        // 【列挙型(Enum)関連の呼称整理】
        //   列挙体(enum)            ：DayOfWeek 列挙体や PlatformID 列挙体など、特定の列挙型のこと。
        //   列挙型(enumeration type)：System.Enum 型から派生し、関連する複数の名前付き定数を定義している型のこと。ある特定の列挙型を指す場合は「列挙体(enum)」と呼ぶことが多い。
        //   列挙値(enumeration)(*2) ：列挙型の中の特定の値のこと。実体は内部整数値であり、列挙定数名とは１対１で対応しないこともある。
        //   列挙定数名(*1)          ：列挙型定数につけた名前(name of the constant)のこと。
        //   内部整数値(*1)          ：列挙型定数の実体である数値(value of the constant)のこと。内部表現型の整数。
        //   内部表現型(*1)          ：列挙体の基になる型(underlying type)のこと。byte, sbyte, short, ushort, int, uint, long, ulong のいずれか。
        //   (*1)便宜上の呼称であり、正式名称ではない。
        //   (*2)単に「列挙値」という場合、その実体は「列挙定数名で指定した内部整数値」であることが多い。
        //       (DayOfWeek)65535 のようにキャストして、「列挙体に存在しない列挙値」になっている場合もあるので注意が必要。
        //
        // ＜例：DayOfWeek 列挙体の場合＞
        //   列挙型名  ：DayOfWeek
        //   内部表現型：int
        //
        //   列挙値            ：列挙定数名：内部整数値
        //   ------------------：----------：----------
        //   DayOfWeek.Sunday  ："Sunday"  ：  0
        //   DayOfWeek.Monday  ："Monday"  ：  1
        //     ：              ：   ：     ：  :
        //   DayOfWeek.Saturday："Saturday"：  6
        //
        // ＜例：列挙体定義の場合＞
        //   public enum ＜列挙型名＞ [: ＜内部表現型＞]
        //   {
        //       ＜列挙定数名＞ [= ＜内部整数値＞],       <-- 列挙子(名前付き定数)  --+
        //       ＜列挙定数名＞ [= ＜内部整数値＞],       <-- 列挙子(名前付き定数)    +-- 列挙子リスト
        //           ：                                                               |
        //       ＜列挙定数名＞ [= ＜内部整数値＞]        <-- 列挙子(名前付き定数)  --+
        //   }
        //
        // ＜ヘルプで使用されているその他の呼称(コメント記入時にはあまり使わない)＞
        //   名前付き定数(named constants)：「<列挙定数名> = <内部整数値>」で定義する１つの定数のこと。
        //   列挙型変数(enum variable)    ：System.Enum 型やその派生型のインスタンスのこと。
        //   バイナリ値(binary values)    ：内部整数値のバイナリ表現のこと。Enum.GetValues が返す配列の要素の並び順はこの値に基づく。
        //                                ：内部表現型が符号付きの場合、0 ～ MaxValue ～ MinValue ～ -1 という並び順になる。
        //
        // ＜enum ステートメントでのみ使う呼称＞
        //   列挙子(enumerator)           ：enum ステートメントの中で定義する名前付き定数のうちの１つのこと。
        //   列挙子リスト(enumerator list)：enum ステートメントの中で定義する名前付き定数の集まりのこと。
        //
        //====================================================================================================


        //[-] 保留：必要になったら(XEnum)。列挙型配列のソート機能
        //          内部整数値 [昇順／降順]    MinValue ～ 0 ～ MaxValue
        //          バイナリ値 [昇順／降順]    0 ～ MinValue ～ MaxValue ～ -1  <-  Enum.GetValues はこれ
        //          列挙定数名 [昇順／降順]
        //          列挙値表示名 [昇順／降順]

    } // class XEnum


#if DEBUG
    //====================================================================================================
    /// <summary>
    /// 【メモ】１つの列挙体に同じ内部整数値を持つ列挙値が複数重複定義されている場合について<br/>
    /// 列挙型の値は内部的には整数値で扱われているため、１つの列挙体に同じ内部整数値を持つ列挙値が複数重複定義されている場合は、
    /// 比較処理や列挙定数名との変換処理では注意が必要です。<br/>
    /// <br/>
    /// <code>
    /// 【例：Environment.SpecialFolder 列挙体の場合】
    /// ・列挙定数名 Environment.SpecialFolder.Personal    に対応する内部整数値は 5。
    /// ・列挙定数名 Environment.SpecialFolder.MyDocuments に対応する内部整数値も 5。
    /// ・Environment.SpecialFolder.Personal と Environment.SpecialFolder.MyDocuments は同一と判定される。
    /// ・内部整数値 5 に対応する列挙定数名を取得するとき、"Personal" になるか "MyDocuments" になるかは、未定義であり不定。
    /// </code>
    /// </summary>
    //====================================================================================================
    public class Memo_DuplicateEnumValue : ZZZ
    {
        [ManualTestMethod("検証実験：１つの列挙体に同じ内部整数値を持つ列挙値が複数重複定義されている場合について")]
        public static void Examination()
        {
            Debug.Print("【Enum.ToString】同じ内部整数値を持つ列挙値が複数定義されている場合、ToString() がどの列挙定数名を返すかは未定義であり不定");
            Debug.Print("Personal    = " + Environment.SpecialFolder.Personal);
            Debug.Print("MyDocuments = " + Environment.SpecialFolder.MyDocuments);
            Debug.Print("");

            Debug.Print("【Enum.GetName】同じ内部整数値を持つ列挙値が複数定義されている場合、GetName() がどの列挙定数名を返すかは未定義であり不定");
            Debug.Print("Personal    = " + Enum.GetName(typeof(Environment.SpecialFolder), Environment.SpecialFolder.Personal));
            Debug.Print("MyDocuments = " + Enum.GetName(typeof(Environment.SpecialFolder), Environment.SpecialFolder.MyDocuments));
            Debug.Print("");

            Debug.Print("【Enum.Parse】同じ内部整数値を持つ列挙値が複数定義されている場合、どの列挙定数名からでも同じ列挙値に変換される");
            Debug.Print("Personal    = " + Enum.Parse(typeof(Environment.SpecialFolder), "Personal").ToString());
            Debug.Print("MyDocuments = " + Enum.Parse(typeof(Environment.SpecialFolder), "MyDocuments").ToString());
            Debug.Print("");

            Debug.Print("【== で比較】内部整数値が同じであれば同一と判定される");
            Debug.Print("Desktop  == default    :" + (Environment.SpecialFolder.Desktop == (Environment.SpecialFolder)default));
            Debug.Print("Personal == MyDocuments:" + (Environment.SpecialFolder.Personal == Environment.SpecialFolder.MyDocuments));
            Debug.Print("");

            Debug.Print("【Equals で比較】内部整数値が同じであれば同一と判定される");
            Debug.Print("Desktop.Equals(default)     :" + Environment.SpecialFolder.Desktop.Equals((Environment.SpecialFolder)default));
            Debug.Print("Personal.Equals(MyDocuments):" + Environment.SpecialFolder.Personal.Equals(Environment.SpecialFolder.MyDocuments));
            Debug.Print("");

            Debug.Print("【異なる列挙型を Equals で比較】内部整数値が同じでも同一でないと判定される");
            Debug.Print("DayOfWeek.Sunday.Equals(PlatformID.Win32S)：" + DayOfWeek.Sunday.Equals(PlatformID.Win32S));        // 同じ内部整数値を持つ異なる列挙体の列挙値を Equalsで比較 = 不一致
            Debug.Print("int型に変換して == で比較すれば当然等しい ：" + ((int)DayOfWeek.Sunday == (int)PlatformID.Win32S)); // 同じ内部整数値を持つ異なる列挙体の列挙値を int型に変換して比較 = 等しい
            Debug.Print("");


#if JIKKEN  // 内部整数値が同じ列挙定数名は switch ステートメントの中に存在できない(コンパイルエラーになる)
            var enumValue = Environment.SpecialFolder.Personal;
            switch (enumValue)
            {
                case Environment.SpecialFolder.Personal:
                    break;

                case Environment.SpecialFolder.MyDocuments:
                    break;
            }
#endif

        }

    } // class Memo_DuplicateEnumValue

#endif


    //====================================================================================================
    /// <summary>
    /// 【文字列形式で保持・シリアル化する列挙値】<br/>
    /// 文字列形式で保持・シリアル化し、認識できない列挙定数名(後のバージョンで追加された列挙値)があっても例外を発生させずに代替手段でカバーできるようにします。<br/>
    /// 列挙値で取得・設定する際は、アクセサー経由で変換します。<br/>
    /// </summary>
    /// <remarks>
    /// ＜補足事項＞この属性クラスはいまのところ機能的役割はありません。<br/>
    /// ・文字列形式で保持・シリアル化することで、認識できない列挙定数名も、意図的に更新しない限り元の内容を維持する。(*1)<br/>
    /// ・取り扱いは列挙値で行う。取得・設定時に文字列形式に変換し、文字列形式で保持・シリアル化する。<br/>
    /// ・列挙値に変換して取得する際、null や認識できない列挙定数名はデフォルト値として取得する。<br/>
    /// ・列挙値から変換して設定する際、デフォルト値は null文字列に変換し、XMLファイルでは省略されるようにする。<br/>
    /// <br/>
    /// *1列挙値で保持し、シリアル化／逆シリアル化のときに文字列型に変換する方法もあるが、
    ///   そのやり方だとこれが実現できず、認識できない列挙定数名はデフォルト値に書き換わってしまう。<br/>
    /// </remarks>
    //====================================================================================================
    public class EnumStrAttribute : Attribute
    {
        // public enum WeatherKind  // 天候種別
        // {
        //     Shiny,      // 晴天 <- これがデフォルト値とする
        //     Cloudy,     // 曇天
        //     Rainy,      // 雨天
        //     Snowfall,   // 雪   <- 後のバージョンで追加したという想定
        // }
        //
        // ＜このバージョンが "Snowfall" を認識できない場合＞
        // ・XMLファイルからは文字列形式 "Snowfall" のまま読み込む
        // ・バッキングフィールドには文字列形式 "Snowfall" のまま保持する
        // ・列挙値で取得する際は、認識できない列挙定数名なのでデフォルト値(WeatherKind.Shiny)になる
        // ・XMLファイルに保存するときは、バッキングフィールド内の文字列形式 "Snowfall" のまま書き込む
        //
        // ＜列挙値で保持し、シリアル化／逆シリアル化のときに文字列型に変換する方法だと＞
        // ・XMLファイルから読み込む際、認識できない列挙定数名なのでデフォルト値(WeatherKind.Shiny)に書き換わってしまう
        // ・そのまま保存すると、"Snowfall" が "Shiny" に書き換わってしまう

    } // class

} // namespace
