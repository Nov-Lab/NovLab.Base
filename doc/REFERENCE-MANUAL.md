# NovLabBase クラス ライブラリ・リファレンス マニュアル


## 名前空間
- NovLab                          ：特定のカテゴリに属さない基本的かつ汎用的な機能を提供します。
- NovLab.DebugStation             ：デバッグ支援アプリ DebugStation との連携機能を提供します。
- NovLab.DebugSupport             ：デバッグ支援機能を提供します。
- NovLab.EnumDisplayNameUti       ：列挙値から対応する表示名を取得するための仕組みを提供します。
- NovLab.Globalization            ：カルチャに関連する機能を提供します。
- NovLab.IO                       ：入出力操作に関連する機能を提供します。
- NovLab.IO.Mailslot              ：メールスロットによるプロセス間通信機能を提供します。
- NovLab.IO.Pipes                 ：パイプ操作に関連する機能を提供します。
- NovLab.Threading                ：マルチスレッド プログラミングのための機能を提供します。
- NovLab.Win32                    ：Win32 API の操作機能を提供します。
- NovLab.Windows.ApplicationHelper：Windowsアプリケーション用のヘルパー機能を提供します。

- NovLabDraft                     ：仮実装やテスト不十分など、開発途上にある下書き版のものを格納しています。

### ＜補足＞
- 既存のクラス名やルート名前空間名と衝突するなど、問題が生じそうな場合はプリフィックス「NL」をつけます。


## 補足事項
- ソースファイルを格納するフォルダー名は、基本的に名前空間名と合致させるようにしていますが、以下のような例外もあります。
  - `Base`    ：名前空間「NovLab」直下のものを格納しています。
  - `Win32API`：フォルダー名を「Win32」にすると Git のソース管理から除外されてしまうため、それを避けるために「Win32API」という名前にしています。
