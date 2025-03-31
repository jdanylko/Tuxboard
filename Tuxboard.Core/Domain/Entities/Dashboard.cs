namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public partial class Dashboard<T> : DashboardBase where T : struct
{
    /// <summary>
    /// 
    /// </summary>
    public T? UserId { get; set; }
}
