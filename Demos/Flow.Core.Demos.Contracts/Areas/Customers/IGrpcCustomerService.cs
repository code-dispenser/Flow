using Flow.Core.Areas.Returns;
using Flow.Core.Common.Models;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow.Core.Demos.Contracts.Areas.Customers;

[Service]
public interface IGrpcCustomerService
{
    [Operation]
    Task<Flow<None>> AddCustomer(AddCustomer instruction, CallContext context = default);

    [Operation]
    Task<Flow<CustomerSearchResponse>> CustomerSearch(CustomerSearch instruction,  CallContext context = default);

    [Operation]
    Task<Flow<None>> RaiseGrpcServiceException(CallContext context = default);

    [Operation]
    Task<Flow<bool>> ApproveApplication(CallContext context = default);
}
