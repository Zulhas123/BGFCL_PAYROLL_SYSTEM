namespace BgfclApp.ViewModels
{
    public class ResponseViewModel
    {
        public int StatusCode { get; set; }
        public string ResponseMessage { get; set; }
        public Dictionary<string, string> Errors { get; set; }

        public object Data { get; set; }
        public List<Dictionary<string, string>> ErrorsList { get; set; } = new();
    }
}
