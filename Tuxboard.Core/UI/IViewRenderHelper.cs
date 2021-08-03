namespace Tuxboard.Core.UI
{
    public interface IViewRenderHelper
    {
        string RenderToString(string viewName, object model, string viewPath);
    }
}