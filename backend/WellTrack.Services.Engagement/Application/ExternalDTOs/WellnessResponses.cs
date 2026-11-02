namespace WellTrack.Services.Engagement.Application.ExternalDTOs
{
	/// <summary>
	/// Minimal models that mirror the Wellness service (port 5002) API response shapes.
	/// Only the fields we actually use for engagement calculation are mapped.
	/// </summary>

	internal class ExternalApiResponse<T>
	{
		public bool Succeeded { get; set; }
		public T? Data { get; set; }
	}

	internal class ExternalPagedResult<T>
	{
		public List<T> Items { get; set; } = new();
	}

	internal class AttendanceResponse
	{
		public Guid EmployeeID { get; set; }
		public bool IsPresent { get; set; }
		public DateTime? JoinTime { get; set; }
		public DateTime? LeaveTime { get; set; }
	}

	/// <summary>
	/// Mirrors the Wellness service's ActivityDto. Wellness has no separate
	/// "Sessions" entity — every session IS an activity, so attendance and
	/// consistency scoring both go through the activities endpoint.
	/// </summary>
	internal class ActivityResponse
	{
		public Guid Id { get; set; }
		public Guid ProgramID { get; set; }
		/// <summary>
		/// StatusType enum serialised as integer by the Wellness service (no JsonStringEnumConverter).
		/// 1=Active, 2=Inactive, 3=Pending, 4=Completed, 5=Cancelled, 6=Archived
		/// </summary>
		public int Status { get; set; }
		public DateTime Date { get; set; }
	}
}
