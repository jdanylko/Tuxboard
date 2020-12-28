using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Tuxboard.Core.UI
{
    public class ViewRenderHelper : IViewRenderHelper
    {
        private readonly IServiceProvider _serviceProvider;

        public ViewRenderHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string RenderToString(string viewName, object model, string viewPath)
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());

            var engine = _serviceProvider.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
            if (engine == null)
            {
                throw new Exception("Can't find IRazorViewEngine");
            }

            var tempDataProvider = _serviceProvider.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

            var viewEngineResult = engine.FindView(actionContext, viewPath, false);

            if (!viewEngineResult.Success)
            {
                throw new InvalidOperationException($"Couldn't find view '{viewName}'");
            }

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };


            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(actionContext, viewEngineResult.View,
                    viewDictionary, new TempDataDictionary(actionContext.HttpContext, tempDataProvider),
                    output, new HtmlHelperOptions());

                viewEngineResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();

                return output.ToString();
            }
        }
    }
}