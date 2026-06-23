## 1. 替換 @primevue/themes → @primeuix/themes

- [x] 1.1 在 `vueappadmin.client` 目錄執行 `pnpm remove @primevue/themes && pnpm add @primeuix/themes`
- [x] 1.2 修改 `src/main.ts`：將 `import Aura from '@primevue/themes/aura'` 改為 `import Aura from '@primeuix/themes/aura'`
- [x] 1.3 確認 `package.json` 中 `@primevue/themes` 已移除、`@primeuix/themes` 已加入

## 2. 升級 vue-router 4 → 5

- [x] 2.1 在 `vueappadmin.client` 目錄執行 `pnpm add vue-router@5`
- [x] 2.2 執行 `pnpm run type-check`（即 `vue-tsc --build`），確認無 TypeScript 型別錯誤

## 3. 升級 devDependencies

- [x] 3.1 執行 `pnpm add -D dotenv@17`
- [x] 3.2 執行 `pnpm add -D npm-run-all2@9`

## 4. 驗收

- [x] 4.1 執行 `pnpm install` 確認無 peer dependency 衝突
- [x] 4.2 執行 `pnpm run build` 確認 `run-p type-check "build-only"` 正常完成
- [x] 4.3 執行 `pnpm run dev`，確認開發伺服器正常啟動（HTTPS + proxy）
