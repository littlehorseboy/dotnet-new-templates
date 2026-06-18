## Context

VueApp1 是一個 Vue 3 + ASP.NET Core Web API 的全端方案，目前以普通專案目錄存在。`dotnet new` 的範本機制需要在方案根目錄下放置 `.template.config/template.json`，才能讓 SDK 辨識並安裝。

本次設定參考既有的 `template.json` 結構，調整 identity、short name 與 GUID。

## Goals / Non-Goals

**Goals:**
- 建立 `VueApp1/.template.config/template.json`
- 支援 `sourceName` 替換（專案名稱自動套用至檔名與內容）
- 支援 `nameLower` symbol（處理 `vueapp1.client` 小寫資料夾名稱）
- 排除不應納入範本的檔案（bin、obj、node_modules 等）

**Non-Goals:**
- 不修改 VueApp1 現有的任何程式碼或設定檔
- 不發佈至 NuGet；僅支援本機 `dotnet new install` 安裝
- 不處理多語言或條件式檔案包含

## Decisions

**短名稱使用 `vue-app-demo`**
避免與其他已安裝範本的 short name 衝突。

**`sourceName` 設為 `"VueApp1"`**
`.NET SDK` 的範本引擎會將所有出現 `VueApp1` 的地方（檔名、資料夾名、檔案內容）替換為使用者指定的 `-n` 名稱。方案名稱、專案名稱、命名空間均由此自動處理。

**`nameLower` symbol 處理小寫資料夾**
前端目錄名稱為 `vueapp1.client`（全小寫），無法由 `sourceName` 直接處理大小寫變形。透過 `derived` symbol + `lowerCase` transform 加上 `fileRename: "vueapp1"` 來解決。

**GUID 使用新值**
每個範本的 `guids` 需唯一，確保不同範本產生的專案 GUID 不會衝突。

## Risks / Trade-offs

- **GUID 衝突** → 產生全新 GUID
- **short name 拼錯或重複** → 安裝前先執行 `dotnet new list` 確認無衝突
- **排除清單不完整** → 逐一確認排除項目

## Migration Plan

1. 建立 `.template.config/template.json`
2. 執行 `dotnet new install .\VueApp1` 安裝
3. 執行 `dotnet new vue-app-demo -n TestApp` 驗證產生結果
4. 確認 `TestApp/` 下所有 `VueApp1` 字串與資料夾名稱均已替換

## Open Questions

- Short name `vue-app-demo` 是否為最終名稱，或日後要改為其他識別字（如 `aspnet-vue3`）？
