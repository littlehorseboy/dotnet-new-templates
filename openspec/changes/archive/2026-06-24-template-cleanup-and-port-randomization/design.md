## Context

`vue-app-admin-dotnet8` 是一個 `dotnet new` 範本，透過 `.template.config/template.json` 控制產出行為。目前存在三個獨立問題：

1. **Artifacts 洩漏**：`dist/` 與 `package-lock.json` 未排除，會進入消費者產出目錄
2. **Port 衝突風險**：HTTP（5159）、HTTPS（7173）、SPA proxy（23288）三個 port 全部硬碼，多人使用同一台機器時容易衝突
3. **格式問題**：`.sln` 是舊格式；`.vscode/launch.json` 含 `type: "edge"` 造成 VS Code 警告

Template engine 版本：.NET SDK 內建的 `dotnet new` template engine（Microsoft.TemplateEngine）。

## Goals / Non-Goals

**Goals:**
- 確保產出目錄不含 `dist/`、`package-lock.json`
- `dotnet new` 時五個 port 各自隨機化，範圍不重疊，五位數（含 IIS Express）
- `.vscode/launch.json` 只保留 chrome，port 由 symbol 替換
- 方案檔升級為 `.slnx`（VS 2022 17.9+）

**Non-Goals:**
- pnpm 相關文件不處理
- `esproj` SDK 版本不更新
- `.vscode/settings.json` 不修改

## Decisions

### 決策 1：以現有 port 值作為 symbol placeholder

**選項 A（採用）**：直接用現有的 port 值（`5159`、`7173`、`23288`）作為 `replaces` 目標，template 檔案不需修改。

**選項 B**：使用人工佔位值（如 `5001`、`7001`、`23001`），需要在所有 template 檔案中先做一次搜尋替換。

**選 A 的理由**：
- 現有 port 值夠特殊，不會意外替換到其他內容
- Template 本身在開發測試時仍為合法的可執行狀態
- `23288` 本身已落在預定的 20000–29999 範圍內，無需特別處理

---

### 決策 2：五個 port symbol 使用不重疊範圍

```
HttpPort:    16100–16199  replaces 5159
HttpsPort:   16200–16299  replaces 7173
IisPort:     16300–16399  replaces 21655
IisSslPort:  16400–16499  replaces 44385
SpaPort:     16500–16599  replaces 23288
```

五個 symbol 集中在 16100–16599，連號、好認，明顯有別於系統 port 與開發慣例帶（5000–9999）；各段不重疊確保同一次 `dotnet new` 的五個 port 不會產生相同值。

Windows 在啟用 Hyper-V/WSL2/Docker Desktop 時會自動排除 50000+ 的 port 區段，50xxx 範圍因此不可靠；16xxx 不受此影響。各 symbol 帶有 `fallback` 參數，萬一整段均不可用時使用 range 低端值而非 `0`。

`port` generator 在指定範圍內找出一個當下機器未使用的 port，不只是隨機數字。

---

### 決策 3：.slnx 格式，同步更新 guids

`.slnx` 是 XML 格式，可讀性高，VS 2022 17.9+ 原生支援，`dotnet` CLI 兩者皆支援。

方案 GUID（`D03907AB-36C4-4CD5-B032-407E50BBB78C`）需納入 `template.json` 的 `guids` 清單，確保產出的 `.slnx` GUID 會被替換為新值。

---

### 決策 4：移除 launch.json 的 edge configuration

僅移除 `type: "edge"` 的 configuration block，保留 `type: "chrome"`。不刪除整個 `.vscode/launch.json`，讓消費者仍有基本的 chrome debug config。

## Risks / Trade-offs

- **`port` generator 的範圍邊界**：若指定機器的某個範圍內所有 port 都被佔用（極端情況），generator 行為未明確記錄。→ 實際風險極低；範圍足夠大（HttpPort 1000 個、SpaPort 10000 個）
- **`.slnx` 不向後相容**：使用 VS 2022 17.8 以下或部分 CI 工具可能無法讀取。→ 此範本已標記 VS 17.14，消費者門檻對齊

## Migration Plan

此變更只影響 template 本身的設定與格式，不影響已產生的專案。消費者重新安裝範本（`dotnet new install` 後更新）即可享有新行為，舊的已產生專案不受影響。
