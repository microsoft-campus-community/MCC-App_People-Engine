using System;

namespace Microsoft.CampusCommunity.Infrastructure.Entities.Dto {
	public class NewUser {
		public string FirstName {get;set;}
		public string LastName {get;set;}
		public string Email {get;set;}
		public Guid CampusId {get;set;}
	}
}