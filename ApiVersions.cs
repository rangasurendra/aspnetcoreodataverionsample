using Microsoft.AspNetCore.Mvc;

namespace WebODataDataService
{
    public static class ApiVersions
    {
        internal static readonly ApiVersion V1 = new ApiVersion(1, 0);
        internal static readonly ApiVersion V2 = new ApiVersion(2, 0);
    }
}