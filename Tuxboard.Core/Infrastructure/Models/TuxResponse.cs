using System.Collections.Generic;
using Tuxboard.Core.Domain.Entities;

namespace Tuxboard.Core.Infrastructure.Models
{
    public class TuxResponse
    {
        public bool Success { get; set; }
        public TuxViewMessage Message { get; set; }
        public List<LayoutError> Errors { get; set; }
    }

}