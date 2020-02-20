using ALE.ETLBox.DataFlow;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETLBoxIssue
{
	public class People : MergeableRow
	{
		[CompareColumn]
		public string FirstName { get; set; } 

		[CompareColumn]
		public string LastName { get; set; } 

		[IdColumn]
		public string ExternalId { get; set; } 
	}
}
