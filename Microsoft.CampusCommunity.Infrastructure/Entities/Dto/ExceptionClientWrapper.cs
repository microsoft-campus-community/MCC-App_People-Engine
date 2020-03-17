
using System;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto {
	public class ExceptionClientWrapper {
		public int ErrorCode { get; set; }
		public string ExceptionMessage { get; set; }
		public Guid ExceptionAppInsightsId { get; set; }
		public string ExceptionTrace { get; set; }
	}
}