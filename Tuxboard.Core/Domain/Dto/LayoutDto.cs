﻿using System;
using System.Collections.Generic;

namespace Tuxboard.Core.Domain.Dto;

public class LayoutDto
{
    public Guid LayoutId { get; set; }
    public int LayoutIndex { get; set; }
    public List<LayoutRowDto> LayoutRows { get; set; }
}