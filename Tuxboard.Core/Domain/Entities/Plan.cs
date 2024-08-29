using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// <see cref="Plan"/> is an optional table containing an ID and a title. The plan table is meant to provide functionality similar to roles&lt;-&gt;widgets.
/// A WidgetPlan table can be added onto the DbContext to provide widgets/dashboards for specific subscription plans.
/// An example of plans in the table could include:
/// <list type="table">
///     <listheader>
///         <term>PlanID</term>
///         <description>Title</description>
///     </listheader>
///     <item>
///         <term>1</term>
///         <description>Bronze</description>
///     </item>
///     <item>
///         <term>2</term>
///         <description>Gold</description>
///     </item>
///     <item>
///         <term>3</term>
///         <description>Silver</description>
///     </item>
///     <item>
///         <term>4</term>
///         <description>Platinum</description>
///     </item>
///  </list>
/// Each record in the WidgetPlan table would relate to a widget id and plan id as to what a subscriber can use.
/// </summary>
/// <remarks>For an example, <see href="https://www.danylkoweb.com/Blog/creating-default-widgets-using-roles-UA">Creating Default Widgets Using Roles</see></remarks>
public partial class Plan
{
    /// <summary>
    /// AutoGen (int)
    /// </summary>
    public int PlanId { get; set; }
    
    /// <summary>
    /// Get or set the plan title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Get or set a collection of default dashboards related to a Plan
    /// </summary>
    public virtual ICollection<DashboardDefault> DashboardDefaults { get; set; } = new HashSet<DashboardDefault>();
    
    /// <summary>
    /// Get or set a collection of widgets related to a plan
    /// </summary>
    public virtual ICollection<Widget> Widgets { get; set; } = new HashSet<Widget>();
}