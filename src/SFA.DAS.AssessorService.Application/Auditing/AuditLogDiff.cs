using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFA.DAS.AssessorService.Application.Auditing
{
    public class AuditLogDiff
    {
        private string _previousValue;
        private string _currentValue;

        public string ProperyChanged { get; set; }
        public string FieldChanged { get; set; }

        public string PreviousValue
        {
            get
            {
                return _previousValue;
            }
            set
            {
                _previousValue = RemoveWhitespace(value);
            }
        }

        public string CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = RemoveWhitespace(value);
            }
        }

        private string RemoveWhitespace(string str)
        {
            return new string(str.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
    }
}
