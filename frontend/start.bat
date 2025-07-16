@echo off
echo 正在启动 Quiz Exam 前端项目...
echo.

echo 检查 Node.js 版本...
node --version
if %errorlevel% neq 0 (
    echo 错误: 未找到 Node.js，请先安装 Node.js
    pause
    exit /b 1
)

echo.
echo 安装依赖...
npm install
if %errorlevel% neq 0 (
    echo 错误: 依赖安装失败
    pause
    exit /b 1
)

echo.
echo 启动开发服务器...
npm run dev

pause 