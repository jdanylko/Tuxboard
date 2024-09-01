using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities;

/// <summary>
/// <see cref="LayoutRow"/> contains a definition of a structured container for widgets.
///
/// Each layout row is defined by a single <see cref="LayoutType"/> which contains a column definition.
/// Column definitions are stored in the layout type table and can have an unlimited number of layout types.
///
/// A layout row also contains a list of one or more <see cref="WidgetPlacement"/>.
///
/// In a <see cref="Layout"/>, the list of LayoutRows are stored in the <see cref="Entities.Layout.LayoutRows"/>.
/// </summary>
public partial class LayoutRow
{
    /// <summary>
    /// Creates an HTML Layout based on a <see cref="LayoutType"/>.
    /// </summary>
    /// <returns>String</returns>
    public string GetHtmlLayout()
    {
        var rowTemplate = $"<div class=\"row-template border\" data-id=\"{LayoutRowId}\">";
        var sb = new StringBuilder(rowTemplate);
        var index = 1;
        foreach (var column in GetColumnLayout())
        {
            sb.AppendFormat("<div class=\"column {0}\" data-column=\"{1}\"></div>", column.ColumnClass, index);
            index++;
        }
        sb.Append("<div class=\"clearfix\"></div>");
        sb.Append("</div>");

        return sb.ToString();
    }

    /// <summary>
    /// Creates a list of columns based on a layout type defined in the table.
    /// </summary>
    /// <returns><see cref="List{Column}"/></returns>
    public List<Column> GetColumnLayout() =>
        this.LayoutType.Layout.Split('/')
            .Select((item, index) => new Column
            {
                Index = index,
                ColumnClass = item
            })
            .ToList();

    /// <summary>
    /// Return a <see cref="LayoutRowDto"/> from the existing <see cref="LayoutRow"/>.
    /// </summary>
    /// <returns><see cref="LayoutRowDto"/></returns>
    public LayoutRowDto ToDto() =>
        new()
        {
            LayoutRowId = this.LayoutRowId,
            RowIndex = this.RowIndex,
            Columns = this.GetColumnLayout(),
            HtmlLayout = this.GetHtmlLayout()
        };

    /// <summary>
    /// Returns whether this row contains any widgets
    /// </summary>
    /// <returns>true if widgets exist on this row, false if not</returns>
    public bool RowContainsWidgets() => WidgetPlacements.Any();

    /// <summary>
    /// Create a <see cref="WidgetPlacement"/> instance from a <see cref="Widget"/> (template)
    /// By default, any new widget will be placed at the bottom of the first column of the first layout row.
    /// </summary>
    /// <param name="widget"><see cref="Widget"/></param>
    /// <returns><see cref="WidgetPlacement"/></returns>
    public WidgetPlacement CreateFromWidget(Widget widget) =>
        new()
        {
            WidgetPlacementId = Guid.NewGuid(),
            LayoutRowId = LayoutRowId,
            WidgetId = widget.WidgetId,
            ColumnIndex = 0,
            WidgetIndex = WidgetPlacements != null && WidgetPlacements.Any()
                ? WidgetPlacements.Count + 1
                : 0,
            Collapsed = false,
            UseSettings = true,
            UseTemplate = true
        };

    /// <summary>
    /// Message specific for a <see cref="LayoutRow"/>.
    /// </summary>
    [NotMapped]
    public TuxViewMessage Message { get; set; }
}