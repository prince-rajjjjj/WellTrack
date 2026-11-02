namespace WellTrack.Services.Reporting.Infrastructure.HttpClients;

/// <summary>Mirrors ApiResponse&lt;T&gt; from SharedKernel — for HTTP deserialization only.</summary>
public class ServiceResult<T>
{
    public bool Succeeded { get; set; }
    public T? Data { get; set; }
}

/// <summary>Mirrors PagedResult&lt;T&gt; from SharedKernel.</summary>
public class PagedData<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

// ── Wellness service mirrors ──────────────────────────────────────────────
public class DProgramDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    /// <summary>StatusType enum integer: Active=1, Inactive=2, Pending=3, Completed=4, Cancelled=5, Archived=6</summary>
    public int Status { get; set; }
}

// ── Engagement service mirrors ────────────────────────────────────────────
public class DEngagementDto
{
    public Guid Id { get; set; }
    public Guid EmployeeID { get; set; }
    public Guid ProgramID { get; set; }
    public decimal TotalScore { get; set; }
    public DateTime LastCalculatedAt { get; set; }
}

// ── Compliance service mirrors ────────────────────────────────────────────
public class DAuditDto
{
    public Guid Id { get; set; }
    /// <summary>"Draft" | "Submitted" | "Approved" | "Rejected" | "Closed"</summary>
    public string Status { get; set; } = string.Empty;
}

public class DComplianceRecordDto
{
    public Guid Id { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

// ── Participation service mirrors ─────────────────────────────────────────
public class DParticipationDto
{
    public Guid Id { get; set; }
    public Guid ProgramID { get; set; }
    public Guid EmployeeID { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

// ── Identity service mirrors ──────────────────────────────────────────────
public class DUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
