using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly AssessorDbContext _context;

        public CertificateRepository(AssessorDbContext context)
        {
            _context = context;
        }

        public async Task<Certificate> New(Certificate certificate)
        {
            await _context.Certificates.AddAsync(certificate);
            await _context.SaveChangesAsync();

            return certificate;
        }

        public async Task<Certificate> GetCertificate(Guid id)
        {
            return await _context.Certificates.SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Certificate> GetCertificate(long uln, int standardCode)
        {
            return await _context.Certificates.SingleOrDefaultAsync(c => c.Uln == uln && c.StandardCode == standardCode);
        }

        public async Task<List<Certificate>> GetCompletedCertificatesFor(long uln)
        {
            return await _context.Certificates.Where(c => c.Uln == uln && (c.Status == CertificateStatus.Printed || c.Status == CertificateStatus.Submitted)).ToListAsync();
        }

        public async Task<List<Certificate>> GetCertificates(string status)
        {
            return await _context.Certificates
                .Include(q => q.Organisation)
                .ToListAsync();
        }

        public async Task<int> GenerateBatchNumber()
        {
            if (await _context.Certificates.AnyAsync(q => q.BatchNumber != 0))
            {
                return await _context.Certificates.MaxAsync(q => q.BatchNumber) + 1;
            }

            return 1;
        }

        public async Task<Certificate> Update(Certificate certificate, string username)
        {
            var cert = await GetCertificate(certificate.Id);

            cert.CertificateData = certificate.CertificateData;
            cert.UpdatedBy = username;
            cert.Status = certificate.Status;

            await _context.SaveChangesAsync();

            return cert;
        }
    }
}