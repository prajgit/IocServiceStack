﻿#region License
// Copyright (c) 2016 Rajeswara-Rao-Jinaga
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

namespace NInterservice.Tests
{
    using DependentServiceLibrary;
    using DependentServiceLibrary.Test;
    using Helper;
    using NInterservice;
    using NUnit.Framework;
    using PrimaryServiceLibrary;
    using PrimaryServiceLibrary.Test;

    public class DependencyInjectionTests
    {
        [Test]
        public void ReplaceService_Test()
        {
            //Arrange
            var factoryService = Helper.TestsHelper.FactoryServicePointer.GetFactoryService();

            /*Dependency Injection*/

            factoryService.Replace<ICustomer>(typeof(CustomerService2))
                          .Subcontract
                          .Replace<ICustomerRepository>(typeof(CustomerRepository2));
            

            //Act
            var service = ServiceManager.GetService<ICustomer>();

            //Assert
            Assert.IsInstanceOf<CustomerService2>(service);
            Assert.IsInstanceOf<CustomerRepository2>(service.GetRepository());

            //Reset for other tests
            RevertToOrignal();
        }

        private void RevertToOrignal()
        {
            var factoryService = Helper.TestsHelper.FactoryServicePointer.GetFactoryService();

            factoryService.Replace<ICustomer>(typeof(CustomerService))
                          .Subcontract
                          .Replace<ICustomerRepository>(typeof(CustomerRepository));
            
        }
    }
}
