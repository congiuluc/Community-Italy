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
        public const string ApproveFromHttpEvent = nameof(ApproveFromHttpEvent);
        public const string ConfirmEvent_Http = nameof(ConfirmEvent_Http);
        public const string ConfirmEventHuman = nameof(ConfirmEventHuman);
        public const string ConfirmOrchestratorEvent = nameof(ConfirmOrchestratorEvent);
        public const string ApproveCancelEventOnCosmos = nameof(ApproveCancelEventOnCosmos);


        public const string SendMailCommunity = nameof(SendMailCommunity);
        public const string CreateCommunity = nameof(CreateCommunity);
        public const string ApproveFromHttpCommunity = nameof(ApproveFromHttpCommunity);
        public const string ConfirmCommunity_Http = nameof(ConfirmCommunity_Http);
        public const string ConfirmCommunityHuman = nameof(ConfirmCommunityHuman);
        public const string ConfirmOrchestratorCommunity = nameof(ConfirmOrchestratorCommunity);
        public const string ApproveCancelCommunityOnCosmos = nameof(ApproveCancelCommunityOnCosmos);


        public const string ConfirmCommunity = nameof(ConfirmCommunity);
        public const string ConfirmPerson = nameof(ConfirmPerson);
    }
}
