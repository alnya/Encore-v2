namespace Encore.Domain.Entities
{
    public enum RequestStatus
    {
        Pending,
        InProgress,
        Failed,
        Complete
    }

    public enum AuditType
    {
        Login,
        ForgottenPassword,
        Add,
        Edit,
        Delete,
        Logout,
        TaskManager
    }

    public enum UserRole
    {
        Admin = 0,
        Standard
    }
}
