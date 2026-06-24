# Security Policy

AI Workbox v0 はセキュリティ sandbox ではありません。

## サポートする範囲

今の範囲は process lifecycle control です。

- command を起動する
- root process を Windows Job Object に入れる
- local process tree と listening port を見る
- 名前付き workbox を止める

## サポートしない範囲

AI Workbox v0 を、以下の目的には使わないでください。

- secret の隔離
- file system の隔離
- network の隔離
- 悪意ある code の封じ込め
- 企業向け endpoint protection
- 会社管理端末の policy enforcement

process は、今の user が読める file や使える network をそのまま使えます。

## 報告

問題を見つけたら GitHub issue で報告してください。

secret、credential、private source code、`.env`、browser profile data、会社の情報は貼らないでください。

## 安全に試す

最初は throwaway command か local fixture で試してください。

```powershell
workbox run --name smoke --timeout-seconds 5 -- pwsh -NoProfile -Command "Start-Sleep -Seconds 2; 'done'"
```

third-party tool を Workbox の中で動かす場合も、その tool は現在の user と同じ権限で file と network を使える、と考えてください。
