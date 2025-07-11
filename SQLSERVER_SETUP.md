# SQL Server 设置指南

本项目已从 MySQL 迁移到 SQL Server。以下是设置和运行项目的步骤：

## 1. 安装 SQL Server

### 选项 1: SQL Server Express (免费)
- 下载并安装 [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- 选择 "Express" 版本

### 选项 2: SQL Server Developer Edition (免费)
- 下载并安装 [SQL Server Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- 选择 "Developer" 版本

### 选项 3: Docker (推荐用于开发)
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Test@123" \
  -p 1433:1433 --name sqlserver --hostname sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

## 2. 连接字符串配置

在 `src/Quiz.Exam.Web/appsettings.Development.json` 中，连接字符串已配置为：

```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=QuizExam;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

### 如果使用 SQL Server 身份验证：
```json
{
  "ConnectionStrings": {
    "SqlServer": "Server=localhost;Database=QuizExam;User Id=sa;Password=Test@123;TrustServerCertificate=true;"
  }
}
```

## 3. 创建数据库

### 方法 1: 使用 Entity Framework 迁移
```bash
cd src/Quiz.Exam.Infrastructure
dotnet ef database update
```

### 方法 2: 手动创建
```sql
CREATE DATABASE QuizExam;
```

## 4. 运行项目

```bash
cd src/Quiz.Exam.Web
dotnet run
```

## 5. 测试

项目包含集成测试，使用 Testcontainers 自动启动 SQL Server 容器：

```bash
cd test/Quiz.Exam.Web.Tests
dotnet test
```

## 注意事项

1. **TrustServerCertificate=true**: 在开发环境中，这允许连接到没有有效SSL证书的SQL Server实例
2. **Trusted_Connection=true**: 使用Windows身份验证（推荐用于开发）
3. 生产环境中应使用适当的SSL证书和强密码

## 迁移历史

- 从 MySQL 8.0 迁移到 SQL Server 2022
- 更新了所有相关的 NuGet 包
- 修改了 Entity Framework 配置
- 更新了 CAP (分布式事务) 配置
- 更新了测试容器配置 