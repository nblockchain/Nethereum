﻿using System;
using JsonRpcSharp.Client;
using Nethereum.RPC.Eth;
using Xunit;

namespace Nethereum.RPC.UnitTests.InterceptorTests
{
    public class OverridingInterceptorTest
    {
        private static TimeSpan defaultTimeOutForTests = TimeSpan.FromSeconds(30.0);

        [Fact]
        public async void ShouldInterceptNoParamsRequest()
        {
            var client = new HttpClient(new Uri("http://localhost:8545/"), defaultTimeOutForTests);
      
            client.OverridingRequestInterceptor = new OverridingInterceptorMock();
            var ethAccounts = new EthAccounts(client);
            var accounts = await ethAccounts.SendRequestAsync();
            Assert.True(accounts.Length == 2);
            Assert.Equal("hello", accounts[0]);
        }

        [Fact]
        public async void ShouldInterceptParamsRequest()
        {
            var client = new HttpClient(new Uri("http://localhost:8545/"), defaultTimeOutForTests);

            client.OverridingRequestInterceptor = new OverridingInterceptorMock();
            var ethGetCode = new EthGetCode(client);
            var code = await ethGetCode.SendRequestAsync("address");
            Assert.Equal("the code", code);
        }
    }
}
