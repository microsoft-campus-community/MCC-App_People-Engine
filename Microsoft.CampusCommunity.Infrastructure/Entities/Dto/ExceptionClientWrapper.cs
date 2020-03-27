using System;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto
{
    /// <summary>
    /// Class that is returned to application in case of an exception
    /// </summary>
    public class ExceptionClientWrapper
    {
        public int ErrorCode { get; set; }
        public string ExceptionMessage { get; set; }
        public Guid ExceptionAppInsightsId { get; set; }
        public string ExceptionTrace { get; set; }
    }
}