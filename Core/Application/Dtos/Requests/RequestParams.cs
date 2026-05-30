namespace Core.Application.Dtos.Requests
{
    namespace Shared
    {
        public class RequestParams
        {
            public int PageSize { get; set; } = 20;
            public int PageNumber { get; set; } = 1;
            public SortParam Sort { get; set; } = new SortParam { SortOrder = "ASC", SortValue = "CreatedAt" };
            public string SearchTerm { get; set; }
            public List<string> Filters { get; set; }

            public RequestParams()
            {
                Filters = new List<string>();
            }
        }

        public class SortParam
        {
            public string SortValue { get; set; }
            public string SortOrder { get; set; }
        }
    }
}
