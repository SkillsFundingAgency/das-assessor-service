using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class DeliveryAreaModel : TestModel
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public string Status { get; set; }
    }
}
