using System.Linq;

namespace SFA.DAS.AssessorService.Domain.Consts
{
    public static class CertificateStatus
    {
        public const string Draft = "Draft";
        public const string Ready = "Ready";
        public const string Submitted = "Submitted";
        public const string ToBeApproved = "ToBeApproved";
        public const string SentForApproval = "SentForApproval";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string SentToPrinter = "SentToPrinter";
        public const string Printed = "Printed";
        public const string Delivered = "Delivered";
        public const string NotDelivered = "NotDelivered";
        public const string Cancelled = "Cancelled";
        public const string NoCertificate = "NoCertificate";
        public const string Deleted = "Deleted";
        public const string Reprint = "Reprint";
        public const string Error = "Error";

        public static string[] PrintProcessStatus = new[] { SentToPrinter, Printed, Delivered, NotDelivered, Reprint };
        public static string[] PrintNotificationStatus = new[] { SentToPrinter, Printed, Delivered, NotDelivered };

        public static bool HasPrintProcessStatus(string status)
        {
            return PrintProcessStatus.Contains(status);
        }

        public static bool HasPrintNotificateStatus(string status)
        {
            return PrintNotificationStatus.Contains(status);
        }

        public static bool CanRequestReprintCertificate(string status)
        {
            var allowed = PrintProcessStatus;
            return allowed.Contains(status);
        }

        public static bool CanAmendCertificate(string status)
        {
            var allowed = new[] { Draft, Submitted };
            return allowed.Contains(status);
        }

        public static string GetDisplayName(string status)
        {
            switch (status)
            {
                case ToBeApproved:
                    return "Submitted";

                case SentForApproval:
                    return "Sent for approval";

                case SentToPrinter:
                    return "Sent to printer";

                case NotDelivered:
                    return "Not delivered";

                case Reprint:
                    return "Reprint requested";

                default:
                    return status;
            }
        }

        public static string GetStatusDateDisplayName(string status)
        {
            switch (status)
            {
                case ToBeApproved:
                    return "Date submitted";

                case Submitted:
                    return "Date submitted";

                case SentForApproval:
                    return "Date sent for approval";

                case SentToPrinter:
                    return "Date sent to printer";

                case NotDelivered:
                    return "Date not delivered";

                case Reprint:
                    return "Date reprint requested";

                case Delivered:
                    return "Date delivered";

                case Printed:
                    return "Date printed";

                default:
                    return $"Date {status}";
            }
        }
    }
}