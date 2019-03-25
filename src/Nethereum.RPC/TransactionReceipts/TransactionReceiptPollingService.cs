﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.TransactionManagers;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.RPC.Eth;
using Nethereum.RPC.Eth.Exceptions;

namespace Nethereum.RPC.TransactionReceipts
{

#if !DOTNET35
    public class TransactionReceiptServiceFactory
    {
        public static ITransactionReceiptService GetDefaultransactionReceiptService(ITransactionManager transactionManager)
        {
            return new TransactionReceiptPollingService(transactionManager);
        }
    }

    public class TransactionReceiptPollingService : ITransactionReceiptService
    {
        private readonly ITransactionManager _transactionManager;
        private readonly int _retryMiliseconds;

        public TransactionReceiptPollingService(ITransactionManager transactionManager, int retryMiliseconds = 100)
        {
            _transactionManager = transactionManager;
            _retryMiliseconds = retryMiliseconds;
        }

        public Task<TransactionReceipt> SendRequestAndWaitForReceiptAsync(TransactionInput transactionInput,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendRequestAndWaitForReceiptAsync(() => _transactionManager.SendTransactionAsync(transactionInput), cancellationToken);
        }

        public Task<List<TransactionReceipt>> SendRequestsAndWaitForReceiptAsync(IEnumerable<TransactionInput> transactionInputs,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var funcs = new List<Func<Task<string>>>();
            foreach (var transactionInput in transactionInputs)
            {
                funcs.Add(() => _transactionManager.SendTransactionAsync(transactionInput));
            }
            return SendRequestsAndWaitForReceiptAsync(funcs.ToArray(), cancellationToken);
        }

        public async Task<TransactionReceipt> SendRequestAndWaitForReceiptAsync(Func<Task<string>> transactionFunction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var transaction = await transactionFunction().ConfigureAwait(false);
            return await PollForReceiptAsync(transaction, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TransactionReceipt> PollForReceiptAsync(string transaction, CancellationToken cancellationToken = default(CancellationToken))
        {
            var getTransactionReceipt = new EthGetTransactionReceipt(_transactionManager.Client);
            var receipt = await getTransactionReceipt.SendRequestAsync(transaction).ConfigureAwait(false);
            while (receipt == null)
            {
                await Task.Delay(_retryMiliseconds).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                receipt = await getTransactionReceipt.SendRequestAsync(transaction).ConfigureAwait(false);
            }
            return receipt;
        }

        public async Task<List<TransactionReceipt>> SendRequestsAndWaitForReceiptAsync(IEnumerable<Func<Task<string>>> transactionFunctions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var txnList = new List<string>();
            foreach (var transactionFunction in transactionFunctions)
            {
                txnList.Add(await transactionFunction().ConfigureAwait(false));
            }

            var receipts = new List<TransactionReceipt>();
            foreach (var transaction in txnList)
            {
                var receipt = await PollForReceiptAsync(transaction, cancellationToken).ConfigureAwait(false);
                receipts.Add(receipt);
            }
            return receipts;
        }

        public async Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Func<Task<string>> deployFunction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var transactionReceipt = await SendRequestAndWaitForReceiptAsync(deployFunction, cancellationToken).ConfigureAwait(false);
            var contractAddress = transactionReceipt.ContractAddress;
            var ethGetCode = new EthGetCode(_transactionManager.Client);
            var code = await ethGetCode.SendRequestAsync(contractAddress).ConfigureAwait(false);
            if (code == "0x") throw new ContractDeploymentException("Code not deployed succesfully", transactionReceipt);
            return transactionReceipt;
        }

        public async Task<string> DeployContractAndGetAddressAsync(Func<Task<string>> deployFunction,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var transactionReceipt = await DeployContractAndWaitForReceiptAsync(deployFunction, cancellationToken).ConfigureAwait(false);
            return transactionReceipt.ContractAddress;
        }

        public Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(TransactionInput transactionInput, CancellationToken cancellationToken = default(CancellationToken))
        {
             return DeployContractAndWaitForReceiptAsync(() => _transactionManager.SendTransactionAsync(transactionInput), cancellationToken);
        }
    }
#endif
}
