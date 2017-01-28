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

namespace IocServiceStack.Tests
{
    using BusinessContractLibrary;
    using BusinessService;
    using NUnit.Framework;

    public class IsolatedContainerTests
    {
        [Test]
        public void IsolateTest()
        {
            //Arrange
            var ioc1 = IocServiceProvider.CreateIocContainer(config => { /*No auto setup*/ });
            var ioc2 = IocServiceProvider.CreateIocContainer(config =>{ /*No auto setup*/ });

            ioc1.GetServiceFactory().Add<ICustomer, CustomerService>();
            ioc2.GetServiceFactory().Add<ICustomer, Helper.CustomerService2>();

            //Act
            var ioc1Customer = ioc1.GetIocContainer().ServiceProvider.GetService<ICustomer>();
            var ioc2Customer = ioc2.GetIocContainer().ServiceProvider.GetService<ICustomer>();

            //Assert
            Assert.IsInstanceOf<CustomerService>(ioc1Customer);
            Assert.IsInstanceOf<Helper.CustomerService2>(ioc2Customer);

        }
    }
}
