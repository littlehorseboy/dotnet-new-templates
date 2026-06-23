## Context

`vueappadmin.client` 的 4 個套件需要升級或替換：

| 套件 | 現況 | 目標 | 類型 |
|------|------|------|------|
| `@primevue/themes` | `^4.3.5` | 替換為 `@primeuix/themes` | deprecated 遷移 |
| `vue-router` | `^4.6.4` | `^5.x` | 大版本升級 |
| `dotenv` | `^16.5.0` | `^17.x` | 大版本升級 (devDep) |
| `npm-run-all2` | `^8.0.4` | `^9.x` | 大版本升級 (devDep) |

Font Awesome 維持 v6，因 `vue-fontawesome@4.x` 尚未發佈。

## Goals / Non-Goals

**Goals:**
- 替換 `@primevue/themes` → `@primeuix/themes`，更新 `main.ts` 中的 import 路徑
- 升級 `vue-router`、`dotenv`、`npm-run-all2` 至最新大版本
- 確認升級後 TypeScript 型別正確、開發伺服器與 build 可正常執行

**Non-Goals:**
- Font Awesome 6 → 7 升級（等待 `vue-fontawesome@4.x`）
- 其他套件（`primevue`、`pinia`、`axios` 等）版本異動
- 新增功能或架構調整

## Decisions

### 決策一：`@primeuix/themes` 直接替換

`@primevue/themes` 已官方宣告 deprecated，`@primeuix/themes` 為正式繼任套件，API 相容。

現況（`main.ts`）：
```ts
import Aura from '@primevue/themes/aura'
```
升級後：
```ts
import Aura from '@primeuix/themes/aura'
```

只有一個 import 需要改，風險極低。

### 決策二：vue-router v5 採用後確認 API

`router/index.ts` 的現有用法僅涵蓋核心 API：
- `createRouter` / `createWebHistory`
- `router.beforeEach` 單一 guard
- `route.meta` 存取（`noAuthRequired`、`showInSidebar`）

這些 API 在 vue-router v5 預期維持相容。安裝後需執行 `vue-tsc --build` 確認無型別錯誤。

### 決策三：dotenv v17 以現有 API 使用

`vite.config.ts` 的使用方式為 `dotenv.config()`（無特殊選項），屬於最基本用法。  
dotenv v17 主要變動集中在新功能與 ESM 處理，`config()` function 的基本呼叫介面預期向下相容。  
升級後需確認 `dotenv.config()` 仍能正確載入 `.env`。

### 決策四：npm-run-all2 v9 只影響 CLI

`package.json` 的 scripts 使用 `run-p` 與 `run-s`，屬純 CLI 用法，不涉及程式碼 import。  
升級後執行 `pnpm run build` 確認 `run-p type-check "build-only"` 指令正常即可。

## Risks / Trade-offs

- **vue-router v5 TypeScript 型別** → 執行 `vue-tsc --build` 確認；如有型別錯誤依錯誤訊息修正
- **dotenv v17 ESM 行為** → `vite.config.ts` 使用 `import dotenv from 'dotenv'`（ESM），若 v17 有 CJS/ESM 分離調整，需確認 import 方式是否需改為 named import
- **`@primevue/themes` peer dependency** → 安裝 `@primeuix/themes` 後需確認 `primevue` 的 peer 要求無衝突
