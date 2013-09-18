using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Validation;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class LuhnTest
    {
        readonly List<string> knownPassingNumbers = new List<string>
                                        {
                                            "42",
                                            "331",
                                            "8631",
                                            "50278",
                                            "387654",
                                            "6833883",
                                            "84148444",
                                            "748440187",
                                            "1484628126",
                                            "46004862671",
                                            "204432714846",
                                            "4160676762253",
                                            "64860214172454",
                                            "602283132772587",
                                            "2841825224526532"
                                         };

        readonly List<string> knownFailingNumbers = new List<string>
                                        {
                                            "52",
                                            "321",
                                            "8661",
                                            "54878",
                                            "383484",
                                            "6833286",
                                            "84148424",
                                            "748848467",
                                            "1486428126",
                                            "46004846671",
                                            "286455714846",
                                            "4160656762253",
                                            "64860214264654",
                                            "602283132775647",
                                            "2841825224586432"
                                         };

        [TestMethod]
        public void TestLuhnAgainstKnownNumbers()
        {
            foreach (string knownNumber in knownPassingNumbers)
                Assert.IsTrue(knownNumber.LuhnCheck());
            foreach (string knownNumber in knownFailingNumbers)
                Assert.IsFalse(knownNumber.LuhnCheck());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestLuhnRejectsNull()
        {
            Luhn.LuhnCheck(null);
        }
    }
}
