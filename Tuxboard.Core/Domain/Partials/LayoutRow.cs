using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Tuxboard.Core.Domain.Dto;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Domain.Entities;

public partial class LayoutRow
{
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

    public List<Column> GetColumnLayout()
    {
        var columns = new List<Column>();

        var widths = this.LayoutType.Layout.Split('/');
        var index = 0;
        foreach (var width in widths)
        {
            columns.Add(new Column
            {
                Index = index,
                ColumnClass = width
            });
            index++;
        }

        return columns;
    }

    public LayoutRowDto ToDto() =>
        new()
        {
            LayoutRowId = this.LayoutRowId,
            RowIndex = this.RowIndex,
            Columns = this.GetColumnLayout(),
            HtmlLayout = this.GetHtmlLayout()
        };

    public bool RowContainsWidgets()
    {
        return WidgetPlacements.Any<WidgetPlacement>();
    }

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

    [NotMapped]
    public TuxViewMessage Message { get; set; }
}