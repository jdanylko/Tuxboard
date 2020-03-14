using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tuxboard.Core.Domain.Entities;
using Tuxboard.Core.UI;

namespace $rootnamespace$.Widgets.Table
{
    [ViewComponent(Name="table")]
    public class TableViewComponent : ViewComponent
    {
        // EXTEND: Hook - Can use any Context for your application.
        // private readonly IMyDbContext _context;

        //public TableViewComponent(IMyDbContext context)
        //{
        //    _context = context;
        //}

        public TableViewComponent() { }

        public IViewComponentResult Invoke(WidgetPlacement placement)
        {
            var model = new TableModel
            {
                Placement = placement,
                Products = new List<Product>
                {
                    new Product {Id = 1, Title = "Product1", Price = new decimal(15.00)},
                    new Product {Id = 2, Title = "Product2", Price = new decimal(45.00)},
                    new Product {Id = 3, Title = "Product3", Price = new decimal(120.00)}
                }
            };


            return this.WidgetView(model);
        }
    }
}
