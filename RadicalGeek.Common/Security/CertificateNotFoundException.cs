namespace RadicalGeek.Common.Security
{
    public sealed class CertificateNotFoundException : CertificateException
    {
        public CertificateNotFoundException(CertificateInfo certificateInfo)
            : base(string.Format("No certificate was found with the supplied CertificateInfo parameters:\r\n{0}", GetCertificateInfo(certificateInfo)))
        {
            CertificateInfo = certificateInfo;
        }
    }
}