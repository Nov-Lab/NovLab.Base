# NovLab.Base クラスライブラリ

基本的な部品を提供する .NET Framework用クラスライブラリです。

基本的なアセンブリにのみ依存します。


# 動作環境

- Windows 8.1以降
- .NET Framework 4.0 以降、または互換性のある .NET 実装


# 特長

こまごまとした部品を多数そろえています。一例として、以下のような機能があります。


# XString クラス

System.String クラスに拡張メソッドを追加します。

## string.XReplaceNewLineChars 拡張メソッド
文字列中の改行文字群 (CR+LF, CR, LF) を別の文字列に置換します。

###### 使用例：strValue 中の改行文字群 (CR+LF, CR, LF) を半角スペースに置換して一行にまとめます。
```
Debug.Print(strValue.XReplaceNewLineChars(" "));
```

###### 使用例：strValue 中の改行文字群 (CR+LF, CR, LF) を CR+LF に統一します。
```
Debug.Print(strValue.XReplaceNewLineChars("\r\n"));
```

## string.XEscape 拡張メソッド
エスケープコンバーターを使用して文字列をエスケープします。

###### 使用例：strValue 中の制御コード文字を制御機能用記号に置き換えて可視化します。
```
Debug.Print(strValue.XEscape(EscapeConverter.CcVisualization));
```


# XInt クラス

System.Int クラスに拡張メソッドを追加します。

## int.XIsInRange 拡張メソッド
int値が最小値から最大値の範囲内にあるかどうかをチェックします。

###### 使用例：int値が 32～127 の範囲にあるかをチェックします。
```
bool result = intValue.XIsInRange(32, 127);
```


# XICollection クラス

コレクションI/Fに拡張メソッドを追加します。

## ICollection.XAppend 拡張メソッド
指定したコレクションの全要素を末尾に追加します。

###### 使用例：文字列リストに文字列配列の全要素を追加します。
```
var stocks = new List<string>();  // 在庫品リスト
stocks.Add("Apple");
stocks.Add("Orange");

var arrivals = new string[] { "Lemon", "Strawberry" };  // 入荷品

stocks.XAppend(arrivals);                               // 在庫品リストに入荷品を追加する
```


# フォルダー構成

- `binfile` ：コンパイル済みのバイナリーファイルです。
- `doc` ：ドキュメント類です。
- `NovLab.Base` ：クラスライブラリ本体のプロジェクトです。
- `Test_NovLab` ：テスト用プロジェクトです。


# ライセンス

本ソフトウェアは、MITライセンスに基づいてライセンスされています。

ただし、改変する場合は、namespace の名前を変えて重複や混乱を避けることを強く推奨します。


# 開発環境

## 開発ツール、SDKなど
- Visual Studio Community 2019
  - ワークロード：.NET デスクトップ開発

## 言語
- C#


# その他

Nov-Lab 独自の記述ルールと用語については [NovLabRule.md](https://github.com/Nov-Lab/Nov-Lab/NovLabRule.md) を参照してください。
