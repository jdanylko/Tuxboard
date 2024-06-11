using System;
using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Domain.Dto;

public class LayoutRowDto
{
    public Guid LayoutRowId { get; set; }
    public int RowIndex { get; set; }
    public List<Column> Columns { get; set; }
    public string HtmlLayout { get; internal set; }
}