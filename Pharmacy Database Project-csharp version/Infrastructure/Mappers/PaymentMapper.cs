using Domain.Models;
using Infrastructure.Models;
using Riok.Mapperly.Abstractions;

namespace Infrastructure.Mappers;

[Mapper]
public abstract partial class PaymentMapper
{
    public partial PaymentDomain MapToDomain(Payment payment);

    public partial Payment MapToEntity(PaymentDomain paymentDomain);

    public partial void MapToEntity(PaymentDomain source, Payment destination);

    public partial PendingPayment MapToPendingPayment(SupplierOrder order);

    public partial Batch MapToEntity(BatchDomain batchDomain);
}