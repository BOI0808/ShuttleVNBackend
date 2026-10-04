namespace ShuttleVNBackend.Application.Common;

public record PageRequest
{
    public PageRequest(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public int PageNumber
    {
        get;
        private init => field = Math.Max(1, value);
    } = 1;

    public int PageSize
    {
        get;
        private init => field = Math.Max(1, value);
    } = 20;
}