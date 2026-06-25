## ADDED Requirements

### Requirement: 使用者可手動切換深色／淺色主題
Header 右上角 SHALL 提供一個 icon 按鈕，點擊後即時切換深色與淺色主題。深色模式顯示太陽 icon，淺色模式顯示月亮 icon。

#### Scenario: 淺色模式下點擊切換
- **WHEN** 目前為淺色模式，使用者點擊 toggle 按鈕
- **THEN** 頁面 SHALL 立即切換為深色主題，按鈕顯示太陽 icon

#### Scenario: 深色模式下點擊切換
- **WHEN** 目前為深色模式，使用者點擊 toggle 按鈕
- **THEN** 頁面 SHALL 立即切換為淺色主題，按鈕顯示月亮 icon

---

### Requirement: 主題偏好持久化於 localStorage
使用者切換的主題偏好 SHALL 儲存至 localStorage（key: `theme`，value: `'dark'` 或 `'light'`），重新整理或重新開啟後 SHALL 恢復上次選擇。

#### Scenario: 重新整理後恢復主題
- **WHEN** 使用者設定深色主題後重新整理頁面
- **THEN** 頁面 SHALL 以深色主題呈現，不閃爍回淺色再切換

#### Scenario: 首次訪問無記錄
- **WHEN** localStorage 無 `theme` 記錄
- **THEN** SHALL 依 `prefers-color-scheme` 決定初始主題

---

### Requirement: PrimeVue 與 Bootstrap 主題同步
切換主題時，PrimeVue 與 Bootstrap 的深色主題 SHALL 同步套用，不得出現其中一套仍顯示淺色的情況。

#### Scenario: 深色切換時雙框架同步
- **WHEN** 切換為深色主題
- **THEN** `<html>` SHALL 同時具有 `.p-dark` class 與 `data-bs-theme="dark"` attribute

#### Scenario: 淺色切換時雙框架同步
- **WHEN** 切換為淺色主題
- **THEN** `<html>` SHALL 移除 `.p-dark` class 並設定 `data-bs-theme="light"`
