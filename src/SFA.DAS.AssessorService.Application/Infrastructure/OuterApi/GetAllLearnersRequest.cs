using System;
using System.Text;

namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class GetAllLearnersRequest : IGetApiRequest
    {
        private string _url;

        public string GetUrl { get { return _url; } }

        public GetAllLearnersRequest(DateTime? sinceTime, int batchNumber, int batchSize)
        {
            var urlBuilder = new StringBuilder("learners?sinceTime=");
            if(null != sinceTime && sinceTime.HasValue)
            {
                urlBuilder.Append(sinceTime.Value.ToString("o", System.Globalization.CultureInfo.InvariantCulture));
            }

            urlBuilder.Append($"&batch_number={batchNumber}");
            urlBuilder.Append($"&batch_size={batchSize}");

            _url = urlBuilder.ToString();
        }
    }
}
