using System.Security.Cryptography.X509Certificates;
using RadicalGeek.Common.Collections;

namespace RadicalGeek.Common.Security
{
    public class CertificateInfo
    {
        internal AutoDictionary<X509FindType, string> Searches = new AutoDictionary<X509FindType, string>();

        public string ThumbPrint
        {
            set { Searches[X509FindType.FindByThumbprint] = value; }
            get { return Searches[X509FindType.FindByThumbprint]; }
        }
        public string SerialNumber
        {
            set { Searches[X509FindType.FindBySerialNumber] = value; }
            get { return Searches[X509FindType.FindBySerialNumber]; }
        }
        public string SubjectName
        {
            set { Searches[X509FindType.FindBySubjectName] = value; }
            get { return Searches[X509FindType.FindBySubjectName]; }
        }
        public string KeyUsage
        {
            set { Searches[X509FindType.FindByKeyUsage] = value; }
            get { return Searches[X509FindType.FindByKeyUsage]; }
        }
        public string IssuerName
        {
            set { Searches[X509FindType.FindByIssuerName] = value; }
            get { return Searches[X509FindType.FindByIssuerName]; }
        }
    }
}