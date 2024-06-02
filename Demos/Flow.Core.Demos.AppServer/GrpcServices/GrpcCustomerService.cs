using Flow.Core.Areas.Extensions;
using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using Flow.Core.Demos.AppServer.Areas.Customers;
using Flow.Core.Demos.Contracts.Areas.Customers;
using Flow.Core.Demos.Contracts.Common.CustomFailures;
using Grpc.Core;
using ProtoBuf.Grpc;

namespace Flow.Core.Demos.AppServer.GrpcServices
{
    public class GrpcCustomerService : IGrpcCustomerService
    {
        private readonly AddCustomerCommandHandler  _addHandler;
        private readonly CustomerSearchQueryHandler _searchHandler;

        public GrpcCustomerService(AddCustomerCommandHandler addHandler, CustomerSearchQueryHandler searchHandler) 
        { 
            //Use some sort of message dispatcher, this is just for quick demo.

            _addHandler    = addHandler;
            _searchHandler = searchHandler;
        }
        public async Task<Flow<None>> AddCustomer(AddCustomer instruction, CallContext context = default)

            => await _addHandler.Handle(new AddCustomerCommand(instruction.CustomerData));

        public async Task<Flow<CustomerSearchResponse>> CustomerSearch(CustomerSearch instruction, CallContext context = default)

            => await _searchHandler.Handle(new CustomerSearchQuery(instruction.CompanyName))
                                        .ReturnAs(success => new CustomerSearchResponse(success));

        public Task<Flow<None>> RaiseGrpcServiceException(CallContext context = default)
           
            // Just continues the code execution, this is part of the demo
            => throw new Exception("Unhandled exception");

        public Task<Flow<bool>> ApproveApplication(CallContext context = default)

            => Task.FromResult(Flow<bool>.Failed(new NotApprovedFailure("Failed credit checks.", "the Boss")));            


    }
}
