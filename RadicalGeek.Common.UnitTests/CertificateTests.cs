using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Security;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class CertificateTests
    {
        private const string ExpectedCertificateSubject = "CN=Microsoft Root Certificate Authority, DC=microsoft, DC=com";
        private readonly byte[] expectedSerialNumber = new byte[] { 0x65, 0x2E, 0x13, 0x07, 0xF4, 0x58, 0x73, 0x4C, 0xAD, 0xA5, 0xA0, 0x4A, 0xA1, 0x16, 0xAD, 0x79 };

        [TestMethod,Ignore]
        public void LoadCertificateBySubject()
        {
            X509Certificate certificateFromStore = Certificates.GetCertificateFromStore(StoreName.Root, StoreLocation.LocalMachine, new CertificateInfo { SubjectName = "Microsoft Root Certificate Authority" });
            Assert.AreEqual(ExpectedCertificateSubject, certificateFromStore.Subject);
        }

        [TestMethod]
        [ExpectedException(typeof(MultipleCertificatesException))]
        public void FindMultipleCertificates()
        {
            Certificates.GetCertificateFromStore(StoreName.Root, StoreLocation.LocalMachine,new CertificateInfo());
        }

        [TestMethod]
        [ExpectedException(typeof(CertificateNotFoundException))]
        public void FindNoCertificates()
        {
            Certificates.GetCertificateFromStore(StoreName.Root, StoreLocation.LocalMachine, new CertificateInfo { IssuerName = "Dr. D. Gerrard" });
        }

        [TestMethod]
        public void LoadCertificateBySerialNumber()
        {
            X509Certificate certificateFromStore = Certificates.GetCertificateFromStore(StoreName.Root, StoreLocation.LocalMachine, new CertificateInfo { SerialNumber = "79AD16A14AA0A5AD4C7358F407132E65" });
            byte[] serialNumber = certificateFromStore.GetSerialNumber();
            for (int i = 0; i < serialNumber.Length; i++)
                Assert.AreEqual(expectedSerialNumber[i], serialNumber[i]);
        }
    }
}