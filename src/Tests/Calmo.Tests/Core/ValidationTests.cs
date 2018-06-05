using System;
using System.Collections.Generic;
using Calmo.Core.Validator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calmo.Core.ExceptionHandling;

namespace Calmo.Tests.Core
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void ValidateModelList()
        {
            var list = new List<Model>
            {
                new Model {Name = "John"},
                new Model {Name = null},
                new Model {Name = "Ton"}
            };

            var validation = list.Validate<Model>()
                                 .Using()
                                 .Rule(p => p.Name, String.IsNullOrEmpty);

            Assert.IsTrue(!validation.Success);
        }

        [TestMethod]
        public void ValidateModelUsingClassMethod()
        {
            var model = new Model { Name = "John" };
            var validation = model.Validate()
                                  .Using()
                                  .Rule(p => p.Name, n => !String.IsNullOrEmpty(n));

            Assert.IsTrue(validation.Success);
        }

        [TestMethod]
        public void ValidateModelUsingOperator()
        {
            var model = new Model { Name = "John" };
            var validation = model.Validate()
                                  .Using()
                                  .Rule(p => p.Name, n => n.Name != null);

            Assert.IsTrue(validation.Success);
        }

        [TestMethod]
        public void ValidatingCPFFormat()
        {
            var model = new Model { CPF = "000.000.000-00" };
            var validation = model.Validate()
                                  .Using()
                                  .Rule(p => p.CPF, FormatValidation.Brazil.CPF.Validate);

            Assert.IsTrue(validation.Success);
        }

        [TestMethod]
        public void ValidatingCPFData()
        {
            var model = new Model { CPF = "000.000.000-00" };

            var validation = model.Validate()
                                  .Using()
                                  .Rule(p => p.CPF, DocumentValidation.Brazil.CPF.Validate);

            Assert.IsTrue(!validation.Success);
        }

        [TestMethod]
        [ExpectedException(typeof(DomainException), "CPF não atende a regra especificada.")]
        public void ValidatingCPFDataWithException()
        {
            var model = new Model { CPF = "000.000.000-00" };

            model.Validate()
                 .Using()
                 .Rule(p => p.CPF, DocumentValidation.Brazil.CPF.Validate)
                 .ThrowOnFail<DomainException>();
        }

        [TestMethod]
        public void ValidatingValidCPF()
        {
            var model = new Model { CPF = "493.258.320-62" };

            var validation = model.Validate()
                                  .Using()
                                  .Rule(p => p.CPF, FormatValidation.Brazil.CPF.Validate);

            Assert.IsTrue(validation.Success);
        }
    }

    public class Model
    {
        public string Name { get; set; }
        public string CPF { get; set; }
    }
}
