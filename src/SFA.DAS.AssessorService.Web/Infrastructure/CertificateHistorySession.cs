using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using System;

namespace SFA.DAS.AssessorService.Web.Infrastructure
{
    public interface ICertificateHistorySession
    {
        int? CertificateHistoryPageIndex { get; set; }

        GetCertificateHistoryRequest.SortColumns CertificateHistorySortColumn { get; set; }
        string CertificateHistorySortDirection { get; set; }
        string CertificateHistorySearchTerm { get; set; }
    }

    public class CertificateHistorySession : ICertificateHistorySession
    {
        private readonly ISessionService _sessionService;

        public CertificateHistorySession(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public string CertificateHistorySearchTerm
        {
            get
            {
                return _sessionService.Get(nameof(CertificateHistorySearchTerm));
            }
            set
            {
                _sessionService.Set(nameof(CertificateHistorySearchTerm), value);
            }
        }

        public GetCertificateHistoryRequest.SortColumns CertificateHistorySortColumn
        {
            get
            {
                return Enum.Parse<GetCertificateHistoryRequest.SortColumns>(_sessionService.Get(nameof(CertificateHistorySortColumn)));
            }
            set
            {
                _sessionService.Set(nameof(CertificateHistorySortColumn), value);
            }
        }

        public string CertificateHistorySortDirection
        {
            get
            {
                return _sessionService.Get(nameof(CertificateHistorySortDirection));
            }
            set
            {
                _sessionService.Set(nameof(CertificateHistorySortDirection), value);
            }
        }

        public int? CertificateHistoryPageIndex
        {
            get
            {
                return int.Parse(_sessionService.Get(nameof(CertificateHistoryPageIndex)));
            }
            set
            {
                _sessionService.Set(nameof(CertificateHistoryPageIndex), value);
            }
        }
    }
}