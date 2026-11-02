using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WellTrack.Services.Compliance.Domain.Entities;

namespace WellTrack.Services.Compliance.Application.DTOs
{
	public class PolicyDto
	{
		public Guid Id { get; set; }
		public string PolicyName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string Severity { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty;
		public DateTime CreatedDate { get; set; }
		public DateTime CreatedAt { get; set; }
	}

	public class CreatePolicyDto
	{
		public string PolicyName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string Severity { get; set; } = "Medium";
	}

	public class UpdatePolicyDto
	{
		public string PolicyName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string Severity { get; set; } = "Medium";
	}

	public class SetPolicyStatusDto
	{
		public Guid Id { get; set; }
		public PolicyStatus Status { get; set; }
	}

}
