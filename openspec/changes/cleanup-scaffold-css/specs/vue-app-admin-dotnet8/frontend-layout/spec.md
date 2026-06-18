## MODIFIED Requirements

### Requirement: MainSidebar 從 router meta 衍生選單
`MainSidebar.vue` SHALL 讀取 router 中 `meta.showInSidebar === true` 的路由，自動產生選單連結（顯示 `meta.sidebarLabel`，連結至該路由 path）。元件 SHALL 依賴 Vue Router 的全域注冊取得 `<RouterLink>`，不得在 `<script setup>` 中明確 import `RouterLink`。

#### Scenario: 路由加入 meta.showInSidebar 後自動出現在選單
- **WHEN** router 中有路由設定 `meta: { showInSidebar: true, sidebarLabel: 'Example Items' }`
- **THEN** Sidebar 顯示該選單項目，點擊後導向對應路由

#### Scenario: 無 showInSidebar 的路由不出現在選單
- **WHEN** router 中有路由未設定 `meta.showInSidebar`（或設為 `false`）
- **THEN** Sidebar 不顯示該路由的選單項目
