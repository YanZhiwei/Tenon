using Microsoft.EntityFrameworkCore;

namespace Tenon.EntityFrameworkCore.Extensions;

/// <summary>
/// DbContext 的事务扩展方法
/// </summary>
/// <remarks>
/// 使用示例：
/// <code>
/// // 不带成功判定条件
/// var result = await dbContext.ExecuteInTransactionAsync(
///     async () =>
///     {
///         dbContext.Users.Add(new User { Name = "Alice" });
///         await dbContext.SaveChangesAsync();
///         return true;
///     });
///
/// // 带成功判定条件
/// var isSuccess = await dbContext.ExecuteInTransactionAsync(
///     async () =>
///     {
///         var user = new User { Name = "Bob" };
///         dbContext.Users.Add(user);
///         await dbContext.SaveChangesAsync();
///         return user.Id;
///     },
///     result => result > 0);  // Id > 0 表示成功
/// </code>
/// </remarks>
public static class DbContextTransactionExtensions
{
    /// <summary>
    ///     在数据库执行策略中执行带有事务的操作，并可选择在操作结果不符合预期时回滚事务。
    /// </summary>
    /// <typeparam name="TResult">返回结果类型</typeparam>
    /// <param name="dbContext">数据库上下文</param>
    /// <param name="operation">要执行的操作</param>
    /// <param name="isSuccessful">判断操作是否成功的函数（可选）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作的结果</returns>
    public static async Task<TResult> ExecuteInTransactionAsync<TResult>(
        this DbContext dbContext,
        Func<Task<TResult>> operation,
        Func<TResult, bool>? isSuccessful = null,
        CancellationToken cancellationToken = default)
    {
        var executionStrategy = dbContext.Database.CreateExecutionStrategy();

        return await executionStrategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await operation();

                if (isSuccessful == null || isSuccessful(result))
                    await transaction.CommitAsync(cancellationToken);
                else
                    await transaction.RollbackAsync(cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception("Transaction failed.", ex);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        });
    }
}