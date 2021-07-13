namespace StaticDashboard.Pages.Shared.Components.Chart.ChartJsModel
{
    public class Dataset
    {
        public string label { get; set; }
        public int[] data { get; set; }
        public string[] backgroundColor { get; set; }
        public string[] borderColor { get; set; }
        public int borderWidth { get; set; }
        public string yAxisID { get; set; }
        public string xAxisID { get; set; }
}
}