using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SprintManagementDashboard
{
    /// <summary>
    /// ViewModel that exposes sprint KPIs and chart datasets to the dashboard UI.
    /// Implements INotifyPropertyChanged for binding updates and provides helpers to seed demo data and switch sprints.
    /// </summary>
    public partial class DashboardViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="n">Name of the property that changed. Automatically supplied.</param>
        void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));

        /// <summary>
        /// Available sprint names shown in the sprint selector.
        /// </summary>
        public ObservableCollection<string> Sprints { get; } = new(["All", "Sprint 1", "Sprint 2", "Sprint 3", "Sprint 4", "Sprint 5"]);

        private string _selectedSprint = "All";

        /// <summary>
        /// Currently selected sprint. Changing this reloads all dashboard metrics and series.
        /// </summary>
        public string SelectedSprint
        {
            get => _selectedSprint;
            set
            {
                if (_selectedSprint == value) return;
                _selectedSprint = value;
                UpdateForSprint();
                Raise();
            }
        }

        private int _totalWorkedHours;

        /// <summary>
        /// Total person-hours worked in the selected sprint.
        /// </summary>
        public int TotalWorkedHours { get => _totalWorkedHours; set { _totalWorkedHours = value; Raise(); } }

        private int _tasksCompleted;

        /// <summary>
        /// Number of tasks completed in the selected sprint.
        /// </summary>
        public int TasksCompleted { get => _tasksCompleted; set { _tasksCompleted = value; Raise(); } }

        private int _tasksAssigned;

        /// <summary>
        /// Number of tasks assigned in the selected sprint.
        /// </summary>
        public int TasksAssigned { get => _tasksAssigned; set { _tasksAssigned = value; Raise(); } }

        private double _storyCompleted;

        /// <summary>
        /// Completed story points for the selected sprint.
        /// </summary>
        public double StoryPointsCompleted { get => _storyCompleted; set { _storyCompleted = value; Raise(); } }

        private double _storyPlanned;

        /// <summary>
        /// Planned story points for the selected sprint.
        /// </summary>
        public double StoryPointsPlanned { get => _storyPlanned; set { _storyPlanned = value; Raise(); } }

        /// <summary>
        /// Palette used by charts and visual elements in the dashboard.
        /// </summary>
        public List<Brush> CustomBrushes { get; set; }

        /// <summary>
        /// Distribution of task counts by status for the selected sprint.
        /// </summary>
        public ObservableCollection<TaskStatusSlice> TaskStatus { get; } = new();

        /// <summary>
        /// Planned versus completed counts grouped by task type for the selected sprint.
        /// </summary>
        public ObservableCollection<TaskTypeBreakdown> TaskTypes { get; } = new();

        /// <summary>
        /// Scope change metrics across sprints.
        /// </summary>
        public ObservableCollection<ScopeChange> ScopeChanges { get; } = new();

        /// <summary>
        /// Incomplete task nodes by project and priority for treemap visuals.
        /// </summary>
        public ObservableCollection<IncompleteTaskNode> IncompleteTasks { get; } = new();

        /// <summary>
        /// In-memory dataset keyed by sprint name.
        /// </summary>
        private readonly Dictionary<string, Sprint> _data = new();

        /// <summary>
        /// Initializes the ViewModel, seeds demo data, loads scope changes, sets default sprint,
        /// and prepares a default brush palette.
        /// </summary>
        public DashboardViewModel()
        {
            Seed();
            LoadScopeChanges();
            UpdateForSprint();
            CustomBrushes = new List<Brush>
            {
                new SolidColorBrush(Color.FromRgb(60, 173, 104)),
                new SolidColorBrush(Color.FromRgb(117, 87, 73)),
                new SolidColorBrush(Color.FromRgb(245, 103, 0)),
                new SolidColorBrush(Color.FromRgb(96, 4, 168)),
                new SolidColorBrush(Color.FromRgb(3, 113, 234)),
                new SolidColorBrush(Color.FromRgb(12, 32, 152))
            };
        }

        /// <summary>
        /// Calculates total person-hours based on working days minus holidays.
        /// </summary>
        /// <param name="workingDays">Number of working days in the sprint.</param>
        /// <param name="holidays">Number of holidays within the sprint period.</param>
        /// <returns>Total hours calculated as (workingDays - holidays) * 8, never below zero.</returns>
        private static int CalculateWorkedHours(int workingDays, int holidays = 0) => Math.Max(0, (workingDays - holidays)) * 8;

        /// <summary>
        /// Clamps task counts to a reasonable demo range.
        /// </summary>
        /// <param name="value">Input task count.</param>
        /// <returns>Value clamped to the inclusive range [20, 25].</returns>
        private static int ClampTasks(int value) => Math.Min(25, Math.Max(10, value));

        /// <summary>
        /// Clamps story points to a reasonable demo range.
        /// </summary>
        /// <param name="value">Input story points.</param>
        /// <returns>Value clamped to the inclusive range [75.0, 80.0].</returns>
        private static double ClampStoryPoints(double value) => Math.Min(80.0, Math.Max(65.0, value));

        /// <summary>
        /// Seeds the in-memory sprint dataset with deterministic sample values used by the dashboard.
        /// </summary>
        void Seed()
        {
            Sprint Build(string name, int hours, int completed, int assigned, double spCompleted, double spPlanned,
                (int closed, int inProg, int open, int onHold, int review, int validated) status,
                (int bugP, int bugC, int storyP, int storyC, int blogP, int blogC, int kbP, int kbC, int ugP, int ugC) types,
                (int aH, int aM, int aL, int bH, int bM, int bL, int cH, int cM, int cL, int dH, int dM, int dL) treemap)
            {
                var s = new Sprint
                {
                    Name = name,
                    TotalWorkedHours = hours,
                    TasksCompleted = completed,
                    TasksAssigned = assigned,
                    StoryPointsCompleted = spCompleted,
                    StoryPointsPlanned = spPlanned,
                };

                s.TaskStatus = new ObservableCollection<TaskStatusSlice>
                {
                    new() { Status = "Closed", Count = status.closed },
                    new() { Status = "Validated", Count = status.validated },
                    new() { Status = "Review", Count = status.review },
                    new() { Status = "Open", Count = status.open },
                    new() { Status = "On Hold", Count = status.onHold },
                    new() { Status = "In Progress", Count = status.inProg },
                };

                s.TaskTypes = new ObservableCollection<TaskTypeBreakdown>
                {
                    new() { Type = "Bug", Planned = types.bugP, Completed = types.bugC },
                    new() { Type = "Feature", Planned = types.storyP, Completed = types.storyC },
                    new() { Type = "Blog", Planned = types.blogP, Completed = types.blogC },
                    new() { Type = "KB", Planned = types.kbP, Completed = types.kbC },
                    new() { Type = "UG", Planned = types.ugP, Completed = types.ugC }
                };

                s.IncompleteTasks = new ObservableCollection<IncompleteTaskNode>
                {
                    new() { Project = "Project A", Priority = "High", Count = treemap.aH },
                    new() { Project = "Project A", Priority = "Medium", Count = treemap.aM },
                    new() { Project = "Project A", Priority = "Low", Count = treemap.aL },
                    new() { Project = "Project B", Priority = "High", Count = treemap.bH },
                    new() { Project = "Project B", Priority = "Medium", Count = treemap.bM },
                    new() { Project = "Project B", Priority = "Low", Count = treemap.bL },
                    new() { Project = "Project C", Priority = "High", Count = treemap.cH },
                    new() { Project = "Project C", Priority = "Medium", Count = treemap.cM },
                    new() { Project = "Project C", Priority = "Low", Count = treemap.cL },
                    new() { Project = "Project D", Priority = "High", Count = treemap.dH },
                    new() { Project = "Project D", Priority = "Medium", Count = treemap.dM },
                    new() { Project = "Project D", Priority = "Low", Count = treemap.dL },
                };

                return s;
            }

            _data["Sprint 1"] = Build(
                "Sprint 1",
                CalculateWorkedHours(11, 0),
                ClampTasks(15),
                ClampTasks(25),
                ClampStoryPoints(66.0),
                ClampStoryPoints(70.0),
                (closed: 15, inProg: 3, open: 1, onHold: 1, review: 1, validated: 1),
                (6, 4, 8, 4, 4, 2, 4, 3, 3, 2),
                (aH: 3, aM: 2, aL: 1, bH: 2, bM: 2, bL: 1, cH: 2, cM: 1, cL: 1, dH: 1, dM: 1, dL: 1)
            );

            _data["Sprint 2"] = Build(
                "Sprint 2",
                CalculateWorkedHours(11, 1),
                ClampTasks(14),
                ClampTasks(22),
                ClampStoryPoints(65.0),
                ClampStoryPoints(68.0),
                (closed: 14, inProg: 3, open: 1, onHold: 1, review: 1, validated: 1),
                (7, 3, 7, 4, 3, 3, 3, 2, 2, 2),
                (aH: 2, aM: 2, aL: 1, bH: 2, bM: 1, bL: 1, cH: 2, cM: 1, cL: 1, dH: 1, dM: 1, dL: 1)
            );

            _data["Sprint 3"] = Build(
                "Sprint 3",
                CalculateWorkedHours(11, 0),
                ClampTasks(16),
                ClampTasks(25),
                ClampStoryPoints(68.0),
                ClampStoryPoints(70.0),
                (closed: 16, inProg: 4, open: 1, onHold: 1, review: 2, validated: 0),
                (9, 4, 9, 5, 3, 2, 2, 3, 2, 2),
                (aH: 2, aM: 3, aL: 1, bH: 2, bM: 2, bL: 1, cH: 2, cM: 2, cL: 1, dH: 1, dM: 1, dL: 1)
            );

            _data["Sprint 4"] = Build(
                "Sprint 4",
                CalculateWorkedHours(11, 1),
                ClampTasks(13),
                ClampTasks(21),
                ClampStoryPoints(65.0),
                ClampStoryPoints(67.0),
                (closed: 13, inProg: 3, open: 1, onHold: 1, review: 1, validated: 1),
                (7, 2, 7, 4, 3, 2, 2, 2, 2, 3),
                (aH: 2, aM: 1, aL: 1, bH: 2, bM: 1, bL: 1, cH: 2, cM: 1, cL: 1, dH: 1, dM: 1, dL: 1)
            );

            _data["Sprint 5"] = Build(
                "Sprint 5",
                CalculateWorkedHours(11, 0),
                ClampTasks(15),
                ClampTasks(24),
                ClampStoryPoints(67.0),
                ClampStoryPoints(70.0),
                (closed: 15, inProg: 4, open: 1, onHold: 1, review: 1, validated: 1),
                (6, 3, 7, 5, 4, 2, 4, 3, 3, 2),
                (aH: 2, aM: 2, aL: 1, bH: 2, bM: 2, bL: 1, cH: 2, cM: 2, cL: 1, dH: 1, dM: 1, dL: 1)
            );
        }

        /// <summary>
        /// Loads scope change series data for all sprints.
        /// </summary>
        void LoadScopeChanges()
        {
            ScopeChanges.Clear();
            ScopeChanges.Add(new ScopeChange { SprintName = "Sprint 1", Planned = 66, Added = 30, Removed = 26 });
            ScopeChanges.Add(new ScopeChange { SprintName = "Sprint 2", Planned = 65, Added = 27, Removed = 24 });
            ScopeChanges.Add(new ScopeChange { SprintName = "Sprint 3", Planned = 68, Added = 25, Removed = 23 });
            ScopeChanges.Add(new ScopeChange { SprintName = "Sprint 4", Planned = 65, Added = 28, Removed = 26 });
            ScopeChanges.Add(new ScopeChange { SprintName = "Sprint 5", Planned = 67, Added = 31, Removed = 28 });
        }

        /// <summary>
        /// Updates all bindable properties and collections to reflect the currently selected sprint.
        /// </summary>
        void UpdateForSprint()
        {
            IEnumerable<Sprint> sprints = SelectedSprint == "All" ? _data.Values.AsEnumerable() : new[] { _data[SelectedSprint] };

            TotalWorkedHours = sprints.Sum(s => s.TotalWorkedHours);
            TasksCompleted = sprints.Sum(s => s.TasksCompleted);
            TasksAssigned = sprints.Sum(s => s.TasksAssigned);
            StoryPointsCompleted = sprints.Sum(s => s.StoryPointsCompleted);
            StoryPointsPlanned = sprints.Sum(s => s.StoryPointsPlanned);

            TaskStatus.Clear();
            foreach (var g in sprints
                .SelectMany(s => s.TaskStatus)
                .GroupBy(t => t.Status))
                TaskStatus.Add(new TaskStatusSlice { Status = g.Key, Count = g.Sum(x => x.Count) });

            TaskTypes.Clear();
            foreach (var g in sprints
                .SelectMany(s => s.TaskTypes))
                TaskTypes.Add(new TaskTypeBreakdown { Type = g.Type, Planned = g.Planned, Completed = g.Completed });

            if (SelectedSprint == "All")
            {
                var agg = TaskTypes
                    .GroupBy(t => t.Type)
                    .Select(g => new TaskTypeBreakdown { Type = g.Key, Planned = g.Sum(x => x.Planned), Completed = g.Sum(x => x.Completed) })
                    .ToList();
                TaskTypes.Clear();
                foreach (var it in agg) TaskTypes.Add(it);
            }

            IncompleteTasks.Clear();

            if (SelectedSprint == "All")
            {
                var topAll = sprints
                    .SelectMany(s => s.IncompleteTasks)
                    .GroupBy(n => new { n.Project, n.Priority })
                    .Select(g => new IncompleteTaskNode
                    {
                        Project = g.Key.Project,
                        Priority = g.Key.Priority,
                        Count = g.Sum(x => x.Count)
                    })
                    .OrderByDescending(n => n.Count)
                    .Take(14);

                foreach (var n in topAll)
                    IncompleteTasks.Add(n);
            }
            else
            {
                var single = sprints
                    .SelectMany(s => s.IncompleteTasks)
                    .OrderByDescending(n => n.Count)
                    .Take(12);

                foreach (var n in single)
                    IncompleteTasks.Add(n);
            }
        }
    }
}
