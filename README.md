# dotnet-new-templates

自訂 `dotnet new` 專案範本集合。

---

## 安裝方式

Clone 此儲存庫後，進入對應範本目錄安裝：

```bash
git clone <this-repo>
cd dotnet-new-templates
dotnet new install .\<範本目錄>
```

安裝完成後，使用 `dotnet new <簡短名稱> -n <專案名稱>` 建立新專案。

---

## 範本清單

| 範本名稱 | 簡短名稱 | 語言 | 標記 |
|---------|---------|------|------|
| ASP.NET Core + Vue 3 | `vue-app-demo` | [C#] | Web/SPA/Vue/ASP.NET Core |

---

## 可用範本

### VueApp1 — Vue 3 + ASP.NET Core Web API

以 Visual Studio 內建的 **Vue 和 ASP.NET Core** 專案類型為基礎，擴充額外工具鏈的全端專案範本。

![新增專案對話框](assets/VueApp1/2026-06-17_15-56-02.png)
![專案設定](assets/VueApp1/2026-06-17_13-28-50.png)
![方案結構](assets/VueApp1/2026-06-17_13-29-00.png)

**前端**

- Vue 3（Composition API）
- Vite 8，支援 HMR 熱更新
- TypeScript
- ESLint + oxlint

**後端**

- ASP.NET Core Web API（.NET 8）
- Swagger / OpenAPI

**開發體驗**

- 一個 `dotnet run` 同時啟動前後端
- Vite 開發伺服器透過 SpaProxy 將 API 請求代理至 ASP.NET Core
- 透過 `dotnet dev-certs` 自動設定 HTTPS

**前置需求**

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) >= 20.19.0

**安裝**

```bash
dotnet new install .\VueApp1
```

**建立新專案**

```bash
dotnet new vue-app-demo -n MyApp
cd MyApp
dotnet run
```
