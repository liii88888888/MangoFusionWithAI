# 🥭 MangoFusionWithAI — AI 智能餐厅点餐系统

> 基于 ASP.NET Core + Vue 3 的全栈餐厅管理系统，集成 **DeepSeek 大模型** 实现智能营销文案生成与自然语言菜品搜索。

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Vue](https://img.shields.io/badge/Vue-3.x-4FC08D?logo=vuedotjs)](https://vuejs.org/)
[![DeepSeek](https://img.shields.io/badge/AI-DeepSeek-4B32C3)](https://www.deepseek.com/)

---

## ✨ 核心功能

### 🤖 AI 智能驱动（自主开发）

| 功能 | 说明 |
|------|------|
| **营销文案一键生成** | 管理端调用 DeepSeek，根据菜品名称与价格自动生成 50-80 字营销描述，支持口感风格定制 |
| **自然语言搜索** | 用户端输入「想吃辣的便宜的菜」→ AI 提取关键词 → 数据库模糊匹配 → 返回精准结果 |

### 🍽️ 餐厅业务（自主重构）

| 模块 | 功能 |
|------|------|
| **菜品管理** | 增删改查、图片上传、分类筛选、评分展示 |
| **购物车** | 实时数量调整、价格计算 |
| **订单系统** | 下单 → 确认 → 备餐中 → 已完成 / 已取消，完整状态流转 |
| **用户系统** | JWT 认证、管理员 / 顾客双角色、注册登录 |

---

## 🏗️ 技术架构

```
浏览器 (Vue 3 + Vite)
        │
        ▼
ASP.NET Core Web API (.NET 9)
        │
   ┌────┴────┐
   ▼         ▼
SQL Server  DeepSeek API
(EF Core)   (AI 大模型)
```

| 层级 | 技术栈 |
|------|--------|
| **前端** | Vue 3 (Composition API)、Vite、Pinia、Vue Router、Axios、Bootstrap 5 |
| **后端** | ASP.NET Core 9 Web API、EF Core、JWT 认证、Identity |
| **AI** | DeepSeek Chat API（temperature 动态调节：创意 0.8 / 精确 0.1） |
| **数据库** | SQL Server（Code First + Seed Data） |

---

## 🚀 快速启动

### 环境要求

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server](https://www.microsoft.com/sql-server)（本地或远程）

### 1. 克隆项目

```bash
git clone https://github.com/liii88888888/MangoFusionWithAI.git
cd MangoFusionWithAI
```

### 2. 配置后端

```bash
# 复制配置模板（Windows 用 copy，Mac/Linux 用 cp）
cd MangoFusionWithAI_APi
cp appsettings.example.json appsettings.json
```

编辑 `appsettings.json`，填入你的配置：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=你的服务器;Database=MangoFusionWithAI;User ID=你的用户名;Password=你的密码;TrustServerCertificate=True;"
  },
  "ApiSettings": {
    "Secret": "你的JWT密钥（任意复杂字符串）"
  },
  "DeepSeek": {
    "ApiKey": "你的DeepSeek API Key",
    "BaseUrl": "https://api.deepseek.com/v1"
  }
}
```

### 3. 启动后端

```bash
dotnet run
# 后端运行在 https://localhost:5074
```

数据库和种子数据（9 道示例菜品）会自动创建。

### 4. 启动前端

```bash
cd VueMangoFusion
npm install
npm run dev
# 前端运行在 http://localhost:5173
```

### 5. 注册管理员

访问前端注册页面，角色选择 **Admin** 即可创建管理员账号。

---

## 📡 API 概览

| 方法 | 端点 | 权限 | 说明 |
|------|------|------|------|
| `POST` | `/api/Auth/register` | 公开 | 用户注册 |
| `POST` | `/api/Auth/login` | 公开 | 用户登录 |
| `GET` | `/api/MenuItem` | 公开 | 获取全部菜品 |
| `POST` | `/api/MenuItem` | Admin | 新增菜品 |
| `PUT` | `/api/MenuItem/{id}` | Admin | 更新菜品 |
| `DELETE` | `/api/MenuItem/{id}` | Admin | 删除菜品 |
| `POST` | `/api/OrderHeader` | 登录 | 创建订单 |
| `GET` | `/api/OrderHeader` | 登录 | 查询订单 |
| `PUT` | `/api/OrderHeader/{id}` | Admin | 更新订单状态 |
| `PUT` | `/api/OrderDetails/{id}` | 登录 | 菜品评分 |
| `POST` | `/api/Ai/search` | 公开 | 🤖 AI 自然语言搜索 |
| `POST` | `/api/Ai/generate-description` | Admin | 🤖 AI 生成营销文案 |
| `PATCH` | `/api/Ai/apply-description` | Admin | 🤖 应用 AI 描述到菜品 |

---

## 🙏 致谢

本项目前端 UI 与后端基础架构参考了 [@Bhrugen Patel](https://github.com/bhrugen/MangoFusion_API) 的开源项目，在此表示衷心感谢！

在此基础上，我独立完成了以下开发工作：

- ✅ 集成 DeepSeek 大模型，实现 AI 营销文案生成与自然语言搜索
- ✅ 订单状态流转优化
- ✅ 重构优化系统架构
- ✅ 全部前端页面中文本地化
- ✅ 项目安全配置（`.gitignore` 敏感信息保护、`appsettings.example.json` 模板）

---

## 📄 许可证

本项目仅用于学习与展示目的。
