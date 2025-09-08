using ShrinkLink.Contracts.DataTransferObjects.AuthorizationService;
using ShrinkLink.Contracts.DataTransferObjects.LinkShortenerService;
using ShrinkLink.UrlShortener.Service.Models.Interfaces;

namespace ShrinkLink.UrlShortener.Service.Services
{
    public class VisitorService : IVisitorService
    {
        private readonly IVisitorRepository _visitorRepository;

        public VisitorService(IVisitorRepository visitorRepository)
        {
            _visitorRepository = visitorRepository;
        }

        public async Task<VisitsResponseTransferObject> FetchAllVisitsByLinkCodeAsync(Guid userId,
           string code, CancellationToken ct = default)
        {


            if (string.IsNullOrEmpty(code))
            {
                return new VisitsResponseTransferObject()
                {
                    Status = VisitorsByCodeResultStatus.InvalidCode,
                    Message = "The provided Code is invalid."
                };
            }
            try
            {
                var visitors = await _visitorRepository.ReadVisitsByUserIdAndCodeAsync(
                    userId, code, ct);

                if (visitors == null)
                {
                    return new VisitsResponseTransferObject
                    {
                        Status = VisitorsByCodeResultStatus.Failed,
                        Message = "Error: No data returned from repository.",
                    };
                }
                else if (!visitors.Any())
                {
                    return new VisitsResponseTransferObject
                    {
                        Status = VisitorsByCodeResultStatus.VisitorNotFound,
                        Message = "No visits found for the provided code.",
                        VisitorsAsData = new List<VisitAsData>()
                    };
                }

                // Map to VisitAsData
                var rawVisitDataList = visitors.Select(v => new VisitAsData
                {
                    PrimaryId = v.PrimaryId,
                    ProcessedUrlCode = v.ProcessedUrlCode,
                    IpAddress = v.IpAddress,
                    Country = v.Country,
                    City = v.City,
                    OperatingSystem = v.OperatingSystem,
                    Browser = v.Browser,
                    DeviceType = v.DeviceType,
                    UserAgent = v.UserAgent,
                    ClickedAt = v.ClickedAt
                }).ToList();

                return new VisitsResponseTransferObject
                {
                    Status = VisitorsByCodeResultStatus.Success,
                    Message = "Fetching visits was successful.",
                    VisitorsAsData = rawVisitDataList
                };
            }
            catch (ArgumentNullException)
            {
                return new VisitsResponseTransferObject
                {
                    Status = VisitorsByCodeResultStatus.Failed,
                    Message = "Invalid request parameters provided.",
                    VisitorsAsData = new List<VisitAsData>()
                };
            }
            catch (Exception)
            {
                return new VisitsResponseTransferObject
                {
                    Status = VisitorsByCodeResultStatus.Failed,
                    Message = "An unexpected error occurred while fetching visits.",
                    VisitorsAsData = new List<VisitAsData>()
                };
            }
        }

        public async Task<VisitsResponseTransferObject> FetchAllVisitsByUserIdAsync(
            Guid userId, CancellationToken ct = default)
        {
            try
            {
                var visitors = await _visitorRepository.ReadByUserIdAsync(userId, ct);

                if (visitors == null)
                {
                    return new VisitsResponseTransferObject
                    {
                        Status = VisitorsByCodeResultStatus.Failed,
                        Message = "Error: No data returned from repository.",
                    };
                } 
                if (!visitors.Any())
                {
                    return new VisitsResponseTransferObject
                    {
                        Status = VisitorsByCodeResultStatus.VisitorNotFound,
                        Message = "No visits found for the provided user.",
                        VisitorsAsData = new List<VisitAsData>()
                    };
                }

                var rawVisitDataList = visitors.Select(v => new VisitAsData
                {
                    PrimaryId = v.PrimaryId,
                    ProcessedUrlCode = v.ProcessedUrlCode,
                    IpAddress = v.IpAddress,
                    Country = v.Country,
                    City = v.City,
                    OperatingSystem = v.OperatingSystem,
                    Browser = v.Browser,
                    DeviceType = v.DeviceType,
                    UserAgent = v.UserAgent,
                    ClickedAt = v.ClickedAt
                }).ToList();

                return new VisitsResponseTransferObject
                {
                    Status = VisitorsByCodeResultStatus.Success,
                    Message = "Visitors fetched successfully for user.",
                    VisitorsAsData = rawVisitDataList
                };
            }
            catch (ArgumentNullException)
            {
                return new VisitsResponseTransferObject
                {
                    Status = VisitorsByCodeResultStatus.Failed,
                    Message = "Invalid user identifier provided.",
                };
            }
            catch (Exception)
            {
                return new VisitsResponseTransferObject
                {
                    Status = VisitorsByCodeResultStatus.Failed,
                    Message = "An unexpected error occurred while fetching visits.",
                    VisitorsAsData = new List<VisitAsData>()
                };
            }
        }
    }
}