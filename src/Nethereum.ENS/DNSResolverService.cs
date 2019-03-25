﻿using System.Threading;
using System.Threading.Tasks;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.ENS.DNSResolver.ContractDefinition;
using Nethereum.RPC.Eth.DTOs;

namespace Nethereum.ENS
{

    public partial class DNSResolverService
    {
    
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, DNSResolverDeployment dNSResolverDeployment, CancellationToken cancellationToken = default(CancellationToken))
        {
            return web3.Eth.GetContractDeploymentHandler<DNSResolverDeployment>().SendRequestAndWaitForReceiptAsync(dNSResolverDeployment, cancellationToken);
        }
        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, DNSResolverDeployment dNSResolverDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<DNSResolverDeployment>().SendRequestAsync(dNSResolverDeployment);
        }
        public static async Task<DNSResolverService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, DNSResolverDeployment dNSResolverDeployment, CancellationToken cancellationToken = default(CancellationToken))
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, dNSResolverDeployment, cancellationToken);
            return new DNSResolverService(web3, receipt.ContractAddress);
        }
    
        protected Nethereum.Web3.Web3 Web3{ get; }
        
        public ContractHandler ContractHandler { get; }
        
        public DNSResolverService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }
    
        public Task<bool> SupportsInterfaceQueryAsync(SupportsInterfaceFunction supportsInterfaceFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }

        
        public Task<bool> SupportsInterfaceQueryAsync(byte[] interfaceID, BlockParameter blockParameter = null)
        {
            var supportsInterfaceFunction = new SupportsInterfaceFunction();
                supportsInterfaceFunction.InterfaceID = interfaceID;
            
            return ContractHandler.QueryAsync<SupportsInterfaceFunction, bool>(supportsInterfaceFunction, blockParameter);
        }



        public Task<byte[]> DnsrrQueryAsync(DnsrrFunction dnsrrFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DnsrrFunction, byte[]>(dnsrrFunction, blockParameter);
        }

        
        public Task<byte[]> DnsrrQueryAsync(byte[] node, BlockParameter blockParameter = null)
        {
            var dnsrrFunction = new DnsrrFunction();
                dnsrrFunction.Node = node;
            
            return ContractHandler.QueryAsync<DnsrrFunction, byte[]>(dnsrrFunction, blockParameter);
        }



        public Task<string> SetDnsrrRequestAsync(SetDnsrrFunction setDnsrrFunction)
        {
             return ContractHandler.SendRequestAsync(setDnsrrFunction);
        }

        public Task<TransactionReceipt> SetDnsrrRequestAndWaitForReceiptAsync(SetDnsrrFunction setDnsrrFunction, CancellationToken cancellationToken = default(CancellationToken))
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setDnsrrFunction, cancellationToken);
        }

        public Task<string> SetDnsrrRequestAsync(byte[] node, byte[] data)
        {
            var setDnsrrFunction = new SetDnsrrFunction();
                setDnsrrFunction.Node = node;
                setDnsrrFunction.Data = data;
            
             return ContractHandler.SendRequestAsync(setDnsrrFunction);
        }

        public Task<TransactionReceipt> SetDnsrrRequestAndWaitForReceiptAsync(byte[] node, byte[] data, CancellationToken cancellationToken = default(CancellationToken))
        {
            var setDnsrrFunction = new SetDnsrrFunction();
                setDnsrrFunction.Node = node;
                setDnsrrFunction.Data = data;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(setDnsrrFunction, cancellationToken);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }


    }
}
