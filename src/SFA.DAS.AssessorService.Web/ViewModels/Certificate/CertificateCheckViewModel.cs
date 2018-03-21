﻿using System;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Web.ViewModels.Certificate
{
    public class CertificateCheckViewModel : CertificateBaseViewModel, ICertificateViewModel
    {
        public long Uln { get; set; }
        public int Level { get; set; }
        public string Option { get; set; }
        public string SelectedGrade { get; set; }
        public DateTime AchievementDate { get; set; }

        public string Name { get; set; }
        public string Dept { get; set; }
        public string Employer { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }

        public void FromCertificate(Domain.Entities.Certificate cert)
        {
            BaseFromCertificate(cert);

            Uln = cert.Uln;
            Level = CertificateData.StandardLevel;
            Option = CertificateData.CourseOption;
            SelectedGrade = CertificateData.OverallGrade;
            AchievementDate = CertificateData.AchievementDate.Value;

            Name = CertificateData.ContactName;
            Dept = CertificateData.Department;
            Employer = CertificateData.ContactOrganisation;
            AddressLine1 = CertificateData.ContactAddLine1;
            AddressLine2 = CertificateData.ContactAddLine2;
            AddressLine3 = CertificateData.ContactAddLine3;
            City = CertificateData.ContactAddLine4;
            Postcode = CertificateData.ContactPostCode;
        }

        public CertificateData GetCertificateDataFromViewModel(CertificateData data)
        {
        //    data.ContactName = Name;
        //    data.Department = Dept;
        //    data.ContactOrganisation = Employer;
        //    data.ContactAddLine1 = AddressLine1;
        //    data.ContactAddLine2 = AddressLine2;
        //    data.ContactAddLine3 = AddressLine3;
        //    data.ContactAddLine4 = City;
        //    data.ContactPostCode = Postcode;

            return data;
        }
    }
}