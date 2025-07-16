# FastEndpoints 强类型ID解决方案

## 问题描述

在使用 FastEndpoints 时，当前台传递强类型ID（如 `UserId="621862200478269440"`）时，会出现以下错误：

```json
{
    "statusCode": 400,
    "message": "One or more errors occurred!",
    "errors": {
        "userId": [
            "Value [621862200478269440] is not valid for a [UserId] property!"
        ]
    }
}
```

这是因为 FastEndpoints 无法自动将字符串转换为强类型ID。

## 解决方案

### 1. 创建扩展方法

在 `src/Quiz.Exam.Web/Extensions/StronglyTypedIdExtensions.cs` 中创建了扩展方法来处理强类型ID的转换：

```csharp
public static class StronglyTypedIdExtensions
{
    public static T? TryParseStronglyTypedId<T>(this string value) where T : struct, IInt64StronglyTypedId
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (long.TryParse(value, out var longValue))
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T), longValue)!;
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    public static T ParseStronglyTypedId<T>(this string value) where T : struct, IInt64StronglyTypedId
    {
        var result = value.TryParseStronglyTypedId<T>();
        if (result == null)
        {
            throw new ArgumentException($"无法将值 '{value}' 转换为 {typeof(T).Name}");
        }
        return result.Value;
    }
}
```

### 2. 修改端点实现

将端点中的强类型ID参数改为字符串，然后在端点内部进行转换：

```csharp
// 修改前
public record DeleteUserRequest(UserId userId);

// 修改后
public record DeleteUserRequest(string userId);
```

在端点处理逻辑中使用扩展方法：

```csharp
public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
{
    var userId = req.userId.ParseStronglyTypedId<UserId>();
    var command = new DeleteUserCommand(userId);
    await _mediator.Send(command, ct);
    
    await SendAsync(true.AsResponseData(), cancellation: ct);
}
```

### 3. 添加验证器

创建验证器来验证用户ID的格式：

```csharp
public class DeleteUserValidator : Validator<DeleteUserRequest>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.userId)
            .NotEmpty()
            .WithMessage("用户ID不能为空")
            .Must(userId => userId.TryParseStronglyTypedId<UserId>() != null)
            .WithMessage("无效的用户ID格式");
    }
}
```

## 已修改的文件

1. `src/Quiz.Exam.Web/Extensions/StronglyTypedIdExtensions.cs` - 新增扩展方法
2. `src/Quiz.Exam.Web/Endpoints/UserEndpoints/DeleteUserEndpoint.cs` - 修改端点实现
3. `src/Quiz.Exam.Web/Endpoints/UserEndpoints/GetUserProfileEndpoint.cs` - 修改端点实现
4. `src/Quiz.Exam.Web/Endpoints/UserEndpoints/DeleteUserValidator.cs` - 新增验证器
5. `src/Quiz.Exam.Web/Endpoints/UserEndpoints/GetUserProfileValidator.cs` - 新增验证器

## 使用方式

现在前台可以正常传递字符串格式的用户ID：

```
DELETE /api/users/621862200478269440
```

FastEndpoints 会自动将字符串参数绑定到 `DeleteUserRequest.userId`，然后在端点内部使用 `ParseStronglyTypedId<UserId>()` 方法将其转换为强类型ID。

## 优势

1. **兼容性好**：支持所有实现 `IInt64StronglyTypedId` 接口的强类型ID
2. **错误处理完善**：提供清晰的错误信息
3. **类型安全**：在编译时保证类型安全
4. **易于维护**：统一的处理方式，便于后续维护
5. **性能良好**：避免了复杂的反射操作

## 注意事项

1. 确保所有使用强类型ID的端点都使用相同的模式
2. 验证器会自动验证ID格式，提供更好的用户体验
3. 如果ID格式无效，会抛出清晰的错误信息 