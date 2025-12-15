using System.Collections.ObjectModel;

namespace SprintManagementDashboardSample.Models;

/// <summary>
/// Represents a scrum sprint with overall metrics and detailed datasets consumed by the dashboard.
/// </summary>
public class Sprint
{
    /// <summary>
    /// Display name of the sprint (e.g., "Sprint 1").
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Total person-hours worked during the sprint.
    /// </summary>
    public int TotalWorkedHours { get; set; }

    /// <summary>
    /// Count of tasks marked as completed in the sprint.
    /// </summary>
    public int TasksCompleted { get; set; }

    /// <summary>
    /// Count of tasks assigned during the sprint.
    /// </summary>
    public int TasksAssigned { get; set; }

    /// <summary>
    /// Total completed story points for the sprint.
    /// </summary>
    public double StoryPointsCompleted { get; set; }

    /// <summary>
    /// Total planned story points for the sprint.
    /// </summary>
    public double StoryPointsPlanned { get; set; }

    /// <summary>
    /// Distribution of task counts by status (e.g., Closed, In Progress).
    /// </summary>
    public ObservableCollection<TaskStatusSlice> TaskStatus { get; set; } = new();

    /// <summary>
    /// Planned vs completed counts grouped by task type (Bug, Feature, etc.).
    /// </summary>
    public ObservableCollection<TaskTypeBreakdown> TaskTypes { get; set; } = new();

    /// <summary>
    /// Hierarchical-like flat list representing incomplete tasks by project and priority for treemap visuals.
    /// </summary>
    public ObservableCollection<IncompleteTaskNode> IncompleteTasks { get; set; } = new();
}

/// <summary>
/// Represents a task status category with its aggregated count.
/// </summary>
public class TaskStatusSlice
{
    /// <summary>
    /// Name of the status (e.g., Open, Closed, In Progress).
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Number of tasks in this status.
    /// </summary>
    public int Count { get; set; }
}

/// <summary>
/// Planned vs completed counts for a single task type.
/// </summary>
public class TaskTypeBreakdown
{
    /// <summary>
    /// Task type name (e.g., Bug, Feature).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Number of tasks planned of this type.
    /// </summary>
    public int Planned { get; set; }

    /// <summary>
    /// Number of tasks completed of this type.
    /// </summary>
    public int Completed { get; set; }
}

/// <summary>
/// Represents scope change metrics per sprint.
/// </summary>
public class ScopeChange
{
    /// <summary>
    /// The sprint the scope change applies to.
    /// </summary>
    public string SprintName { get; set; } = string.Empty;

    /// <summary>
    /// Planned story points at sprint start.
    /// </summary>
    public int Planned { get; set; }

    /// <summary>
    /// Story points added during the sprint.
    /// </summary>
    public int Added { get; set; }

    /// <summary>
    /// Story points removed during the sprint.
    /// </summary>
    public int Removed { get; set; }
}

/// <summary>
/// Node describing incomplete task counts by project and priority for treemap-like visualization.
/// </summary>
public class IncompleteTaskNode
{
    /// <summary>
    /// Project name the tasks belong to.
    /// </summary>
    public string Project { get; set; } = string.Empty;

    /// <summary>
    /// Priority bucket (High, Medium, Low).
    /// </summary>
    public string Priority { get; set; } = string.Empty;

    /// <summary>
    /// Number of incomplete tasks for the given project and priority.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Convenience label combining project and priority for UI display.
    /// </summary>
    public string Label => $"{Project}\n{Priority}";
}