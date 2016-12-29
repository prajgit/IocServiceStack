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
    using NUnit.Framework;
    
    
    /*This test module doesn't support parallel execution*/
    [SetUpFixture]
    public class SetupTests
    {
        [OneTimeSetUp]
        public void RegisterTest()
        {
            var configRef = ServiceInjector.Configure(config =>
            {

                config.AddServices((serviceConfig) => { serviceConfig.Namespaces = new[] { "PrimaryServiceLibrary.Test" }; serviceConfig.Assemblies = new[] { "PrimaryServiceLibrary" }; })
                      .AddDependencies((serviceConfig) => { serviceConfig.Namespaces = new[] { "DependentServiceLibrary.Test" }; serviceConfig.Assemblies = new[] { "DependentServiceLibrary" }; });

                config.EnableStrictMode();

            });

            //Hold the pointer of serviceConfig in a static field to run further tests of dependecy injection.
            Helper.TestsHelper.FactoryServicePointer = configRef;
        }
    }
}
