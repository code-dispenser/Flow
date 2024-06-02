using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using Flow.Core.Demos.AppClient.Common.Extensions;
using Flow.Core.Demos.Contracts.Areas.Customers;
using Grpc.Net.Client;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;

namespace Flow.Core.Demos.AppClient.Services
{
    public class GrpcCustomerService
    {
        private readonly IGrpcCustomerService _customerService;

        public GrpcCustomerService(IGrpcCustomerService customerService)

            => _customerService = customerService;

        public async Task<Flow<CustomerSearchResponse>> CustomerSearch(string companyName)

            => await _customerService.CustomerSearch(new CustomerSearch(companyName)).TryCatchGrpcResult();

        public async Task<Flow<None>> AddCustomer(CustomerData customer)

            => await _customerService.AddCustomer(new AddCustomer(customer)).TryCatchGrpcResult();

        public async Task<Flow<None>> RaiseGrpcServiceException()

            => await _customerService.RaiseGrpcServiceException().TryCatchGrpcResult();

        
        public async Task<Flow<bool>> ApproveApplication()

            => await _customerService.ApproveApplication().TryCatchGrpcResult();
    }
}
