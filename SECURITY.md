# Security Policy

AI Workbox v0 はセキュリティ sandbox ではありません。

名前に workbox と付いていますが、悪い code を閉じ込める箱ではないです。

## できる範囲

今の範囲は process lifecycle control です。

- command を起動する
- root process を Windows Job Object に入れる
- local process tree と listening port を見る
- 名前付き workbox を止める

## できない範囲

AI Workbox v0 に、ここは期待しないでください。

- secret の隔離
- file system の隔離
- network の隔離
- 悪意ある code の封じ込め
- 企業向け endpoint protection
- 会社管理端末の policy enforcement

workbox の中で動く process は、今の user が読める file を読めます。network も使えます。

## 報告

問題を見つけたら GitHub issue に書いてください。

secret、credential、private source code、`.env`、browser profile data、会社の情報は貼らないでください。

## 安全に試す

最初は捨ててもいい command で試してください。

```powershell
workbox run --name smoke --timeout-seconds 5 -- pwsh -NoProfile -Command "Start-Sleep -Seconds 2; 'done'"
```

third-party tool を Workbox の中で動かす場合も、その tool は現在の user と同じ権限で動く、と考えてください。
