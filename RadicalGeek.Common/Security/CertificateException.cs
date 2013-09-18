using System;
using System.Reflection;

namespace RadicalGeek.Common.Security
{
    public abstract class CertificateException : Exception
    {
        protected CertificateException(string message)
            : base(message)
        {
        }

        public CertificateInfo CertificateInfo { get; set;}

        protected static string GetCertificateInfo(CertificateInfo certificateInfo)
        {
            string result = string.Empty;

            PropertyInfo[] propertyInfos = typeof (CertificateInfo).GetProperties();
            foreach (PropertyInfo info in propertyInfos)
            {
                object value = info.GetValue(certificateInfo, null);
                string reportedValue = value == null ? "null" : string.Format("\"{0}\"", value);

                result += string.Format("\t{0}: {1}\r\n", info.Name,reportedValue);
            }

            return result;
        }
    }
}