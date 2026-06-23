## Why

`vueappadmin.client` 的部分相依套件已發布新的大版本，其中 `@primevue/themes` 已標記為 deprecated，需遷移至替代套件 `@primeuix/themes`。為維持 template 的長期可維護性，統一進行此次大版本升級。

Font Awesome 升級（6 → 7）因 `@fortawesome/vue-fontawesome` 尚無對應的 v4 版本而暫緩，保持 v6 不動。

## What Changes

- 將 `@primevue/themes` 替換為 `@primeuix/themes`（deprecated 套件遷移）
- 升級 `vue-router` 4 → 5（大版本）
- 升級 `dotenv` 16 → 17（devDependency，大版本）
- 升級 `npm-run-all2` 8 → 9（devDependency，大版本）
- **不升級** `@fortawesome/*`：`vue-fontawesome@4.x` 尚未發佈，FA7 升級暫緩

## Capabilities

### New Capabilities

（本次為套件維護，無新增使用者功能）

### Modified Capabilities

- `primevue-theme-setup`：`@primevue/themes` 替換為 `@primeuix/themes`，import 路徑需全面更新
- `router-setup`：`vue-router` 升至 v5，需確認 API 相容性與 TypeScript 型別是否有 breaking change

## Impact

- `package.json`：修改套件版本聲明，移除 `@primevue/themes`，加入 `@primeuix/themes`
- `main.ts`（或 PrimeVue 初始化處）：`definePreset` / preset import 路徑
- `router/index.ts`：確認 `createRouter`、`createWebHistory`、navigation guards 語法
- `vite.config.ts`（或 `.env` 相關設定）：dotenv v17 API 是否有異動
- `package.json` scripts：`run-p` / `run-s` 指令確認 npm-run-all2 v9 相容性
