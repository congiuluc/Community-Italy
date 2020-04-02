namespace CommunityItaly.Server.Utilities
{
    public class UploadTasks
    {
        public const string ResizeImagePeople = nameof(ResizeImagePeople);
        public const string ResizeImageCommunity = nameof(ResizeImageCommunity);
        public const string ResizeImageEvent = nameof(ResizeImageEvent);
    }

    public class ConfirmationTask
    {
        public const string SendMailEvent = nameof(SendMailEvent);
        public const string CreateEvent = nameof(CreateEvent);
        public const string ApproveEvent = nameof(ApproveEvent);
        public const string ConfirmEvent_Http = nameof(ConfirmEvent_Http);
        public const string ConfirmEventHuman = nameof(ConfirmEventHuman);
        public const string ConfirmOrchestrator = nameof(ConfirmOrchestrator);
        public const string ApproveCancelEventOnCosmos = nameof(ApproveCancelEventOnCosmos);


        public const string ConfirmCommunity = nameof(ConfirmCommunity);
        public const string ConfirmPerson = nameof(ConfirmPerson);
    }
}
