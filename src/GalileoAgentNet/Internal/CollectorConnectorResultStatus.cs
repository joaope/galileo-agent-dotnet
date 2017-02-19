namespace GalileoAgentNet.Internal
{
    internal enum CollectorConnectorResultStatus
    {
        Success,
        PartialSuccess,
        BadRequest,
        EntriesPayloadTooLarge,
        CollectorConnectionError,
        UnknownResponseStatus,
        UnknownServerError
    }
}