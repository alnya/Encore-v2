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
}
