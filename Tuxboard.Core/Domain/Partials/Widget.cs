using Tuxboard.Core.Domain.Dto;

namespace Tuxboard.Core.Domain.Entities
{

    public partial class Widget
    {
        public WidgetDto ToDto()
        {
            return new WidgetDto
            {
                Name = this.Name,
                Description = this.Description,
                GroupName = this.GroupName,
                ImageUrl = this.ImageUrl,
                Permission = this.Permission,
                Title = this.Title,
                WidgetId = this.WidgetId
            };
        }
    }
}
