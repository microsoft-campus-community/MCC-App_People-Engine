using System;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto {
	public class Event {
		public Guid Id {get;set;}
		public string Name {get;set;}
		public Guid CampusId {get;set;}
	}
}